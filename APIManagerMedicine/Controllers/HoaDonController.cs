﻿using APIManagerMedicine.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace APIManagerMedicine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HoaDonController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public HoaDonController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy danh sách hóa đơn
        [HttpGet("ListHoaDon")]
        public ActionResult<Response> GetAllHoaDon()
        {
            Response response = new Response();
            List<HoaDon> lstHoaDon = new List<HoaDon>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM hoadon", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    HoaDon hd = new HoaDon
                    {
                        MaHD = Convert.ToString(dt.Rows[i]["MaHD"]),
                        MaNV = Convert.ToString(dt.Rows[i]["MaNV"]),
                        MaKH = Convert.ToString(dt.Rows[i]["MaKH"]),
                        NgayBan = Convert.ToDateTime(dt.Rows[i]["NgayBan"]),
                        TongGia = Convert.ToDecimal(dt.Rows[i]["TongGia"]),
                        MaCN = Convert.ToString(dt.Rows[i]["MaCN"]),
                        GiaTruocGiam = Convert.ToDecimal(dt.Rows[i]["GiaTruocGiam"]), // NEW
                        GiamGia = Convert.ToInt32(dt.Rows[i]["GiamGia"])             // NEW
                    };
                    lstHoaDon.Add(hd);
                }

                response.StatusCode = 200;
                response.StatusMessage = "Success";
                response.ListHoaDon = lstHoaDon;

                return Ok(response);
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "No invoices found.";
                return NotFound(response);
            }
        }

        // Lấy hóa đơn theo mã
        [HttpGet("GetHoaDon/{maHD}")]
        public ActionResult<Response> GetHoaDonByMaHD(string maHD)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM hoadon WHERE MaHD = @MaHD", connection);
            da.SelectCommand.Parameters.AddWithValue("@MaHD", maHD);

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                HoaDon hd = new HoaDon
                {
                    MaHD = Convert.ToString(row["MaHD"]),
                    MaNV = Convert.ToString(row["MaNV"]),
                    MaKH = Convert.ToString(row["MaKH"]),
                    NgayBan = Convert.ToDateTime(row["NgayBan"]),
                    TongGia = Convert.ToDecimal(row["TongGia"]),
                    MaCN = Convert.ToString(row["MaCN"]),
                    GiaTruocGiam = Convert.ToDecimal(row["GiaTruocGiam"]), // NEW
                    GiamGia = Convert.ToInt32(row["GiamGia"])             // NEW
                };

                response.StatusCode = 200;
                response.StatusMessage = "Success";
                response.ListHoaDon = new List<HoaDon> { hd };

                return Ok(response);
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "Invoice not found.";
                return NotFound(response);
            }
        }

        [HttpPost("AddHoaDon")]
        public ActionResult<Response> AddHoaDon([FromBody] HoaDon newHoaDon)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                // Lấy thời gian hiện tại theo định dạng yyyyMMddHHmmss
                string timePrefix = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                // Tạo GUID mới và kết hợp với thời gian
                string sequentialGuid = timePrefix + Guid.NewGuid().ToString().Substring(8); // Thêm thời gian vào GUID để đảm bảo tính tuần tự

                newHoaDon.MaHD = sequentialGuid;  // Sử dụng GUID có tính tuần tự

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO hoadon (MaHD, MaNV, MaKH, NgayBan, TongGia, MaCN, GiaTruocGiam, GiamGia) 
              VALUES (@MaHD, @MaNV, @MaKH, @NgayBan, @TongGia, @MaCN, @GiaTruocGiam, @GiamGia)", connection);

                cmd.Parameters.AddWithValue("@MaHD", newHoaDon.MaHD);
                cmd.Parameters.AddWithValue("@MaNV", newHoaDon.MaNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaKH", newHoaDon.MaKH ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayBan", newHoaDon.NgayBan ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TongGia", newHoaDon.TongGia ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCN", newHoaDon.MaCN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GiaTruocGiam", newHoaDon.GiaTruocGiam ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GiamGia", newHoaDon.GiamGia);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 201;
                    response.StatusMessage = "Invoice added successfully.";
                    response.MaHD = newHoaDon.MaHD;
                    return CreatedAtAction(nameof(GetHoaDonByMaHD), new { maHD = newHoaDon.MaHD }, response);
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusMessage = "Failed to add invoice.";
                    return BadRequest(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = $"Internal server error: {ex.Message}";
                return StatusCode(500, response);
            }
            finally
            {
                connection.Close();
            }
        }

        // Cập nhật hóa đơn
        [HttpPut("UpdateHoaDon/{maHD}")]
        public ActionResult<Response> UpdateHoaDon(string maHD, [FromBody] HoaDon updatedHoaDon)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(
                    @"UPDATE hoadon 
                      SET MaNV = @MaNV, MaKH = @MaKH, NgayBan = @NgayBan, TongGia = @TongGia, MaCN = @MaCN, 
                          GiaTruocGiam = @GiaTruocGiam, GiamGia = @GiamGia 
                      WHERE MaHD = @MaHD", connection);

                cmd.Parameters.AddWithValue("@MaHD", maHD);
                cmd.Parameters.AddWithValue("@MaNV", updatedHoaDon.MaNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaKH", updatedHoaDon.MaKH ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayBan", updatedHoaDon.NgayBan ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@TongGia", updatedHoaDon.TongGia ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCN", updatedHoaDon.MaCN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GiaTruocGiam", updatedHoaDon.GiaTruocGiam ?? (object)DBNull.Value); // NEW
                cmd.Parameters.AddWithValue("@GiamGia", updatedHoaDon.GiamGia);                                    // NEW

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Invoice updated successfully.";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "Invoice not found.";
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = $"Internal server error: {ex.Message}";
                return StatusCode(500, response);
            }
            finally
            {
                connection.Close();
            }
        }

        // Xóa hóa đơn theo mã
        [HttpDelete("DeleteHoaDon/{maHD}")]
        public ActionResult<Response> DeleteHoaDon(string maHD)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM hoadon WHERE MaHD = @MaHD", connection);
                cmd.Parameters.AddWithValue("@MaHD", maHD);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Invoice deleted successfully.";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "Invoice not found.";
                    return NotFound(response);
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = $"Internal server error: {ex.Message}";
                return StatusCode(500, response);
            }
            finally
            {
                connection.Close();
            }
        }

        // Thống kê doanh thu của tất cả chi nhánh và chi nhánh hiện tại
        [HttpGet("RevenueStatistics/{branchId}")]
        public ActionResult<Response> GetRevenueStatistics(string branchId)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                SqlCommand totalRevenueCmd = new SqlCommand("SELECT SUM(TongGia) AS TotalRevenue FROM linksvqlthuoc.QLThuoc.dbo.hoadon", connection);
                decimal totalRevenue = (decimal)(totalRevenueCmd.ExecuteScalar() ?? 0);

                SqlCommand branchRevenueCmd = new SqlCommand("SELECT SUM(TongGia) AS BranchRevenue FROM hoadon WHERE MaCN = @BranchId", connection);
                branchRevenueCmd.Parameters.AddWithValue("@BranchId", branchId);
                decimal branchRevenue = (decimal)(branchRevenueCmd.ExecuteScalar() ?? 0);

                decimal branchPercentage = totalRevenue > 0 ? (branchRevenue * 100.0m / totalRevenue) : 0;

                response.StatusCode = 200;
                response.StatusMessage = "Success";
                response.TotalRevenue = totalRevenue;
                response.BranchRevenue = branchRevenue;
                response.BranchPercentage = branchPercentage;

                return Ok(response);
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = $"Internal server error: {ex.Message}";
                return StatusCode(500, response);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}

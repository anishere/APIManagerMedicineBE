using APIManagerMedicine.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;

namespace APIManagerMedicine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ThuocTrongHDController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ThuocTrongHDController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy danh sách ThuocTrongHD theo MaHD
        [HttpGet("ListThuocTrongHD/{maHD}")]
        public ActionResult<Response> GetThuocTrongHDByMaHD(string maHD)
        {
            Response response = new Response();
            List<ThuocTrongHD> lstThuocTrongHD = new List<ThuocTrongHD>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM thuoc_trong_hoa_don WHERE MaHD = @MaHD", connection);
            da.SelectCommand.Parameters.AddWithValue("@MaHD", maHD);

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ThuocTrongHD tthd = new ThuocTrongHD
                    {
                        MaHD = Convert.ToString(dt.Rows[i]["MaHD"]),
                        MaThuoc = Convert.ToString(dt.Rows[i]["MaThuoc"]),
                        SoLuongBan = Convert.ToInt32(dt.Rows[i]["SoLuongBan"]),
                        MaCN = Convert.ToString(dt.Rows[i]["MaCN"])
                    };
                    lstThuocTrongHD.Add(tthd);
                }

                response.StatusCode = 200;
                response.StatusMessage = "Success";
                response.ListThuocTrongHD = lstThuocTrongHD;

                return Ok(response);
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "No medicines found in this invoice.";
                return NotFound(response);
            }
        }

        // Lấy danh sách tất cả ThuocTrongHD
        [HttpGet("ListThuocTrongHD")]
        public ActionResult<Response> GetAllThuocTrongHD()
        {
            Response response = new Response();
            List<ThuocTrongHD> lstThuocTrongHD = new List<ThuocTrongHD>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            // Truy vấn tất cả bản ghi trong bảng thuoc_trong_hoa_don mà không dựa vào MaHD
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM thuoc_trong_hoa_don", connection);

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    ThuocTrongHD tthd = new ThuocTrongHD
                    {
                        MaHD = Convert.ToString(dt.Rows[i]["MaHD"]),
                        MaThuoc = Convert.ToString(dt.Rows[i]["MaThuoc"]),
                        SoLuongBan = Convert.ToInt32(dt.Rows[i]["SoLuongBan"]),
                        MaCN = Convert.ToString(dt.Rows[i]["MaCN"])
                    };
                    lstThuocTrongHD.Add(tthd);
                }

                response.StatusCode = 200;
                response.StatusMessage = "Lấy dữ liệu thành công!";
                response.ListThuocTrongHD = lstThuocTrongHD;
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "Không có dữ liệu!";
            }

            return Ok(response);
        }

        // Thêm ThuocTrongHD theo MaHD và MaThuoc
        [HttpPost("AddThuocTrongHD")]
        public ActionResult<Response> AddThuocTrongHD([FromBody] ThuocTrongHD newThuocTrongHD)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO thuoc_trong_hoa_don (MaHD, MaThuoc, SoLuongBan, MaCN) 
                    VALUES (@MaHD, @MaThuoc, @SoLuongBan, @MaCN)", connection);

                cmd.Parameters.AddWithValue("@MaHD", newThuocTrongHD.MaHD);
                cmd.Parameters.AddWithValue("@MaThuoc", newThuocTrongHD.MaThuoc);
                cmd.Parameters.AddWithValue("@SoLuongBan", newThuocTrongHD.SoLuongBan ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCN", newThuocTrongHD.MaCN ?? (object)DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 201; // 201 Created
                    response.StatusMessage = "Medicine added to invoice successfully.";
                    return CreatedAtAction(nameof(GetThuocTrongHDByMaHD), new { maHD = newThuocTrongHD.MaHD }, response);
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusMessage = "Failed to add medicine to invoice.";
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

        // Xóa tất cả các thuốc trong hóa đơn theo MaHD
        [HttpDelete("DeleteAllThuocTrongHD/{maHD}")]
        public ActionResult<Response> DeleteAllThuocTrongHDByMaHD(string maHD)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM thuoc_trong_hoa_don WHERE MaHD = @MaHD", connection);
                cmd.Parameters.AddWithValue("@MaHD", maHD);

                int rowsAffected = cmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "All medicines in the invoice have been successfully deleted.";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "No medicines found for this invoice.";
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
    }
}

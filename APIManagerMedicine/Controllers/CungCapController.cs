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
    public class CungCapController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public CungCapController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy danh sách cung cấp
        [HttpGet("ListCungCap")]
        public ActionResult<Response> GetAllCungCap()
        {
            Response response = new Response();
            List<CungCap> lstCungCap = new List<CungCap>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM cungcap", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    CungCap cc = new CungCap
                    {
                        MaNV = Convert.ToString(dt.Rows[i]["MaNV"]),
                        MaNCC = Convert.ToString(dt.Rows[i]["MaNCC"]),
                        MaThuoc = Convert.ToString(dt.Rows[i]["MaThuoc"]),
                        NgayCungCap = Convert.ToDateTime(dt.Rows[i]["NgayCungCap"]),
                        SoLuongThuocNhap = Convert.ToInt32(dt.Rows[i]["SoLuongThuocNhap"]),
                        MaCN = Convert.ToString(dt.Rows[i]["MaCN"]),
                        GiaNhap = Convert.ToDecimal(dt.Rows[i]["GiaNhap"])
                    };
                    lstCungCap.Add(cc);
                }

                response.StatusCode = 200;
                response.StatusMessage = "Success";
                response.ListCungCap = lstCungCap;

                return Ok(response);
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "No records found.";
                return NotFound(response);
            }
        }

        // Thêm cung cấp
        [HttpPost("AddCungCap")]
        public ActionResult<Response> AddCungCap([FromBody] CungCap newCungCap)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO cungcap (MaNV, MaNCC, MaThuoc, NgayCungCap, SoLuongThuocNhap, MaCN, GiaNhap) 
                      VALUES (@MaNV, @MaNCC, @MaThuoc, @NgayCungCap, @SoLuongThuocNhap, @MaCN, @GiaNhap)", connection);

                cmd.Parameters.AddWithValue("@MaNV", newCungCap.MaNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaNCC", newCungCap.MaNCC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaThuoc", newCungCap.MaThuoc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayCungCap", newCungCap.NgayCungCap);
                cmd.Parameters.AddWithValue("@SoLuongThuocNhap", newCungCap.SoLuongThuocNhap);
                cmd.Parameters.AddWithValue("@MaCN", newCungCap.MaCN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GiaNhap", newCungCap.GiaNhap);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 201;
                    response.StatusMessage = "Record added successfully.";
                    return CreatedAtAction(nameof(GetAllCungCap), response);
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusMessage = "Failed to add record.";
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

        // Cập nhật cung cấp
        [HttpPut("UpdateCungCap")]
        public ActionResult<Response> UpdateCungCap([FromBody] CungCap updatedCungCap)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE cungcap 
                      SET MaNV = @MaNV, MaNCC = @MaNCC, NgayCungCap = @NgayCungCap, SoLuongThuocNhap = @SoLuongThuocNhap, MaCN = @MaCN, GiaNhap = @GiaNhap 
                      WHERE MaThuoc = @MaThuoc", connection);

                cmd.Parameters.AddWithValue("@MaNV", updatedCungCap.MaNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaNCC", updatedCungCap.MaNCC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaThuoc", updatedCungCap.MaThuoc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayCungCap", updatedCungCap.NgayCungCap);
                cmd.Parameters.AddWithValue("@SoLuongThuocNhap", updatedCungCap.SoLuongThuocNhap);
                cmd.Parameters.AddWithValue("@MaCN", updatedCungCap.MaCN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GiaNhap", updatedCungCap.GiaNhap);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Record updated successfully.";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "Record not found.";
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

        // Xóa cung cấp theo mã thuốc
        [HttpDelete("DeleteCungCap/{maThuoc}")]
        public ActionResult<Response> DeleteCungCap(string maThuoc)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM cungcap WHERE MaThuoc = @MaThuoc", connection);
                cmd.Parameters.AddWithValue("@MaThuoc", maThuoc);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Record deleted successfully.";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "Record not found.";
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

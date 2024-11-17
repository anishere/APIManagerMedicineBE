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
                        IDCungCap = Convert.ToString(dt.Rows[i]["IDCungCap"]),
                        MaNV = Convert.ToString(dt.Rows[i]["MaNV"]),
                        MaNCC = Convert.ToString(dt.Rows[i]["MaNCC"]),
                        MaThuoc = Convert.ToString(dt.Rows[i]["MaThuoc"]),
                        NgayCungCap = dt.Rows[i]["NgayCungCap"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[i]["NgayCungCap"]) : null,
                        SoLuongThuocNhap = dt.Rows[i]["SoLuongThuocNhap"] != DBNull.Value ? Convert.ToInt32(dt.Rows[i]["SoLuongThuocNhap"]) : null,
                        MaCN = Convert.ToString(dt.Rows[i]["MaCN"]),
                        GiaNhap = dt.Rows[i]["GiaNhap"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[i]["GiaNhap"]) : null
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

        // Lấy cung cấp theo khóa chính
        [HttpGet("GetCungCap")]
        public ActionResult<Response> GetCungCapById(string idCungCap)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM cungcap WHERE IDCungCap = @IDCungCap", connection);
                da.SelectCommand.Parameters.AddWithValue("@IDCungCap", idCungCap);

                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count > 0)
                {
                    CungCap cc = new CungCap
                    {
                        IDCungCap = Convert.ToString(dt.Rows[0]["IDCungCap"]),
                        MaNV = Convert.ToString(dt.Rows[0]["MaNV"]),
                        MaNCC = Convert.ToString(dt.Rows[0]["MaNCC"]),
                        MaThuoc = Convert.ToString(dt.Rows[0]["MaThuoc"]),
                        NgayCungCap = dt.Rows[0]["NgayCungCap"] != DBNull.Value ? Convert.ToDateTime(dt.Rows[0]["NgayCungCap"]) : null,
                        SoLuongThuocNhap = dt.Rows[0]["SoLuongThuocNhap"] != DBNull.Value ? Convert.ToInt32(dt.Rows[0]["SoLuongThuocNhap"]) : null,
                        MaCN = Convert.ToString(dt.Rows[0]["MaCN"]),
                        GiaNhap = dt.Rows[0]["GiaNhap"] != DBNull.Value ? Convert.ToDecimal(dt.Rows[0]["GiaNhap"]) : null
                    };

                    response.StatusCode = 200;
                    response.StatusMessage = "Success";
                    response.ListCungCap = new List<CungCap> { cc };
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "No record found.";
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

        // Thêm cung cấp
        [HttpPost("AddCungCap")]
        public ActionResult<Response> AddCungCap([FromBody] CungCap newCungCap)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                // Tạo mã IDCungCap ngẫu nhiên
                string idCungCap = Guid.NewGuid().ToString();

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO cungcap (IDCungCap, MaNV, MaNCC, MaThuoc, NgayCungCap, SoLuongThuocNhap, MaCN, GiaNhap) 
                      VALUES (@IDCungCap, @MaNV, @MaNCC, @MaThuoc, @NgayCungCap, @SoLuongThuocNhap, @MaCN, @GiaNhap)", connection);

                cmd.Parameters.AddWithValue("@IDCungCap", idCungCap);
                cmd.Parameters.AddWithValue("@MaNV", newCungCap.MaNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaNCC", newCungCap.MaNCC ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaThuoc", newCungCap.MaThuoc ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgayCungCap", newCungCap.NgayCungCap ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SoLuongThuocNhap", newCungCap.SoLuongThuocNhap ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCN", newCungCap.MaCN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GiaNhap", newCungCap.GiaNhap ?? (object)DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 201;
                    response.StatusMessage = "Record added successfully.";
                    return CreatedAtAction(nameof(GetCungCapById), new { idCungCap }, response);
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
        public ActionResult<Response> UpdateCungCap(string idCungCap, [FromBody] CungCap updatedCungCap)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(
                    @"UPDATE cungcap 
                      SET NgayCungCap = @NgayCungCap, SoLuongThuocNhap = @SoLuongThuocNhap, MaCN = @MaCN, GiaNhap = @GiaNhap 
                      WHERE IDCungCap = @IDCungCap", connection);

                cmd.Parameters.AddWithValue("@IDCungCap", idCungCap);
                cmd.Parameters.AddWithValue("@NgayCungCap", updatedCungCap.NgayCungCap ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SoLuongThuocNhap", updatedCungCap.SoLuongThuocNhap ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCN", updatedCungCap.MaCN ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GiaNhap", updatedCungCap.GiaNhap ?? (object)DBNull.Value);

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

        // Xóa cung cấp
        [HttpDelete("DeleteCungCap")]
        public ActionResult<Response> DeleteCungCap(string idCungCap)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand(
                    @"DELETE FROM cungcap 
                      WHERE IDCungCap = @IDCungCap", connection);

                cmd.Parameters.AddWithValue("@IDCungCap", idCungCap);

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

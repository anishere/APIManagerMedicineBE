using APIManagerMedicine.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using System.Data;

namespace APIManagerMedicine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KhachHangController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public KhachHangController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy danh sách khách hàng
        [HttpGet("ListKhachHang")]
        public ActionResult<Response> GetAllKhachHang()
        {
            Response response = new Response();
            List<KhachHang> lstKhachHang = new List<KhachHang>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM khachhang", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    KhachHang kh = new KhachHang
                    {
                        MaKH = Convert.ToString(dt.Rows[i]["MaKH"]),
                        TenKH = Convert.ToString(dt.Rows[i]["TenKH"]),
                        SDT = Convert.ToString(dt.Rows[i]["SDT"]),
                        GT = Convert.ToString(dt.Rows[i]["GT"]),
                        MaCN = Convert.ToString(dt.Rows[i]["MaCN"]),
                        RowGuid = Guid.Parse(Convert.ToString(dt.Rows[i]["rowguid"]))
                    };
                    lstKhachHang.Add(kh);
                }

                response.StatusCode = 200;
                response.StatusMessage = "Success";
                response.ListKhachHang = lstKhachHang;

                return Ok(response);
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "No customers found.";
                return NotFound(response);
            }
        }

        // Lấy khách hàng theo số điện thoại
        [HttpGet("GetKhachHang/{sdt}")]
        public ActionResult<Response> GetKhachHangBySDT(string sdt)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM khachhang WHERE SDT = @SDT", connection);
            da.SelectCommand.Parameters.AddWithValue("@SDT", sdt);

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                KhachHang kh = new KhachHang
                {
                    MaKH = Convert.ToString(row["MaKH"]),
                    TenKH = Convert.ToString(row["TenKH"]),
                    SDT = Convert.ToString(row["SDT"]),
                    GT = Convert.ToString(row["GT"]),
                    MaCN = Convert.ToString(row["MaCN"]),
                    RowGuid = Guid.Parse(Convert.ToString(row["rowguid"]))
                };

                response.StatusCode = 200;
                response.StatusMessage = "Success";
                response.ListKhachHang = new List<KhachHang> { kh };

                return Ok(response);
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "Customer not found.";
                return NotFound(response);
            }
        }

        // Thêm khách hàng
        // Thêm khách hàng
        [HttpPost("AddKhachHang")]
        public ActionResult<Response> AddKhachHang([FromBody] KhachHang newKhachHang)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                // Tạo MaKH tự động bằng cách sử dụng GUID
                newKhachHang.MaKH = Guid.NewGuid().ToString();

                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO khachhang (MaKH, TenKH, SDT, GT, MaCN, rowguid) 
              VALUES (@MaKH, @TenKH, @SDT, @GT, @MaCN, NEWID())", connection);

                cmd.Parameters.AddWithValue("@MaKH", newKhachHang.MaKH);
                cmd.Parameters.AddWithValue("@TenKH", newKhachHang.TenKH ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SDT", newKhachHang.SDT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GT", newKhachHang.GT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCN", newKhachHang.MaCN ?? (object)DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 201; // 201 Created
                    response.StatusMessage = "Customer added successfully.";
                    return CreatedAtAction(nameof(GetKhachHangBySDT), new { sdt = newKhachHang.SDT }, response);
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusMessage = "Failed to add customer.";
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

        // Cập nhật thông tin khách hàng
        [HttpPut("UpdateKhachHang/{maKH}")]
        public ActionResult<Response> UpdateKhachHang(string maKH, [FromBody] KhachHang updatedKhachHang)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(
                    @"UPDATE khachhang 
                      SET TenKH = @TenKH, SDT = @SDT, GT = @GT, MaCN = @MaCN 
                      WHERE MaKH = @MaKH", connection);

                cmd.Parameters.AddWithValue("@MaKH", maKH);
                cmd.Parameters.AddWithValue("@TenKH", updatedKhachHang.TenKH ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@SDT", updatedKhachHang.SDT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@GT", updatedKhachHang.GT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCN", updatedKhachHang.MaCN ?? (object)DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Customer updated successfully.";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "Customer not found.";
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

        // Xóa khách hàng theo mã
        [HttpDelete("DeleteKhachHang/{maKH}")]
        public ActionResult<Response> DeleteKhachHang(string maKH)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM khachhang WHERE MaKH = @MaKH", connection);
                cmd.Parameters.AddWithValue("@MaKH", maKH);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Customer deleted successfully.";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "Customer not found.";
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

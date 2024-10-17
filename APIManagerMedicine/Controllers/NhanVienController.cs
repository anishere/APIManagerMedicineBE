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
    public class NhanVienController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public NhanVienController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Lấy danh sách nhân viên
        [HttpGet]
        [Route("ListNhanVien")]
        public ActionResult<Response> GetAllNhanVien()
        {
            Response response = new Response();
            List<NhanVien> lstNhanVien = new List<NhanVien>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM nhanvien", connection);
            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    NhanVien nv = new NhanVien
                    {
                        MaNV = Convert.ToString(dt.Rows[i]["MaNV"]),
                        TenNV = Convert.ToString(dt.Rows[i]["TenNV"]),
                        Gt = Convert.ToString(dt.Rows[i]["Gt"]),
                        NgaySinh = dt.Rows[i]["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(dt.Rows[i]["NgaySinh"]),
                        SDT = Convert.ToString(dt.Rows[i]["SDT"]),
                        Luong = dt.Rows[i]["Luong"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(dt.Rows[i]["Luong"]),
                        MaCN = Convert.ToString(dt.Rows[i]["MaCN"]),
                        RowGuid = Guid.Parse(Convert.ToString(dt.Rows[i]["RowGuid"]))
                    };
                    lstNhanVien.Add(nv);
                }

                response.StatusCode = 200;
                response.StatusMessage = "Success";
                response.ListNhanVien = lstNhanVien;

                return Ok(response);
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "No employees found.";
                return NotFound(response);
            }
        }

        // Lấy nhân viên theo mã
        [HttpGet]
        [Route("GetNhanVien/{maNV}")]
        public ActionResult<Response> GetNhanVienByMaNV(string maNV)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM nhanvien WHERE MaNV = @MaNV", connection);
            da.SelectCommand.Parameters.AddWithValue("@MaNV", maNV);

            DataTable dt = new DataTable();
            da.Fill(dt);

            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                NhanVien nv = new NhanVien
                {
                    MaNV = Convert.ToString(row["MaNV"]),
                    TenNV = Convert.ToString(row["TenNV"]),
                    Gt = Convert.ToString(row["Gt"]),
                    NgaySinh = row["NgaySinh"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["NgaySinh"]),
                    SDT = Convert.ToString(row["SDT"]),
                    Luong = row["Luong"] == DBNull.Value ? (decimal?)null : Convert.ToDecimal(row["Luong"]),
                    MaCN = Convert.ToString(row["MaCN"]),
                    RowGuid = Guid.Parse(Convert.ToString(row["RowGuid"]))
                };

                response.StatusCode = 200;
                response.StatusMessage = "Success";
                response.ListNhanVien = new List<NhanVien> { nv };

                return Ok(response);
            }
            else
            {
                response.StatusCode = 404;
                response.StatusMessage = "Employee not found.";
                return NotFound(response);
            }
        }

        // Xóa nhân viên theo mã
        [HttpDelete]
        [Route("DeleteNhanVien/{maNV}")]
        public ActionResult<Response> DeleteNhanVien(string maNV)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM nhanvien WHERE MaNV = @MaNV", connection);
                cmd.Parameters.AddWithValue("@MaNV", maNV);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Employee deleted successfully.";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "Employee not found.";
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

        [HttpPost]
        [Route("AddNhanVien")]
        public ActionResult<Response> AddNhanVien([FromBody] NhanVien newNhanVien)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                // SQL Command để thêm nhân viên mới
                SqlCommand cmd = new SqlCommand(
                    @"INSERT INTO nhanvien (MaNV, TenNV, Gt, NgaySinh, SDT, Luong, MaCN, rowguid) 
              VALUES (@MaNV, @TenNV, @Gt, @NgaySinh, @SDT, @Luong, @MaCN, NEWID())", connection);

                // Thêm các parameter vào câu lệnh SQL
                cmd.Parameters.AddWithValue("@MaNV", newNhanVien.MaNV);
                cmd.Parameters.AddWithValue("@TenNV", newNhanVien.TenNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Gt", newNhanVien.Gt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgaySinh", newNhanVien.NgaySinh.HasValue ? (object)newNhanVien.NgaySinh.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@SDT", newNhanVien.SDT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Luong", newNhanVien.Luong.HasValue ? (object)newNhanVien.Luong.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCN", newNhanVien.MaCN ?? (object)DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 201; // 201 Created
                    response.StatusMessage = "Employee added successfully.";
                    return CreatedAtAction(nameof(GetNhanVienByMaNV), new { maNV = newNhanVien.MaNV }, response);
                }
                else
                {
                    response.StatusCode = 400;
                    response.StatusMessage = "Failed to add employee.";
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


        // Cập nhật thông tin nhân viên
        [HttpPut]
        [Route("UpdateNhanVien/{maNV}")]
        public ActionResult<Response> UpdateNhanVien(string maNV, [FromBody] NhanVien updatedNhanVien)
        {
            Response response = new Response();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                SqlCommand cmd = new SqlCommand(
                    @"UPDATE nhanvien 
                      SET TenNV = @TenNV, Gt = @Gt, NgaySinh = @NgaySinh, SDT = @SDT, Luong = @Luong, MaCN = @MaCN 
                      WHERE MaNV = @MaNV", connection);

                cmd.Parameters.AddWithValue("@MaNV", maNV);
                cmd.Parameters.AddWithValue("@TenNV", updatedNhanVien.TenNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Gt", updatedNhanVien.Gt ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@NgaySinh", updatedNhanVien.NgaySinh.HasValue ? (object)updatedNhanVien.NgaySinh.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@SDT", updatedNhanVien.SDT ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@Luong", updatedNhanVien.Luong.HasValue ? (object)updatedNhanVien.Luong.Value : DBNull.Value);
                cmd.Parameters.AddWithValue("@MaCN", updatedNhanVien.MaCN ?? (object)DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Employee updated successfully.";
                    return Ok(response);
                }
                else
                {
                    response.StatusCode = 404;
                    response.StatusMessage = "Employee not found.";
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

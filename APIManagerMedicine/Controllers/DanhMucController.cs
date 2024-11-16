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
    public class DanhMucController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DanhMucController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllCategories")]
        public ActionResult<Response> GetAllCategories()
        {
            List<DanhMuc> danhMucList = new List<DanhMuc>();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                string query = "SELECT MaDanhMuc, TenDanhMuc FROM DanhMuc";
                SqlCommand command = new SqlCommand(query, connection);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    DanhMuc danhMuc = new DanhMuc
                    {
                        MaDanhMuc = reader["MaDanhMuc"].ToString(),
                        TenDanhMuc = reader["TenDanhMuc"].ToString()
                    };
                    danhMucList.Add(danhMuc);
                }

                connection.Close();
            }

            Response response = new Response
            {
                StatusCode = danhMucList.Count > 0 ? 200 : 100,
                StatusMessage = danhMucList.Count > 0 ? "Data found" : "No data found",
                ListCategories = danhMucList
            };

            return response;
        }

        [HttpGet]
        [Route("GetCategoryById/{maDM}")]
        public ActionResult<DanhMuc> GetCategoryById(string maDM)
        {
            DanhMuc danhMuc = null;
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                string query = "SELECT MaDanhMuc, TenDanhMuc FROM DanhMuc WHERE MaDanhMuc = @MaDanhMuc";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaDanhMuc", maDM);

                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    danhMuc = new DanhMuc
                    {
                        MaDanhMuc = reader["MaDanhMuc"].ToString(),
                        TenDanhMuc = reader["TenDanhMuc"].ToString()
                    };
                }

                connection.Close();
            }

            return danhMuc != null ? Ok(danhMuc) : NotFound("Danh mục không tồn tại");
        }

        [HttpPost]
        [Route("AddCategory")]
        public ActionResult<Response> AddCategory(DanhMuc newCategory)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                string query = "INSERT INTO DanhMuc (MaDanhMuc, TenDanhMuc) VALUES (@MaDanhMuc, @TenDanhMuc)";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaDanhMuc", newCategory.MaDanhMuc);
                command.Parameters.AddWithValue("@TenDanhMuc", newCategory.TenDanhMuc);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    return new Response { StatusCode = 200, StatusMessage = "Thêm danh mục thành công" };
                }
                else
                {
                    return new Response { StatusCode = 100, StatusMessage = "Thêm danh mục thất bại" };
                }
            }
        }

        [HttpPut]
        [Route("UpdateCategory")]
        public ActionResult<Response> UpdateCategory(DanhMuc updatedCategory)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                string query = "UPDATE DanhMuc SET TenDanhMuc = @TenDanhMuc WHERE MaDanhMuc = @MaDanhMuc";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaDanhMuc", updatedCategory.MaDanhMuc);
                command.Parameters.AddWithValue("@TenDanhMuc", updatedCategory.TenDanhMuc);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    return new Response { StatusCode = 200, StatusMessage = "Cập nhật danh mục thành công" };
                }
                else
                {
                    return new Response { StatusCode = 100, StatusMessage = "Cập nhật danh mục thất bại" };
                }
            }
        }

        [HttpDelete]
        [Route("DeleteCategory/{maDM}")]
        public ActionResult<Response> DeleteCategory(string maDM)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                string query = "DELETE FROM DanhMuc WHERE MaDanhMuc = @MaDanhMuc";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaDanhMuc", maDM);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                if (rowsAffected > 0)
                {
                    return new Response { StatusCode = 200, StatusMessage = "Xóa danh mục thành công" };
                }
                else
                {
                    return new Response { StatusCode = 100, StatusMessage = "Xóa danh mục thất bại" };
                }
            }
        }
    }
}

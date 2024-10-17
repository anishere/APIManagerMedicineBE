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
                string query = "SELECT MaDanhMuc, TenDanhMuc FROM DanhMuc"; // Thay 'DanhMuc' bằng tên bảng của bạn
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
                ListCategories = danhMucList // Danh sách danh mục
            };

            return response;
        }
    }
}

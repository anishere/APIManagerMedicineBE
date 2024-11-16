using APIManagerMedicine.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace APIManagerMedicine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NhaCungCapController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public NhaCungCapController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllSuppliers")]
        public Response GetAllSuppliers()
        {
            List<NhaCungCap> lstSuppliers = new List<NhaCungCap>();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM NhaCungCap ORDER BY MaNCC", connection);
                DataTable dt = new DataTable();
                da.Fill(dt);

                Response response = new Response();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        NhaCungCap supplier = new NhaCungCap
                        {
                            MaNCC = Convert.ToString(dt.Rows[i]["MaNCC"]),
                            TenNCC = Convert.ToString(dt.Rows[i]["TenNCC"]),
                            DiaChi = Convert.ToString(dt.Rows[i]["DiaChi"]),
                            SDT = Convert.ToString(dt.Rows[i]["SDT"]),
                            Email = Convert.ToString(dt.Rows[i]["Email"])
                        };
                        lstSuppliers.Add(supplier);
                    }

                    response.StatusCode = 200;
                    response.StatusMessage = "Data found";
                    response.ListNhaCungCap = lstSuppliers;
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "No data found";
                    response.ListNhaCungCap = null;
                }

                return response;
            }
        }

        [HttpGet]
        [Route("GetSupplierById/{id}")]
        public ActionResult<Response> GetSupplierById(string id)
        {
            NhaCungCap supplier = null;
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM NhaCungCap WHERE MaNCC = @MaNCC", connection);
                da.SelectCommand.Parameters.AddWithValue("@MaNCC", id);

                DataTable dt = new DataTable();
                da.Fill(dt);

                Response response = new Response();
                if (dt.Rows.Count > 0)
                {
                    supplier = new NhaCungCap
                    {
                        MaNCC = Convert.ToString(dt.Rows[0]["MaNCC"]),
                        TenNCC = Convert.ToString(dt.Rows[0]["TenNCC"]),
                        DiaChi = Convert.ToString(dt.Rows[0]["DiaChi"]),
                        SDT = Convert.ToString(dt.Rows[0]["SDT"]),
                        Email = Convert.ToString(dt.Rows[0]["Email"])
                    };

                    response.StatusCode = 200;
                    response.StatusMessage = "Data found";
                    response.ListNhaCungCap = new List<NhaCungCap> { supplier };
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "No data found";
                    response.ListNhaCungCap = null;
                }

                return response;
            }
        }

        [HttpPost]
        [Route("AddSupplier")]
        public ActionResult<Response> AddSupplier([FromBody] NhaCungCap supplier)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                string query = "INSERT INTO NhaCungCap (MaNCC, TenNCC, DiaChi, SDT, Email) " +
                               "VALUES (@MaNCC, @TenNCC, @DiaChi, @SDT, @Email)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaNCC", supplier.MaNCC);
                command.Parameters.AddWithValue("@TenNCC", supplier.TenNCC);
                command.Parameters.AddWithValue("@DiaChi", supplier.DiaChi);
                command.Parameters.AddWithValue("@SDT", supplier.SDT);
                command.Parameters.AddWithValue("@Email", supplier.Email);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                Response response = new Response();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Supplier added successfully";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Failed to add supplier";
                }

                return response;
            }
        }

        [HttpPut]
        [Route("UpdateSupplier/{id}")]
        public ActionResult<Response> UpdateSupplier(string id, [FromBody] NhaCungCap supplier)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                string query = "UPDATE NhaCungCap SET TenNCC = @TenNCC, DiaChi = @DiaChi, SDT = @SDT, Email = @Email WHERE MaNCC = @MaNCC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaNCC", id);
                command.Parameters.AddWithValue("@TenNCC", supplier.TenNCC);
                command.Parameters.AddWithValue("@DiaChi", supplier.DiaChi);
                command.Parameters.AddWithValue("@SDT", supplier.SDT);
                command.Parameters.AddWithValue("@Email", supplier.Email);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                Response response = new Response();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Supplier updated successfully";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Failed to update supplier";
                }

                return response;
            }
        }

        [HttpDelete]
        [Route("DeleteSupplier/{id}")]
        public ActionResult<Response> DeleteSupplier(string id)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                string query = "DELETE FROM NhaCungCap WHERE MaNCC = @MaNCC";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaNCC", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                Response response = new Response();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Supplier deleted successfully";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Failed to delete supplier";
                }

                return response;
            }
        }
    }
}

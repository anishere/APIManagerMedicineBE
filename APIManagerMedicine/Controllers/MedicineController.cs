using APIManagerMedicine.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Numerics;

namespace APIManagerMedicine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicineController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public MedicineController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("ListMedicine")]
        public Response GetAllMedicines()
        {
            List<Medicine> lstmedicines = new List<Medicine>();
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                // Thực hiện truy vấn lấy tất cả thuốc
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM thuoc ORDER BY MaThuoc", connection);

                DataTable dt = new DataTable();
                da.Fill(dt);

                Response response = new Response();
                if (dt.Rows.Count > 0)
                {
                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        Medicine medicine = new Medicine
                        {
                            MaThuoc = Convert.ToString(dt.Rows[i]["MaThuoc"]),
                            TenThuoc = Convert.ToString(dt.Rows[i]["TenThuoc"]),
                            GiaBan = Convert.ToDecimal(dt.Rows[i]["GiaBan"]),
                            NgaySanXuat = Convert.ToDateTime(dt.Rows[i]["NgaySanXuat"]),
                            NgayHetHan = Convert.ToDateTime(dt.Rows[i]["NgayHetHan"]),
                            SoLuongThuocCon = Convert.ToInt32(dt.Rows[i]["SoLuongThuocCon"]),
                            CongDung = Convert.ToString(dt.Rows[i]["CongDung"]),
                            DVT = Convert.ToString(dt.Rows[i]["DVT"]),
                            HinhAnh = Convert.ToString(dt.Rows[i]["HinhAnh"]),
                            MaDanhMuc = Convert.ToString(dt.Rows[i]["MaDanhMuc"]),
                            Rowguid = Guid.Parse(dt.Rows[i]["rowguid"].ToString()),
                            KeDon = Convert.ToString(dt.Rows[i]["KeDon"]),
                            XuatXu = Convert.ToString(dt.Rows[i]["XuatXu"]), // Thêm thuộc tính XuatXu
                            KhuVucLuuTru = Convert.ToString(dt.Rows[i]["KhuVucLuuTru"]) // Thêm thuộc tính KhuVucLuuTru
                        };
                        lstmedicines.Add(medicine);
                    }

                    response.StatusCode = 200;
                    response.StatusMessage = "Data found";
                    response.ListMedicine = lstmedicines;
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "No data found";
                    response.ListMedicine = null;
                }

                return response;
            }
        }

        [HttpGet]
        [Route("GetMedicineById/{id}")]
        public ActionResult<Response> GetMedicineById(string id)
        {
            Medicine medicine = null;
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM thuoc WHERE MaThuoc = @MaThuoc", connection);
                da.SelectCommand.Parameters.AddWithValue("@MaThuoc", id);

                DataTable dt = new DataTable();
                da.Fill(dt);

                Response response = new Response();
                if (dt.Rows.Count > 0)
                {
                    medicine = new Medicine
                    {
                        MaThuoc = Convert.ToString(dt.Rows[0]["MaThuoc"]),
                        TenThuoc = Convert.ToString(dt.Rows[0]["TenThuoc"]),
                        GiaBan = Convert.ToDecimal(dt.Rows[0]["GiaBan"]),
                        NgaySanXuat = Convert.ToDateTime(dt.Rows[0]["NgaySanXuat"]),
                        NgayHetHan = Convert.ToDateTime(dt.Rows[0]["NgayHetHan"]),
                        SoLuongThuocCon = Convert.ToInt32(dt.Rows[0]["SoLuongThuocCon"]),
                        CongDung = Convert.ToString(dt.Rows[0]["CongDung"]),
                        DVT = Convert.ToString(dt.Rows[0]["DVT"]),
                        HinhAnh = Convert.ToString(dt.Rows[0]["HinhAnh"]),
                        MaDanhMuc = Convert.ToString(dt.Rows[0]["MaDanhMuc"]),
                        Rowguid = Guid.Parse(dt.Rows[0]["rowguid"].ToString()),
                        KeDon = Convert.ToString(dt.Rows[0]["KeDon"]),
                        XuatXu = Convert.ToString(dt.Rows[0]["XuatXu"]), // Thêm thuộc tính XuatXu
                        KhuVucLuuTru = Convert.ToString(dt.Rows[0]["KhuVucLuuTru"]) // Thêm thuộc tính KhuVucLuuTru
                    };

                    response.StatusCode = 200;
                    response.StatusMessage = "Data found";
                    response.ListMedicine = new List<Medicine> { medicine };
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "No data found";
                    response.ListMedicine = null;
                }

                return response;
            }
        }

        [HttpPost]
        [Route("AddMedicine")]
        public ActionResult<Response> AddMedicine([FromBody] Medicine medicine)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                string query = "INSERT INTO thuoc (MaThuoc, TenThuoc, GiaBan, NgaySanXuat, NgayHetHan, SoLuongThuocCon, CongDung, DVT, HinhAnh, MaDanhMuc, KeDon, XuatXu, KhuVucLuuTru) " + // Thêm XuatXu và KhuVucLuuTru vào câu truy vấn
                               "VALUES (@MaThuoc, @TenThuoc, @GiaBan, @NgaySanXuat, @NgayHetHan, @SoLuongThuocCon, @CongDung, @DVT, @HinhAnh, @MaDanhMuc, @KeDon, @XuatXu, @KhuVucLuuTru)";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaThuoc", medicine.MaThuoc);
                command.Parameters.AddWithValue("@TenThuoc", medicine.TenThuoc);
                command.Parameters.AddWithValue("@GiaBan", medicine.GiaBan);
                command.Parameters.AddWithValue("@NgaySanXuat", medicine.NgaySanXuat);
                command.Parameters.AddWithValue("@NgayHetHan", medicine.NgayHetHan);
                command.Parameters.AddWithValue("@SoLuongThuocCon", medicine.SoLuongThuocCon);
                command.Parameters.AddWithValue("@CongDung", medicine.CongDung);
                command.Parameters.AddWithValue("@DVT", medicine.DVT);
                command.Parameters.AddWithValue("@HinhAnh", medicine.HinhAnh);
                command.Parameters.AddWithValue("@MaDanhMuc", medicine.MaDanhMuc);
                command.Parameters.AddWithValue("@KeDon", medicine.KeDon);
                command.Parameters.AddWithValue("@XuatXu", medicine.XuatXu); // Thêm thuộc tính XuatXu
                command.Parameters.AddWithValue("@KhuVucLuuTru", medicine.KhuVucLuuTru); // Thêm thuộc tính KhuVucLuuTru

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                Response response = new Response();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Medicine added successfully";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Failed to add medicine";
                }

                return response;
            }
        }

        [HttpPost]
        [Route("UploadImage")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { message = "File not selected" });
            }

            try
            {
                // Đường dẫn tới thư mục lưu trữ hình ảnh trong thư mục public
                var folderPath = Path.Combine("D:", "QLThuocSetup", "QLThuoc", "QLThuocApp", "public", "imgStore");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                // Tạo tên file duy nhất (để tránh trùng lặp)
                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
                var filePath = Path.Combine(folderPath, fileName);

                // Lưu file vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Trả về đường dẫn của file đã lưu
                var imageUrl = $"/imgStore/{fileName}"; // Đường dẫn từ client
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "File upload failed", error = ex.Message });
            }
        }

        [HttpPut]
        [Route("UpdateMedicine/{id}")]
        public ActionResult<Response> UpdateMedicine(string id, [FromBody] Medicine medicine)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                // Bước 1: Lấy thông tin thuốc hiện tại từ CSDL
                SqlDataAdapter da = new SqlDataAdapter("SELECT HinhAnh FROM thuoc WHERE MaThuoc = @MaThuoc", connection);
                da.SelectCommand.Parameters.AddWithValue("@MaThuoc", id);
                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    return NotFound(new { message = "Medicine not found" });
                }

                // Lấy tên ảnh cũ
                string oldImage = Convert.ToString(dt.Rows[0]["HinhAnh"]);

                // Bước 2: Kiểm tra nếu ảnh cũ khác ảnh mới và tiến hành xóa
                if (!string.IsNullOrEmpty(oldImage) && oldImage != medicine.HinhAnh)
                {
                    var oldImagePath = Path.Combine("D:", "QLThuocSetup", "QLThuoc", "QLThuocApp", "public", "imgStore", Path.GetFileName(oldImage));
                    if (System.IO.File.Exists(oldImagePath))
                    {
                        System.IO.File.Delete(oldImagePath);
                    }
                }

                // Bước 3: Cập nhật thông tin thuốc và ảnh mới
                string query = "UPDATE thuoc SET TenThuoc = @TenThuoc, GiaBan = @GiaBan, NgaySanXuat = @NgaySanXuat, NgayHetHan = @NgayHetHan, " +
                               "SoLuongThuocCon = @SoLuongThuocCon, CongDung = @CongDung, DVT = @DVT, HinhAnh = @HinhAnh, MaDanhMuc = @MaDanhMuc, KeDon = @KeDon, " +
                               "XuatXu = @XuatXu, KhuVucLuuTru = @KhuVucLuuTru WHERE MaThuoc = @MaThuoc";

                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaThuoc", id);
                command.Parameters.AddWithValue("@TenThuoc", medicine.TenThuoc);
                command.Parameters.AddWithValue("@GiaBan", medicine.GiaBan);
                command.Parameters.AddWithValue("@NgaySanXuat", medicine.NgaySanXuat);
                command.Parameters.AddWithValue("@NgayHetHan", medicine.NgayHetHan);
                command.Parameters.AddWithValue("@SoLuongThuocCon", medicine.SoLuongThuocCon);
                command.Parameters.AddWithValue("@CongDung", medicine.CongDung);
                command.Parameters.AddWithValue("@DVT", medicine.DVT);
                command.Parameters.AddWithValue("@HinhAnh", medicine.HinhAnh); // Đường dẫn ảnh mới
                command.Parameters.AddWithValue("@MaDanhMuc", medicine.MaDanhMuc);
                command.Parameters.AddWithValue("@KeDon", medicine.KeDon);
                command.Parameters.AddWithValue("@XuatXu", medicine.XuatXu);
                command.Parameters.AddWithValue("@KhuVucLuuTru", medicine.KhuVucLuuTru);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                Response response = new Response();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Medicine updated successfully";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Failed to update medicine";
                }

                return response;
            }
        }

        [HttpDelete]
        [Route("DeleteMedicine/{id}")]
        public ActionResult<Response> DeleteMedicine(string id)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                // Bước 1: Lấy thông tin hình ảnh từ thuốc
                SqlDataAdapter da = new SqlDataAdapter("SELECT HinhAnh FROM thuoc WHERE MaThuoc = @MaThuoc", connection);
                da.SelectCommand.Parameters.AddWithValue("@MaThuoc", id);

                DataTable dt = new DataTable();
                da.Fill(dt);

                if (dt.Rows.Count == 0)
                {
                    return NotFound(new { message = "Medicine not found" });
                }

                // Bước 2: Xóa hình ảnh nếu tồn tại
                string imagePath = Convert.ToString(dt.Rows[0]["HinhAnh"]);
                if (!string.IsNullOrEmpty(imagePath))
                {
                    var fullImagePath = Path.Combine("D:", "QLThuocSetup", "QLThuoc", "QLThuocApp", "public", "imgStore", Path.GetFileName(imagePath));
                    if (System.IO.File.Exists(fullImagePath))
                    {
                        System.IO.File.Delete(fullImagePath);
                    }
                }

                // Bước 3: Xóa thuốc từ cơ sở dữ liệu
                string query = "DELETE FROM thuoc WHERE MaThuoc = @MaThuoc";
                SqlCommand command = new SqlCommand(query, connection);
                command.Parameters.AddWithValue("@MaThuoc", id);

                connection.Open();
                int rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                Response response = new Response();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200;
                    response.StatusMessage = "Medicine and associated image deleted successfully";
                }
                else
                {
                    response.StatusCode = 100;
                    response.StatusMessage = "Failed to delete medicine";
                }

                return response;
            }
        }
    }
}

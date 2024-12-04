using APIManagerMedicine.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace APIManagerMedicine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public AccountController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("ListUser")]
        public ActionResult<Response> GetAllUsers()
        {
            List<Account> lstUsers = new List<Account>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            // Thực hiện truy vấn lấy tất cả người dùng
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Account ORDER BY UserID", connection);

            DataTable dt = new DataTable();
            da.Fill(dt);

            Response response = new Response();
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    Account user = new Account
                    {
                        UserID = Convert.ToInt32(dt.Rows[i]["UserID"]),
                        UserName = Convert.ToString(dt.Rows[i]["UserName"]),
                        Password = Convert.ToString(dt.Rows[i]["Password"]),
                        Avatar = Convert.ToString(dt.Rows[i]["Avatar"]),
                        UserType = Convert.ToString(dt.Rows[i]["UserType"]),
                        ActiveStatus = Convert.ToInt32(dt.Rows[i]["ActiveStatus"]),
                        VisibleFunction = Convert.ToString(dt.Rows[i]["VisibleFunction"]),
                        StartTime = dt.Rows[i]["StartTime"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(dt.Rows[i]["StartTime"].ToString()),
                        EndTime = dt.Rows[i]["EndTime"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(dt.Rows[i]["EndTime"].ToString()),
                        WorkDayofWeek = Convert.ToString(dt.Rows[i]["WorkDayofWeek"]),
                        ActivationDate = Convert.ToDateTime(dt.Rows[i]["ActivationDate"]),
                        DeactivationDate = Convert.ToDateTime(dt.Rows[i]["DeactivationDate"]),
                        WorkShedule = Convert.ToInt32(dt.Rows[i]["WorkShedule"]),
                        ActiveShedule = Convert.ToInt32(dt.Rows[i]["ActiveShedule"]),
                        MaNV = dt.Rows[i]["MaNV"] == DBNull.Value ? null : Convert.ToString(dt.Rows[i]["MaNV"]),
                        MaCN = dt.Rows[i]["MaCN"] == DBNull.Value ? null : Convert.ToString(dt.Rows[i]["MaCN"])
                    };
                    lstUsers.Add(user);
                }

                response.StatusCode = 200;
                response.StatusMessage = "Data found";
                response.User = lstUsers;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "No data found";
                response.User = null;
            }

            return response;
        }

        [HttpGet]
        [Route("GetUserByID")]
        public ActionResult<Response> GetUserByID(int userID)
        {
            Account user = null;
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [Account] WHERE UserID = @UserID", connection);
            da.SelectCommand.Parameters.AddWithValue("@UserID", userID);

            DataTable dt = new DataTable();
            da.Fill(dt);

            Response response = new Response();
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                user = new Account
                {
                    UserID = Convert.ToInt32(row["UserID"]),
                    UserName = Convert.ToString(row["UserName"]),
                    Password = Convert.ToString(row["Password"]),
                    Avatar = Convert.ToString(row["Avatar"]),
                    UserType = Convert.ToString(row["UserType"]),
                    ActiveStatus = Convert.ToInt32(row["ActiveStatus"]),
                    VisibleFunction = Convert.ToString(row["VisibleFunction"]),
                    StartTime = row["StartTime"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(row["StartTime"].ToString()),
                    EndTime = row["EndTime"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(row["EndTime"].ToString()),
                    WorkDayofWeek = Convert.ToString(row["WorkDayofWeek"]),
                    ActivationDate = Convert.ToDateTime(row["ActivationDate"]),
                    DeactivationDate = Convert.ToDateTime(row["DeactivationDate"]),
                    WorkShedule = Convert.ToInt32(row["WorkShedule"]),
                    ActiveShedule = Convert.ToInt32(row["ActiveShedule"]),
                    MaNV = row["MaNV"] == DBNull.Value ? null : Convert.ToString(row["MaNV"]),
                    MaCN = row["MaCN"] == DBNull.Value ? null : Convert.ToString(row["MaCN"])
                };

                response.StatusCode = 200;
                response.StatusMessage = "Data found";
                response.User = new List<Account> { user }; // Wrap user in a list
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "No data found";
                response.User = null;
            }

            return response;
        }

        //tới login
        [HttpPost]
        [Route("Login")]
        public ActionResult<Response> Login([FromBody] APIManagerMedicine.Model.LoginRequest loginRequest)
        {
            Account user = null;
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                // Kiểm tra thông tin đăng nhập
                SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM [Account] WHERE UserName = @UserName AND Password = @Password", connection);
                da.SelectCommand.Parameters.AddWithValue("@UserName", loginRequest.UserName);
                da.SelectCommand.Parameters.AddWithValue("@Password", loginRequest.Password);

                DataTable dt = new DataTable();
                da.Fill(dt);

                Response response = new Response();
                if (dt.Rows.Count > 0)
                {
                    DataRow row = dt.Rows[0];
                    user = new Account
                    {
                        UserID = Convert.ToInt32(row["UserID"]),
                        UserName = Convert.ToString(row["UserName"]),
                        Password = Convert.ToString(row["Password"]),
                        Avatar = Convert.ToString(row["Avatar"]),
                        UserType = Convert.ToString(row["UserType"]),
                        ActiveStatus = Convert.ToInt32(row["ActiveStatus"]),
                        VisibleFunction = Convert.ToString(row["VisibleFunction"]),
                        StartTime = row["StartTime"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(row["StartTime"].ToString()),
                        EndTime = row["EndTime"] == DBNull.Value ? (TimeSpan?)null : TimeSpan.Parse(row["EndTime"].ToString()),
                        WorkDayofWeek = Convert.ToString(row["WorkDayofWeek"]),
                        ActivationDate = Convert.ToDateTime(row["ActivationDate"]),
                        DeactivationDate = Convert.ToDateTime(row["DeactivationDate"]),
                        WorkShedule = Convert.ToInt32(row["WorkShedule"]),
                        ActiveShedule = Convert.ToInt32(row["ActiveShedule"]),
                        MaNV = row["MaNV"] == DBNull.Value ? null : Convert.ToString(row["MaNV"]),
                        MaCN = row["MaCN"] == DBNull.Value ? null : Convert.ToString(row["MaCN"])
                    };

                    response.StatusCode = 200;
                    response.StatusMessage = "Login successful";
                    response.User = new List<Account> { user }; // Trả về thông tin người dùng đã đăng nhập
                }
                else
                {
                    response.StatusCode = 401; // Unauthorized
                    response.StatusMessage = "Invalid username or password";
                    response.User = null;
                }

                return response;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    StatusCode = 500,
                    StatusMessage = $"Internal server error: {ex.Message}"
                };
            }
            finally
            {
                connection.Close();
            }
        }

        [HttpPost]
        [Route("CreateAccount")]
        public ActionResult<Response> CreateAccount([FromBody] Account newAccount)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());
            Response response = new Response();

            // Validate StartTime
            if (newAccount.StartTime != null && !TimeSpan.TryParse(newAccount.StartTime.ToString(), out var startTime))
            {
                response.StatusCode = 400;
                response.StatusMessage = "Invalid start time format";
                return response;
            }

            // Validate EndTime
            if (newAccount.EndTime != null && !TimeSpan.TryParse(newAccount.EndTime.ToString(), out var endTime))
            {
                response.StatusCode = 400;
                response.StatusMessage = "Invalid end time format";
                return response;
            }

            // Validate input data
            if (newAccount == null)
            {
                return new Response
                {
                    StatusCode = 400,
                    StatusMessage = "New account data is required."
                };
            }

            try
            {
                connection.Open();

                // Check if username already exists
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(1) FROM [Account] WHERE UserName = @UserName", connection);
                checkCmd.Parameters.AddWithValue("@UserName", newAccount.UserName ?? (object)DBNull.Value);

                int userCount = (int)checkCmd.ExecuteScalar();
                if (userCount > 0)
                {
                    response.StatusCode = 400; // Username already exists
                    response.StatusMessage = "Username already exists";
                    return response;
                }

                // Insert new account into database
                SqlCommand insertCmd = new SqlCommand("INSERT INTO [Account] (UserName, Password, Avatar, UserType, ActiveStatus, VisibleFunction, StartTime, EndTime, WorkDayofWeek, ActivationDate, DeactivationDate, WorkShedule, ActiveShedule, MaNV, MaCN) VALUES (@UserName, @Password, @Avatar, @UserType, @ActiveStatus, @VisibleFunction, @StartTime, @EndTime, @WorkDayofWeek, @ActivationDate, @DeactivationDate, @WorkShedule, @ActiveShedule, @MaNV, @MaCN)", connection);

                insertCmd.Parameters.AddWithValue("@UserName", newAccount.UserName ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Password", newAccount.Password ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@Avatar", newAccount.Avatar ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@UserType", newAccount.UserType ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ActiveStatus", newAccount.ActiveStatus);
                insertCmd.Parameters.AddWithValue("@VisibleFunction", newAccount.VisibleFunction ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@StartTime", newAccount.StartTime.HasValue ? (object)newAccount.StartTime.Value : DBNull.Value);
                insertCmd.Parameters.AddWithValue("@EndTime", newAccount.EndTime.HasValue ? (object)newAccount.EndTime.Value : DBNull.Value);
                insertCmd.Parameters.AddWithValue("@WorkDayofWeek", newAccount.WorkDayofWeek ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@ActivationDate", newAccount.ActivationDate);
                insertCmd.Parameters.AddWithValue("@DeactivationDate", newAccount.DeactivationDate);
                insertCmd.Parameters.AddWithValue("@WorkShedule", newAccount.WorkShedule);
                insertCmd.Parameters.AddWithValue("@ActiveShedule", newAccount.ActiveShedule);
                insertCmd.Parameters.AddWithValue("@MaNV", newAccount.MaNV ?? (object)DBNull.Value);
                insertCmd.Parameters.AddWithValue("@MaCN", newAccount.MaCN ?? (object)DBNull.Value);

                int rowsAffected = insertCmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200; // Created
                    response.StatusMessage = "Account created successfully";
                }
                else
                {
                    response.StatusCode = 500; // Internal Server Error
                    response.StatusMessage = "Failed to create account";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500;
                response.StatusMessage = $"Internal server error: {ex.Message}";
            }
            finally
            {
                connection.Close();
            }

            return response;
        }

        [HttpPut]
        [Route("ChangePassword")]
        public ActionResult<Response> ChangePassword(int userID, [FromBody] ChangePasswordRequest passwordRequest)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            try
            {
                connection.Open();

                // Kiểm tra người dùng có tồn tại
                SqlCommand checkCmd = new SqlCommand("SELECT Password FROM [Account] WHERE UserID = @UserID", connection);
                checkCmd.Parameters.AddWithValue("@UserID", userID);
                string currentPassword = checkCmd.ExecuteScalar()?.ToString();

                Response response = new Response();
                if (currentPassword == null)
                {
                    response.StatusCode = 404; // Not Found
                    response.StatusMessage = "User not found";
                    return response;
                }

                // Kiểm tra mật khẩu cũ có khớp
                if (currentPassword != passwordRequest.OldPassword)
                {
                    response.StatusCode = 400; // Bad Request
                    response.StatusMessage = "Incorrect old password";
                    return response;
                }

                // Cập nhật mật khẩu mới
                SqlCommand cmd = new SqlCommand("UPDATE [Account] SET Password = @NewPassword WHERE UserID = @UserID", connection);
                cmd.Parameters.AddWithValue("@NewPassword", passwordRequest.NewPassword);
                cmd.Parameters.AddWithValue("@UserID", userID);

                int rowsAffected = cmd.ExecuteNonQuery();
                if (rowsAffected > 0)
                {
                    response.StatusCode = 200; // OK
                    response.StatusMessage = "Password changed successfully";
                }
                else
                {
                    response.StatusCode = 500; // Internal Server Error
                    response.StatusMessage = "Failed to change password";
                }

                return response;
            }
            catch (Exception ex)
            {
                return new Response
                {
                    StatusCode = 500,
                    StatusMessage = $"Internal server error: {ex.Message}"
                };
            }
            finally
            {
                connection.Close();
            }
        }

        [HttpPut]
        [Route("UpdateAccount")]
        public ActionResult<Response> UpdateAccount([FromQuery] int userID, [FromBody] Account updatedAccount)
        {
            using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString()))
            {
                Response response = new Response();

                try
                {
                    connection.Open();

                    // Bước 1: Lấy thông tin avatar hiện tại của tài khoản từ CSDL
                    SqlDataAdapter da = new SqlDataAdapter("SELECT Avatar FROM Account WHERE UserID = @UserID", connection);
                    da.SelectCommand.Parameters.AddWithValue("@UserID", userID);
                    DataTable dt = new DataTable();
                    da.Fill(dt);

                    if (dt.Rows.Count == 0)
                    {
                        return NotFound(new { message = "Account not found" });
                    }

                    // Lấy tên ảnh avatar cũ
                    string oldAvatar = Convert.ToString(dt.Rows[0]["Avatar"]);

                    // Bước 2: Kiểm tra nếu ảnh cũ khác ảnh mới và tiến hành xóa
                    if (!string.IsNullOrEmpty(oldAvatar) && oldAvatar != updatedAccount.Avatar)
                    {
                        var oldAvatarPath = Path.Combine("D:", "QLThuocSetup", "QLThuoc", "QLThuocApp", "public", "avatarStore", Path.GetFileName(oldAvatar));
                        if (System.IO.File.Exists(oldAvatarPath))
                        {
                            System.IO.File.Delete(oldAvatarPath);
                        }
                    }

                    // Bước 3: Cập nhật thông tin tài khoản và avatar mới
                    string query = "UPDATE Account SET " +
                                   "UserName = @UserName, Password = @Password, Avatar = @Avatar, UserType = @UserType, " +
                                   "ActiveStatus = @ActiveStatus, VisibleFunction = @VisibleFunction, StartTime = @StartTime, " +
                                   "EndTime = @EndTime, WorkDayofWeek = @WorkDayofWeek, ActivationDate = @ActivationDate, " +
                                   "DeactivationDate = @DeactivationDate, WorkShedule = @WorkShedule, ActiveShedule = @ActiveShedule, " +
                                   "MaNV = @MaNV, MaCN = @MaCN " +
                                   "WHERE UserID = @UserID";

                    SqlCommand updateCmd = new SqlCommand(query, connection);
                    updateCmd.Parameters.AddWithValue("@UserID", userID);
                    updateCmd.Parameters.AddWithValue("@UserName", updatedAccount.UserName ?? (object)DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@Password", updatedAccount.Password ?? (object)DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@Avatar", updatedAccount.Avatar ?? (object)DBNull.Value); // Đường dẫn ảnh mới
                    updateCmd.Parameters.AddWithValue("@UserType", updatedAccount.UserType ?? (object)DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@ActiveStatus", updatedAccount.ActiveStatus);
                    updateCmd.Parameters.AddWithValue("@VisibleFunction", updatedAccount.VisibleFunction ?? (object)DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@StartTime", updatedAccount.StartTime.HasValue ? (object)updatedAccount.StartTime.Value : DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@EndTime", updatedAccount.EndTime.HasValue ? (object)updatedAccount.EndTime.Value : DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@WorkDayofWeek", updatedAccount.WorkDayofWeek ?? (object)DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@ActivationDate", updatedAccount.ActivationDate);
                    updateCmd.Parameters.AddWithValue("@DeactivationDate", updatedAccount.DeactivationDate);
                    updateCmd.Parameters.AddWithValue("@WorkShedule", updatedAccount.WorkShedule);
                    updateCmd.Parameters.AddWithValue("@ActiveShedule", updatedAccount.ActiveShedule);
                    updateCmd.Parameters.AddWithValue("@MaNV", updatedAccount.MaNV ?? (object)DBNull.Value);
                    updateCmd.Parameters.AddWithValue("@MaCN", updatedAccount.MaCN ?? (object)DBNull.Value);

                    int rowsAffected = updateCmd.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        response.StatusCode = 200; // OK
                        response.StatusMessage = "Account updated successfully";
                    }
                    else
                    {
                        response.StatusCode = 500; // Internal Server Error
                        response.StatusMessage = "Failed to update account";
                    }
                }
                catch (Exception ex)
                {
                    response.StatusCode = 500;
                    response.StatusMessage = $"Internal server error: {ex.Message}";
                }
                finally
                {
                    connection.Close();
                }

                return response;
            }
        }

        [HttpDelete]
        [Route("DeleteUser")]
        public ActionResult<Response> DeleteUser(int userID)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());
            Response response = new Response();

            try
            {
                connection.Open();

                // Kiểm tra xem User có tồn tại không
                SqlCommand checkCmd = new SqlCommand("SELECT COUNT(1) FROM [Account] WHERE UserID = @UserID", connection);
                checkCmd.Parameters.AddWithValue("@UserID", userID);
                int userCount = (int)checkCmd.ExecuteScalar();

                if (userCount == 0)
                {
                    response.StatusCode = 404; // Not Found
                    response.StatusMessage = "User not found";
                    return response;
                }

                // Bước 2: Lấy thông tin ảnh đại diện của user (nếu có)
                SqlDataAdapter da = new SqlDataAdapter("SELECT Avatar FROM [Account] WHERE UserID = @UserID", connection);
                da.SelectCommand.Parameters.AddWithValue("@UserID", userID);

                DataTable dt = new DataTable();
                da.Fill(dt);

                // Nếu có ảnh đại diện, xóa ảnh
                if (dt.Rows.Count > 0)
                {
                    string avatarPath = Convert.ToString(dt.Rows[0]["Avatar"]);
                    if (!string.IsNullOrEmpty(avatarPath))
                    {
                        var fullImagePath = Path.Combine("D:", "QLThuocSetup", "QLThuoc", "QLThuocApp", "public", "avatarStore", Path.GetFileName(avatarPath));
                        if (System.IO.File.Exists(fullImagePath))
                        {
                            System.IO.File.Delete(fullImagePath);
                        }
                    }
                }

                // Thực hiện xóa User
                SqlCommand deleteCmd = new SqlCommand("DELETE FROM [Account] WHERE UserID = @UserID", connection);
                deleteCmd.Parameters.AddWithValue("@UserID", userID);
                int rowsAffected = deleteCmd.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    response.StatusCode = 200; // OK
                    response.StatusMessage = "User deleted successfully";
                }
                else
                {
                    response.StatusCode = 500; // Internal Server Error
                    response.StatusMessage = "Failed to delete user";
                }
            }
            catch (Exception ex)
            {
                response.StatusCode = 500; // Internal Server Error
                response.StatusMessage = $"Internal server error: {ex.Message}";
            }
            finally
            {
                connection.Close();
            }

            return response;
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
                // "D:", "QLThuoc", "QLThuocApp", "public", "avatarStore"
                var folderPath = Path.Combine("D:", "QLThuocSetup", "QLThuoc", "QLThuocApp", "public", "avatarStore");

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
                var imageUrl = $"/avatarStore/{fileName}"; // Đường dẫn từ client
                return Ok(new { imageUrl });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "File upload failed", error = ex.Message });
            }
        }
    }
}

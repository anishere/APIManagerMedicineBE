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
        public ActionResult<Response> GetUsersByPage(int page = 1, int pageSize = 20)
        {
            List<Account> lstUsers = new List<Account>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            int startIndex = (page - 1) * pageSize;

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM Account ORDER BY UserID OFFSET @StartIndex ROWS FETCH NEXT @PageSize ROWS ONLY", connection);
            da.SelectCommand.Parameters.AddWithValue("@StartIndex", startIndex);
            da.SelectCommand.Parameters.AddWithValue("@PageSize", pageSize);

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

    }
}

using APIManagerMedicine.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;

namespace APIManagerMedicine.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotePersonalController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public NotePersonalController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        [Route("GetAllNotes")]
        public ActionResult<Response> GetAllNotes(string maNV)
        {
            List<NotePersonal> notes = new List<NotePersonal>();
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());

            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ghichu WHERE MaNV = @MaNV ORDER BY idGhiChu", connection);
            da.SelectCommand.Parameters.AddWithValue("@MaNV", maNV);

            DataTable dt = new DataTable();
            da.Fill(dt);

            Response response = new Response();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow row in dt.Rows)
                {
                    notes.Add(new NotePersonal
                    {
                        idGhiChu = Convert.ToInt32(row["idGhiChu"]),
                        MaNV = row["MaNV"]?.ToString(),
                        tieuDe = row["tieuDe"]?.ToString(),
                        noiDung = row["noiDung"]?.ToString(),
                        ngayTao = row["ngayTao"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ngayTao"]),
                        hanChot = row["hanChot"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["hanChot"]),
                        trangThai = row["trangThai"]?.ToString()
                    });
                }

                response.StatusCode = 200;
                response.StatusMessage = "Data found";
                response.ListNotePersonal = notes;
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "No data found";
                response.ListNotePersonal = null;
            }

            return response;
        }

        [HttpGet]
        [Route("GetNoteByID")]
        public ActionResult<Response> GetNoteByID(int id)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());
            SqlDataAdapter da = new SqlDataAdapter("SELECT * FROM ghichu WHERE idGhiChu = @idGhiChu", connection);
            da.SelectCommand.Parameters.AddWithValue("@idGhiChu", id);

            DataTable dt = new DataTable();
            da.Fill(dt);

            Response response = new Response();
            if (dt.Rows.Count > 0)
            {
                DataRow row = dt.Rows[0];
                NotePersonal note = new NotePersonal
                {
                    idGhiChu = Convert.ToInt32(row["idGhiChu"]),
                    MaNV = row["MaNV"]?.ToString(),
                    tieuDe = row["tieuDe"]?.ToString(),
                    noiDung = row["noiDung"]?.ToString(),
                    ngayTao = row["ngayTao"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["ngayTao"]),
                    hanChot = row["hanChot"] == DBNull.Value ? (DateTime?)null : Convert.ToDateTime(row["hanChot"]),
                    trangThai = row["trangThai"]?.ToString()
                };

                response.StatusCode = 200;
                response.StatusMessage = "Data found";
                response.ListNotePersonal = new List<NotePersonal> { note };
            }
            else
            {
                response.StatusCode = 100;
                response.StatusMessage = "No data found";
                response.ListNotePersonal = null;
            }

            return response;
        }

        [HttpPost]
        [Route("CreateNote")]
        public ActionResult<Response> CreateNote([FromBody] NotePersonal note)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());
            Response response = new Response();

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO ghichu (MaNV, tieuDe, noiDung, ngayTao, hanChot, trangThai) " +
                                                "VALUES (@MaNV, @tieuDe, @noiDung, @ngayTao, @hanChot, @trangThai)", connection);
                cmd.Parameters.AddWithValue("@MaNV", note.MaNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@tieuDe", note.tieuDe ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@noiDung", note.noiDung ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ngayTao", note.ngayTao ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@hanChot", note.hanChot ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@trangThai", note.trangThai ?? (object)DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                response.StatusCode = rowsAffected > 0 ? 200 : 500;
                response.StatusMessage = rowsAffected > 0 ? "Note created successfully" : "Failed to create note";
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
        [Route("UpdateNote")]
        public ActionResult<Response> UpdateNote(int id, [FromBody] NotePersonal note)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());
            Response response = new Response();

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("UPDATE ghichu SET MaNV = @MaNV, tieuDe = @tieuDe, noiDung = @noiDung, " +
                                                "ngayTao = @ngayTao, hanChot = @hanChot, trangThai = @trangThai " +
                                                "WHERE idGhiChu = @idGhiChu", connection);
                cmd.Parameters.AddWithValue("@idGhiChu", id);
                cmd.Parameters.AddWithValue("@MaNV", note.MaNV ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@tieuDe", note.tieuDe ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@noiDung", note.noiDung ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@ngayTao", note.ngayTao ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@hanChot", note.hanChot ?? (object)DBNull.Value);
                cmd.Parameters.AddWithValue("@trangThai", note.trangThai ?? (object)DBNull.Value);

                int rowsAffected = cmd.ExecuteNonQuery();
                response.StatusCode = rowsAffected > 0 ? 200 : 500;
                response.StatusMessage = rowsAffected > 0 ? "Note updated successfully" : "Failed to update note";
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

        [HttpDelete]
        [Route("DeleteNote")]
        public ActionResult<Response> DeleteNote(int id)
        {
            SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("ManagerMedicineDB").ToString());
            Response response = new Response();

            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand("DELETE FROM ghichu WHERE idGhiChu = @idGhiChu", connection);
                cmd.Parameters.AddWithValue("@idGhiChu", id);

                int rowsAffected = cmd.ExecuteNonQuery();
                response.StatusCode = rowsAffected > 0 ? 200 : 500;
                response.StatusMessage = rowsAffected > 0 ? "Note deleted successfully" : "Failed to delete note";
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
}

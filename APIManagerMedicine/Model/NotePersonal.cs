namespace APIManagerMedicine.Model
{
    public class NotePersonal
    {
        public int idGhiChu { get; set; }

        public string? MaNV { get; set; }
        public string? tieuDe {  get; set; }

        public string? noiDung {  get; set; }

        public DateTime? ngayTao { get; set; }
        public DateTime? hanChot { get; set; }
        public string? trangThai { get; set; }
    }
}

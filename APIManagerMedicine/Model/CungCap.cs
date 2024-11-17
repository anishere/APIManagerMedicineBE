namespace APIManagerMedicine.Model
{
    public class CungCap
    {
        public string? IDCungCap {  get; set; }
        public string? MaNV { get; set; }
        public string? MaNCC { get; set; }
        public string? MaThuoc { get; set; }

        public DateTime? NgayCungCap { get; set; }
        public int? SoLuongThuocNhap { get; set; }
        public string? MaCN { get; set; }

        public decimal? GiaNhap { get; set; }

    }
}

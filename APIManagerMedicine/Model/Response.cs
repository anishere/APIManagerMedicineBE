using APIManagerMedicine.Controllers;

namespace APIManagerMedicine.Model
{
    public class Response
    {
        public int StatusCode { get; set; }

        public string? StatusMessage { get; set; }
        public string? MaHD { get; set; }

        public List<Medicine>? ListMedicine {  get; set; } 

        public List<Account>? User { get; set; }

        public List<DanhMuc> ListCategories { get; set; }

        public List<NhanVien> ListNhanVien { get; set; }

        public List<KhachHang> ListKhachHang { get; set; }

        public List<HoaDon> ListHoaDon { get; set; }

        public List<ThuocTrongHD> ListThuocTrongHD { get; set; }

        // tới login
    }
}

namespace APIManagerMedicine.Model
{
    public class NhanVien
    {
        public string MaNV { get; set; } // Mã nhân viên
        public string? TenNV { get; set; } // Tên nhân viên
        public string? Gt { get; set; } // Giới tính
        public DateTime? NgaySinh { get; set; } // Ngày sinh
        public string? SDT { get; set; } // Số điện thoại
        public decimal? Luong { get; set; } // Lương
        public string? MaCN { get; set; } // Mã chi nhánh
        public Guid RowGuid { get; set; } // GUID (ROWGUIDCOL)
    }
}

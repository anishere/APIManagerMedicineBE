using System;

namespace APIManagerMedicine.Model
{
    public class KhachHang
    {
        public string MaKH { get; set; }           // Mã khách hàng (không null)
        public string? TenKH { get; set; }         // Tên khách hàng (có thể null)
        public string? SDT { get; set; }           // Số điện thoại (có thể null)
        public string? GT { get; set; }            // Giới tính (có thể null)
        public string? MaCN { get; set; }          // Mã chi nhánh (có thể null)
        public Guid RowGuid { get; set; }          // GUID duy nhất, mặc định là newsequentialid()

        // Constructor mặc định
        public KhachHang()
        {
            RowGuid = Guid.NewGuid();              // Tạo GUID ngẫu nhiên nếu không cung cấp
        }
    }
}

using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace QuanLyCuaHangCauLong.Data
{
    public class ThongKeKhachHang
    {
        private readonly string chuoiKetNoi;

        public ThongKeKhachHang(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }
        //Lấy thống kê tổng quan cho 4 card
        public ThongKeTongQuanKhachHang LayThongKeChoCacTheKhachHang()
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();

                    // Tổng số khách hàng
                    string truyVanTongKhachHhang = @"SELECT COUNT(*) FROM KhachHang";

                    // Số khách hàng có tài khoản (bắt đầu bằng KH)
                    string truyVanKhachHangCoTaiKhoan = @"SELECT COUNT(*) FROM KhachHang WHERE MaKH LIKE 'KH%'";

                    // Số khách hàng lẻ (bắt đầu bằng KL)
                    string truyVanKhachHangLe = @"SELECT COUNT(*) FROM KhachHang WHERE MaKH LIKE 'KL%'";

                    // Số khách hàng đang hoạt động (trạng thái = 1)
                    string truyVanKhachHangConHoatDong = @"SELECT COUNT(*) FROM KhachHang WHERE TrangThai = 1";

                    // Số khách hàng ngừng hoạt động (trạng thái = 0)
                    string truyVanKhachHangNgungHoatDong = @"SELECT COUNT(*) FROM KhachHang WHERE TrangThai = 0";

                    var thongKe = new ThongKeTongQuanKhachHang();

                    // Lấy tổng số khách hàng
                    using (SqlCommand cmd = new SqlCommand(truyVanTongKhachHhang, ketNoi))
                    {;
                        thongKe.TongSoKhachHang = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy tổng số khách hàng có tài khoản
                    using (SqlCommand cmd = new SqlCommand(truyVanKhachHangCoTaiKhoan, ketNoi))
                    {
                        thongKe.KhachHangCoTaiKhaon = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy tổng số khách hàng lẻ
                    using (SqlCommand cmd = new SqlCommand(truyVanKhachHangLe, ketNoi))
                    {
                        thongKe.KhachHangLe = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy tổng số khách hàng còn giao dịch
                    using (SqlCommand cmd = new SqlCommand(truyVanKhachHangConHoatDong, ketNoi))
                    {
                        thongKe.KhachHangConGiaoDich = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy tổng số khách hàng ngưng giao dịch
                    using (SqlCommand cmd = new SqlCommand(truyVanKhachHangNgungHoatDong, ketNoi))
                    {
                        thongKe.KhachHangNgungGiaoDich = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    return thongKe;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tổng quan: {ex.Message}", ex);
            }
        }

        public DataTable LayDanhSachKhachHangTheoTieuChi(string loaiKhachHang, string trangThaiGiaoDich, int diemToiThieu)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();

                    string truyVan = @"
                                       SELECT 
                                            kh.NgayTao,
                                            kh.TenKH,
                                            CASE 
                                                WHEN kh.TenKH LIKE N'%lẻ%' THEN N'Khách lẻ'
                                                ELSE N'Khách có tài khoản'
                                            END AS 'LoaiKhachHang',
                                            CASE 
                                                WHEN kh.TrangThai = 1 THEN N'Đang giao dịch'
                                                ELSE N'Ngưng giao dịch'
                                            END AS 'TrangThai',
                                            kh.DiemTichLuy,
                                            COUNT(hd.MaHD) AS 'SoHoaDon',
                                            ISNULL(SUM(hd.TongTien), 0) AS 'TongTien'
                                        FROM 
                                            KhachHang kh
                                            LEFT JOIN HoaDon hd ON kh.MaKH = hd.MaKH
                                        WHERE 
                                            DaXoa = 0";

                    // Thêm điều kiện lọc theo loại khách hàng
                    if (!string.IsNullOrEmpty(loaiKhachHang))
                    {
                        if (loaiKhachHang.ToLower() == "lẻ")
                        {
                            truyVan += " AND kh.TenKH LIKE N'%lẻ%'";
                        }
                        else if(loaiKhachHang.ToLower() == "đã có tài khoản")
                        {
                            truyVan += " AND kh.TenKH NOT LIKE N'%lẻ%'";
                        }
                    }

                    // Thêm điều kiện lọc theo trạng thái giao dịch
                    if (!string.IsNullOrEmpty(trangThaiGiaoDich))
                    {
                        if (trangThaiGiaoDich.ToLower() == "còn giao dịch")
                        {
                            truyVan += " AND kh.TrangThai = 1";
                        }
                        else if (trangThaiGiaoDich.ToLower() == "ngưng giao dịch")
                        {
                            truyVan += " AND kh.TrangThai = 0";
                        }
                    }

                    // Thêm điều kiện lọc theo điểm tích lũy tối thiểu
                    truyVan += " AND kh.DiemTichLuy >= @DiemToiThieu";

                    // Nhóm theo thông tin khách hàng
                    truyVan += @"
                                GROUP BY 
                                    kh.MaKH, kh.TenKH, kh.NgayTao, kh.TrangThai, kh.DiemTichLuy
                                ORDER BY 
                                    kh.NgayTao DESC";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.Add("@DiemToiThieu", SqlDbType.Int).Value = diemToiThieu;

                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable ketQua = new DataTable();
                            adapter.Fill(ketQua);
                            return ketQua;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách khách hàng: {ex.Message}", ex);
            }
        }
        public class ThongKeTongQuanKhachHang
        {
            public int TongSoKhachHang { get; set; }
            public int KhachHangCoTaiKhaon { get; set; }
            public int KhachHangLe { get; set; }
            public int KhachHangConGiaoDich { get; set; }
            public int KhachHangNgungGiaoDich { get; set; }
        }
        public class BaoCaoKhachHang
        {
            public string thoiGian { get; set; }
            public string tenKH { get; set; }
            public string loaiKH { get; set; }
            public string trangThai { get; set; }
            public int soHoaDon { get; set; }
            public int diemTichLuy { get; set; }
            public decimal tongTien { get; set; }
        }
    }
}

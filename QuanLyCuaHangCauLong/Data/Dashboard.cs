using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace QuanLyCuaHangCauLong.Data
{
    public class Dashboard
    {
        private readonly string chuoiKetNoi;

        public Dashboard(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

        // Lấy thống kê tổng quan (4 cards)
        public ThongKeTongQuan LayThongKeTongQuan(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();

                    // Doanh thu
                    string truyVanDoanhThu = @"
                        SELECT ISNULL(SUM(TongTien), 0) AS DoanhThu
                        FROM HoaDon 
                        WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc";

                    // Số hóa đơn
                    string truyVanSoHoaDon = @"
                        SELECT COUNT(*) AS SoHoaDon
                        FROM HoaDon 
                        WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc";

                    // Khách hàng đã đăng ký (trong khoảng thời gian)
                    string truyVanKhachHang = @"
                        SELECT COUNT(*) AS SoKhachHang
                        FROM KhachHang 
                        WHERE NgayTao >= @NgayBatDau AND NgayTao < @NgayKetThuc AND DaXoa = 0
                        AND TenKH != N'lẻ'";

                    // Số sản phẩm bán ra
                    string truyVanSanPhamBan = @"
                        SELECT ISNULL(SUM(cthd.SoLuong), 0) AS SoSanPhamBan
                        FROM ChiTietHoaDon cthd
                        INNER JOIN HoaDon hd ON cthd.MaHD = hd.MaHD
                        WHERE hd.NgayLap >= @NgayBatDau AND hd.NgayLap < @NgayKetThuc";

                    var thongKe = new ThongKeTongQuan();

                    // Lấy doanh thu
                    using (SqlCommand cmd = new SqlCommand(truyVanDoanhThu, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.AddDays(1));
                        thongKe.DoanhThu = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                    // Lấy số hóa đơn
                    using (SqlCommand cmd = new SqlCommand(truyVanSoHoaDon, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.AddDays(1));
                        thongKe.SoHoaDon = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy số khách hàng
                    using (SqlCommand cmd = new SqlCommand(truyVanKhachHang, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.AddDays(1));
                        thongKe.SoKhachHangMoi = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy số sản phẩm bán
                    using (SqlCommand cmd = new SqlCommand(truyVanSanPhamBan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.AddDays(1));
                        thongKe.SoSanPhamBan = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    return thongKe;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tổng quan: {ex.Message}", ex);
            }
        }

        // Top 10 sản phẩm bán chạy
        public DataTable LayTop10SanPhamBanChay(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT TOP 10
                            sp.MaSP,
                            sp.TenSP,
                            SUM(cthd.SoLuong) AS SoLuongBan,
                            SUM(cthd.SoLuong * cthd.DonGia) AS DoanhThu
                        FROM ChiTietHoaDon cthd
                        INNER JOIN SanPham sp ON cthd.MaSP = sp.MaSP
                        INNER JOIN HoaDon hd ON cthd.MaHD = hd.MaHD
                        WHERE hd.NgayLap >= @NgayBatDau AND hd.NgayLap < @NgayKetThuc AND sp.DaXoa = 0
                        GROUP BY sp.MaSP, sp.TenSP
                        ORDER BY SUM(cthd.SoLuong) DESC";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.AddDays(1));

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
                throw new Exception($"Lỗi khi lấy top 5 sản phẩm bán chạy: {ex.Message}", ex);
            }
        }

        // Top 10 hóa đơn có giá trị cao
        public DataTable LayTop10HoaDonGiaTriCao(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT TOP 10
                            hd.MaHD,
                            hd.NgayLap,
                            kh.TenKH,
                            nv.TenNV,
                            hd.TongTien
                        FROM HoaDon hd
                        INNER JOIN NhanVien nv ON hd.MaNV = nv.MaNV
                        LEFT JOIN KhachHang kh ON hd.MaKH = kh.MaKH
                        WHERE hd.NgayLap >= @NgayBatDau AND hd.NgayLap < @NgayKetThuc
                        ORDER BY hd.TongTien DESC";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.AddDays(1));

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
                throw new Exception($"Lỗi khi lấy top 5 hóa đơn giá trị cao: {ex.Message}", ex);
            }
        }

        // Sản phẩm tồn kho thấp (dưới 10)
        public DataTable LaySanPhamTonKhoThap()
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT 
                            sp.MaSP,
                            sp.TenSP,
                            sp.SoLuongSP,
                            lsp.TenLoaiSP,
                            th.TenTH
                        FROM SanPham sp
                        INNER JOIN LoaiSanPham lsp ON sp.MaLoaiSP = lsp.MaLoaiSP
                        INNER JOIN ThuongHieu th ON sp.MaTH = th.MaTH
                        WHERE sp.SoLuongSP < 10 AND sp.TrangThai = 1 AND sp.DaXoa = 0
                        ORDER BY sp.SoLuongSP ASC";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
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
                throw new Exception($"Lỗi khi lấy sản phẩm tồn kho thấp: {ex.Message}", ex);
            }
        }

        // Top 10 khách hàng có giao dịch nhiều tiền
        public DataTable LayTop10KhachHangGiaoDichNhieu(DateTime ngayBatDau, DateTime ngayKetThuc)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT TOP 10
                            kh.MaKH,
                            kh.TenKH,
                            kh.SDT,
                            kh.DiemTichLuy,
                            COUNT(hd.MaHD) AS SoGiaoDich,
                            SUM(hd.TongTien) AS TongGiaTriGiaoDich
                        FROM KhachHang kh
                        INNER JOIN HoaDon hd ON kh.MaKH = hd.MaKH
                        WHERE hd.NgayLap >= @NgayBatDau AND hd.NgayLap < @NgayKetThuc
                        AND kh.TenKH != N'lẻ' AND kh.DaXoa = 0
                        GROUP BY kh.MaKH, kh.TenKH, kh.SDT, kh.DiemTichLuy
                        ORDER BY SUM(hd.TongTien) DESC";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.AddDays(1));

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
                throw new Exception($"Lỗi khi lấy top 5 khách hàng giao dịch nhiều: {ex.Message}", ex);
            }
        }

        public DataTable LayDuLieuBieuDoDoanhThu(DateTime ngayBatDau, DateTime ngayKetThuc, string loaiThongKe)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "";

                    switch (loaiThongKe.ToLower())
                    {
                        case "ngày":
                            truyVan = @"
                                SELECT 
                                    CAST(NgayLap AS DATE) AS ThoiGian,
                                    SUM(TongTien) AS DoanhThu
                                FROM HoaDon
                                WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc
                                GROUP BY CAST(NgayLap AS DATE)
                                ORDER BY CAST(NgayLap AS DATE)";
                            break;
                        case "tháng":
                            truyVan = @"
                                SELECT 
                                    CONCAT(MONTH(NgayLap), '/', YEAR(NgayLap)) AS ThoiGian,
                                    SUM(TongTien) AS DoanhThu
                                FROM HoaDon
                                WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc
                                GROUP BY YEAR(NgayLap), MONTH(NgayLap)
                                ORDER BY YEAR(NgayLap), MONTH(NgayLap)";
                            break;
                        case "năm":
                            truyVan = @"
                                SELECT 
                                    YEAR(NgayLap) AS ThoiGian,
                                    SUM(TongTien) AS DoanhThu
                                FROM HoaDon
                                WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc
                                GROUP BY YEAR(NgayLap)
                                ORDER BY YEAR(NgayLap)";
                            break;
                        default:
                            truyVan = @"
                                SELECT 
                                    CAST(NgayLap AS DATE) AS ThoiGian,
                                    SUM(TongTien) AS DoanhThu
                                FROM HoaDon
                                WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc
                                GROUP BY CAST(NgayLap AS DATE)
                                ORDER BY CAST(NgayLap AS DATE)";
                            break;
                    }

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.AddDays(1));

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
                throw new Exception($"Lỗi khi lấy dữ liệu biểu đồ doanh thu: {ex.Message}", ex);
            }
        }

        // Dữ liệu cho biểu đồ số hóa đơn theo thời gian
        public DataTable LayDuLieuBieuDoSoHoaDon(DateTime ngayBatDau, DateTime ngayKetThuc, string loaiThongKe)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "";

                    switch (loaiThongKe.ToLower())
                    {
                        case "ngày":
                            truyVan = @"
                                SELECT 
                                    CAST(NgayLap AS DATE) AS ThoiGian,
                                    COUNT(*) AS SoHoaDon
                                FROM HoaDon
                                WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc
                                GROUP BY CAST(NgayLap AS DATE)
                                ORDER BY CAST(NgayLap AS DATE)";
                            break;
                        case "tháng":
                            truyVan = @"
                                SELECT 
                                    CONCAT(MONTH(NgayLap), '/', YEAR(NgayLap)) AS ThoiGian,
                                    COUNT(*) AS SoHoaDon
                                FROM HoaDon
                                WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc
                                GROUP BY YEAR(NgayLap), MONTH(NgayLap)
                                ORDER BY YEAR(NgayLap), MONTH(NgayLap)";
                            break;
                        case "năm":
                            truyVan = @"
                                SELECT 
                                    YEAR(NgayLap) AS ThoiGian,
                                    COUNT(*) AS SoHoaDon
                                FROM HoaDon
                                WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc
                                GROUP BY YEAR(NgayLap)
                                ORDER BY YEAR(NgayLap)";
                            break;
                        default:
                            truyVan = @"
                                SELECT 
                                    CAST(NgayLap AS DATE) AS ThoiGian,
                                    COUNT(*) AS SoHoaDon
                                FROM HoaDon
                                WHERE NgayLap >= @NgayBatDau AND NgayLap < @NgayKetThuc
                                GROUP BY CAST(NgayLap AS DATE)
                                ORDER BY CAST(NgayLap AS DATE)";
                            break;
                    }

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@NgayBatDau", ngayBatDau);
                        cmd.Parameters.AddWithValue("@NgayKetThuc", ngayKetThuc.AddDays(1));

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
                throw new Exception($"Lỗi khi lấy dữ liệu biểu đồ số hóa đơn: {ex.Message}", ex);
            }
        }
        // Dữ liệu cho biểu đồ số sản phẩm theo loại sản phẩm
        public DataTable LayDuLieuBieuDoLoaiSanPham()
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT 
                            lsp.TenLoaiSP AS TenLoaiSP,
                            COUNT(sp.MaSP) AS SoSanPham
                        FROM 
                            LoaiSanPham lsp
                        LEFT JOIN 
                            SanPham sp ON lsp.MaLoaiSP = sp.MaLoaiSP
                        GROUP BY 
                            lsp.TenLoaiSP
                        ORDER BY 
                            COUNT(sp.MaSP) DESC";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
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
                throw new Exception($"Lỗi khi lấy dữ liệu biểu đồ loại sản phẩm: {ex.Message}", ex);
            }
        }

        // Dữ liệu cho biểu đồ số sản phẩm theo thương hiệu
        public DataTable LayDuLieuBieuDoThuongHieu()
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT 
                            th.TenTH AS TenTH,
                            COUNT(sp.MaSP) AS SoSanPham
                        FROM 
                            ThuongHieu th
                        LEFT JOIN 
                            SanPham sp ON th.MaTH = sp.MaTH
                        GROUP BY 
                            th.TenTH
                        ORDER BY 
                            COUNT(sp.MaSP) DESC";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
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
                throw new Exception($"Lỗi khi lấy dữ liệu biểu đồ thương hiệu: {ex.Message}", ex);
            }
        }
    }

    // Class để chứa thông tin thống kê tổng quan
    public class ThongKeTongQuan
    {
        public decimal DoanhThu { get; set; }
        public int SoHoaDon { get; set; }
        public int SoKhachHangMoi { get; set; }
        public int SoSanPhamBan { get; set; }
    }
}

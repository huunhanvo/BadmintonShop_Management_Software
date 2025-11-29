using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangCauLong.Data
{
    public class ThongKeDoanhThu
    {
        private readonly string chuoiKetNoi;

        public ThongKeDoanhThu(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }
        public ThongKeTongQuan LayThongKeTongQuan()
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();

                    // Doanh thu
                    string truyVanDoanhThu = @"
                        SELECT ISNULL(SUM(TongTien), 0) AS DoanhThu
                        FROM HoaDon";

                    // Số hóa đơn
                    string truyVanSoHoaDon = @"
                        SELECT COUNT(*) AS SoHoaDon
                        FROM HoaDon";


                    var thongKe = new ThongKeTongQuan();

                    // Lấy doanh thu
                    using (SqlCommand cmd = new SqlCommand(truyVanDoanhThu, ketNoi))
                    {
                        thongKe.DoanhThu = Convert.ToDecimal(cmd.ExecuteScalar());
                    }

                    // Lấy số hóa đơn
                    using (SqlCommand cmd = new SqlCommand(truyVanSoHoaDon, ketNoi))
                    {
                        thongKe.SoHoaDon = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    return thongKe;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tổng quan: {ex.Message}", ex);
            }
        }
        public DataTable LayDuLieuDoanhThu(DateTime ngayBatDau, DateTime ngayKetThuc, string loaiThoiGian, string maNhanVien = null, string maLoaiSanPham = null, string maThuongHieu = null)
        {
            try
            {
                // Kiểm tra giá trị loaiThoiGian
                if (!new[] { "Ngày", "Tháng", "Năm" }.Contains(loaiThoiGian, StringComparer.OrdinalIgnoreCase))
                    throw new ArgumentException("Loại thời gian phải là 'Ngày', 'Tháng' hoặc 'Năm'.");

                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();

                    // Xây dựng truy vấn dựa trên loại thời gian
                    string selectClause = "";
                    string groupByClause = "";
                    string orderByClause = "";

                    switch (loaiThoiGian.ToUpper())
                    {
                        case "NGÀY":
                            selectClause = "CONVERT(VARCHAR(10), hd.NgayLap, 103) AS ThoiGian"; // dd/MM/yyyy
                            groupByClause = "CONVERT(VARCHAR(10), hd.NgayLap, 103)";
                            orderByClause = "CONVERT(VARCHAR(10), hd.NgayLap, 103)";
                            break;
                        case "THÁNG":
                            selectClause = "RIGHT('0' + CAST(MONTH(hd.NgayLap) AS VARCHAR(2)), 2) + '/' + CAST(YEAR(hd.NgayLap) AS VARCHAR(4)) AS ThoiGian";
                            groupByClause = "YEAR(hd.NgayLap), MONTH(hd.NgayLap)";
                            orderByClause = "YEAR(hd.NgayLap), MONTH(hd.NgayLap)";
                            break;
                        case "NĂM":
                            selectClause = "CAST(YEAR(hd.NgayLap) AS VARCHAR(4)) AS ThoiGian";
                            groupByClause = "YEAR(hd.NgayLap)";
                            orderByClause = "YEAR(hd.NgayLap)";
                            break;
                    }

                    string truyVan = $@"
                        SELECT 
                            {selectClause},
                            nv.TenNV,
                            lsp.TenLoaiSP,
                            th.TenTH,
                            SUM(cthd.SoLuong * cthd.DonGia) AS TongTien
                        FROM HoaDon hd 
                            INNER JOIN NhanVien nv ON hd.MaNV = nv.MaNV 
                            INNER JOIN ChiTietHoaDon cthd ON hd.MaHD = cthd.MaHD 
                            INNER JOIN SanPham sp ON cthd.MaSP = sp.MaSP 
                            INNER JOIN LoaiSanPham lsp ON sp.MaLoaiSP = lsp.MaLoaiSP 
                            INNER JOIN ThuongHieu th ON sp.MaTH = th.MaTH
                        WHERE hd.NgayLap >= @NgayBatDau AND hd.NgayLap < @NgayKetThuc";

                    // Thêm điều kiện lọc
                    if (!string.IsNullOrEmpty(maNhanVien))
                    {
                        truyVan += " AND nv.MaNV = @MaNhanVien";
                    }
                    if (!string.IsNullOrEmpty(maLoaiSanPham))
                    {
                        truyVan += " AND sp.MaLoaiSP = @MaLoaiSP";
                    }
                    if (!string.IsNullOrEmpty(maThuongHieu))
                    {
                        truyVan += " AND sp.MaTH = @MaThuongHieu";
                    }

                    // Thêm GROUP BY và ORDER BY
                    truyVan += $@"
                        GROUP BY 
                            {groupByClause},
                            nv.TenNV, 
                            lsp.TenLoaiSP, 
                            th.TenTH
                        ORDER BY 
                            {orderByClause}, nv.TenNV, lsp.TenLoaiSP, th.TenTH";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        // Thêm tham số với kiểu dữ liệu rõ ràng
                        cmd.Parameters.Add("@NgayBatDau", SqlDbType.DateTime).Value = ngayBatDau;
                        cmd.Parameters.Add("@NgayKetThuc", SqlDbType.DateTime).Value = ngayKetThuc.AddDays(1);

                        if (!string.IsNullOrEmpty(maNhanVien))
                            cmd.Parameters.Add("@MaNhanVien", SqlDbType.VarChar, 20).Value = maNhanVien;
                        if (!string.IsNullOrEmpty(maLoaiSanPham))
                            cmd.Parameters.Add("@MaLoaiSP", SqlDbType.VarChar, 20).Value = maLoaiSanPham;
                        if (!string.IsNullOrEmpty(maThuongHieu))
                            cmd.Parameters.Add("@MaThuongHieu", SqlDbType.VarChar, 20).Value = maThuongHieu;

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
                throw new Exception($"Lỗi khi lấy dữ liệu doanh thu: {ex.Message}", ex);
            }
        }
        public class BaoCaoDoanhThu
        {
            public string thoiGian { get; set; }
            public string tenNV { get; set; }
            public string tenLoaiSP { get; set; }
            public string tenTH { get; set; }
            public decimal tongTien { get; set; }
        }
    }
}
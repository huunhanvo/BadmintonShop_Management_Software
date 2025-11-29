using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangCauLong.Data
{
    public class ThongKeNhanVien
    {
        private readonly string chuoiKetNoi;

        public ThongKeNhanVien(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

        public ThongKeTongQuanNhanVien LayThongKeChoCacTheNhanVien()
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();

                    // Tổng số nhân viên
                    string truyVanTongNhanVien = @"SELECT COUNT(*) FROM NhanVien";

                    // Số nhân viên Admin
                    string truyVanNhanVienAdmin = @"SELECT COUNT(*) FROM NhanVien WHERE VaiTro LIKE 'Admin'";

                    // Số nhân viên Thu ngân
                    string truyVanNhanVienThuNgan = @"SELECT COUNT(*) FROM NhanVien WHERE VaiTro LIKE 'Thu ngân'";

                    // Số nhân viên Thường
                    string truyVanNhanVienThuong = @"SELECT COUNT(*) FROM NhanVien WHERE VaiTro LIKE 'Nhân viên'"; ;

                    // Số nhân viên Còn làm
                    string truyVanNhanVienConLam = @"SELECT COUNT(*) FROM NhanVien WHERE TrangThai = 1";

                    // Số nhân viên Nghĩ làm
                    string truyVanNhanVienNghiLam = @"SELECT COUNT(*) FROM NhanVien WHERE TrangThai = 0";



                    var thongKe = new ThongKeTongQuanNhanVien();

                    // Lấy tổng số nhân viên
                    using (SqlCommand cmd = new SqlCommand(truyVanTongNhanVien, ketNoi))
                    {
                        thongKe.TongSoNhanVien = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy tổng số nhân viên Admin
                    using (SqlCommand cmd = new SqlCommand(truyVanNhanVienAdmin, ketNoi))
                    {
                        thongKe.NhanVienAdmin = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy tổng số nhân viên Thu ngân
                    using (SqlCommand cmd = new SqlCommand(truyVanNhanVienThuNgan, ketNoi))
                    {
                        thongKe.NhanVienThuNgan = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy tổng số nhân viên Thường
                    using (SqlCommand cmd = new SqlCommand(truyVanNhanVienThuong, ketNoi))
                    {
                        thongKe.NhanVienThuong = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy tổng số nhân viên Còn làm
                    using (SqlCommand cmd = new SqlCommand(truyVanNhanVienConLam, ketNoi))
                    {
                        thongKe.NhanVienConLam = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    // Lấy tổng số nhân viên Nghĩ làm
                    using (SqlCommand cmd = new SqlCommand(truyVanNhanVienNghiLam, ketNoi))
                    {
                        thongKe.NhanVienNghiLam = Convert.ToInt32(cmd.ExecuteScalar());
                    }

                    return thongKe;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy thống kê tổng quan: {ex.Message}", ex);
            }
        }

        public DataTable LayDanhSachNhanVienTheoTieuChi(int tuoiBatDau, int tuoiKetThuc, string vaiTro, string trangThai)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();

                    string truyVan = @"
                                       SELECT 
                                            nv.NgaySinh,
                                            nv.TenNV,
                                            nv.VaiTro,
                                            CASE 
                                                WHEN nv.TrangThai = 1 THEN N'Đang làm việc'
                                                ELSE N'Ngưng làm việc'
                                            END AS 'TrangThai',
                                            COUNT(hd.MaHD) AS 'SoHoaDon',
                                            ISNULL(SUM(hd.TongTien), 0) AS 'TongTien'
                                        FROM 
                                            NhanVien nv
                                            LEFT JOIN HoaDon hd ON nv.MaNV = hd.MaNV
                                        WHERE 
                                            DATEDIFF(YEAR, nv.NgaySinh, GETDATE()) BETWEEN @TuoiBatDau AND @TuoiKetThuc
                                            AND nv.DaXoa = 0";

                    // Thêm điều kiện lọc theo vai trò nhân viên
                    if (!string.IsNullOrEmpty(vaiTro))
                    {
                        if (vaiTro.ToLower() == "admin")
                        {
                            truyVan += " AND nv.VaiTro LIKE N'Admin'";
                        }
                        else if (vaiTro.ToLower() == "thu ngân")
                        {
                            truyVan += " AND nv.VaiTro LIKE N'Thu ngân'";
                        }
                        else if(vaiTro.ToLower() == "nhân viên")
                        {
                            truyVan += " AND nv.VaiTro LIKE N'Nhân viên'";
                        }
                    }

                    // Thêm điều kiện lọc theo trạng thái làm việc
                    if (!string.IsNullOrEmpty(trangThai))
                    {
                        if (trangThai.ToLower() == "đang làm")
                        {
                            truyVan += " AND nv.TrangThai = 1";
                        }
                        else if (trangThai.ToLower() == "nghĩ làm")
                        {
                            truyVan += " AND nv.TrangThai = 0";
                        }
                    }

                    // Nhóm theo thông tin nhân viên
                    truyVan += @"GROUP BY 
                                        nv.NgaySinh, nv.TenNV, nv.VaiTro, nv.TrangThai";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.Add("@TuoiBatDau", SqlDbType.Int).Value = tuoiBatDau;
                        cmd.Parameters.Add("@TuoiKetThuc", SqlDbType.Int).Value = tuoiKetThuc;
                        //cmd.Parameters.Add("@VaiTro", SqlDbType.NVarChar).Value = vaiTro;
                        //cmd.Parameters.Add("@TrangThai", SqlDbType.Int).Value = trangThai;


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
                throw new Exception($"Lỗi khi lấy danh sách nhân viên: {ex.Message}", ex);
            }
        }
        public class ThongKeTongQuanNhanVien
        {
            public int TongSoNhanVien { get; set; }
            public int NhanVienAdmin { get; set; }
            public int NhanVienThuNgan { get; set; }
            public int NhanVienThuong { get; set; }
            public int NhanVienConLam { get; set; }
            public int NhanVienNghiLam { get; set; }

        }
        public class BaoCaoNhanVien
        {
            public string thoiGian { get; set; }
            public string tenNV { get; set; }
            public string vaiTro { get; set; }
            public string trangThai { get; set; }
            public int soHoaDon { get; set; }
            public decimal tongTien { get; set; }
        }
    }
}

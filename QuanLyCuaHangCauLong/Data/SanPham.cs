using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;

namespace QuanLyCuaHangCauLong.Data
{
    public class SanPham
    {
        private readonly string chuoiKetNoi;

        public SanPham(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

        public DataTable LayDanhSachSanPham()
        {
            return LayDanhSachSanPham(null, null, null, null);
        }

        public DataTable LayDanhSachSanPham(string maLoaiSP, string maTH, string tuKhoa, bool? trangThai = null)
        {
            DataTable bangDuLieu = new DataTable();
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"SELECT sp.MaSP, sp.TenSP, sp.DonGiaSP, sp.SoLuongSP, sp.DuongDanAnh, sp.HinhAnh, 
                                     CASE sp.TrangThai WHEN 1 THEN N'Hiện' WHEN 0 THEN N'Ẩn' END AS TrangThai, 
                                     lsp.TenLoaiSP, th.TenTH, sp.MaLoaiSP, sp.MaTH
                                     FROM SanPham sp 
                                     INNER JOIN LoaiSanPham lsp ON sp.MaLoaiSP = lsp.MaLoaiSP 
                                     INNER JOIN ThuongHieu th ON sp.MaTH = th.MaTH
                                     WHERE sp.DaXoa=0";

                    // Thêm điều kiện lọc
                    if (!string.IsNullOrEmpty(maLoaiSP))
                    {
                        truyVan += " AND sp.MaLoaiSP = @MaLoaiSP";
                    }
                    if (!string.IsNullOrEmpty(maTH))
                    {
                        truyVan += " AND sp.MaTH = @MaTH";
                    }
                    if (!string.IsNullOrEmpty(tuKhoa))
                    {
                        truyVan += " AND (sp.MaSP LIKE @TuKhoa OR sp.TenSP LIKE @TuKhoa)";
                    }
                    if (trangThai.HasValue)
                    {
                        truyVan += " AND sp.TrangThai = @TrangThai";
                    }

                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        // Thêm tham số
                        if (!string.IsNullOrEmpty(maLoaiSP))
                        {
                            lenh.Parameters.AddWithValue("@MaLoaiSP", maLoaiSP);
                        }
                        if (!string.IsNullOrEmpty(maTH))
                        {
                            lenh.Parameters.AddWithValue("@MaTH", maTH);
                        }
                        if (!string.IsNullOrEmpty(tuKhoa))
                        {
                            lenh.Parameters.AddWithValue("@TuKhoa", "%" + tuKhoa + "%");
                        }
                        if (trangThai.HasValue)
                        {
                            lenh.Parameters.AddWithValue("@TrangThai", trangThai.Value ? 1 : 0);
                        }

                        using (SqlDataAdapter boDieuChinh = new SqlDataAdapter(lenh))
                        {
                            boDieuChinh.Fill(bangDuLieu);
                        }
                    }

                    // Xử lý cột hình ảnh
                    if (!bangDuLieu.Columns.Contains("HinhAnhHienThi"))
                    {
                        bangDuLieu.Columns.Add("HinhAnhHienThi", typeof(Image));
                    }

                    foreach (DataRow dong in bangDuLieu.Rows)
                    {
                        if (dong["HinhAnh"] != DBNull.Value)
                        {
                            byte[] duLieuAnh = (byte[])dong["HinhAnh"];
                            using (MemoryStream luong = new MemoryStream(duLieuAnh))
                            {
                                dong["HinhAnhHienThi"] = Image.FromStream(luong);
                            }
                        }
                        else
                        {
                            dong["HinhAnhHienThi"] = null;
                        }
                    }

                    if (bangDuLieu.Columns.Contains("HinhAnh"))
                    {
                        bangDuLieu.Columns.Remove("HinhAnh");
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách sản phẩm: " + ex.Message);
            }
            return bangDuLieu;
        }

        // Thêm sản phẩm mới
        public void ThemSanPham(string maSP, string tenSP, decimal donGia, int soLuong, string duongDanAnh, Image hinhAnh, bool trangThai, string maLoaiSP, string maTH)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"INSERT INTO SanPham (MaSP, TenSP, DonGiaSP, SoLuongSP, DuongDanAnh, HinhAnh, TrangThai, MaLoaiSP, MaTH)
                                     VALUES (@MaSP, @TenSP, @DonGiaSP, @SoLuongSP, @DuongDanAnh, @HinhAnh, @TrangThai, @MaLoaiSP, @MaTH)";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaSP", maSP);
                        lenh.Parameters.AddWithValue("@TenSP", tenSP);
                        lenh.Parameters.AddWithValue("@DonGiaSP", donGia);
                        lenh.Parameters.AddWithValue("@SoLuongSP", soLuong);
                        lenh.Parameters.AddWithValue("@DuongDanAnh", (object)duongDanAnh ?? DBNull.Value);
                        lenh.Parameters.AddWithValue("@TrangThai", trangThai ? 1 : 0);
                        lenh.Parameters.AddWithValue("@MaLoaiSP", maLoaiSP);
                        lenh.Parameters.AddWithValue("@MaTH", maTH);

                        if (hinhAnh != null)
                        {
                            // Tạo bản sao của Image để tránh lỗi GDI+
                            using (Bitmap bitmap = new Bitmap(hinhAnh))
                            {
                                using (MemoryStream luong = new MemoryStream())
                                {
                                    bitmap.Save(luong, hinhAnh.RawFormat);
                                    lenh.Parameters.AddWithValue("@HinhAnh", luong.ToArray());
                                }
                            }
                        }
                        else
                        {
                            lenh.Parameters.AddWithValue("@HinhAnh", DBNull.Value);
                        }

                        lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm sản phẩm: " + ex.Message);
            }
        }

        // Cập nhật sản phẩm
        public void CapNhatSanPham(string maSP, string tenSP, decimal donGia, int soLuong, string duongDanAnh, Image hinhAnh, bool trangThai, string maLoaiSP, string maTH)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"UPDATE SanPham 
                                      SET TenSP = @TenSP, DonGiaSP = @DonGiaSP, SoLuongSP = @SoLuongSP, 
                                          DuongDanAnh = @DuongDanAnh, HinhAnh = @HinhAnh, TrangThai = @TrangThai, 
                                          MaLoaiSP = @MaLoaiSP, MaTH = @MaTH 
                                      WHERE MaSP = @MaSP AND DaXoa = 0";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaSP", maSP);
                        lenh.Parameters.AddWithValue("@TenSP", tenSP);
                        lenh.Parameters.AddWithValue("@DonGiaSP", donGia);
                        lenh.Parameters.AddWithValue("@SoLuongSP", soLuong);
                        lenh.Parameters.AddWithValue("@DuongDanAnh", (object)duongDanAnh ?? DBNull.Value);
                        lenh.Parameters.AddWithValue("@TrangThai", trangThai ? 1 : 0);
                        lenh.Parameters.AddWithValue("@MaLoaiSP", maLoaiSP);
                        lenh.Parameters.AddWithValue("@MaTH", maTH);

                        if (hinhAnh != null)
                        {
                            // Tạo bản sao của Image để tránh lỗi GDI+
                            using (Bitmap bitmap = new Bitmap(hinhAnh))
                            {
                                using (MemoryStream luong = new MemoryStream())
                                {
                                    bitmap.Save(luong, hinhAnh.RawFormat);
                                    lenh.Parameters.AddWithValue("@HinhAnh", luong.ToArray());
                                }
                            }
                        }
                        else
                        {
                            lenh.Parameters.AddWithValue("@HinhAnh", DBNull.Value);
                        }

                        int soDongAnhHuong = lenh.ExecuteNonQuery();
                        if (soDongAnhHuong == 0)
                        {
                            throw new Exception("Không tìm thấy sản phẩm với mã này!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật sản phẩm: " + ex.Message);
            }
        }

        // Xóa sản phẩm
        public void XoaSanPham(string maSP)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "DELETE FROM SanPham WHERE MaSP = @MaSP";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaSP", maSP);
                        int soDongAnhHuong = lenh.ExecuteNonQuery();
                        if (soDongAnhHuong == 0)
                        {
                            throw new Exception("Không tìm thấy sản phẩm với mã này!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa sản phẩm: " + ex.Message);
            }
        }
        public class ThongTinSanpham
        {
            public string maSP { get; set; }
            public string tenSP { get; set; }
            public int soLuong { get; set; }
            public decimal donGia { get; set; }
            public string loaiSP { get; set; }
            public string thuongHieuSP { get; set; }
            public string duongDanAnh { get; set; }
            public string trangThai { get; set; }
        }
    }
}
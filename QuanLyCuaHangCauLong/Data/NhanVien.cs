using System;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCuaHangCauLong.Data
{
    public class NhanVien
    {
        private readonly string chuoiKetNoi;

        public NhanVien(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

        public DataTable LayDanhSachNhanVien()
        {
            return LayDanhSachNhanVien(null, null);
        }

        public DataTable LayDanhSachNhanVien(string vaiTro, string tuKhoa)
        {
            DataTable bangDuLieu = new DataTable();
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"SELECT MaNV, TenNV, NgaySinh, DiaChi, SDT,
                                      CASE TrangThai WHEN 1 THEN N'Đang làm' WHEN 0 THEN N'Đã nghỉ' END AS TrangThai,
                                      VaiTro, TenDangNhap, MatKhau, 
                                      CASE CoTheDangNhap WHEN 1 THEN N'Có' WHEN 0 THEN N'Không' END AS CoTheDangNhap
                                      FROM NhanVien
                                      WHERE DaXoa=0 ";

                    if (!string.IsNullOrEmpty(vaiTro) && vaiTro != "Tất cả")
                    {
                        truyVan += " AND VaiTro = @VaiTro";
                    }

                    if (!string.IsNullOrEmpty(tuKhoa))
                    {
                        truyVan += " AND (MaNV LIKE @TuKhoa OR TenNV LIKE @TuKhoa)";
                    }

                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        if (!string.IsNullOrEmpty(vaiTro) && vaiTro != "Tất cả")
                        {
                            lenh.Parameters.AddWithValue("@VaiTro", vaiTro);
                        }
                        if (!string.IsNullOrEmpty(tuKhoa))
                        {
                            lenh.Parameters.AddWithValue("@TuKhoa", "%" + tuKhoa + "%");
                        }

                        using (SqlDataAdapter boDieuChinh = new SqlDataAdapter(lenh))
                        {
                            boDieuChinh.Fill(bangDuLieu);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách nhân viên: " + ex.Message);
            }
            return bangDuLieu;
        }

        public void ThemNhanVien(string maNV, string tenNV, DateTime ngaySinh, string diaChi, string sdt,
                                bool trangThai, string vaiTro, string tenDangNhap, string matKhau, bool coTheDangNhap)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"INSERT INTO NhanVien (MaNV, TenNV, NgaySinh, DiaChi, SDT, TrangThai, VaiTro, TenDangNhap, MatKhau, CoTheDangNhap)
                                      VALUES (@MaNV, @TenNV, @NgaySinh, @DiaChi, @SDT, @TrangThai, @VaiTro, @TenDangNhap, @MatKhau, @CoTheDangNhap)";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaNV", maNV);
                        lenh.Parameters.AddWithValue("@TenNV", tenNV);
                        lenh.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                        lenh.Parameters.AddWithValue("@DiaChi", (object)diaChi ?? DBNull.Value);
                        lenh.Parameters.AddWithValue("@SDT", (object)sdt ?? DBNull.Value);
                        lenh.Parameters.AddWithValue("@TrangThai", trangThai ? 1 : 0);
                        lenh.Parameters.AddWithValue("@VaiTro", vaiTro);
                        lenh.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        lenh.Parameters.AddWithValue("@MatKhau", matKhau);
                        lenh.Parameters.AddWithValue("@CoTheDangNhap", coTheDangNhap ? 1 : 0);

                        lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm nhân viên: " + ex.Message);
            }
        }

        public void CapNhatNhanVien(string maNV, string tenNV, DateTime ngaySinh, string diaChi, string sdt,
                                   bool trangThai, string vaiTro, string tenDangNhap, string matKhau, bool coTheDangNhap)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"UPDATE NhanVien 
                                      SET TenNV = @TenNV, NgaySinh = @NgaySinh, DiaChi = @DiaChi, SDT = @SDT,
                                          TrangThai = @TrangThai, VaiTro = @VaiTro, TenDangNhap = @TenDangNhap,
                                          MatKhau = @MatKhau, CoTheDangNhap = @CoTheDangNhap
                                      WHERE MaNV = @MaNV";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaNV", maNV);
                        lenh.Parameters.AddWithValue("@TenNV", tenNV);
                        lenh.Parameters.AddWithValue("@NgaySinh", ngaySinh);
                        lenh.Parameters.AddWithValue("@DiaChi", (object)diaChi ?? DBNull.Value);
                        lenh.Parameters.AddWithValue("@SDT", (object)sdt ?? DBNull.Value);
                        lenh.Parameters.AddWithValue("@TrangThai", trangThai ? 1 : 0);
                        lenh.Parameters.AddWithValue("@VaiTro", vaiTro);
                        lenh.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        lenh.Parameters.AddWithValue("@MatKhau", matKhau);
                        lenh.Parameters.AddWithValue("@CoTheDangNhap", coTheDangNhap ? 1 : 0);

                        int soDongAnhHuong = lenh.ExecuteNonQuery();
                        if (soDongAnhHuong == 0)
                        {
                            throw new Exception("Không tìm thấy nhân viên với mã này!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật nhân viên: " + ex.Message);
            }
        }

        public void XoaNhanVien(string maNV)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "DELETE FROM NhanVien WHERE MaNV = @MaNV";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaNV", maNV);
                        int soDongAnhHuong = lenh.ExecuteNonQuery();
                        if (soDongAnhHuong == 0)
                        {
                            throw new Exception("Không tìm thấy nhân viên với mã này!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa nhân viên: " + ex.Message);
            }
        }
        public class ThongTinNhanVien
        {
            public string maNV { get; set; }
            public string tenNV { get; set; }
            public string ngaySinh { get; set; }
            public string diaChi { get; set; }
            public string sdt { get; set; }
            public string trangThai { get; set; }
            public string vaiTro { get; set; }
            public string tenDangNhap { get; set; }
            public string matKhau { get; set; }
            public string coTheDangNhap { get; set; }

        }
    }
}
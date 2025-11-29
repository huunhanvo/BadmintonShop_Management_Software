using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCuaHangCauLong.Data
{
    public class KhachHang
    {
        private readonly string chuoiKetNoi;

        public KhachHang(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

        public DataTable LayDanhSachKhachHang()
        {
            return LayDanhSachKhachHang(null, null, null);
        }

        public DataTable LayDanhSachKhachHang(string loaiKhachHang, string tuKhoa, bool? trangThai = null)
        {
            DataTable bangDuLieu = new DataTable();
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"SELECT MaKH, TenKH, DiaChi, SDT, Email, NgayTao, DiemTichLuy,
                                      CASE TrangThai WHEN 1 THEN N'Còn Giao Dịch' WHEN 0 THEN N'Ngưng Giao Dịch' END AS TrangThai
                                      FROM KhachHang
                                      WHERE DaXoa = 0";

                    if (!string.IsNullOrEmpty(loaiKhachHang) && loaiKhachHang != "Tất cả")
                    {
                        if (loaiKhachHang == "Lẻ")
                        {
                            truyVan += " AND (Email IS NULL OR Email = 'Không có')";
                        }
                        else if (loaiKhachHang == "Đã có tài khoản")
                        {
                            truyVan += " AND (Email IS NOT NULL AND Email != 'Không có')";
                        }
                    }

                    if (!string.IsNullOrEmpty(tuKhoa))
                    {
                        truyVan += " AND (MaKH LIKE @TuKhoa OR TenKH LIKE @TuKhoa)";

                    }
                    if (trangThai.HasValue)
                    {
                        truyVan += " AND TrangThai = @TrangThai";
                    }

                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
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
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách khách hàng: " + ex.Message);
            }
            return bangDuLieu;
        }

        public void ThemKhachHang(string maKH, string tenKH = null, string diaChi = null, string sdt = null, string email = null, DateTime? ngayTao = null, int? diemTichLuy = null, bool? trangThai = null)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    // Xây dựng câu INSERT động
                    string truyVan = "INSERT INTO KhachHang (MaKH";
                    string giaTri = "VALUES (@MaKH";
                    var parameters = new List<SqlParameter> { new SqlParameter("@MaKH", maKH) };

                    if (!string.IsNullOrEmpty(tenKH))
                    {
                        truyVan += ", TenKH";
                        giaTri += ", @TenKH";
                        parameters.Add(new SqlParameter("@TenKH", tenKH));
                    }
                    if (!string.IsNullOrEmpty(diaChi))
                    {
                        truyVan += ", DiaChi";
                        giaTri += ", @DiaChi";
                        parameters.Add(new SqlParameter("@DiaChi", diaChi));
                    }
                    if (!string.IsNullOrEmpty(sdt))
                    {
                        truyVan += ", SDT";
                        giaTri += ", @SDT";
                        parameters.Add(new SqlParameter("@SDT", sdt));
                    }
                    if (!string.IsNullOrEmpty(email))
                    {
                        truyVan += ", Email";
                        giaTri += ", @Email";
                        parameters.Add(new SqlParameter("@Email", email));
                    }
                    if (ngayTao.HasValue)
                    {
                        truyVan += ", NgayTao";
                        giaTri += ", @NgayTao";
                        parameters.Add(new SqlParameter("@NgayTao", ngayTao.Value));
                    }
                    if (diemTichLuy.HasValue)
                    {
                        truyVan += ", DiemTichLuy";
                        giaTri += ", @DiemTichLuy";
                        parameters.Add(new SqlParameter("@DiemTichLuy", diemTichLuy.Value));
                    }
                    if (trangThai.HasValue)
                    {
                        truyVan += ", TrangThai";
                        giaTri += ", @TrangThai";
                        parameters.Add(new SqlParameter("@TrangThai", trangThai.Value ? 1 : 0));
                    }

                    truyVan += ") " + giaTri + ")";

                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddRange(parameters.ToArray());
                        lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi thêm khách hàng: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm khách hàng: " + ex.Message);
            }
        }

        public void CapNhatKhachHang(string maKH, string tenKH, string diaChi, string sdt, string email, DateTime ngayTao, int diemTichLuy, bool trangThai)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"UPDATE KhachHang 
                                      SET TenKH = @TenKH, DiaChi = @DiaChi, SDT = @SDT, Email = @Email, NgayTao = @NgayTao, DiemTichLuy = @DiemTichLuy, TrangThai = @TrangThai 
                                      WHERE MaKH = @MaKH";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaKH", maKH);
                        lenh.Parameters.AddWithValue("@TenKH", tenKH);
                        lenh.Parameters.AddWithValue("@DiaChi", (object)diaChi ?? DBNull.Value);
                        lenh.Parameters.AddWithValue("@SDT", (object)sdt ?? DBNull.Value);
                        lenh.Parameters.AddWithValue("@Email", (object)email ?? DBNull.Value);
                        lenh.Parameters.AddWithValue("@NgayTao", ngayTao);
                        lenh.Parameters.AddWithValue("@DiemTichLuy", diemTichLuy);
                        lenh.Parameters.AddWithValue("@TrangThai", trangThai ? 1 : 0);

                        int soDongAnhHuong = lenh.ExecuteNonQuery();
                        if (soDongAnhHuong == 0)
                        {
                            throw new Exception("Không tìm thấy khách hàng với mã này!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật khách hàng: " + ex.Message);
            }
        }

        public void XoaKhachHang(string maKH)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "DELETE FROM KhachHang WHERE MaKH = @MaKH";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaKH", maKH);
                        int soDongAnhHuong = lenh.ExecuteNonQuery();
                        if (soDongAnhHuong == 0)
                        {
                            throw new Exception("Không tìm thấy khách hàng với mã này!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa khách hàng: " + ex.Message);
            }
        }
        public void CapNhatDiemTichLuy(string maKH, int themDiemTichLuy)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        UPDATE KhachHang
                        SET DiemTichLuy = DiemTichLuy + @DiemTichLuy
                        WHERE MaKH = @MaKH";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@MaKH", maKH);
                        cmd.Parameters.AddWithValue("@DiemTichLuy", themDiemTichLuy);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi cập nhật điểm tích lũy: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật điểm tích lũy: {ex.Message}", ex);
            }
        }
        public class ThongTinKhachHang
        {
            public string maKH { get; set; }
            public string tenKH { get; set; }
            public string diaChi { get; set; }
            public string sdt { get; set; }
            public string email { get; set; }
            public string ngayTao { get; set; }
            public int diemTichLuy { get; set; }
            public string trangThai { get; set; }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCuaHangCauLong.Data
{
    public class HoaDon
    {
        private readonly string chuoiKetNoi;

        public HoaDon(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }
        public DataTable LayDanhSachHoaDon(string tuKhoa = null, DateTime? ngayBatDau = null, DateTime? ngayKetThuc = null, string loaiKhachHang = null)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT 
                            hd.MaHD,
                            hd.MaNV,
                            nv.TenNV,
                            hd.MaKH,
                            kh.TenKH,
                            hd.NgayLap,
                            hd.TongTien,
                            hd.MaCSDTL,
                            CASE hd.DaHuy 
                                WHEN 1 THEN N'Đã huỷ' 
                                WHEN 0 THEN N'Chưa huỷ' 
                            END AS TrangThaiHuy,
                            cs.PhanTramGiam
                        FROM HoaDon hd
                        INNER JOIN NhanVien nv ON hd.MaNV = nv.MaNV
                        LEFT JOIN KhachHang kh ON hd.MaKH = kh.MaKH
                        LEFT JOIN ChinhSachDiemTichLuy cs ON hd.MaCSDTL = cs.MaCSDTL
                        WHERE 1=1";

                    var parameters = new List<SqlParameter>();

                    // Lọc theo từ khóa
                    if (!string.IsNullOrWhiteSpace(tuKhoa))
                    {
                        truyVan += " AND (hd.MaHD LIKE @TuKhoa OR hd.MaNV LIKE @TuKhoa OR hd.MaKH LIKE @TuKhoa)";
                        parameters.Add(new SqlParameter("@TuKhoa", $"%{tuKhoa}%"));
                    }

                    // Lọc theo ngày bắt đầu
                    if (ngayBatDau.HasValue)
                    {
                        truyVan += " AND hd.NgayLap >= @NgayBatDau";
                        parameters.Add(new SqlParameter("@NgayBatDau", ngayBatDau.Value.Date));
                    }

                    // Lọc theo ngày kết thúc
                    if (ngayKetThuc.HasValue)
                    {
                        truyVan += " AND hd.NgayLap < @NgayKetThuc";
                        parameters.Add(new SqlParameter("@NgayKetThuc", ngayKetThuc.Value.Date.AddDays(1)));
                    }

                    // Lọc theo loại khách hàng
                    if (!string.IsNullOrEmpty(loaiKhachHang))
                    {
                        if (loaiKhachHang == "Lẻ")
                        {
                            truyVan += " AND kh.TenKH = N'lẻ'";
                        }
                        else 
                        {
                            truyVan += " AND kh.TenKH != N'lẻ'";
                        }
                    }

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddRange(parameters.ToArray());
                        using (SqlDataAdapter adapter = new SqlDataAdapter(cmd))
                        {
                            DataTable ketQua = new DataTable();
                            adapter.Fill(ketQua);
                            return ketQua;
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi lấy danh sách hóa đơn: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách hóa đơn: {ex.Message}", ex);
            }
        }
        public void ThemHoaDon(string maHD, string maNV, string maKH, DateTime ngayLap, decimal tongTien, string maCSDTL, DataTable chiTietHoaDon)
        {
            using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
            {
                ketNoi.Open();
                using (SqlTransaction giaoDich = ketNoi.BeginTransaction())
                {
                    try
                    {
                        // Thêm hóa đơn
                        string truyVanHoaDon = @"INSERT INTO HoaDon (MaHD, MaNV, MaKH, NgayLap, TongTien, MaCSDTL)
                                                VALUES (@MaHD, @MaNV, @MaKH, @NgayLap, @TongTien, @MaCSDTL)";
                        using (SqlCommand lenhHoaDon = new SqlCommand(truyVanHoaDon, ketNoi, giaoDich))
                        {
                            lenhHoaDon.Parameters.AddWithValue("@MaHD", maHD);
                            lenhHoaDon.Parameters.AddWithValue("@MaNV", maNV);
                            lenhHoaDon.Parameters.AddWithValue("@MaKH", (object)maKH ?? DBNull.Value);
                            lenhHoaDon.Parameters.AddWithValue("@NgayLap", ngayLap);
                            lenhHoaDon.Parameters.AddWithValue("@TongTien", tongTien);
                            lenhHoaDon.Parameters.AddWithValue("@MaCSDTL", (object)maCSDTL ?? DBNull.Value);
                            lenhHoaDon.ExecuteNonQuery();
                        }

                        // Thêm chi tiết hóa đơn
                        foreach (DataRow dong in chiTietHoaDon.Rows)
                        {
                            string truyVanChiTiet = @"INSERT INTO ChiTietHoaDon (MaHD, MaSP, SoLuong, DonGia)
                                                     VALUES (@MaHD, @MaSP, @SoLuong, @DonGia)";
                            using (SqlCommand lenhChiTiet = new SqlCommand(truyVanChiTiet, ketNoi, giaoDich))
                            {
                                lenhChiTiet.Parameters.AddWithValue("@MaHD", maHD);
                                lenhChiTiet.Parameters.AddWithValue("@MaSP", dong["MaSP"]);
                                lenhChiTiet.Parameters.AddWithValue("@SoLuong", dong["SoLuong"]);
                                lenhChiTiet.Parameters.AddWithValue("@DonGia", dong["DonGia"]);
                                lenhChiTiet.ExecuteNonQuery();
                            }

                            // Cập nhật số lượng sản phẩm
                            string truyVanCapNhatSP = @"UPDATE SanPham SET SoLuongSP = SoLuongSP - @SoLuong
                                                        WHERE MaSP = @MaSP";
                            using (SqlCommand lenhCapNhatSP = new SqlCommand(truyVanCapNhatSP, ketNoi, giaoDich))
                            {
                                lenhCapNhatSP.Parameters.AddWithValue("@SoLuong", dong["SoLuong"]);
                                lenhCapNhatSP.Parameters.AddWithValue("@MaSP", dong["MaSP"]);
                                lenhCapNhatSP.ExecuteNonQuery();
                            }
                        }

                        // Cập nhật điểm tích lũy khách hàng
                        if (!string.IsNullOrEmpty(maCSDTL))
                        {
                            string truyVanDiem = @"UPDATE KhachHang SET DiemTichLuy = DiemTichLuy - (SELECT DiemToiThieu FROM ChinhSachDiemTichLuy WHERE MaCSDTL = @MaCSDTL)
                                                  WHERE MaKH = @MaKH";
                            using (SqlCommand lenhDiem = new SqlCommand(truyVanDiem, ketNoi, giaoDich))
                            {
                                lenhDiem.Parameters.AddWithValue("@MaCSDTL", maCSDTL);
                                lenhDiem.Parameters.AddWithValue("@MaKH", maKH);
                                lenhDiem.ExecuteNonQuery();
                            }
                        }

                        giaoDich.Commit();
                    }
                    catch
                    {
                        giaoDich.Rollback();
                        throw;
                    }
                }
            }
        }
        public void HuyHoaDon(string maHD)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"UPDATE HoaDon 
                                      SET DaHuy = 1 
                                      WHERE MaHD = @MaHD";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaHD", maHD);
                        lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi huỷ hoá đơn: " + ex.Message);
            }
        }
        public class XuatChiTietHoaDon
        {
            public string tenSP { get; set; }
            public int soLuong { get; set; }
            public decimal donGia { get; set; }
            public decimal thanhTien { get; set; }
        }
    }
}
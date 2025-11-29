using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuanLyCuaHangCauLong.Data
{
    public class PhieuNhap
    {
        private readonly string chuoiKetNoi;
        public PhieuNhap(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }
        public DataTable LayDanhSachPhieuNhap(string tuKhoa = null, DateTime? ngayBatDau = null, DateTime? ngayKetThuc = null)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT 
                            pn.MaPN,
                            pn.MaNCC,
                            ncc.TenNCC,
                            pn.MaNV,
                            nv.TenNV,
                            pn.ThoiGianNhap,
                            pn.TongTien,
                            CASE pn.DaHuy 
                                WHEN 1 THEN N'Đã huỷ' 
                                WHEN 0 THEN N'Chưa huỷ' 
                            END AS TrangThaiHuy
                        FROM PhieuNhap pn
                        INNER JOIN NhaCungCap ncc ON pn.MaNCC = ncc.MaNCC
                        INNER JOIN NhanVien nv ON pn.MaNV = nv.MaNV
                        WHERE 1=1";

                    var parameters = new List<SqlParameter>();

                    // Lọc theo từ khóa
                    if (!string.IsNullOrWhiteSpace(tuKhoa))
                    {
                        truyVan += " AND (pn.MaPN LIKE @TuKhoa OR pn.MaNCC LIKE @TuKhoa OR ncc.TenNCC LIKE @TuKhoa OR pn.MaNV LIKE @TuKhoa OR nv.TenNV LIKE @TuKhoa)";
                        parameters.Add(new SqlParameter("@TuKhoa", $"%{tuKhoa}%"));
                    }

                    // Lọc theo ngày bắt đầu
                    if (ngayBatDau.HasValue)
                    {
                        truyVan += " AND pn.ThoiGianNhap >= @NgayBatDau";
                        parameters.Add(new SqlParameter("@NgayBatDau", ngayBatDau.Value.Date));
                    }

                    // Lọc theo ngày kết thúc
                    if (ngayKetThuc.HasValue)
                    {
                        truyVan += " AND pn.ThoiGianNhap < @NgayKetThuc";
                        parameters.Add(new SqlParameter("@NgayKetThuc", ngayKetThuc.Value.Date.AddDays(1)));
                    }

                    // Sắp xếp theo thời gian nhập mới nhất
                    truyVan += " ORDER BY pn.ThoiGianNhap DESC";

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
                throw new Exception($"Lỗi SQL khi lấy danh sách phiếu nhập: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách phiếu nhập: {ex.Message}", ex);
            }
        }
        public void ThemPhieuNhap(string maPN, string maNCC, string maNV, DateTime thoiGianNhap, decimal tongTien, DataTable chiTietPhieuNhap)
        {
            using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
            {
                ketNoi.Open();
                using (SqlTransaction giaoDich = ketNoi.BeginTransaction())
                {
                    try
                    {
                        // Thêm hóa đơn
                        string truyVanHoaDon = @"INSERT INTO PhieuNhap (MaPN, MaNCC, MaNV, ThoiGianNhap, TongTien)
                                                VALUES (@MaPN, @MaNCC, @MaNV, @ThoiGianNhap, @TongTien)";
                        using (SqlCommand lenhHoaDon = new SqlCommand(truyVanHoaDon, ketNoi, giaoDich))
                        {
                            lenhHoaDon.Parameters.AddWithValue("@MaPN", maPN);
                            lenhHoaDon.Parameters.AddWithValue("@MaNCC", maNCC);
                            lenhHoaDon.Parameters.AddWithValue("@MaNV", maNV);
                            lenhHoaDon.Parameters.AddWithValue("@ThoiGianNhap", thoiGianNhap);
                            lenhHoaDon.Parameters.AddWithValue("@TongTien", tongTien);
                            lenhHoaDon.ExecuteNonQuery();
                        }

                        // Thêm chi tiết hóa đơn
                        foreach (DataRow dong in chiTietPhieuNhap.Rows)
                        {
                            string truyVanChiTiet = @"INSERT INTO ChiTietPhieuNhap (MaPN, MaSP, SoLuong, DonGia)
                                                     VALUES (@MaPN, @MaSP, @SoLuong, @DonGia)";
                            using (SqlCommand lenhChiTiet = new SqlCommand(truyVanChiTiet, ketNoi, giaoDich))
                            {
                                lenhChiTiet.Parameters.AddWithValue("@MaPN", maPN);
                                lenhChiTiet.Parameters.AddWithValue("@MaSP", dong["MaSP"]);
                                lenhChiTiet.Parameters.AddWithValue("@SoLuong", dong["SoLuong"]);
                                lenhChiTiet.Parameters.AddWithValue("@DonGia", dong["DonGia"]);
                                lenhChiTiet.ExecuteNonQuery();
                            }

                            // Cập nhật số lượng sản phẩm
                            string truyVanCapNhatSP = @"UPDATE SanPham SET SoLuongSP = SoLuongSP + @SoLuong
                                                        WHERE MaSP = @MaSP";
                            using (SqlCommand lenhCapNhatSP = new SqlCommand(truyVanCapNhatSP, ketNoi, giaoDich))
                            {
                                lenhCapNhatSP.Parameters.AddWithValue("@SoLuong", dong["SoLuong"]);
                                lenhCapNhatSP.Parameters.AddWithValue("@MaSP", dong["MaSP"]);
                                lenhCapNhatSP.ExecuteNonQuery();
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
        public void HuyPhieuNhap(string maPN)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"UPDATE PhieuNhap 
                                      SET DaHuy = 1 
                                      WHERE MaPN = @MaPN";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaPN", maPN);
                        lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi huỷ phiếu nhập: " + ex.Message);
            }
        }
        public class XuatChiTietPhieuNhap
        {
            public string tenSP { get; set; }
            public int soLuong { get; set; }
            public decimal donGia { get; set; }
            public decimal thanhTien { get; set; }
        }
    }
}

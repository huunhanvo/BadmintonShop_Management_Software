using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCuaHangCauLong.Data
{
    public class ChiTietPhieuNhap
    {
        private readonly string chuoiKetNoi;

        public ChiTietPhieuNhap(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

        public DataTable LayDanhSachChiTietPhieuNhap(string maPN)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT 
                            ct.MaPN,
                            ct.MaSP,
                            sp.TenSP,
                            ct.SoLuong,
                            ct.DonGia,
                            (ct.SoLuong * ct.DonGia) AS ThanhTien,
                            lsp.TenLoaiSP,
                            th.TenTH
                        FROM ChiTietPhieuNhap ct
                        INNER JOIN SanPham sp ON ct.MaSP = sp.MaSP
                        INNER JOIN LoaiSanPham lsp ON sp.MaLoaiSP = lsp.MaLoaiSP
                        INNER JOIN ThuongHieu th ON sp.MaTH = th.MaTH
                        WHERE ct.MaPN = @MaPN
                        ORDER BY ct.MaSP";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@MaPN", maPN);
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
                throw new Exception($"Lỗi SQL khi lấy chi tiết phiếu nhập: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy chi tiết phiếu nhập: {ex.Message}", ex);
            }
        }

        public void ThemChiTietPhieuNhap(string maPN, string maSP, int soLuong, decimal donGia)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"INSERT INTO ChiTietPhieuNhap (MaPN, MaSP, SoLuong, DonGia)
                                      VALUES (@MaPN, @MaSP, @SoLuong, @DonGia)";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@MaPN", maPN);
                        cmd.Parameters.AddWithValue("@MaSP", maSP);
                        cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                        cmd.Parameters.AddWithValue("@DonGia", donGia);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi thêm chi tiết phiếu nhập: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi thêm chi tiết phiếu nhập: {ex.Message}", ex);
            }
        }

        public void XoaChiTietPhieuNhap(string maPN, string maSP)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "DELETE FROM ChiTietPhieuNhap WHERE MaPN = @MaPN AND MaSP = @MaSP";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@MaPN", maPN);
                        cmd.Parameters.AddWithValue("@MaSP", maSP);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi xóa chi tiết phiếu nhập: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi xóa chi tiết phiếu nhập: {ex.Message}", ex);
            }
        }

        public void CapNhatChiTietPhieuNhap(string maPN, string maSP, int soLuong, decimal donGia)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"UPDATE ChiTietPhieuNhap 
                                      SET SoLuong = @SoLuong, DonGia = @DonGia
                                      WHERE MaPN = @MaPN AND MaSP = @MaSP";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@MaPN", maPN);
                        cmd.Parameters.AddWithValue("@MaSP", maSP);
                        cmd.Parameters.AddWithValue("@SoLuong", soLuong);
                        cmd.Parameters.AddWithValue("@DonGia", donGia);
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi cập nhật chi tiết phiếu nhập: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi cập nhật chi tiết phiếu nhập: {ex.Message}", ex);
            }
        }

        public bool KiemTraChiTietPhieuNhapTonTai(string maPN, string maSP)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "SELECT COUNT(*) FROM ChiTietPhieuNhap WHERE MaPN = @MaPN AND MaSP = @MaSP";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@MaPN", maPN);
                        cmd.Parameters.AddWithValue("@MaSP", maSP);
                        int count = (int)cmd.ExecuteScalar();
                        return count > 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi kiểm tra chi tiết phiếu nhập: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra chi tiết phiếu nhập: {ex.Message}", ex);
            }
        }

        public decimal TinhTongTienPhieuNhap(string maPN)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "SELECT SUM(SoLuong * DonGia) FROM ChiTietPhieuNhap WHERE MaPN = @MaPN";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@MaPN", maPN);
                        var result = cmd.ExecuteScalar();
                        return result != DBNull.Value ? Convert.ToDecimal(result) : 0;
                    }
                }
            }
            catch (SqlException ex)
            {
                throw new Exception($"Lỗi SQL khi tính tổng tiền phiếu nhập: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tính tổng tiền phiếu nhập: {ex.Message}", ex);
            }
        }
    }
}
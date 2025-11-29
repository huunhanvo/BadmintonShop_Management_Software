using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;


namespace QuanLyCuaHangCauLong.Data
{
    public class ChiTietHoaDon
    {
        private readonly String chuoiKetNoi;
        public ChiTietHoaDon(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

        public DataTable LayDanhSachChiTietHoaDon(string maHD)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"
                        SELECT 
                            cthd.MaHD,
                            cthd.MaSP,
                            sp.TenSP,
                            cthd.SoLuong,
                            cthd.DonGia,
                            (cthd.SoLuong * cthd.DonGia) AS ThanhTien
                        FROM ChiTietHoaDon cthd
                        INNER JOIN SanPham sp ON cthd.MaSP = sp.MaSP
                        WHERE cthd.MaHD = @MaHD";

                    using (SqlCommand cmd = new SqlCommand(truyVan, ketNoi))
                    {
                        cmd.Parameters.AddWithValue("@MaHD", maHD);
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
                throw new Exception($"Lỗi SQL khi lấy danh sách chi tiết hóa đơn: {ex.Message}", ex);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh sách chi tiết hóa đơn: {ex.Message}", ex);
            }
        }
    }
}

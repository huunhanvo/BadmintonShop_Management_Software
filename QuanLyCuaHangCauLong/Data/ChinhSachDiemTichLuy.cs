using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace QuanLyCuaHangCauLong.Data
{
    public class ChinhSachDiemTichLuy
    {
        private readonly string chuoiKetNoi;

        public ChinhSachDiemTichLuy(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

        public DataTable LayDanhSachChinhSachDiemTichLuy()
        {
            return LayDanhSachChinhSachDiemTichLuy(null);
        }
        // Lấy danh sách chính sách điểm tích lũy cho DiemTichLuyUC
        public DataTable LayDanhSachChinhSachDiemTichLuy(string tuKhoa)
        {
            DataTable bangDuLieu = new DataTable();
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"SELECT MaCSDTL, DiemToiThieu, PhanTramGiam
                                     FROM ChinhSachDiemTichLuy
                                     WHERE DaXoa = 0";

                    // Thêm điều kiện lọc
                    if (!string.IsNullOrEmpty(tuKhoa))
                    {
                        truyVan += " AND (MaCSDTL LIKE @TuKhoa OR DiemToiThieu LIKE @TuKhoa)";
                    }

                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
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
                throw new Exception("Lỗi khi lấy danh sách điểm tích luỹ: " + ex.Message);
            }
            return bangDuLieu;
        }
        // Lấy danh sách chính sách điểm tích lũy cho ThanhToanForm
        public DataTable LayDanhSachChinhSachDiemTichLuy(int diemTichLuy)
        {
            DataTable bangDuLieu = new DataTable();
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"SELECT MaCSDTL, DiemToiThieu, PhanTramGiam
                                      FROM ChinhSachDiemTichLuy
                                      WHERE DiemToiThieu <= @DiemTichLuy AND DaXoa = 0
                                      ORDER BY DiemToiThieu DESC";

                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@DiemTichLuy", diemTichLuy);
                        using (SqlDataAdapter boDieuChinh = new SqlDataAdapter(lenh))
                        {
                            boDieuChinh.Fill(bangDuLieu);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách chính sách điểm tích lũy: " + ex.Message);
            }
            return bangDuLieu;
        }
        //Thêm sản chính sách điểm tích luỹ
        public void ThemChinhSachDiem(string maCSDTL, string diemToiThieu, string phanTramGiam)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"INSERT INTO ChinhSachDiemTichLuy (MaCSDTL, DiemToiThieu, PhanTramGiam)
                                     VALUES (@MaCSDTL, @DiemToiThieu, @PhanTramGiam)";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaCSDTL", maCSDTL);
                        lenh.Parameters.AddWithValue("@DiemToiThieu", diemToiThieu);
                        lenh.Parameters.AddWithValue("@PhanTramGiam", phanTramGiam);

                        lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm chính sách điểm tích luỹ: " + ex.Message);
            }
        }

        // Cập nhật chính sách điểm tích luỹ
        public void CapNhatChinhSachDiem(string maCSDTL, string diemToiThieu, string phanTramGiam)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"UPDATE ChinhSachDiemTichLuy
                                      SET DiemToiThieu = @DiemToiThieu, PhanTramGiam = @PhanTramGiam
                                      WHERE MaCSDTL = @MaCSDTL";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaCSDTL", maCSDTL);
                        lenh.Parameters.AddWithValue("@DiemToiThieu", diemToiThieu);
                        lenh.Parameters.AddWithValue("@PhanTramGiam", phanTramGiam);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật chính sách điểm tích luỹ: " + ex.Message);
            }
        }

        // Xóa chính sách điểm tích luỹ
        public void XoaChinhSachDiem(string maCSDTL)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "DELETE FROM ChinhSachDiemTichLuy WHERE MaCSDTL = @MaCSDTL";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaCSDTL", maCSDTL);
                        int soDongAnhHuong = lenh.ExecuteNonQuery();
                        if (soDongAnhHuong == 0)
                        {
                            throw new Exception("Không tìm thấy chính sách điểm tích luỹ với mã này!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa chính sách điểm tích luỹ: " + ex.Message);
            }
        }
    }
}

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
    public class NhaCungCap

    {
        private readonly string chuoiKetNoi;

        public NhaCungCap(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

        public DataTable LayDanhSachNhaCungCap()
        {
            return LayDanhSachNhaCungCap(null);
        }

        public DataTable LayDanhSachNhaCungCap(string tuKhoa)
        {
            DataTable bangDuLieu = new DataTable();
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"SELECT MaNCC, TenNCC, DiaChi, SDT
                                     FROM NhaCungCap
                                     WHERE DaXoa = 0";

                    // Thêm điều kiện lọc
                    if (!string.IsNullOrEmpty(tuKhoa))
                    {
                        truyVan += " AND (MaNCC LIKE @TuKhoa OR TenNCC LIKE @TuKhoa)";
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
                throw new Exception("Lỗi khi lấy danh sách nhà cung cấp: " + ex.Message);
            }
            return bangDuLieu;
        }

        //Thêm sản chính sách nhà cung cấp
        public void ThemNhaCungCap(string maNCC, string tenNCC, string diaChi, string sdt)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"INSERT INTO NhaCungCap (MaNCC, TenNCC, DiaChi, SDT)
                                     VALUES (@MaNCC, @TenNCC, @DiaChi, @SDT)";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaNCC", maNCC);
                        lenh.Parameters.AddWithValue("@TenNCC", tenNCC);
                        lenh.Parameters.AddWithValue("@DiaChi", diaChi);
                        lenh.Parameters.AddWithValue("@SDT", sdt);

                        lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi thêm chính sách nhà cung cấp: " + ex.Message);
            }
        }

        // Cập nhật chính sách nhà cung cấp
        public void CapNhatNhaCungCap(string maNCC, string tenNCC, string diaChi, string sdt)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = @"UPDATE NhaCungCap
                                      SET TenNCC = @TenNCC, DiaChi = @DiaChi, SDT = @SDT
                                      WHERE MaNCC = @MaNCC";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaNCC", maNCC);
                        lenh.Parameters.AddWithValue("@TenNCC", tenNCC);
                        lenh.Parameters.AddWithValue("@DiaChi", diaChi);
                        lenh.Parameters.AddWithValue("@SDT", sdt);
                        lenh.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi cập nhật chính sách nhà cung cấp: " + ex.Message);
            }
        }

        // Xóa chính sách nhà cung cấp
        public void XoaNhaCungCap(string maNCC)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "DELETE FROM NhaCungCap WHERE MaNCC = @MaNCC";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@MaNCC", maNCC);
                        int soDongAnhHuong = lenh.ExecuteNonQuery();
                        if (soDongAnhHuong == 0)
                        {
                            throw new Exception("Không tìm thấy chính sách nhà cung cấp với mã này!");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi xóa chính sách nhà cung cấp: " + ex.Message);
            }
        }
    }
}

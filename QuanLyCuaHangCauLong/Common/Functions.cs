using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Configuration;
using static System.ComponentModel.Design.ObjectSelectorEditor;
using System.Windows.Forms;

namespace QuanLyCuaHangCauLong.Common
{
    public static class Functions
    {
        private static readonly string ChuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;


        // Tạo mã tự tăng cho bảng, ví dụ: SP001, KH001
        public static string TaoMaTuTang(string tenBang, string cotMa, string tienTo, int soChuSo)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(ChuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan;
                    if(tienTo == "KH")
                        truyVan = $"SELECT MAX(MaKH) FROM KhachHang WHERE MaKH LIKE 'KH%'";
                    else
                        truyVan = $"SELECT MAX({cotMa}) FROM {tenBang}";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        object ketQua = lenh.ExecuteScalar();
                        if (ketQua != null && !DBNull.Value.Equals(ketQua))
                        {
                            string maCuoi = ketQua.ToString();
                            if (maCuoi.StartsWith(tienTo) && int.TryParse(maCuoi.Substring(tienTo.Length), out int so))
                            {
                                return $"{tienTo}{(so + 1).ToString("D" + soChuSo)}";
                            }
                        }
                        return $"{tienTo}{1.ToString("D" + soChuSo)}";
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi tạo mã tự tăng cho {tenBang}: {ex.Message}");
            }
        }

        // Kiểm tra mã có trùng trong bảng không
        public static bool KiemTraMaTrung(string tenBang, string cotMa, string ma)
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(ChuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = $"SELECT COUNT(*) FROM {tenBang} WHERE {cotMa} = @Ma";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@Ma", ma);
                        return (int)lenh.ExecuteScalar() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi kiểm tra mã trùng trong {tenBang}: {ex.Message}");
            }
        }

        // Kiểm tra chuỗi không trống
        public static bool KiemTraChuoiKhongTrong(string giaTri, string tenTruong, out string thongBaoLoi)
        {
            if (string.IsNullOrWhiteSpace(giaTri))
            {
                thongBaoLoi = $"Vui lòng nhập {tenTruong}!";
                return false;
            }
            thongBaoLoi = string.Empty;
            return true;
        }

        // Kiểm tra số thực dương
        public static bool KiemTraSoThucDuong(string giaTri, string tenTruong, out string thongBaoLoi, out decimal ketQua)
        {
            if (!decimal.TryParse(giaTri, out ketQua) || ketQua <= 0)
            {
                thongBaoLoi = $"{tenTruong} phải là số thực dương!";
                return false;
            }
            thongBaoLoi = string.Empty;
            return true;
        }
    }
}

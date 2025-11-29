using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCuaHangCauLong.Forms
{
    public partial class LoginForm : Form
    {
        public LoginForm()
        {
            InitializeComponent();
        }

        private void DangNhap_btn_Click(object sender, EventArgs e)
        {
            string tenDangNhap = TenDangNhap_txt.Text.Trim();
            string matKhau = MatKhau_txt.Text.Trim();

            if (string.IsNullOrEmpty(tenDangNhap) || string.IsNullOrEmpty(matKhau))
            {
                ThongBao_lbl.Text = "Vui lòng nhập đầy đủ tên đăng nhập và mật khẩu!!!!";
                TenDangNhap_txt.Focus();
                ThongBao_lbl.ForeColor = Color.Red;
                return;
            }

            string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
            using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
            {
                try
                {
                    ketNoi.Open();
                    string truyVan = "SELECT MaNV, TenNV, VaiTro, CoTheDangNhap FROM NhanVien WHERE TenDangNhap = @TenDangNhap AND MatKhau = @MatKhau AND CoTheDangNhap = 1 AND DaXoa = 0";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        lenh.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        lenh.Parameters.AddWithValue("@MatKhau", matKhau);

                        using (SqlDataReader doc = lenh.ExecuteReader())
                        {
                            if (doc.Read())
                            {
                                string maNV = doc["MaNV"].ToString();
                                string tenNV = doc["TenNV"].ToString();
                                string vaiTro = doc["VaiTro"].ToString();

                                this.Hide();
                                MainForm mainForm = new MainForm(maNV, tenNV, vaiTro);
                                mainForm.ShowDialog();
                                this.Close();
                            }
                            else
                            {
                                ThongBao_lbl.Text = "Tên đăng nhập hoặc mật khẩu đã sai, vui lòng nhập lại!!!";
                                TenDangNhap_txt.Focus();
                                ThongBao_lbl.ForeColor = Color.Red;
                                TenDangNhap_txt.Text = "";
                                MatKhau_txt.Text = "";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi kết nối cơ sở dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        //Ẩn-Hiện mật khẩu
        private void HienMK_ckb_CheckStateChanged(object sender, EventArgs e)
        {
            if (HienMK_ckb.Checked)
            {
                // Hiển thị mật khẩu dạng văn bản thường
                MatKhau_txt.PasswordChar = '\0';
            }
            else
            {
                // Ẩn mật khẩu dạng ký tự *
                MatKhau_txt.PasswordChar = '*';
            }
        }
       
        private void TenDangNhap_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                MatKhau_txt.Focus();
        }

        private void MatKhau_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                DangNhap_btn_Click(sender, e);
        }
    }
}

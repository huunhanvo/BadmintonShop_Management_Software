using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyCuaHangCauLong.UserControls;

namespace QuanLyCuaHangCauLong.Forms
{
    public partial class MainForm : Form
    {
        // Biến lưu thông tin người dùng
        private readonly string maNV, tenNV, vaiTro;

        // Các UserControl cho các chức năng khác nhau
        private UserControls.BanHang.BanHangUC ucBanHang;
        private UserControls.HoaDon.HoaDonUC ucHoaDon;
        private UserControls.KhachHang.KhachHangUC ucKhachHang;
        private UserControls.KhachHang.LichSuUC ucLichSu;
        private UserControls.SanPham.SanPhamUC ucSanPham;
        private UserControls.SanPham.NhapKhoUC ucNhapKho;
        private UserControls.SanPham.PhieuNhapUC ucPhieuNhap;
        private UserControls.Dashboard.DashboardUC ucDashboard;
        private UserControls.ThongKe.DoanhThuUC ucDoanhThuTK;
        private UserControls.ThongKe.KhachHangUC ucKhachHangTK;
        private UserControls.ThongKe.NhanVienUC ucNhanVienTK;
        private UserControls.QuanLy.NhanVienUC quanLyNhanVienUC;
        private UserControls.QuanLy.DiemTichLuyUC quanLyDiemTichLuyUC;
        private UserControls.QuanLy.NhaCungCapUC quanLyCungCapUC;

        // Constructor khởi tạo form
        public MainForm(string maNV, string tenNV, string vaiTro)
        {
            InitializeComponent();
            this.maNV = maNV;
            this.tenNV = tenNV;
            this.vaiTro = vaiTro;
            NguoiDangNhap_lbl.Text = "Nhân viên: " + tenNV + "(" + vaiTro + ")";

            // Khởi tạo và thiết lập các UserControl
            ucBanHang = new UserControls.BanHang.BanHangUC(maNV, tenNV);
            ucBanHang.Dock = DockStyle.Fill;

            ucHoaDon = new UserControls.HoaDon.HoaDonUC();
            ucHoaDon.Dock = DockStyle.Fill;

            ucKhachHang = new UserControls.KhachHang.KhachHangUC();
            ucKhachHang.Dock = DockStyle.Fill;
            ucLichSu = new UserControls.KhachHang.LichSuUC();
            ucLichSu.Dock = DockStyle.Fill;

            ucSanPham = new UserControls.SanPham.SanPhamUC();
            ucSanPham.Dock = DockStyle.Fill;
            ucNhapKho = new UserControls.SanPham.NhapKhoUC(maNV, tenNV);
            ucNhapKho.Dock = DockStyle.Fill;
            ucPhieuNhap = new UserControls.SanPham.PhieuNhapUC();
            ucPhieuNhap.Dock = DockStyle.Fill;

            ucDashboard = new UserControls.Dashboard.DashboardUC();
            ucDashboard.Dock = DockStyle.Fill;

            ucDoanhThuTK = new UserControls.ThongKe.DoanhThuUC(tenNV);
            ucDoanhThuTK.Dock = DockStyle.Fill;
            ucKhachHangTK = new UserControls.ThongKe.KhachHangUC(tenNV);
            ucKhachHangTK.Dock = DockStyle.Fill;
            ucNhanVienTK = new UserControls.ThongKe.NhanVienUC(tenNV);
            ucNhanVienTK.Dock = DockStyle.Fill;

            quanLyNhanVienUC = new UserControls.QuanLy.NhanVienUC();
            quanLyNhanVienUC.Dock = DockStyle.Fill;
            quanLyDiemTichLuyUC = new UserControls.QuanLy.DiemTichLuyUC();
            quanLyDiemTichLuyUC.Dock = DockStyle.Fill;
            quanLyCungCapUC = new UserControls.QuanLy.NhaCungCapUC();
            quanLyCungCapUC.Dock = DockStyle.Fill;
        }

        // Xử lý sự kiện khi form được tải
        private void MainForm_Load(object sender, EventArgs e)
        {
            Content_panel.Controls.Add(ucBanHang);
            ucBanHang.BringToFront();
            VaiTroDangNhap();
        }

        // Phân quyền dựa trên vai trò người dùng
        private void VaiTroDangNhap()
        {
            if (vaiTro != "Admin")
            {
                // Vô hiệu hóa các nút cho người dùng không phải Admin
                SPSanPham_btn.Enabled = false;
                SPNhapKho_btn.Enabled = false;
                SPPhieuNhap_btn.Enabled = false;
                DBDashboard_btn.Enabled = false;
                TKDoanhThu_btn.Enabled = false;
                TKKhachHang_btn.Enabled = false;
                TKNhanVien_btn.Enabled = false;
                QLDimeTichLuy_btn.Enabled = false;
                QLNhaCungCap_btn.Enabled = false;
                QLNhanVien_btn.Enabled = false;
            }
            else
            {
                // Kích hoạt các nút cho Admin
                SPSanPham_btn.Enabled = true;
                SPNhapKho_btn.Enabled = true;
                SPPhieuNhap_btn.Enabled = true;
                DBDashboard_btn.Enabled = true;
                TKDoanhThu_btn.Enabled = true;
                TKKhachHang_btn.Enabled = true;
                TKNhanVien_btn.Enabled = true;
                QLDimeTichLuy_btn.Enabled = true;
                QLNhaCungCap_btn.Enabled = true;
                QLNhanVien_btn.Enabled = true;
            }
        }

        // Phương thức chung để chuyển đổi UserControl
        private void SwitchUserControl(UserControl uc)
        {
            Content_panel.Controls.Clear();
            Content_panel.Controls.Add(uc);
            uc.BringToFront();
        }

        // Các sự kiện click nút để chuyển đổi UserControl
        private void BHBanHang_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucBanHang);
        private void HDHoaDon_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucHoaDon);
        private void KHKhachHang_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucKhachHang);
        private void KHLichSu_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucLichSu);
        private void SPSanPham_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucSanPham);
        private void SPNhapKho_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucNhapKho);
        private void SPPhieuNhap_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucPhieuNhap);
        private void DBDashboard_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucDashboard);
        private void TKDoanhThu_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucDoanhThuTK);
        private void TKKhachHang_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucKhachHangTK);
        private void TKNhanVien_btn_Click(object sender, EventArgs e) => SwitchUserControl(ucNhanVienTK);
        private void QLNhanVien_btn_Click(object sender, EventArgs e) => SwitchUserControl(quanLyNhanVienUC);
        private void QLDimeTichLuy_btn_Click(object sender, EventArgs e) => SwitchUserControl(quanLyDiemTichLuyUC);
        private void QLNhaCungCap_btn_Click(object sender, EventArgs e) => SwitchUserControl(quanLyCungCapUC);


        // Xử lý sự kiện nút Thoát
        private void Thoat_btn_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Bạn có chắc chắn muốn thoát?", "Xác nhận thoát", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;
            }
            this.Hide();
            LoginForm loginForm = new LoginForm();
            loginForm.ShowDialog();
            this.Close();
        }
    }
}
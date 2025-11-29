using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyCuaHangCauLong.Common;
using Microsoft.Reporting.WinForms;

namespace QuanLyCuaHangCauLong.UserControls.ThongKe
{
    public partial class NhanVienUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly ThongKeNhanVien thongKeNhanVien;
        private string tenNV;

        // Constructor khởi tạo UserControl
        public NhanVienUC(string tenNV)
        {
            this.tenNV = tenNV;
            InitializeComponent();
            thongKeNhanVien = new ThongKeNhanVien(chuoiKetNoi);

            // Thiết lập danh sách vai trò
            VaiTro_cb.Items.AddRange(new string[] { "Tất cả", "Admin", "Thu ngân", "Nhân viên" });
            VaiTro_cb.SelectedIndex = 0;

            // Thiết lập danh sách trạng thái làm việc
            TrangThaiLamViec_cb.Items.AddRange(new string[] { "Tất cả", "Đang làm", "Nghĩ làm" });
            TrangThaiLamViec_cb.SelectedIndex = 0;

            // Load dữ liệu thống kê cho 6 cards
            var thongKe = thongKeNhanVien.LayThongKeChoCacTheNhanVien();
            TongNhanVien_lbl.Text = thongKe.TongSoNhanVien.ToString("N0");
            Admin_lbl.Text = thongKe.NhanVienAdmin.ToString("N0");
            ThuNgan_lbl.Text = thongKe.NhanVienThuNgan.ToString("N0");
            NhanVien_lbl.Text = thongKe.NhanVienThuong.ToString("N0");
            DangLam_lbl.Text = thongKe.NhanVienConLam.ToString("N0");
            NghiLam_lbl.Text = thongKe.NhanVienNghiLam.ToString("N0");

            // Vô hiệu hóa nút xuất báo cáo
            XuatBaoCao_btn.Enabled = false;
        }

        // Hiển thị danh sách nhân viên theo tiêu chí
        private void HienThiDanhSach()
        {
            try
            {
                // Lấy thông tin lọc
                int tuoiBatDau = Convert.ToInt32(TuoiBatDau_txt.Text);
                int tuoiKetThuc = Convert.ToInt32(TuoiKetThuc_txt.Text);
                string vaiTro = VaiTro_cb.SelectedItem?.ToString();
                string trangThai = TrangThaiLamViec_cb.SelectedItem?.ToString();

                // Lấy dữ liệu nhân viên theo tiêu chí
                DataTable ketQua = thongKeNhanVien.LayDanhSachNhanVienTheoTieuChi(tuoiBatDau, tuoiKetThuc, vaiTro, trangThai);

                // Cấu hình DataGridView
                DanhSach_dgv.AutoGenerateColumns = false;
                DanhSach_dgv.DataSource = ketQua;

                // Gán DataPropertyName cho các cột
                DanhSach_dgv.Columns["ThoiGian"].DataPropertyName = "NgaySinh";
                DanhSach_dgv.Columns["NhanVien"].DataPropertyName = "TenNV";
                DanhSach_dgv.Columns["VaiTro"].DataPropertyName = "VaiTro";
                DanhSach_dgv.Columns["TrangThai"].DataPropertyName = "TrangThai";
                DanhSach_dgv.Columns["SoHoaDon"].DataPropertyName = "SoHoaDon";
                DanhSach_dgv.Columns["TongTien"].DataPropertyName = "TongTien";

                // Định dạng cột TongTien
                DanhSach_dgv.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                DanhSach_dgv.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Làm mới DataGridView
                DanhSach_dgv.Refresh();

                // Kiểm tra nếu không có dữ liệu
                if (ketQua.Rows.Count == 0)
                {
                    MessageBox.Show("Không có dữ liệu phù hợp với tiêu chí lọc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                // Kích hoạt nút xuất báo cáo
                XuatBaoCao_btn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hiển thị danh sách: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện thống kê nhân viên
        private void ThongKe_btn_Click(object sender, EventArgs e)
        {
            try
            {
                HienThiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thống kê: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Load lại dữ liệu
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            //LoaiKhachHang_cb.SelectedIndex = 0;
            //TrangThaiGiaoDich_cb.SelectedIndex = 0;
            //MucDiemTichLuy_txt.Text = "0";
            //DanhSach_dgv.DataSource = null;
            //LoadDuLieuThongKe5Card();
            
        }
        // Xử lý sự kiện xuất báo cáo
        private void XuatBaoCao_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy thông tin báo cáo
                DateTime ngayLap = DateTime.Today;
                string tuoiBatDau = TuoiBatDau_txt.Text.ToString();
                string tuoiKetThuc = TuoiKetThuc_txt.Text.ToString();
                string vaiTro = VaiTro_cb.Text ?? "Tất cả";
                string trangThaiLamViec = TrangThaiLamViec_cb.Text ?? "Tất cả";

                // Tính tổng tiền
                decimal tongTien = 0;
                foreach (DataGridViewRow row in DanhSach_dgv.Rows)
                {
                    if (!row.IsNewRow && row.Cells["TongTien"].Value != null)
                        tongTien += Convert.ToDecimal(row.Cells["TongTien"].Value);
                }

                // Tạo danh sách báo cáo
                var danhSachBaoCao = new List<BaoCaoNhanVien>();
                foreach (DataGridViewRow row in DanhSach_dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        danhSachBaoCao.Add(new BaoCaoNhanVien
                        {
                            thoiGian = row.Cells["ThoiGian"].Value?.ToString() ?? "N/A",
                            tenNV = row.Cells["NhanVien"].Value?.ToString() ?? "N/A",
                            vaiTro = row.Cells["VaiTro"].Value?.ToString() ?? "N/A",
                            trangThai = row.Cells["TrangThai"].Value?.ToString() ?? "N/A",
                            soHoaDon = Convert.ToInt32(row.Cells["SoHoaDon"].Value ?? 0),
                            tongTien = Convert.ToDecimal(row.Cells["TongTien"].Value ?? 0)
                        });
                    }
                }

                // Tạo nguồn dữ liệu cho báo cáo
                ReportDataSource baoCaoNguonDuLieu = new ReportDataSource
                {
                    Name = "dtsDanhSachBaoCao",
                    Value = danhSachBaoCao
                };

                // Tạo tham số cho báo cáo
                ReportParameter[] thamSo = new ReportParameter[]
                {
                    new ReportParameter("prmNguoiLap", tenNV),
                    new ReportParameter("prmNgayLap", ngayLap.ToString("dd/MM/yyyy")),
                    new ReportParameter("prmVaiTro", vaiTro),
                    new ReportParameter("prmTrangThai", trangThaiLamViec),
                    new ReportParameter("prmTuoiBatDau", tuoiBatDau),
                    new ReportParameter("prmTuoiKetThuc", tuoiKetThuc),
                    new ReportParameter("prmTongTien", tongTien.ToString("N0"))
                };

                // Hiển thị form báo cáo
                var formBaoCao = new BaoCaoForm(baoCaoNguonDuLieu, thamSo, "nhanvien");
                formBaoCao.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện KeyPress cho textbox tuổi bắt đầu
        private void TuoiBatDau_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        // Xử lý sự kiện KeyPress cho textbox tuổi kết thúc
        private void TuoiKetThuc_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
            if (e.KeyChar == '.' && (sender as TextBox).Text.Contains("."))
            {
                e.Handled = true;
            }
        }

        
    }
}
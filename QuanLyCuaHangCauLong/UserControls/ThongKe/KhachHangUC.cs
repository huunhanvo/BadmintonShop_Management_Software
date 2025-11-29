using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.UserControls.SanPham;
using QuanLyCuaHangCauLong.Common;
using QuanLyCuaHangCauLong.UserControls.Dashboard;
using Microsoft.Reporting.WinForms;
using QuanLyCuaHangCauLong.Forms;
using System.Configuration;

namespace QuanLyCuaHangCauLong.UserControls.ThongKe
{
    public partial class KhachHangUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly ThongKeKhachHang thongKeKhachHang;
        private string tenNV;

        // Constructor khởi tạo UserControl
        public KhachHangUC(string tenNV)
        {
            this.tenNV = tenNV;
            InitializeComponent();
            thongKeKhachHang = new ThongKeKhachHang(chuoiKetNoi);

            // Thiết lập danh sách loại khách hàng
            LoaiKhachHang_cb.Items.AddRange(new string[] { "Tất cả", "Lẻ", "Đã có tài khoản" });
            LoaiKhachHang_cb.SelectedIndex = 0;

            // Thiết lập danh sách trạng thái giao dịch
            TrangThaiGiaoDich_cb.Items.AddRange(new string[] { "Tất cả", "Còn giao dịch", "Ngưng giao dịch" });
            TrangThaiGiaoDich_cb.SelectedIndex = 0;

            LoadDuLieuThongKe5Card();
        }
        //Load dữ liệu thống kê cho 5 cards
        private void LoadDuLieuThongKe5Card()
        {
            var thongKe = thongKeKhachHang.LayThongKeChoCacTheKhachHang();
            TongkhachHang_lbl.Text = thongKe.TongSoKhachHang.ToString("N0");
            KHDaCoTaiKhoan_lbl.Text = thongKe.KhachHangCoTaiKhaon.ToString("N0");
            KHLe_lbl.Text = thongKe.KhachHangLe.ToString("N0");
            KHConGiaoDich_lbl.Text = thongKe.KhachHangConGiaoDich.ToString("N0");
            KHNgungGiaoDich_lbl.Text = thongKe.KhachHangNgungGiaoDich.ToString("N0");
        }
        // Hiển thị danh sách khách hàng theo tiêu chí
        private void HienThiDanhSach()
        {
            try
            {
                // Lấy thông tin lọc
                string loaiKhachHang = LoaiKhachHang_cb.SelectedItem?.ToString();
                string trangThaiGiaoDich = TrangThaiGiaoDich_cb.SelectedItem?.ToString();
                int diemToiThieu = Convert.ToInt32(MucDiemTichLuy_txt.Text);

                // Lấy dữ liệu khách hàng theo tiêu chí
                DataTable ketQua = thongKeKhachHang.LayDanhSachKhachHangTheoTieuChi(loaiKhachHang, trangThaiGiaoDich, diemToiThieu);

                // Cấu hình DataGridView
                DanhSach_dgv.AutoGenerateColumns = false;
                DanhSach_dgv.DataSource = ketQua;

                // Gán DataPropertyName cho các cột
                DanhSach_dgv.Columns["ThoiGian"].DataPropertyName = "NgayTao";
                DanhSach_dgv.Columns["TenKH"].DataPropertyName = "TenKH";
                DanhSach_dgv.Columns["LoaiKhachHang"].DataPropertyName = "LoaiKhachHang";
                DanhSach_dgv.Columns["TrangThai"].DataPropertyName = "TrangThai";
                DanhSach_dgv.Columns["DiemTichLuy"].DataPropertyName = "DiemTichLuy";
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
        //Load lại dữ liệu
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            LoaiKhachHang_cb.SelectedIndex = 0;
            TrangThaiGiaoDich_cb.SelectedIndex = 0;
            MucDiemTichLuy_txt.Text = "0";
            DanhSach_dgv.DataSource = null;
            LoadDuLieuThongKe5Card();
        }
        // Xử lý sự kiện thống kê khách hàng
        private void ThongKe_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (MucDiemTichLuy_txt.Text == "" || MucDiemTichLuy_txt.Text == null)
                {
                    MessageBox.Show("Vui lòng nhập mức điểm tích luỹ", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    MucDiemTichLuy_txt.Focus();
                } 
                else HienThiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thống kê: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện xuất báo cáo
        private void XuatBaoCao_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy thông tin báo cáo
                DateTime ngayLap = DateTime.Today;
                string loaiKhachHang = LoaiKhachHang_cb.Text ?? "Tất cả";
                string trangThaiGiaoDich = TrangThaiGiaoDich_cb.Text ?? "Tất cả";
                string mucDiemTichLuy = MucDiemTichLuy_txt.Text.ToString();

                // Tính tổng tiền
                decimal tongTien = 0;
                foreach (DataGridViewRow row in DanhSach_dgv.Rows)
                {
                    if (!row.IsNewRow && row.Cells["TongTien"].Value != null)
                        tongTien += Convert.ToDecimal(row.Cells["TongTien"].Value);
                }

                // Tạo danh sách báo cáo
                var danhSachBaoCao = new List<BaoCaoKhachHang>();
                foreach (DataGridViewRow row in DanhSach_dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        danhSachBaoCao.Add(new BaoCaoKhachHang
                        {
                            thoiGian = row.Cells["ThoiGian"].Value?.ToString() ?? "N/A",
                            tenKH = row.Cells["TenKH"].Value?.ToString() ?? "N/A",
                            loaiKH = row.Cells["LoaiKhachHang"].Value?.ToString() ?? "N/A",
                            trangThai = row.Cells["TrangThai"].Value?.ToString() ?? "N/A",
                            diemTichLuy = Convert.ToInt32(row.Cells["DiemTichLuy"].Value ?? 0),
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
                    new ReportParameter("prmLoaiKhachHang", loaiKhachHang),
                    new ReportParameter("prmTrangThaiGiaoDich", trangThaiGiaoDich),
                    new ReportParameter("prmMucDiemTichLuy", mucDiemTichLuy),
                    new ReportParameter("prmTongTien", tongTien.ToString("N0"))
                };

                // Hiển thị form báo cáo
                var formBaoCao = new BaoCaoForm(baoCaoNguonDuLieu, thamSo, "khachhang");
                formBaoCao.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện KeyPress cho textbox điểm tích lũy
        private void MucDiemTichLuy_txt_KeyPress(object sender, KeyPressEventArgs e)
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
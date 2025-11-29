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
using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.UserControls;

namespace QuanLyCuaHangCauLong.UserControls.KhachHang
{
    public partial class LichSuUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.KhachHang khachHang;
        private readonly Data.HoaDon hoaDon;
        private readonly Data.ChiTietHoaDon chiTietHoaDon;

        // Constructor khởi tạo UserControl
        public LichSuUC()
        {
            InitializeComponent();
            khachHang = new Data.KhachHang(chuoiKetNoi);
            hoaDon = new Data.HoaDon(chuoiKetNoi);
            chiTietHoaDon = new Data.ChiTietHoaDon(chuoiKetNoi);
        }

        // Xử lý sự kiện khi UserControl được tải
        private void LichSuUC_Load(object sender, EventArgs e)
        {
            HienThiDanhSachKhachHang();
        }

        // Hiển thị danh sách khách hàng lên DataGridView
        private void HienThiDanhSachKhachHang(string tuKhoa = null)
        {
            try
            {
                // Cấu hình và load dữ liệu cho DataGridView khách hàng
                DanhSachKH_dgv.AutoGenerateColumns = false;
                DanhSachKH_dgv.DataSource = khachHang.LayDanhSachKhachHang("Đã có tài khoản", tuKhoa, true);

                // Ánh xạ cột với dữ liệu
                DanhSachKH_dgv.Columns["MaKH"].DataPropertyName = "MaKH";
                DanhSachKH_dgv.Columns["TenKH"].DataPropertyName = "TenKH";
                DanhSachKH_dgv.Columns["DiaChi"].DataPropertyName = "DiaChi";
                DanhSachKH_dgv.Columns["SDT"].DataPropertyName = "SDT";
                DanhSachKH_dgv.Columns["Email"].DataPropertyName = "Email";
                DanhSachKH_dgv.Columns["NgayTao"].DataPropertyName = "NgayTao";
                DanhSachKH_dgv.Columns["DiemTichLuy"].DataPropertyName = "DiemTichLuy";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách khách hàng: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hiển thị danh sách hóa đơn của khách hàng lên DataGridView
        private void HienThiDanhSachHoaDon(string maKH)
        {
            try
            {
                // Cấu hình và load dữ liệu cho DataGridView hóa đơn
                DanhSachHoaDon_dgv.AutoGenerateColumns = false;
                DanhSachHoaDon_dgv.DataSource = hoaDon.LayDanhSachHoaDon(maKH, null, null, null);

                // Ánh xạ cột với dữ liệu
                DanhSachHoaDon_dgv.Columns["MaHD"].DataPropertyName = "MaHD";
                DanhSachHoaDon_dgv.Columns["TenNV"].DataPropertyName = "TenNV";
                DanhSachHoaDon_dgv.Columns["TenKH_HD"].DataPropertyName = "TenKH";
                DanhSachHoaDon_dgv.Columns["NgayLap"].DataPropertyName = "NgayLap";
                DanhSachHoaDon_dgv.Columns["TongTien"].DataPropertyName = "TongTien";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách hoá đơn: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hiển thị chi tiết hóa đơn lên DataGridView
        private void HienThiDanhSachChiTietHoaDon(string maHD)
        {
            try
            {
                // Cấu hình và load dữ liệu cho DataGridView chi tiết hóa đơn
                DanhSachChiTietHoaDon_dgv.AutoGenerateColumns = false;
                DanhSachChiTietHoaDon_dgv.DataSource = chiTietHoaDon.LayDanhSachChiTietHoaDon(maHD);

                // Ánh xạ cột với dữ liệu
                DanhSachChiTietHoaDon_dgv.Columns["MaSP"].DataPropertyName = "MaSP";
                DanhSachChiTietHoaDon_dgv.Columns["TenSP"].DataPropertyName = "TenSP";
                DanhSachChiTietHoaDon_dgv.Columns["SoLuong"].DataPropertyName = "SoLuong";
                DanhSachChiTietHoaDon_dgv.Columns["DonGia"].DataPropertyName = "DonGia";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách hoá đơn: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện click vào ô trong DataGridView khách hàng
        private void DanhSachKH_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                HienThiDanhSachHoaDon(DanhSachKH_dgv.Rows[e.RowIndex].Cells["MaKH"].Value?.ToString());
            }
        }

        // Xử lý sự kiện click vào ô trong DataGridView hóa đơn
        private void DanhSachHoaDon_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                HienThiDanhSachChiTietHoaDon(DanhSachHoaDon_dgv.Rows[e.RowIndex].Cells["MaHD"].Value?.ToString());
                var tongTien = DanhSachHoaDon_dgv.Rows[e.RowIndex].Cells["TongTien"].Value.ToString();
                TongTien_lbl.Text = Convert.ToDecimal(tongTien).ToString("N0") + "đ";
            }
        }

        // Xử lý sự kiện tìm kiếm khách hàng
        private void TimKiem_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy từ khóa tìm kiếm và hiển thị danh sách
                string tuKhoa = TimKiem_txt.Text.Trim();
                HienThiDanhSachKhachHang(tuKhoa);

                // Thông báo nếu không tìm thấy kết quả
                if (DanhSachKH_dgv.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy khách hàng nào khớp với tiêu chí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm khách hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện làm mới danh sách
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            // Xóa từ khóa tìm kiếm và làm mới các DataGridView
            TimKiem_txt.Clear();
            HienThiDanhSachKhachHang();
            DanhSachHoaDon_dgv.DataSource = null;
            DanhSachChiTietHoaDon_dgv.DataSource = null;
            TongTien_lbl.Text = "0";
        }

    }
}
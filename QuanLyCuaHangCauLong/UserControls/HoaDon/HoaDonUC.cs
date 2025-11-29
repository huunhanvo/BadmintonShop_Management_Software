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
using Microsoft.Reporting.WinForms;
using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.Forms;
using QuanLyCuaHangCauLong.UserControls;

namespace QuanLyCuaHangCauLong.UserControls.HoaDon
{
    public partial class HoaDonUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.HoaDon hoaDon;
        private readonly Data.ChiTietHoaDon chiTietHoaDon;

        // Constructor khởi tạo UserControl
        public HoaDonUC()
        {
            InitializeComponent();
            hoaDon = new Data.HoaDon(chuoiKetNoi);
            chiTietHoaDon = new Data.ChiTietHoaDon(chuoiKetNoi);
        }

        // Xử lý sự kiện khi UserControl được tải
        private void HoaDonUC_Load(object sender, EventArgs e)
        {
            // Thiết lập giá trị mặc định cho các điều khiển
            LoaiKhachHang_cb.Items.AddRange(new string[] { "Tất cả", "Lẻ", "Đã có tài khoản" });
            LoaiKhachHang_cb.SelectedIndex = 0;

            NgayBatDau_dtp.Value = DateTime.Now.AddMonths(-1);
            NgayKetThuc_dtp.Value = DateTime.Now;

            Huy_btn.Enabled = false;
            Xuat_btn.Enabled = false;
            HienThiDanhSachHoaDon();
        }

        // Hiển thị danh sách hóa đơn lên DataGridView
        private void HienThiDanhSachHoaDon()
        {
            try
            {
                // Lấy thông tin tìm kiếm
                string tuKhoa = TimKiem_txt.Text.Trim();
                DateTime? ngayBatDau = NgayBatDau_dtp.Checked ? NgayBatDau_dtp.Value : (DateTime?)null;
                DateTime? ngayKetThuc = NgayKetThuc_dtp.Checked ? NgayKetThuc_dtp.Value : (DateTime?)null;
                string loaiKhachHang = LoaiKhachHang_cb.SelectedItem?.ToString();
                if (loaiKhachHang == "Tất cả")
                    loaiKhachHang = null;

                // Cấu hình và load dữ liệu cho DataGridView
                DanhSachHoaDon_dgv.AutoGenerateColumns = false;
                DanhSachHoaDon_dgv.DataSource = hoaDon.LayDanhSachHoaDon(tuKhoa, ngayBatDau, ngayKetThuc, loaiKhachHang);

                // Ánh xạ cột với dữ liệu
                DanhSachHoaDon_dgv.Columns["MaHD"].DataPropertyName = "MaHD";
                DanhSachHoaDon_dgv.Columns["TenNV"].DataPropertyName = "TenNV";
                DanhSachHoaDon_dgv.Columns["TenKH"].DataPropertyName = "TenKH";
                DanhSachHoaDon_dgv.Columns["NgayLap"].DataPropertyName = "NgayLap";
                DanhSachHoaDon_dgv.Columns["TongTien"].DataPropertyName = "TongTien";
                DanhSachHoaDon_dgv.Columns["DaHuy"].DataPropertyName = "TrangThaiHuy";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách hoá đơn: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện tìm kiếm hóa đơn
        private void TimKiem_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra ngày hợp lệ
                if (NgayBatDau_dtp.Checked && NgayKetThuc_dtp.Checked && NgayBatDau_dtp.Value > NgayKetThuc_dtp.Value)
                {
                    MessageBox.Show("Ngày bắt đầu phải nhỏ hơn hoặc bằng ngày kết thúc!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                HienThiDanhSachHoaDon();

                // Thông báo nếu không có kết quả
                if (DanhSachHoaDon_dgv.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy hóa đơn nào phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện hủy hóa đơn
        private void Huy_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DanhSachHoaDon_dgv.SelectedRows.Count > 0)
                {
                    // Lấy MaHD của hóa đơn được chọn
                    string maHD = DanhSachHoaDon_dgv.SelectedRows[0].Cells["MaHD"].Value?.ToString();
                    if (string.IsNullOrEmpty(maHD))
                    {
                        MessageBox.Show("Không thể xác định mã hóa đơn!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Xác nhận trước khi hủy
                    var result = MessageBox.Show($"Bạn có chắc chắn muốn hủy hóa đơn {maHD}?", "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        hoaDon.HuyHoaDon(maHD);
                        MessageBox.Show("Hủy hóa đơn thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        HienThiDanhSachHoaDon(); // Làm mới danh sách hóa đơn
                        DanhSachChiTietHoaDon_dgv.DataSource = null; // Xóa chi tiết hóa đơn
                        TongTien_lbl.Text = "0đ"; // Đặt lại tổng tiền
                        Huy_btn.Enabled = false;
                        Xuat_btn.Enabled = false;
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một hóa đơn để hủy!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi hủy hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện xuất báo cáo hóa đơn
        private void Xuat_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string soHoaDon = DanhSachHoaDon_dgv.SelectedRows[0].Cells["MaHD"].Value.ToString();
                string nguoiLap = DanhSachHoaDon_dgv.SelectedRows[0].Cells["TenNV"].Value.ToString();
                string ngayLap = DanhSachHoaDon_dgv.SelectedRows[0].Cells["NgayLap"].Value.ToString();
                string khachHang = DanhSachHoaDon_dgv.SelectedRows[0].Cells["TenKH"].Value.ToString();
                decimal tongTien = Convert.ToDecimal(DanhSachHoaDon_dgv.SelectedRows[0].Cells["TongTien"].Value);

                // Tạo danh sách chi tiết hóa đơn để xuất báo cáo
                var danhSachXuat = new List<XuatChiTietHoaDon>();
                foreach (DataGridViewRow row in DanhSachChiTietHoaDon_dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        danhSachXuat.Add(new XuatChiTietHoaDon
                        {
                            tenSP = row.Cells["TenSP"].Value?.ToString() ?? "N/A",
                            soLuong = Convert.ToInt32(row.Cells["SoLuong"].Value ?? 0),
                            donGia = Convert.ToDecimal(row.Cells["DonGia"].Value ?? 0),
                            thanhTien = Convert.ToDecimal(Convert.ToInt32(row.Cells["SoLuong"].Value) * Convert.ToDecimal(row.Cells["DonGia"].Value))
                        });
                    }
                }

                // Cấu hình nguồn dữ liệu và tham số cho báo cáo
                ReportDataSource xuatNguonDuLieu = new ReportDataSource
                {
                    Name = "dstDanhSachXuat",
                    Value = danhSachXuat
                };

                ReportParameter[] thamSo = new ReportParameter[]
                {
                    new ReportParameter("prmSoHoaDon", soHoaDon),
                    new ReportParameter("prmNhanVien", nguoiLap),
                    new ReportParameter("prmNgay", ngayLap),
                    new ReportParameter("prmKhachHang", khachHang),
                    new ReportParameter("prmTongThanhToan", tongTien.ToString("N0"))
                };

                // Hiển thị form báo cáo
                var formBaoCao = new BaoCaoForm(xuatNguonDuLieu, thamSo, "hoadon");
                formBaoCao.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất phiếu in: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện làm mới danh sách hóa đơn
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Đặt lại các điều khiển
                TimKiem_txt.Text = "";
                NgayBatDau_dtp.Checked = false;
                NgayKetThuc_dtp.Checked = false;
                LoaiKhachHang_cb.SelectedIndex = 0;
                DanhSachChiTietHoaDon_dgv.DataSource = null;
                HienThiDanhSachHoaDon();
                Huy_btn.Enabled = false;
                Xuat_btn.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lại danh sách hóa đơn: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện cell_click trên DataGridView để hiển thị chi tiết hóa đơn
        private void DanhSachHoaDon_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    string maHD = DanhSachHoaDon_dgv.Rows[e.RowIndex].Cells["MaHD"].Value?.ToString();
                    DanhSachChiTietHoaDon_dgv.AutoGenerateColumns = false;
                    DanhSachChiTietHoaDon_dgv.DataSource = chiTietHoaDon.LayDanhSachChiTietHoaDon(maHD);

                    // Ánh xạ cột với dữ liệu
                    DanhSachChiTietHoaDon_dgv.Columns["MaSP"].DataPropertyName = "MaSP";
                    DanhSachChiTietHoaDon_dgv.Columns["TenSP"].DataPropertyName = "TenSP";
                    DanhSachChiTietHoaDon_dgv.Columns["SoLuong"].DataPropertyName = "SoLuong";
                    DanhSachChiTietHoaDon_dgv.Columns["DonGia"].DataPropertyName = "DonGia";

                    // Hiển thị tổng tiền
                    var tongTien = DanhSachHoaDon_dgv.Rows[e.RowIndex].Cells["TongTien"].Value;
                    TongTien_lbl.Text = Convert.ToDecimal(tongTien).ToString("N0") + "đ";

                    // Kiểm tra trạng thái hóa đơn để bật/tắt nút Hủy
                    string trangThai = DanhSachHoaDon_dgv.Rows[e.RowIndex].Cells["DaHuy"].Value?.ToString();
                    if (trangThai == "Đã huỷ") Huy_btn.Enabled = false;
                    else Huy_btn.Enabled = true;
                    Xuat_btn.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách chi tiết hoá đơn: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
    }
}
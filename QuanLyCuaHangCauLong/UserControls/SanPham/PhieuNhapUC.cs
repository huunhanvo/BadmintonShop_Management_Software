using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.Forms;
using QuanLyCuaHangCauLong.UserControls;
using QuanLyCuaHangCauLong.UserControls.HoaDon;
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

namespace QuanLyCuaHangCauLong.UserControls.SanPham
{
    public partial class PhieuNhapUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.PhieuNhap phieuNhap;
        private readonly Data.ChiTietPhieuNhap chiTietPhieuNhap;

        // Constructor khởi tạo UserControl
        public PhieuNhapUC()
        {
            InitializeComponent();
            phieuNhap = new Data.PhieuNhap(chuoiKetNoi);
            chiTietPhieuNhap = new Data.ChiTietPhieuNhap(chuoiKetNoi);
            HienThiDanhSachPhieuNhap();
        }

        // Xử lý sự kiện khi UserControl được tải
        private void PhieuNhapUC_Load(object sender, EventArgs e)
        {
            // Thiết lập giá trị mặc định cho DateTimePicker
            NgayBatDau_dtp.Value = DateTime.Now.AddMonths(-1);
            NgayKetThuc_dtp.Value = DateTime.Now;

            // Hiển thị danh sách phiếu nhập và vô hiệu hóa các nút
            HienThiDanhSachPhieuNhap();
            Huy_btn.Enabled = false;
            Xuat_btn.Enabled = false;
        }

        // Hiển thị danh sách phiếu nhập lên DataGridView
        private void HienThiDanhSachPhieuNhap()
        {
            try
            {
                // Lấy thông tin tìm kiếm
                string tuKhoa = TimKiem_txt.Text.Trim();
                DateTime? ngayBatDau = NgayBatDau_dtp.Checked ? NgayBatDau_dtp.Value : (DateTime?)null;
                DateTime? ngayKetThuc = NgayKetThuc_dtp.Checked ? NgayKetThuc_dtp.Value : (DateTime?)null;

                // Cấu hình và load dữ liệu cho DataGridView
                DanhSachPhieuNhap_dgv.AutoGenerateColumns = false;
                DanhSachPhieuNhap_dgv.DataSource = phieuNhap.LayDanhSachPhieuNhap(tuKhoa, ngayBatDau, ngayKetThuc);

                // Ánh xạ cột với dữ liệu
                DanhSachPhieuNhap_dgv.Columns["MaPN"].DataPropertyName = "MaPN";
                DanhSachPhieuNhap_dgv.Columns["TenNCC"].DataPropertyName = "TenNCC";
                DanhSachPhieuNhap_dgv.Columns["TenNV"].DataPropertyName = "TenNV";
                DanhSachPhieuNhap_dgv.Columns["ThoiGianNhap"].DataPropertyName = "ThoiGianNhap";
                DanhSachPhieuNhap_dgv.Columns["TongTien"].DataPropertyName = "TongTien";
                DanhSachPhieuNhap_dgv.Columns["DaHuy"].DataPropertyName = "TrangThaiHuy";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách phiếu nhập: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện double-click vào ô trong DataGridView phiếu nhập
        private void DanhSachPhieuNhap_dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    // Lấy mã phiếu nhập
                    string maPN = DanhSachPhieuNhap_dgv.Rows[e.RowIndex].Cells["MaPN"].Value?.ToString();

                    // Hiển thị chi tiết phiếu nhập
                    DanhSachChiTietPhieuNhap_dgv.AutoGenerateColumns = false;
                    DanhSachChiTietPhieuNhap_dgv.DataSource = chiTietPhieuNhap.LayDanhSachChiTietPhieuNhap(maPN);

                    // Ánh xạ cột với dữ liệu chi tiết
                    DanhSachChiTietPhieuNhap_dgv.Columns["MaSP"].DataPropertyName = "MaSP";
                    DanhSachChiTietPhieuNhap_dgv.Columns["TenSP"].DataPropertyName = "TenSP";
                    DanhSachChiTietPhieuNhap_dgv.Columns["SoLuong"].DataPropertyName = "SoLuong";
                    DanhSachChiTietPhieuNhap_dgv.Columns["DonGia"].DataPropertyName = "DonGia";

                    // Hiển thị tổng tiền
                    var tongTien = DanhSachPhieuNhap_dgv.Rows[e.RowIndex].Cells["TongTien"].Value;
                    TongTien_lbl.Text = Convert.ToDecimal(tongTien).ToString("N0") + "đ";

                    // Kiểm tra trạng thái để kích hoạt nút Hủy
                    string trangThai = DanhSachPhieuNhap_dgv.Rows[e.RowIndex].Cells["DaHuy"].Value?.ToString();
                    if (trangThai == "Đã huỷ") Huy_btn.Enabled = false;
                    else Huy_btn.Enabled = true;
                    Xuat_btn.Enabled = true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Lỗi hiển thị chi tiết phiếu nhập: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Xử lý sự kiện tìm kiếm phiếu nhập
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

                // Hiển thị danh sách phiếu nhập
                HienThiDanhSachPhieuNhap();

                // Thông báo nếu không có kết quả
                if (DanhSachPhieuNhap_dgv.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy phiếu nhập nào phù hợp!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm phiếu nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện hủy phiếu nhập
        private void Huy_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DanhSachPhieuNhap_dgv.SelectedRows.Count > 0)
                {
                    // Lấy mã phiếu nhập
                    string maPN = DanhSachPhieuNhap_dgv.SelectedRows[0].Cells["MaPN"].Value?.ToString();
                    if (string.IsNullOrEmpty(maPN))
                    {
                        MessageBox.Show("Không thể xác định mã phiếu nhập!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Xác nhận trước khi hủy
                    var result = MessageBox.Show($"Bạn có chắc chắn muốn hủy phiếu nhập {maPN}?", "Xác nhận hủy", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        // Hủy phiếu nhập
                        phieuNhap.HuyPhieuNhap(maPN);
                        MessageBox.Show("Hủy phiếu nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Làm mới giao diện
                        HienThiDanhSachPhieuNhap();
                        DanhSachChiTietPhieuNhap_dgv.DataSource = null;
                        TongTien_lbl.Text = "0đ";
                        Huy_btn.Enabled = false;
                        Xuat_btn.Enabled = false;
                    }
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn một phiếu nhập để hủy!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi huỷ phiếu nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện xuất báo cáo phiếu nhập
        private void Xuat_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy thông tin phiếu nhập
                string soPhieuNhap = DanhSachPhieuNhap_dgv.SelectedRows[0].Cells["MaPN"].Value.ToString();
                string nguoiLap = DanhSachPhieuNhap_dgv.SelectedRows[0].Cells["TenNV"].Value.ToString();
                string ngayLap = DanhSachPhieuNhap_dgv.SelectedRows[0].Cells["ThoiGianNhap"].Value.ToString();
                string nCC = DanhSachPhieuNhap_dgv.SelectedRows[0].Cells["TenNCC"].Value.ToString();
                decimal tongTien = Convert.ToDecimal(DanhSachPhieuNhap_dgv.SelectedRows[0].Cells["TongTien"].Value);

                // Tạo danh sách chi tiết phiếu nhập để xuất
                var danhSachXuat = new List<XuatChiTietPhieuNhap>();
                foreach (DataGridViewRow row in DanhSachChiTietPhieuNhap_dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        danhSachXuat.Add(new XuatChiTietPhieuNhap
                        {
                            tenSP = row.Cells["TenSP"].Value?.ToString() ?? "N/A",
                            soLuong = Convert.ToInt32(row.Cells["SoLuong"].Value ?? 0),
                            donGia = Convert.ToDecimal(row.Cells["DonGia"].Value ?? 0),
                            thanhTien = Convert.ToDecimal(Convert.ToInt32(row.Cells["SoLuong"].Value) * Convert.ToDecimal(row.Cells["DonGia"].Value))
                        });
                    }
                }

                // Tạo nguồn dữ liệu cho báo cáo
                ReportDataSource xuatNguonDuLieu = new ReportDataSource
                {
                    Name = "dstDanhSachXuat",
                    Value = danhSachXuat
                };

                // Tạo tham số cho báo cáo
                ReportParameter[] thamSo = new ReportParameter[]
                {
                    new ReportParameter("prmSoPhieuNhap", soPhieuNhap),
                    new ReportParameter("prmNhanVien", nguoiLap),
                    new ReportParameter("prmNgay", ngayLap),
                    new ReportParameter("prmNCC", nCC),
                    new ReportParameter("prmTongTien", tongTien.ToString("N0"))
                };

                // Hiển thị form báo cáo
                var formBaoCao = new BaoCaoForm(xuatNguonDuLieu, thamSo, "phieunhap");
                formBaoCao.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất phiếu in: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện làm mới danh sách
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Đặt lại các điều khiển
                TimKiem_txt.Text = "";
                NgayBatDau_dtp.Checked = false;
                NgayKetThuc_dtp.Checked = false;
                DanhSachChiTietPhieuNhap_dgv.DataSource = null;
                TongTien_lbl.Text = "0đ";
                HienThiDanhSachPhieuNhap();
                Huy_btn.Enabled = false;
                Xuat_btn.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải lại danh sách phiếu nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
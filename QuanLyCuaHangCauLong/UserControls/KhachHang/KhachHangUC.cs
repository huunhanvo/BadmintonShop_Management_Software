using Microsoft.Reporting.WinForms;
using QuanLyCuaHangCauLong.Common;
using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.Forms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QuanLyCuaHangCauLong.Data.KhachHang;
using static QuanLyCuaHangCauLong.Data.SanPham;

namespace QuanLyCuaHangCauLong.UserControls.KhachHang
{
    public partial class KhachHangUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.KhachHang khachHang;

        // Constructor khởi tạo UserControl
        public KhachHangUC()
        {
            InitializeComponent();
            khachHang = new Data.KhachHang(chuoiKetNoi);
            HienThiDanhSachKhachHang();
        }

        // Xử lý sự kiện khi UserControl được tải
        private void KhachHangUC_Load(object sender, EventArgs e)
        {
            // Thiết lập trạng thái ban đầu cho các nút
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;

            // Gán sự kiện KeyPress để kiểm tra đầu vào
            SDT_txt.KeyPress += SDT_txt_KeyPress;
            DiemTichLuy_txt.KeyPress += DiemTichLuy_txt_KeyPress;

            // Thiết lập giá trị mặc định cho ComboBox
            LoaiKhachHang_cb.Items.AddRange(new string[] { "Tất cả", "Lẻ", "Đã có tài khoản" });
            LoaiKhachHang_cb.SelectedIndex = 0;
        }

        // Hiển thị danh sách khách hàng lên DataGridView
        private void HienThiDanhSachKhachHang()
        {
            try
            {
                // Cấu hình và load dữ liệu cho DataGridView
                DanhSachKhachHang_dgv.AutoGenerateColumns = false;
                DanhSachKhachHang_dgv.DataSource = khachHang.LayDanhSachKhachHang();

                // Ánh xạ cột với dữ liệu
                DanhSachKhachHang_dgv.Columns["MaKH"].DataPropertyName = "MaKH";
                DanhSachKhachHang_dgv.Columns["TenKH"].DataPropertyName = "TenKH";
                DanhSachKhachHang_dgv.Columns["DiaChi"].DataPropertyName = "DiaChi";
                DanhSachKhachHang_dgv.Columns["SDT"].DataPropertyName = "SDT";
                DanhSachKhachHang_dgv.Columns["Email"].DataPropertyName = "Email";
                DanhSachKhachHang_dgv.Columns["NgayTao"].DataPropertyName = "NgayTao";
                DanhSachKhachHang_dgv.Columns["DiemTichLuy"].DataPropertyName = "DiemTichLuy";
                DanhSachKhachHang_dgv.Columns["TrangThai"].DataPropertyName = "TrangThai";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách khách hàng: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tạo mã khách hàng tự động
        private string TaoMaKhachHang()
        {
            try
            {
                return Functions.TaoMaTuTang("KhachHang", "MaKH", "KH", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "KH001";
            }
        }

        // Kiểm tra tính hợp lệ của tên khách hàng
        private bool KiemTraTenKhachHang()
        {
            if (!Functions.KiemTraChuoiKhongTrong(TenKH_txt.Text, "tên khách hàng", out string thongBaoLoi))
            {
                MessageBox.Show(thongBaoLoi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TenKH_txt.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của số điện thoại
        private bool KiemTraSDT()
        {
            if (string.IsNullOrWhiteSpace(SDT_txt.Text))
            {
                MessageBox.Show("Vui lòng nhập SĐT!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SDT_txt.Focus();
                return false;
            }
            if (!SDT_txt.Text.All(char.IsDigit) || SDT_txt.Text.Length < 10)
            {
                MessageBox.Show("SĐT phải là số và có ít nhất 10 chữ số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SDT_txt.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của email
        private bool KiemTraEmail()
        {
            if (!string.IsNullOrWhiteSpace(Email_txt.Text) && !Email_txt.Text.Contains("@"))
            {
                MessageBox.Show("Email không hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                Email_txt.Focus();
                return false;
            }
            return true;
        }

        // Xóa nội dung các điều khiển nhập liệu
        private void XoaCacDieuKhien()
        {
            MaKH_txt.Text = "";
            TenKH_txt.Text = "";
            DiaChi_txt.Text = "";
            SDT_txt.Text = "";
            Email_txt.Text = "";
            NgayTao_dtp.Value = DateTime.Now;
            DiemTichLuy_txt.Text = "";
            ConGiaoDich_rbtn.Checked = true;
            NgungGiaoDich_rbtn.Checked = false;
        }

        // Xử lý sự kiện click vào ô trong DataGridView
        private void DanhSachKhachHang_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow dong = DanhSachKhachHang_dgv.Rows[e.RowIndex];

                    DataRowView rowView = dong.DataBoundItem as DataRowView;
                    if (rowView != null)
                    {
                        DataRow row = rowView.Row;

                        // Hiển thị thông tin khách hàng lên các điều khiển
                        MaKH_txt.Text = row["MaKH"]?.ToString();
                        TenKH_txt.Text = row["TenKH"]?.ToString();
                        SDT_txt.Text = row["SDT"]?.ToString();
                        DiaChi_txt.Text = row["DiaChi"]?.ToString();
                        Email_txt.Text = row["Email"]?.ToString();
                        string trangThai = row["TrangThai"]?.ToString();
                        ConGiaoDich_rbtn.Checked = (trangThai == "Còn Giao Dịch");
                        NgungGiaoDich_rbtn.Checked = (trangThai == "Ngưng Giao Dịch");
                        DiemTichLuy_txt.Text = row["DiemTichLuy"]?.ToString();
                        if (row["NgayTao"] != null && row["NgayTao"] != DBNull.Value && DateTime.TryParse(row["NgayTao"].ToString(), out DateTime ngayTao))
                        {
                            NgayTao_dtp.Value = ngayTao;
                        }
                        else
                        {
                            NgayTao_dtp.Value = DateTime.Now;
                        }

                        // Kiểm tra khách hàng "lẻ" để bật/tắt các điều khiển
                        if (row["TenKH"].ToString() == "lẻ")
                        {
                            TenKH_txt.Enabled = false;
                            SDT_txt.Enabled = false;
                            DiaChi_txt.Enabled = false;
                            Email_txt.Enabled = false;
                            ConGiaoDich_rbtn.Enabled = false;
                            NgungGiaoDich_rbtn.Enabled = false;
                            DiemTichLuy_txt.Enabled = false;
                        }
                        else
                        {
                            TenKH_txt.Enabled = true;
                            SDT_txt.Enabled = true;
                            DiaChi_txt.Enabled = true;
                            Email_txt.Enabled = true;
                            ConGiaoDich_rbtn.Enabled = true;
                            NgungGiaoDich_rbtn.Enabled = true;
                            DiemTichLuy_txt.Enabled = true;
                        }

                        // Cập nhật trạng thái các nút
                        Xoa_btn.Enabled = true;
                        Sua_btn.Enabled = true;
                        Them_btn.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi hiển thị thông tin khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Xử lý sự kiện KeyPress cho textbox SĐT
        private void SDT_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Xử lý sự kiện KeyPress cho textbox điểm tích lũy
        private void DiemTichLuy_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Xử lý sự kiện thêm khách hàng
        private void Them_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string maKH = TaoMaKhachHang();
                MaKH_txt.Text = maKH;

                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraTenKhachHang() || !KiemTraSDT() || !KiemTraEmail())
                {
                    return;
                }

                // Kiểm tra mã khách hàng trùng
                if (Functions.KiemTraMaTrung("KhachHang", "MaKH", maKH))
                {
                    MessageBox.Show("Mã khách hàng đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra điểm tích lũy
                int diemTichLuy;
                if (string.IsNullOrWhiteSpace(DiemTichLuy_txt.Text))
                {
                    diemTichLuy = 0;
                }
                else if (!int.TryParse(DiemTichLuy_txt.Text, out diemTichLuy) || diemTichLuy < 0)
                {
                    MessageBox.Show("Điểm tích lũy phải là số nguyên không âm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DiemTichLuy_txt.Focus();
                    return;
                }

                // Thêm khách hàng vào cơ sở dữ liệu
                khachHang.ThemKhachHang(
                    maKH,
                    TenKH_txt.Text,
                    DiaChi_txt.Text,
                    SDT_txt.Text,
                    Email_txt.Text,
                    NgayTao_dtp.Value,
                    diemTichLuy,
                    ConGiaoDich_rbtn.Checked
                );

                MessageBox.Show("Thêm khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachKhachHang();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện cập nhật khách hàng
        private void Sua_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra mã khách hàng
                if (string.IsNullOrWhiteSpace(MaKH_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn khách hàng để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraTenKhachHang() || !KiemTraSDT() || !KiemTraEmail())
                {
                    return;
                }

                // Kiểm tra điểm tích lũy
                int diemTichLuy;
                if (string.IsNullOrWhiteSpace(DiemTichLuy_txt.Text))
                {
                    diemTichLuy = 0;
                }
                else if (!int.TryParse(DiemTichLuy_txt.Text, out diemTichLuy) || diemTichLuy < 0)
                {
                    MessageBox.Show("Điểm tích lũy phải là số nguyên không âm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    DiemTichLuy_txt.Focus();
                    return;
                }

                // Cập nhật thông tin khách hàng
                khachHang.CapNhatKhachHang(
                    MaKH_txt.Text,
                    TenKH_txt.Text,
                    DiaChi_txt.Text,
                    SDT_txt.Text,
                    Email_txt.Text,
                    NgayTao_dtp.Value,
                    diemTichLuy,
                    ConGiaoDich_rbtn.Checked
                );

                MessageBox.Show("Cập nhật khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachKhachHang();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện xóa khách hàng
        private void Xoa_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra mã khách hàng
                if (string.IsNullOrWhiteSpace(MaKH_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn khách hàng để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận trước khi xóa
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa khách hàng này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                // Xóa khách hàng khỏi cơ sở dữ liệu
                khachHang.XoaKhachHang(MaKH_txt.Text);

                MessageBox.Show("Xóa khách hàng thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachKhachHang();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện làm mới danh sách khách hàng
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            XoaCacDieuKhien();
            HienThiDanhSachKhachHang();
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;
            Them_btn.Enabled = true;
            MaKH_txt.Text = TaoMaKhachHang();
        }
        //In danh sách khách hàng
        private void InDS_btn_Click(object sender, EventArgs e)
        {
            try
            {
                int tongKH = 0;
                // Tạo danh sách sản phẩm để in danh sách
                var danhSachXuat = new List<Data.KhachHang.ThongTinKhachHang>();
                foreach (DataGridViewRow row in DanhSachKhachHang_dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        danhSachXuat.Add(new ThongTinKhachHang
                        {
                            maKH = row.Cells["MaKH"].Value?.ToString() ?? "N/A",
                            tenKH = row.Cells["TenKH"].Value?.ToString() ?? "N/A",
                            diaChi = row.Cells["DiaChi"].Value?.ToString() ?? "N/A",
                            sdt = row.Cells["SDT"].Value?.ToString() ?? "N/A",
                            email = row.Cells["Email"].Value?.ToString() ?? "N/A",
                            ngayTao = row.Cells["NgayTao"].Value?.ToString() ?? "N/A",
                            diemTichLuy = Convert.ToInt32(row.Cells["DiemTichLuy"].Value ?? 0),
                            trangThai = row.Cells["TrangThai"].Value?.ToString() ?? "N/A"
                        });
                        tongKH++;
                    }
                }

                // Cấu hình nguồn dữ liệu và tham số cho in danh sách
                ReportDataSource xuatNguonDuLieu = new ReportDataSource
                {
                    Name = "dtsDanhSachKhachHang",
                    Value = danhSachXuat
                };

                ReportParameter[] thamSo = new ReportParameter[]
                {
                    new ReportParameter("prmTong", tongKH.ToString())
                };

                // Hiển thị form báo cáo
                var formBaoCao = new BaoCaoForm(xuatNguonDuLieu, thamSo, "danhsachkhachhang");
                formBaoCao.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất phiếu in: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Xử lý sự kiện tìm kiếm khách hàng
        private void TimKiem_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy thông tin tìm kiếm
                string loaiKhachHang = LoaiKhachHang_cb.SelectedItem?.ToString();
                string tuKhoa = TimKiem_txt.Text.Trim();

                // Tìm kiếm và hiển thị kết quả
                DataTable ketQua = khachHang.LayDanhSachKhachHang(
                    loaiKhachHang,
                    string.IsNullOrEmpty(tuKhoa) ? null : tuKhoa,
                    null
                );

                DanhSachKhachHang_dgv.DataSource = ketQua;

                // Thông báo nếu không có kết quả
                if (ketQua.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy khách hàng nào khớp với tiêu chí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm khách hàng: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
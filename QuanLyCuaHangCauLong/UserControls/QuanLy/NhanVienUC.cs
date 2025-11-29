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
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static QuanLyCuaHangCauLong.Data.NhanVien;
using static QuanLyCuaHangCauLong.Data.SanPham;
namespace QuanLyCuaHangCauLong.UserControls.QuanLy
{
    public partial class NhanVienUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.NhanVien nhanVien;

        // Constructor khởi tạo UserControl
        public NhanVienUC()
        {
            InitializeComponent();
            nhanVien = new Data.NhanVien(chuoiKetNoi);
            HienThiDanhSachNhanVien();
        }

        // Xử lý sự kiện khi UserControl được tải
        private void NhanVienUC_Load(object sender, EventArgs e)
        {
            // Thiết lập trạng thái ban đầu cho các điều khiển
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;
            MaNV_txt.ReadOnly = true;
            MaNV_txt.Text = TaoMaNhanVien();

            // Gán sự kiện KeyPress để kiểm tra đầu vào
            SDT_txt.KeyPress += SDT_txt_KeyPress;

            // Thiết lập giá trị mặc định cho ComboBox vai trò nhân viên
            VaiTroNV_cb.Items.AddRange(new string[] { "Admin", "Thu ngân", "Nhân viên" });
            VaiTroNV_cb.SelectedIndex = 0;

            // Thiết lập giá trị mặc định cho ComboBox tìm kiếm vai trò
            VaiTroTK_cb.Items.AddRange(new string[] { "Tất cả", "Admin", "Thu ngân", "Nhân viên" });
            VaiTroTK_cb.SelectedIndex = 0;

            // Tạo và thêm Label cho ComboBox tìm kiếm vai trò
            Label lblVaiTroTK = new Label
            {
                Text = "Vai trò tìm kiếm:",
                Location = new Point(20, 5),
                Size = new Size(100, 15)
            };
            this.Controls.Add(lblVaiTroTK);
        }

        // Hiển thị danh sách nhân viên lên DataGridView
        private void HienThiDanhSachNhanVien()
        {
            try
            {
                // Cấu hình và load dữ liệu cho DataGridView
                DanhSachNhanVien_dgv.AutoGenerateColumns = false;
                DanhSachNhanVien_dgv.DataSource = nhanVien.LayDanhSachNhanVien();

                // Ánh xạ cột với dữ liệu
                DanhSachNhanVien_dgv.Columns["MaNV"].DataPropertyName = "MaNV";
                DanhSachNhanVien_dgv.Columns["TenNV"].DataPropertyName = "TenNV";
                DanhSachNhanVien_dgv.Columns["NgaySinh"].DataPropertyName = "NgaySinh";
                DanhSachNhanVien_dgv.Columns["DiaChi"].DataPropertyName = "DiaChi";
                DanhSachNhanVien_dgv.Columns["SDT"].DataPropertyName = "SDT";
                DanhSachNhanVien_dgv.Columns["TrangThai"].DataPropertyName = "TrangThai";
                DanhSachNhanVien_dgv.Columns["VaiTro"].DataPropertyName = "VaiTro";
                DanhSachNhanVien_dgv.Columns["TenDangNhap"].DataPropertyName = "TenDangNhap";
                DanhSachNhanVien_dgv.Columns["MatKhau"].DataPropertyName = "MatKhau";
                DanhSachNhanVien_dgv.Columns["CoTheDangNhap"].DataPropertyName = "CoTheDangNhap";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách nhân viên: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tạo mã nhân viên tự động
        private string TaoMaNhanVien()
        {
            try
            {
                return Functions.TaoMaTuTang("NhanVien", "MaNV", "NV", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "NV001";
            }
        }

        // Kiểm tra tính hợp lệ của tên nhân viên
        private bool KiemTraTenNhanVien()
        {
            if (!Functions.KiemTraChuoiKhongTrong(TenNV_txt.Text, "tên nhân viên", out string thongBaoLoi))
            {
                MessageBox.Show(thongBaoLoi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TenNV_txt.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của số điện thoại
        private bool KiemTraSDT()
        {
            if (!string.IsNullOrWhiteSpace(SDT_txt.Text))
            {
                if (!SDT_txt.Text.All(char.IsDigit) || SDT_txt.Text.Length != 10)
                {
                    MessageBox.Show("SĐT phải là số và có đúng 10 chữ số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    SDT_txt.Focus();
                    return false;
                }
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của ngày sinh
        private bool KiemTraNgaySinh()
        {
            DateTime ngayHienTai = DateTime.Now;
            DateTime ngaySinh = NgaySinh_dtp.Value;
            if (ngaySinh >= ngayHienTai)
            {
                MessageBox.Show("Ngày sinh phải nhỏ hơn ngày hiện tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NgaySinh_dtp.Focus();
                return false;
            }
            if (ngayHienTai.Year - ngaySinh.Year < 18)
            {
                MessageBox.Show("Nhân viên phải từ 18 tuổi trở lên!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                NgaySinh_dtp.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của tên đăng nhập
        private bool KiemTraTenDangNhap()
        {
            if (!Functions.KiemTraChuoiKhongTrong(TenDangNhap_txt.Text, "tên đăng nhập", out string thongBaoLoi))
            {
                MessageBox.Show(thongBaoLoi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TenDangNhap_txt.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của mật khẩu
        private bool KiemTraMatKhau()
        {
            if (!Functions.KiemTraChuoiKhongTrong(MatKhau_txt.Text, "mật khẩu", out string thongBaoLoi))
            {
                MessageBox.Show(thongBaoLoi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                MatKhau_txt.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tên đăng nhập có bị trùng không
        private bool KiemTraTenDangNhapTrung(string tenDangNhap, string maNV = null)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(chuoiKetNoi))
                {
                    conn.Open();
                    string query = "SELECT COUNT(*) FROM NhanVien WHERE TenDangNhap = @TenDangNhap";
                    if (!string.IsNullOrEmpty(maNV))
                    {
                        query += " AND MaNV != @MaNV";
                    }
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@TenDangNhap", tenDangNhap);
                        if (!string.IsNullOrEmpty(maNV))
                        {
                            cmd.Parameters.AddWithValue("@MaNV", maNV);
                        }
                        return (int)cmd.ExecuteScalar() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi kiểm tra tên đăng nhập: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return true;
            }
        }

        // Xóa nội dung các điều khiển nhập liệu
        private void XoaCacDieuKhien()
        {
            MaNV_txt.Text = "";
            TenNV_txt.Text = "";
            NgaySinh_dtp.Value = DateTime.Now;
            DiaChi_txt.Text = "";
            SDT_txt.Text = "";
            DangLam_rbtn.Checked = true;
            NghiLam_rbtn.Checked = false;
            VaiTroNV_cb.SelectedIndex = 0;
            CoTheDangNhap_checkb.Checked = true;
            TenDangNhap_txt.Text = "";
            MatKhau_txt.Text = "";
        }

        // Xử lý sự kiện click vào ô trong DataGridView
        private void DanhSachNhanVien_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow dong = DanhSachNhanVien_dgv.Rows[e.RowIndex];

                    DataRowView rowView = dong.DataBoundItem as DataRowView;
                    if (rowView != null)
                    {
                        DataRow row = rowView.Row;

                        // Hiển thị thông tin nhân viên lên các điều khiển
                        MaNV_txt.Text = row["MaNV"]?.ToString();
                        TenNV_txt.Text = row["TenNV"]?.ToString();
                        if (row["NgaySinh"] != null && row["NgaySinh"] != DBNull.Value && DateTime.TryParse(row["NgaySinh"].ToString(), out DateTime ngaySinh))
                        {
                            NgaySinh_dtp.Value = ngaySinh;
                        }
                        else
                        {
                            NgaySinh_dtp.Value = DateTime.Now;
                        }
                        DiaChi_txt.Text = row["DiaChi"]?.ToString();
                        SDT_txt.Text = row["SDT"]?.ToString();
                        string trangThai = row["TrangThai"]?.ToString();
                        DangLam_rbtn.Checked = (trangThai == "Đang làm");
                        NghiLam_rbtn.Checked = (trangThai == "Đã nghỉ");
                        string vaiTro = row["VaiTro"]?.ToString();
                        VaiTroNV_cb.SelectedItem = vaiTro;
                        TenDangNhap_txt.Text = row["TenDangNhap"]?.ToString();
                        MatKhau_txt.Text = row["MatKhau"]?.ToString();
                        string coTheDangNhap = row["CoTheDangNhap"]?.ToString();
                        CoTheDangNhap_checkb.Checked = (coTheDangNhap == "Có");

                        // Cập nhật trạng thái các nút
                        Xoa_btn.Enabled = true;
                        Sua_btn.Enabled = true;
                        Them_btn.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi hiển thị thông tin nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Xử lý sự kiện KeyPress cho textbox số điện thoại
        private void SDT_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Xử lý sự kiện thêm nhân viên
        private void Them_btn_Click(object sender, object e)
        {
            try
            {
                string maNV = TaoMaNhanVien();
                MaNV_txt.Text = maNV;

                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraTenNhanVien() || !KiemTraNgaySinh() || !KiemTraSDT() ||
                    !KiemTraTenDangNhap() || !KiemTraMatKhau())
                {
                    return;
                }

                // Kiểm tra mã nhân viên trùng
                if (Functions.KiemTraMaTrung("NhanVien", "MaNV", maNV))
                {
                    MessageBox.Show("Mã nhân viên đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra tên đăng nhập trùng
                if (KiemTraTenDangNhapTrung(TenDangNhap_txt.Text))
                {
                    MessageBox.Show("Tên đã đăng nhập này đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TenDangNhap_txt.Focus();
                    return;
                }

                // Thêm nhân viên vào cơ sở dữ liệu
                nhanVien.ThemNhanVien(
                    maNV,
                    TenNV_txt.Text,
                    NgaySinh_dtp.Value,
                    DiaChi_txt.Text,
                    SDT_txt.Text,
                    DangLam_rbtn.Checked,
                    VaiTroNV_cb.SelectedItem.ToString(),
                    TenDangNhap_txt.Text,
                    MatKhau_txt.Text,
                    CoTheDangNhap_checkb.Checked
                );

                MessageBox.Show("Thêm nhân viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachNhanVien();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện cập nhật nhân viên
        private void Sua_btn_Click(object sender, object e)
        {
            try
            {
                // Kiểm tra mã nhân viên
                if (string.IsNullOrWhiteSpace(MaNV_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn nhân viên để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraTenNhanVien() || !KiemTraNgaySinh() || !KiemTraSDT() ||
                    !KiemTraTenDangNhap() || !KiemTraMatKhau())
                {
                    return;
                }

                // Kiểm tra tên đăng nhập trùng
                if (KiemTraTenDangNhapTrung(TenDangNhap_txt.Text, MaNV_txt.Text))
                {
                    MessageBox.Show("Tên đăng nhập này đã được sử dụng!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    TenDangNhap_txt.Focus();
                    return;
                }

                // Cập nhật thông tin nhân viên trong cơ sở dữ liệu
                nhanVien.CapNhatNhanVien(
                    MaNV_txt.Text,
                    TenNV_txt.Text,
                    NgaySinh_dtp.Value,
                    DiaChi_txt.Text,
                    SDT_txt.Text,
                    DangLam_rbtn.Checked,
                    VaiTroNV_cb.SelectedItem.ToString(),
                    TenDangNhap_txt.Text,
                    MatKhau_txt.Text,
                    CoTheDangNhap_checkb.Checked
                );

                MessageBox.Show("Cập nhật nhân viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachNhanVien();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện xóa nhân viên
        private void Xoa_btn_Click(object sender, object e)
        {
            try
            {
                // Kiểm tra mã nhân viên
                if (string.IsNullOrWhiteSpace(MaNV_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn nhân viên để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận trước khi xóa
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa nhân viên này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                // Xóa nhân viên khỏi cơ sở dữ liệu
                nhanVien.XoaNhanVien(MaNV_txt.Text);

                MessageBox.Show("Xóa nhân viên thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachNhanVien();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa nhân viên: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện làm mới danh sách
        private void TaiLai_btn_Click(object sender, object eventArgs)
        {
            // Xóa nội dung các điều khiển và làm mới danh sách
            XoaCacDieuKhien();
            HienThiDanhSachNhanVien();
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;
            Them_btn.Enabled = true;
            MaNV_txt.Text = TaoMaNhanVien();
        }
        //In danh sách nhân viên
        private void InDS_btn_Click(object sender, EventArgs e)
        {
            try
            {
                int tongNV = 0;
                // Tạo danh sách sản phẩm để in danh sách
                var danhSachXuat = new List<Data.NhanVien.ThongTinNhanVien>();
                foreach (DataGridViewRow row in DanhSachNhanVien_dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        danhSachXuat.Add(new ThongTinNhanVien
                        {
                            maNV = row.Cells["MaNV"].Value?.ToString() ?? "N/A",
                            tenNV = row.Cells["TenNV"].Value?.ToString() ?? "N/A",
                            ngaySinh = row.Cells["NgaySinh"].Value?.ToString() ?? "N/A",
                            diaChi = row.Cells["DiaChi"].Value?.ToString() ?? "N/A",
                            sdt = row.Cells["SDT"].Value?.ToString() ?? "N/A",
                            trangThai = row.Cells["TrangThai"].Value?.ToString() ?? "N/A",
                            vaiTro = row.Cells["VaiTro"].Value?.ToString() ?? "N/A",
                            tenDangNhap = row.Cells["TenDangNhap"].Value?.ToString() ?? "N/A",
                            matKhau = row.Cells["MatKhau"].Value?.ToString() ?? "N/A",
                            coTheDangNhap = row.Cells["CoTheDangNhap"].Value?.ToString() ?? "N/A"
                        });
                        tongNV++;
                    }
                }

                // Cấu hình nguồn dữ liệu và tham số cho in danh sách
                ReportDataSource xuatNguonDuLieu = new ReportDataSource
                {
                    Name = "dtsDanhSachNhanVien",
                    Value = danhSachXuat
                };

                ReportParameter[] thamSo = new ReportParameter[]
                {
                    new ReportParameter("prmTong", tongNV.ToString())
                };

                // Hiển thị form báo cáo
                var formBaoCao = new BaoCaoForm(xuatNguonDuLieu, thamSo, "danhsachnhanvien");
                formBaoCao.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất phiếu in: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Xử lý sự kiện tìm kiếm nhân viên
        private void TimKiem_btn_Click(object sender, object e)
        {
            try
            {
                // Lấy thông tin tìm kiếm
                string vaiTro = VaiTroTK_cb.SelectedItem?.ToString();
                string tuKhoa = TimKiem_txt.Text.Trim();

                // Tìm kiếm và hiển thị kết quả
                DataTable ketQua = nhanVien.LayDanhSachNhanVien(
                    vaiTro,
                    string.IsNullOrEmpty(tuKhoa) ? null : tuKhoa
                );

                DanhSachNhanVien_dgv.DataSource = ketQua;

                // Thông báo nếu không tìm thấy kết quả
                if (ketQua.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy nhân viên nào khớp với tiêu chí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
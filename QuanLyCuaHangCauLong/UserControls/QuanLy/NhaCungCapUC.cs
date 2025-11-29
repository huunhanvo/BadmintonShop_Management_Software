using QuanLyCuaHangCauLong.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuanLyCuaHangCauLong.UserControls.QuanLy
{
    public partial class NhaCungCapUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.NhaCungCap nhaCungCap;

        // Constructor khởi tạo UserControl
        public NhaCungCapUC()
        {
            InitializeComponent();
            nhaCungCap = new Data.NhaCungCap(chuoiKetNoi);
            HienThiDanhSachNhaCungCap();
        }

        // Xử lý sự kiện khi UserControl được tải
        private void NhaCungCapUC_Load(object sender, EventArgs e)
        {
            // Thiết lập trạng thái ban đầu cho các nút
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;

            // Gán sự kiện KeyPress để kiểm tra đầu vào
            SDT_txt.KeyPress += SDT_txt_KeyPress;
        }

        // Hiển thị danh sách nhà cung cấp lên DataGridView
        private void HienThiDanhSachNhaCungCap()
        {
            try
            {
                // Cấu hình và load dữ liệu cho DataGridView
                DanhSachNhaCungCap_dgv.AutoGenerateColumns = false;
                DanhSachNhaCungCap_dgv.DataSource = nhaCungCap.LayDanhSachNhaCungCap();

                // Ánh xạ cột với dữ liệu
                DanhSachNhaCungCap_dgv.Columns["MaNCC"].DataPropertyName = "MaNCC";
                DanhSachNhaCungCap_dgv.Columns["TenNCC"].DataPropertyName = "TenNCC";
                DanhSachNhaCungCap_dgv.Columns["DiaChi"].DataPropertyName = "DiaChi";
                DanhSachNhaCungCap_dgv.Columns["SDT"].DataPropertyName = "SDT";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách nhà cung cấp: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tạo mã nhà cung cấp tự động
        private string TaoMaNhaCungCap()
        {
            try
            {
                return Functions.TaoMaTuTang("NhaCungCap", "MaNCC", "NCC", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "NCC001";
            }
        }

        // Kiểm tra tính hợp lệ của tên nhà cung cấp
        private bool KiemTraTenNhaCungCap()
        {
            if (string.IsNullOrWhiteSpace(TenNCC_txt.Text))
            {
                MessageBox.Show("Vui lòng nhập tên nhà cung cấp!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TenNCC_txt.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của địa chỉ
        private bool KiemTraDiaChi()
        {
            if (string.IsNullOrWhiteSpace(DiaChi_txt.Text))
            {
                MessageBox.Show("Vui lòng nhập địa chỉ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DiaChi_txt.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của số điện thoại
        private bool KiemTraSDT()
        {
            if (string.IsNullOrWhiteSpace(SDT_txt.Text))
            {
                MessageBox.Show("Vui lòng nhập phần trăm giảm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SDT_txt.Focus();
                return false;
            }
            if (!SDT_txt.Text.All(char.IsDigit) || SDT_txt.Text.Length < 10)
            {
                MessageBox.Show("số điện thoại phải là số và có ít nhất 10 số!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                SDT_txt.Focus();
                return false;
            }
            return true;
        }

        // Xóa nội dung các điều khiển nhập liệu
        private void XoaCacDieuKhien()
        {
            MaNCC_txt.Text = "";
            TenNCC_txt.Text = "";
            DiaChi_txt.Text = "";
            SDT_txt.Text = "";
        }

        // Xử lý sự kiện click vào ô trong DataGridView
        private void DanhSachNhaCungCap_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow dong = DanhSachNhaCungCap_dgv.Rows[e.RowIndex];

                    DataRowView rowView = dong.DataBoundItem as DataRowView;
                    if (rowView != null)
                    {
                        DataRow row = rowView.Row;

                        // Hiển thị thông tin nhà cung cấp lên các điều khiển
                        MaNCC_txt.Text = row["MaNCC"]?.ToString();
                        TenNCC_txt.Text = row["TenNCC"]?.ToString();
                        DiaChi_txt.Text = row["DiaChi"]?.ToString();
                        SDT_txt.Text = row["SDT"]?.ToString();

                        // Cập nhật trạng thái các nút
                        Xoa_btn.Enabled = true;
                        Sua_btn.Enabled = true;
                        Them_btn.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi hiển thị thông tin nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

        // Xử lý sự kiện thêm nhà cung cấp
        private void Them_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string maNCC = TaoMaNhaCungCap();
                MaNCC_txt.Text = maNCC;

                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraDiaChi() || !KiemTraSDT() || !KiemTraTenNhaCungCap())
                {
                    return;
                }

                // Kiểm tra mã nhà cung cấp trùng
                if (Functions.KiemTraMaTrung("NhaCungCap", "MaNCC", maNCC))
                {
                    MessageBox.Show("Mã nhà cung cấp đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Thêm nhà cung cấp vào cơ sở dữ liệu
                nhaCungCap.ThemNhaCungCap(
                    maNCC,
                    TenNCC_txt.Text,
                    DiaChi_txt.Text,
                    SDT_txt.Text
                );

                MessageBox.Show("Thêmn nhà cung cấp thành công thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachNhaCungCap();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
                MaNCC_txt.Text = TaoMaNhaCungCap();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm sách nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện cập nhật nhà cung cấp
        private void Sua_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra mã nhà cung cấp
                if (string.IsNullOrWhiteSpace(MaNCC_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn chính sách điểm tích luỹ để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraDiaChi() || !KiemTraSDT() || !KiemTraTenNhaCungCap())
                {
                    return;
                }

                // Cập nhật nhà cung cấp trong cơ sở dữ liệu
                nhaCungCap.CapNhatNhaCungCap(
                    MaNCC_txt.Text,
                    TenNCC_txt.Text,
                    DiaChi_txt.Text,
                    SDT_txt.Text
                );

                MessageBox.Show("Thêm nhà cung cấp thành công thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachNhaCungCap();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
                MaNCC_txt.Text = TaoMaNhaCungCap();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện xóa nhà cung cấp
        private void Xoa_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra mã nhà cung cấp
                if (string.IsNullOrWhiteSpace(MaNCC_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn nhà cung cấp để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận trước khi xóa
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa nhà cung cấp này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                // Xóa nhà cung cấp khỏi cơ sở dữ liệu
                nhaCungCap.XoaNhaCungCap(MaNCC_txt.Text);

                MessageBox.Show("Xóa chính sách nhà cung cấp thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachNhaCungCap();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
                MaNCC_txt.Text = TaoMaNhaCungCap();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa nhà cung cấp: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện làm mới danh sách
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            // Xóa nội dung các điều khiển và làm mới danh sách
            XoaCacDieuKhien();
            HienThiDanhSachNhaCungCap();
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;
            Them_btn.Enabled = true;
            MaNCC_txt.Text = TaoMaNhaCungCap();
        }

        // Xử lý sự kiện tìm kiếm nhà cung cấp
        private void TimKiem_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy từ khóa tìm kiếm và hiển thị danh sách
                string tuKhoa = TimKiem_txt.Text.Trim();
                DataTable ketQua = nhaCungCap.LayDanhSachNhaCungCap(tuKhoa);
                DanhSachNhaCungCap_dgv.DataSource = ketQua;

                // Thông báo nếu không tìm thấy kết quả
                if (ketQua.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy nhà cung cấp nào khớp với tiêu chí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm nhà cung cấp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
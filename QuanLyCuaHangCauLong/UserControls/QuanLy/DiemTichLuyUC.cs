using QuanLyCuaHangCauLong.Common;
using QuanLyCuaHangCauLong.Data;
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

namespace QuanLyCuaHangCauLong.UserControls.QuanLy
{
    public partial class DiemTichLuyUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.ChinhSachDiemTichLuy chinhSachDiemTichLuy;

        // Constructor khởi tạo UserControl
        public DiemTichLuyUC()
        {
            InitializeComponent();
            chinhSachDiemTichLuy = new Data.ChinhSachDiemTichLuy(chuoiKetNoi);
            HienThiDanhSachChinhSachDiemTichLuy();
        }

        // Xử lý sự kiện khi UserControl được tải
        private void DiemTichLuyUC_Load(object sender, EventArgs e)
        {
            // Thiết lập trạng thái ban đầu cho các nút
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;

            // Gán sự kiện KeyPress để kiểm tra đầu vào
            DiemToiThieu_txt.KeyPress += DiemToiThieu_txt_KeyPress;
            PhanTramGian_txt.KeyPress += PhanTramGian_txt_KeyPress;
        }

        // Hiển thị danh sách chính sách điểm tích lũy lên DataGridView
        private void HienThiDanhSachChinhSachDiemTichLuy()
        {
            try
            {
                // Cấu hình và load dữ liệu cho DataGridView
                DanhSachDiemTichLuy_dgv.AutoGenerateColumns = false;
                DanhSachDiemTichLuy_dgv.DataSource = chinhSachDiemTichLuy.LayDanhSachChinhSachDiemTichLuy();

                // Ánh xạ cột với dữ liệu
                DanhSachDiemTichLuy_dgv.Columns["MaCSDTL"].DataPropertyName = "MaCSDTL";
                DanhSachDiemTichLuy_dgv.Columns["DiemToiThieu"].DataPropertyName = "DiemToiThieu";
                DanhSachDiemTichLuy_dgv.Columns["PhanTramGiam"].DataPropertyName = "PhanTramGiam";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách chính sách điểm tích luỹ: ", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tạo mã chính sách điểm tích lũy tự động
        private string TaoMaChinhSachDiemTichLuy()
        {
            try
            {
                return Functions.TaoMaTuTang("ChinhSachDiemTichLuy", "MaCSDTL", "CS", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "CS001";
            }
        }

        // Kiểm tra tính hợp lệ của điểm tối thiểu
        private bool KiemTraDiemToiThieu()
        {
            if (string.IsNullOrWhiteSpace(DiemToiThieu_txt.Text))
            {
                MessageBox.Show("Vui lòng nhập điểm tối thiểu!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DiemToiThieu_txt.Focus();
                return false;
            }
            if (!DiemToiThieu_txt.Text.All(char.IsDigit))
            {
                MessageBox.Show("Điểm tối thiểu phải là số !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DiemToiThieu_txt.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của phần trăm giảm
        private bool KiemTraPhanTramGiam()
        {
            if (string.IsNullOrWhiteSpace(PhanTramGian_txt.Text))
            {
                MessageBox.Show("Vui lòng nhập phần trăm giảm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                PhanTramGian_txt.Focus();
                return false;
            }
            if (!PhanTramGian_txt.Text.All(char.IsDigit))
            {
                MessageBox.Show("phần trăm giảm phải là số !", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                PhanTramGian_txt.Focus();
                return false;
            }
            return true;
        }

        // Xóa nội dung các điều khiển nhập liệu
        private void XoaCacDieuKhien()
        {
            MaCSDTL_txt.Text = "";
            DiemToiThieu_txt.Text = "";
            PhanTramGian_txt.Text = "";
        }

        // Xử lý sự kiện click vào ô trong DataGridView
        private void DanhSachDiemTichLuy_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow dong = DanhSachDiemTichLuy_dgv.Rows[e.RowIndex];

                    DataRowView rowView = dong.DataBoundItem as DataRowView;
                    if (rowView != null)
                    {
                        DataRow row = rowView.Row;

                        // Hiển thị thông tin chính sách điểm tích lũy lên các điều khiển
                        MaCSDTL_txt.Text = row["MaCSDTL"]?.ToString();
                        DiemToiThieu_txt.Text = row["DiemToiThieu"]?.ToString();
                        PhanTramGian_txt.Text = row["PhanTramGiam"]?.ToString();

                        // Cập nhật trạng thái các nút
                        Xoa_btn.Enabled = true;
                        Sua_btn.Enabled = true;
                        Them_btn.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi hiển thị thông tin chính sách điểm tích luỹ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Xử lý sự kiện KeyPress cho textbox điểm tối thiểu
        private void DiemToiThieu_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Xử lý sự kiện KeyPress cho textbox phần trăm giảm
        private void PhanTramGian_txt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        // Xử lý sự kiện thêm chính sách điểm tích lũy
        private void Them_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string maCSDTL = TaoMaChinhSachDiemTichLuy();
                MaCSDTL_txt.Text = maCSDTL;

                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraDiemToiThieu() || !KiemTraPhanTramGiam())
                {
                    return;
                }

                // Kiểm tra mã chính sách trùng
                if (Functions.KiemTraMaTrung("ChinhSachDiemTichLuy", "MaCSDTL", maCSDTL))
                {
                    MessageBox.Show("Mã khách hàng đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Thêm chính sách vào cơ sở dữ liệu
                chinhSachDiemTichLuy.ThemChinhSachDiem(
                    maCSDTL,
                    DiemToiThieu_txt.Text,
                    PhanTramGian_txt.Text
                );

                MessageBox.Show("Thêm chính sách điểm tích luỹ thành công thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachChinhSachDiemTichLuy();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
                MaCSDTL_txt.Text = TaoMaChinhSachDiemTichLuy();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm sách điểm tích luỹ: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện cập nhật chính sách điểm tích lũy
        private void Sua_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra mã chính sách
                if (string.IsNullOrWhiteSpace(MaCSDTL_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn chính sách điểm tích luỹ để sửa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraPhanTramGiam() || !KiemTraDiemToiThieu())
                {
                    return;
                }

                // Cập nhật chính sách trong cơ sở dữ liệu
                chinhSachDiemTichLuy.CapNhatChinhSachDiem(
                    MaCSDTL_txt.Text,
                    DiemToiThieu_txt.Text,
                    PhanTramGian_txt.Text
                );

                MessageBox.Show("Thêm chính sách điểm tích luỹ thành công thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachChinhSachDiemTichLuy();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
                MaCSDTL_txt.Text = TaoMaChinhSachDiemTichLuy();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi cập nhật chính sách điểm tích: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện xóa chính sách điểm tích lũy
        private void Xoa_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra mã chính sách
                if (string.IsNullOrWhiteSpace(MaCSDTL_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn chính sách điểm tích luỹ để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận trước khi xóa
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa chính sách điểm tích luỹ này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                // Xóa chính sách khỏi cơ sở dữ liệu
                chinhSachDiemTichLuy.XoaChinhSachDiem(MaCSDTL_txt.Text);

                MessageBox.Show("Xóa chính sách điểm tích luỹ thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachChinhSachDiemTichLuy();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                Them_btn.Enabled = true;
                MaCSDTL_txt.Text = TaoMaChinhSachDiemTichLuy();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện làm mới danh sách
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            // Xóa nội dung các điều khiển và làm mới danh sách
            XoaCacDieuKhien();
            HienThiDanhSachChinhSachDiemTichLuy();
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;
            Them_btn.Enabled = true;
            MaCSDTL_txt.Text = TaoMaChinhSachDiemTichLuy();
        }

        // Xử lý sự kiện tìm kiếm chính sách điểm tích lũy
        private void TimKiem_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy từ khóa tìm kiếm và hiển thị danh sách
                string tuKhoa = TimKiem_txt.Text.Trim();
                DataTable ketQua = chinhSachDiemTichLuy.LayDanhSachChinhSachDiemTichLuy(tuKhoa);
                DanhSachDiemTichLuy_dgv.DataSource = ketQua;

                // Thông báo nếu không tìm thấy kết quả
                if (ketQua.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy chính sách điểm tích luỹ nào khớp với tiêu chí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm chính sách điểm tích luỹ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
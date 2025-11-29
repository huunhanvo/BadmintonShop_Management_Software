using QuanLyCuaHangCauLong.Common;
using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.UserControls.QuanLy;
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

namespace QuanLyCuaHangCauLong.UserControls.SanPham
{
    public partial class NhapKhoUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.SanPham sanPham;
        private readonly Data.NhaCungCap nhaCungCap;
        private readonly Data.PhieuNhap phieuNhap;
        private DataTable sanPhamDaChon;
        private string maNV;
        private string tenNV;

        // Constructor khởi tạo UserControl
        public NhapKhoUC(string maNV, string tenNV)
        {
            this.maNV = maNV;
            this.tenNV = tenNV;
            InitializeComponent();
            sanPham = new Data.SanPham(chuoiKetNoi);
            nhaCungCap = new Data.NhaCungCap(chuoiKetNoi);
            phieuNhap = new Data.PhieuNhap(chuoiKetNoi);

            // Hiển thị tên nhân viên
            TenNV_txt.Text = tenNV;

            // Khởi tạo dữ liệu ban đầu
            HienThiDanhSachSanPham();
            NapDanhSachLoaiSanPham();
            NapDanhSachThuongHieu();
            NapDanhSachNhaCungCap();
            KhoiTaoSanPhamDaChon();
        }

        // Xử lý sự kiện khi UserControl được tải
        private void NhapKhoUC_Load(object sender, EventArgs e)
        {
            // Gán mã phiếu nhập tự động
            MaPN_txt.Text = TaoMaPhieuNhap();
        }

        // Hiển thị danh sách sản phẩm lên DataGridView
        private void HienThiDanhSachSanPham()
        {
            try
            {
                // Cấu hình và load dữ liệu cho DataGridView
                DanhSachSanPham_dgv.AutoGenerateColumns = false;
                DanhSachSanPham_dgv.DataSource = sanPham.LayDanhSachSanPham();

                // Ánh xạ cột với dữ liệu
                DanhSachSanPham_dgv.Columns["MaSP"].DataPropertyName = "MaSP";
                DanhSachSanPham_dgv.Columns["TenSP"].DataPropertyName = "TenSP";
                DanhSachSanPham_dgv.Columns["DonGiaSP"].DataPropertyName = "DonGiaSP";
                DanhSachSanPham_dgv.Columns["SoLuongSP"].DataPropertyName = "SoLuongSP";
                DanhSachSanPham_dgv.Columns["TenLoaiSP"].DataPropertyName = "TenLoaiSP";
                DanhSachSanPham_dgv.Columns["TenTH"].DataPropertyName = "TenTH";
                DanhSachSanPham_dgv.Columns["HinhAnhHienThi"].DataPropertyName = "HinhAnhHienThi";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nạp danh sách loại sản phẩm vào ComboBox
        private void NapDanhSachLoaiSanPham()
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "SELECT MaLoaiSP, TenLoaiSP FROM LoaiSanPham";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        using (SqlDataReader doc = lenh.ExecuteReader())
                        {
                            var bangDuLieu = new DataTable();
                            bangDuLieu.Load(doc);

                            // Thêm dòng "Tất cả" cho LoaiSanPhamTK_cb
                            DataRow tatCa = bangDuLieu.NewRow();
                            tatCa["MaLoaiSP"] = "";
                            tatCa["TenLoaiSP"] = "Tất cả";
                            bangDuLieu.Rows.InsertAt(tatCa, 0);

                            // Gán DataSource cho LoaiSanPhamTK_cb
                            LoaiSanPhamTK_cb.DataSource = bangDuLieu.Copy();
                            LoaiSanPhamTK_cb.DisplayMember = "TenLoaiSP";
                            LoaiSanPhamTK_cb.ValueMember = "MaLoaiSP";
                            LoaiSanPhamTK_cb.SelectedIndex = 0; // Chọn "Tất cả" mặc định
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nạp danh sách loại sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nạp danh sách thương hiệu vào ComboBox
        private void NapDanhSachThuongHieu()
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "SELECT MaTH, TenTH FROM ThuongHieu";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        using (SqlDataReader doc = lenh.ExecuteReader())
                        {
                            var bangDuLieu = new DataTable();
                            bangDuLieu.Load(doc);

                            // Thêm dòng "Tất cả" cho ThuongHieuSPTK_cb
                            DataRow tatCa = bangDuLieu.NewRow();
                            tatCa["MaTH"] = "";
                            tatCa["TenTH"] = "Tất cả";
                            bangDuLieu.Rows.InsertAt(tatCa, 0);

                            // Gán DataSource cho ThuongHieuSPTK_cb
                            ThuongHieuSPTK_cb.DataSource = bangDuLieu.Copy();
                            ThuongHieuSPTK_cb.DisplayMember = "TenTH";
                            ThuongHieuSPTK_cb.ValueMember = "MaTH";
                            ThuongHieuSPTK_cb.SelectedIndex = 0; // Chọn "Tất cả" mặc định
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nạp danh sách thương hiệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Nạp danh sách nhà cung cấp vào ComboBox
        private void NapDanhSachNhaCungCap()
        {
            try
            {
                DataTable dtNhaCungCap = nhaCungCap.LayDanhSachNhaCungCap();
                if (dtNhaCungCap.Rows.Count == 0)
                {
                    MessageBox.Show("Không có nhà cung cấp nào!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                TenNCC_cb.DataSource = dtNhaCungCap;
                TenNCC_cb.DisplayMember = "TenNCC";
                TenNCC_cb.ValueMember = "MaNCC";
                TenNCC_cb.SelectedIndex = -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp danh sách nhà cung cấp: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tạo mã phiếu nhập tự động
        private string TaoMaPhieuNhap()
        {
            try
            {
                return Functions.TaoMaTuTang("PhieuNhap", "MaPN", "PN", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "PN001";
            }
        }

        // Khởi tạo DataTable cho danh sách sản phẩm đã chọn
        private void KhoiTaoSanPhamDaChon()
        {
            sanPhamDaChon = new DataTable();
            sanPhamDaChon.Columns.Add("MaSP", typeof(string));
            sanPhamDaChon.Columns.Add("TenSP", typeof(string));
            sanPhamDaChon.Columns.Add("SoLuong", typeof(int));
            sanPhamDaChon.Columns.Add("DonGia", typeof(int));

            // Cấu hình DataGridView cho danh sách sản phẩm đã chọn
            DanhSachSanPhamDaChon_dgv.AutoGenerateColumns = false;
            DanhSachSanPhamDaChon_dgv.DataSource = sanPhamDaChon;

            // Ánh xạ cột với dữ liệu
            DanhSachSanPhamDaChon_dgv.Columns["MaSPThem"].DataPropertyName = "MaSP";
            DanhSachSanPhamDaChon_dgv.Columns["TenSPThem"].DataPropertyName = "TenSP";
            DanhSachSanPhamDaChon_dgv.Columns["SoLuongSPThem"].DataPropertyName = "SoLuong";
            DanhSachSanPhamDaChon_dgv.Columns["DonGiaThem"].DataPropertyName = "DonGia";
        }

        // Xóa nội dung các điều khiển nhập liệu
        private void XoaCacDieuKhien()
        {
            sanPhamDaChon.Clear();
            TongTien_txt.Clear();
            TenNCC_cb.SelectedIndex = -1;
            MaPN_txt.Text = TaoMaPhieuNhap();

            // Xóa thông tin sản phẩm
            MaSP_txt.Clear();
            TenSP_txt.Clear();
            DonGia_txt.Clear();
            SoLuong_txt.Clear();
            LoaiSanPham_txt.Clear();
            ThuongHieu_txt.Clear();
        }

        // Xử lý sự kiện click vào ô trong DataGridView sản phẩm
        private void DanhSachSanPham_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow dong = DanhSachSanPham_dgv.Rows[e.RowIndex];

                    // Lấy DataRow từ DataTable nguồn
                    DataRowView rowView = dong.DataBoundItem as DataRowView;
                    if (rowView != null)
                    {
                        DataRow row = rowView.Row;

                        // Hiển thị thông tin sản phẩm lên các điều khiển
                        MaSP_txt.Text = row["MaSP"]?.ToString();
                        TenSP_txt.Text = row["TenSP"]?.ToString();
                        LoaiSanPham_txt.Text = DanhSachSanPham_dgv.Rows[e.RowIndex].Cells["TenLoaiSP"].Value.ToString();
                        ThuongHieu_txt.Text = DanhSachSanPham_dgv.Rows[e.RowIndex].Cells["TenTH"].Value.ToString();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi hiển thị thông tin sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Xử lý sự kiện KeyPress cho textbox đơn giá
        private void DonGia_txt_KeyPress(object sender, KeyPressEventArgs e)
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

        // Xử lý sự kiện KeyPress cho textbox số lượng
        private void SoLuong_txt_KeyPress(object sender, KeyPressEventArgs e)
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

        // Xử lý sự kiện KeyDown cho textbox đơn giá
        private void DonGia_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                SoLuong_txt.Focus();
        }

        // Xử lý sự kiện KeyDown cho textbox số lượng
        private void SoLuong_txt_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                ThemSP_btn_Click(sender, e);
        }

        // Xử lý sự kiện thêm sản phẩm vào danh sách đã chọn
        private void ThemSP_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra mã sản phẩm
                if (string.IsNullOrEmpty(MaSP_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm trước khi thêm!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra đơn giá
                if (string.IsNullOrEmpty(DonGia_txt.Text))
                {
                    MessageBox.Show("Vui lòng nhập đơn giá!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra số lượng
                if (string.IsNullOrEmpty(SoLuong_txt.Text) || !int.TryParse(SoLuong_txt.Text, out int soLuongThem) || soLuongThem <= 0)
                {
                    MessageBox.Show("Vui lòng nhập số lượng hợp lệ (số nguyên dương)!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                decimal donGia = Convert.ToDecimal(DonGia_txt.Text);
                bool sanPhamDaTonTai = false;

                // Kiểm tra sản phẩm đã tồn tại trong danh sách chưa
                foreach (DataRow row in sanPhamDaChon.Rows)
                {
                    if (row["MaSP"].ToString() == MaSP_txt.Text)
                    {
                        // Nếu đã tồn tại, cộng dồn số lượng
                        int soLuongHienTai = Convert.ToInt32(row["SoLuong"]);
                        row["SoLuong"] = soLuongHienTai + soLuongThem;
                        row["DonGia"] = donGia;
                        sanPhamDaTonTai = true;
                        break;
                    }
                }

                if (!sanPhamDaTonTai)
                {
                    // Nếu chưa tồn tại, thêm mới
                    DataRow newRow = sanPhamDaChon.NewRow();
                    newRow["MaSP"] = MaSP_txt.Text;
                    newRow["TenSP"] = TenSP_txt.Text;
                    newRow["SoLuong"] = soLuongThem;
                    newRow["DonGia"] = donGia;
                    sanPhamDaChon.Rows.Add(newRow);
                }

                // Tính toán lại tổng tiền
                decimal tongTienHienTai = 0;
                foreach (DataRow row in sanPhamDaChon.Rows)
                {
                    tongTienHienTai += Convert.ToDecimal(row["DonGia"]) * Convert.ToInt32(row["SoLuong"]);
                }
                TongTien_txt.Text = tongTienHienTai.ToString("N0");

                // Reset form
                DonGia_txt.Clear();
                DonGia_txt.Focus();
                SoLuong_txt.Clear();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện xóa sản phẩm khỏi danh sách đã chọn
        private void Xoa_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DanhSachSanPhamDaChon_dgv.SelectedRows.Count > 0)
                {
                    // Lấy thông tin sản phẩm cần xóa để trừ khỏi tổng tiền
                    DataGridViewRow selectedRow = DanhSachSanPhamDaChon_dgv.SelectedRows[0];
                    decimal donGia = Convert.ToDecimal(selectedRow.Cells["DonGiaThem"].Value);
                    int soLuong = Convert.ToInt32(selectedRow.Cells["SoLuongSPThem"].Value);
                    decimal thanhTien = donGia * soLuong;

                    // Cập nhật tổng tiền
                    decimal tongTienHienTai = Convert.ToDecimal(TongTien_txt.Text);
                    TongTien_txt.Text = (tongTienHienTai - thanhTien).ToString();

                    // Xóa dòng khỏi DataTable
                    DataRowView rowView = selectedRow.DataBoundItem as DataRowView;
                    if (rowView != null)
                    {
                        rowView.Row.Delete();
                    }

                    Xoa_btn.Enabled = false;
                }
                else
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm để xóa!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi xóa sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện hủy danh sách sản phẩm đã chọn
        private void Huy_btn_Click(object sender, EventArgs e)
        {
            sanPhamDaChon.Clear();
            Xoa_btn.Enabled = false;
        }

        // Xử lý sự kiện tìm kiếm sản phẩm
        private void TimKiem_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy giá trị từ các control
                string maLoaiSP = LoaiSanPhamTK_cb.SelectedValue?.ToString();
                string maTH = ThuongHieuSPTK_cb.SelectedValue?.ToString();
                string tuKhoa = TimKiem_txt.Text.Trim();

                // Nếu không có tiêu chí nào, hiển thị toàn bộ danh sách
                if (string.IsNullOrEmpty(maLoaiSP) && string.IsNullOrEmpty(maTH) && string.IsNullOrEmpty(tuKhoa))
                {
                    HienThiDanhSachSanPham();
                    return;
                }

                // Gọi phương thức tìm kiếm
                DataTable ketQua = sanPham.LayDanhSachSanPham(
                    maLoaiSP,
                    maTH,
                    string.IsNullOrEmpty(tuKhoa) ? null : tuKhoa
                );

                // Cập nhật DataGridView
                DanhSachSanPham_dgv.DataSource = ketQua;

                // Thông báo nếu không tìm thấy
                if (ketQua.Rows.Count == 0)
                {
                    MessageBox.Show("Không tìm thấy sản phẩm nào khớp với tiêu chí!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tìm kiếm sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện nhập hàng
        private void NhapHang_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra có sản phẩm nào được chọn không
                if (sanPhamDaChon == null || sanPhamDaChon.Rows.Count == 0)
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm để nhập!!!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra nhà cung cấp
                if (TenNCC_cb.SelectedIndex == -1)
                {
                    MessageBox.Show("Vui lòng chọn nhà cung cấp!!!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Kiểm tra tổng tiền
                if (string.IsNullOrEmpty(TongTien_txt.Text))
                {
                    MessageBox.Show("Tổng tiền không hợp lệ!!!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Lấy thông tin phiếu nhập
                string maPN = MaPN_txt.Text;
                string maNCC = TenNCC_cb.SelectedValue.ToString();
                decimal tongTien = Convert.ToDecimal(TongTien_txt.Text);

                // Thêm phiếu nhập vào cơ sở dữ liệu
                phieuNhap.ThemPhieuNhap(maPN, maNCC, maNV, DateTime.Now, tongTien, sanPhamDaChon);

                MessageBox.Show("Nhập hàng thành công! Mã phiếu nhập: " + maPN, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Reset form sau khi nhập thành công
                XoaCacDieuKhien();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nhập hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
using Microsoft.Reporting.WinForms;
using QuanLyCuaHangCauLong.Common;
using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.Forms;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static QuanLyCuaHangCauLong.Data.SanPham;
using QuanLyCuaHangCauLong.Properties;
namespace QuanLyCuaHangCauLong.UserControls.SanPham
{
    public partial class SanPhamUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.SanPham sanPham;

        // Constructor khởi tạo UserControl
        public SanPhamUC()
        {
            InitializeComponent();
            sanPham = new Data.SanPham(chuoiKetNoi);
            HienThiDanhSachSanPham();
            NapDanhSachLoaiSanPham();
            NapDanhSachThuongHieu();
        }

        // Xử lý sự kiện khi UserControl được tải
        private void SanPhamUC_Load(object sender, EventArgs e)
        {
            // Thiết lập trạng thái ban đầu cho các điều khiển
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;
            DonGia_txt.KeyPress += DonGia_txt_KeyPress_1;
            MaSP_txt.Text = TaoMaSanPham();
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
                DanhSachSanPham_dgv.Columns["DuongDanAnh"].DataPropertyName = "DuongDanAnh";
                DanhSachSanPham_dgv.Columns["TrangThai"].DataPropertyName = "TrangThai";
                DanhSachSanPham_dgv.Columns["MaLoaiSP"].DataPropertyName = "TenLoaiSP";
                DanhSachSanPham_dgv.Columns["MaTH"].DataPropertyName = "TenTH";
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

                            // Gán DataSource cho LoaiSanPham_cb (không cần "Tất cả")
                            LoaiSanPham_cb.DataSource = bangDuLieu.Copy();
                            LoaiSanPham_cb.DisplayMember = "TenLoaiSP";
                            LoaiSanPham_cb.ValueMember = "MaLoaiSP";
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

                            // Gán DataSource cho ThuongHieuSP_cb (không cần "Tất cả")
                            ThuongHieuSP_cb.DataSource = bangDuLieu.Copy();
                            ThuongHieuSP_cb.DisplayMember = "TenTH";
                            ThuongHieuSP_cb.ValueMember = "MaTH";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi nạp danh sách thương hiệu: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tạo mã sản phẩm tự động
        private string TaoMaSanPham()
        {
            try
            {
                return Functions.TaoMaTuTang("SanPham", "MaSP", "SP", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "SP001";
            }
        }

        // Kiểm tra tính hợp lệ của tên sản phẩm
        private bool KiemTraTenSanPham()
        {
            if (!Functions.KiemTraChuoiKhongTrong(TenSP_txt.Text, "tên sản phẩm", out string thongBaoLoi))
            {
                MessageBox.Show(thongBaoLoi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TenSP_txt.Focus();
                return false;
            }
            return true;
        }

        // Kiểm tra tính hợp lệ của đơn giá
        private bool KiemTraDonGia()
        {
            if (!Functions.KiemTraSoThucDuong(DonGia_txt.Text, "Đơn giá", out string thongBaoLoi, out _))
            {
                MessageBox.Show(thongBaoLoi, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                DonGia_txt.Focus();
                return false;
            }
            return true;
        }

        // Xóa nội dung các điều khiển nhập liệu
        private void XoaCacDieuKhien()
        {
            MaSP_txt.Text = "";
            TenSP_txt.Text = "";
            DonGia_txt.Text = "";
            DuongDanAnh_txt.Text = "";
            SoLuong_nud.Value = 0;
            Hien_rbtn.Checked = true;
            An_rbtn.Checked = false;
            LoaiSanPham_cb.SelectedIndex = -1;
            ThuongHieuSP_cb.SelectedIndex = -1;
            HinhAnh_pb.Image = Properties.Resources._default;
        }

        // Xử lý sự kiện click vào ô trong DataGridView
        private void DanhSachSanPham_dgv_CellClick_1(object sender, DataGridViewCellEventArgs e)
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
                        DonGia_txt.Text = row["DonGiaSP"]?.ToString();
                        DuongDanAnh_txt.Text = row["DuongDanAnh"]?.ToString();
                        string trangThai = row["TrangThai"]?.ToString();
                        Hien_rbtn.Checked = (trangThai == "Hiện");
                        An_rbtn.Checked = (trangThai == "Ẩn");

                        // Gán MaLoaiSP vào ComboBox
                        if (row["MaLoaiSP"] != null && row["MaLoaiSP"] != DBNull.Value)
                        {
                            LoaiSanPham_cb.SelectedValue = row["MaLoaiSP"].ToString();
                        }
                        else
                        {
                            LoaiSanPham_cb.SelectedIndex = -1;
                        }

                        // Gán MaTH vào ComboBox
                        if (row["MaTH"] != null && row["MaTH"] != DBNull.Value)
                        {
                            ThuongHieuSP_cb.SelectedValue = row["MaTH"].ToString();
                        }
                        else
                        {
                            ThuongHieuSP_cb.SelectedIndex = -1;
                        }

                        // Gán số lượng
                        if (row["SoLuongSP"] != null && int.TryParse(row["SoLuongSP"].ToString(), out int soLuong))
                        {
                            SoLuong_nud.Value = soLuong;
                        }
                        else
                        {
                            SoLuong_nud.Value = 0;
                        }

                        // Gán hình ảnh
                        HinhAnh_pb.Image = row["HinhAnhHienThi"] != null && row["HinhAnhHienThi"] != DBNull.Value
                            ? (Image)row["HinhAnhHienThi"]
                            : null;

                        // Cập nhật trạng thái các nút
                        Xoa_btn.Enabled = true;
                        Sua_btn.Enabled = true;
                        SoLuong_nud.Enabled = true;
                        Them_btn.Enabled = false;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi hiển thị thông tin sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        // Xử lý sự kiện KeyPress cho textbox đơn giá
        private void DonGia_txt_KeyPress_1(object sender, KeyPressEventArgs e)
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

        // Xử lý sự kiện thêm sản phẩm
        private void Them_btn_Click(object sender, EventArgs e)
        {
            try
            {
                string maSP = TaoMaSanPham();
                MaSP_txt.Text = maSP;

                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraTenSanPham() || !KiemTraDonGia() || LoaiSanPham_cb.SelectedValue == null || ThuongHieuSP_cb.SelectedValue == null)
                {
                    return;
                }

                // Kiểm tra mã sản phẩm trùng
                if (Functions.KiemTraMaTrung("SanPham", "MaSP", maSP))
                {
                    MessageBox.Show("Mã sản phẩm đã tồn tại!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Thêm sản phẩm vào cơ sở dữ liệu
                sanPham.ThemSanPham(
                    maSP,
                    TenSP_txt.Text,
                    decimal.Parse(DonGia_txt.Text),
                    0,
                    DuongDanAnh_txt.Text,
                    HinhAnh_pb.Image,
                    Hien_rbtn.Checked,
                    LoaiSanPham_cb.SelectedValue.ToString(),
                    ThuongHieuSP_cb.SelectedValue.ToString()
                );

                MessageBox.Show("Thêm sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachSanPham();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                SoLuong_nud.Enabled = false;
                Them_btn.Enabled = true;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện cập nhật sản phẩm
        private void Sua_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra dữ liệu đầu vào
                if (!KiemTraTenSanPham() || !KiemTraDonGia() || string.IsNullOrWhiteSpace(MaSP_txt.Text) ||
                    SoLuong_nud.Value < 0 || LoaiSanPham_cb.SelectedValue == null || ThuongHieuSP_cb.SelectedValue == null)
                {
                    return;
                }

                // Cập nhật sản phẩm trong cơ sở dữ liệu
                sanPham.CapNhatSanPham(
                    MaSP_txt.Text,
                    TenSP_txt.Text,
                    decimal.Parse(DonGia_txt.Text),
                    (int)SoLuong_nud.Value,
                    DuongDanAnh_txt.Text,
                    HinhAnh_pb.Image,
                    Hien_rbtn.Checked,
                    LoaiSanPham_cb.SelectedValue.ToString(),
                    ThuongHieuSP_cb.SelectedValue.ToString()
                );

                MessageBox.Show("Cập nhật sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachSanPham();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                SoLuong_nud.Enabled = false;
                Them_btn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện xóa sản phẩm
        private void Xoa_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra mã sản phẩm
                if (string.IsNullOrWhiteSpace(MaSP_txt.Text))
                {
                    MessageBox.Show("Vui lòng chọn sản phẩm để xóa!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Xác nhận trước khi xóa
                if (MessageBox.Show("Bạn có chắc chắn muốn xóa sản phẩm này?", "Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                {
                    return;
                }

                // Xóa sản phẩm khỏi cơ sở dữ liệu
                sanPham.XoaSanPham(MaSP_txt.Text);

                MessageBox.Show("Xóa sản phẩm thành công!", "Thành công", MessageBoxButtons.OK, MessageBoxIcon.Information);
                HienThiDanhSachSanPham();
                XoaCacDieuKhien();
                Xoa_btn.Enabled = false;
                Sua_btn.Enabled = false;
                SoLuong_nud.Enabled = false;
                Them_btn.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện làm mới danh sách
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            // Xóa nội dung các điều khiển và làm mới danh sách
            XoaCacDieuKhien();
            HienThiDanhSachSanPham();
            Xoa_btn.Enabled = false;
            Sua_btn.Enabled = false;
            SoLuong_nud.Enabled = false;
            Them_btn.Enabled = true;
            MaSP_txt.Text = TaoMaSanPham();
        }
        //In danh sách sản phẩm
        private void InDS_btn_Click(object sender, EventArgs e)
        {
            try
            {
                int tongSP = 0;
                // Tạo danh sách sản phẩm để in danh sách
                var danhSachXuat = new List<Data.SanPham.ThongTinSanpham>();
                foreach (DataGridViewRow row in DanhSachSanPham_dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        danhSachXuat.Add(new ThongTinSanpham
                        {
                            maSP = row.Cells["MaSP"].Value?.ToString() ?? "N/A",
                            tenSP = row.Cells["TenSP"].Value?.ToString() ?? "N/A",
                            soLuong = Convert.ToInt32(row.Cells["SoLuongSP"].Value ?? 0),
                            donGia = Convert.ToDecimal(row.Cells["DonGiaSP"].Value ?? 0),
                            loaiSP = row.Cells["MaLoaiSP"].Value?.ToString() ?? "N/A",
                            thuongHieuSP = row.Cells["MaTH"].Value?.ToString() ?? "N/A",
                            duongDanAnh = row.Cells["DuongDanAnh"].Value?.ToString() ?? "N/A",
                            trangThai = row.Cells["TrangThai"].Value?.ToString() ?? "N/A"
                        });
                        tongSP++;
                    }
                }

                // Cấu hình nguồn dữ liệu và tham số cho in danh sách
                ReportDataSource xuatNguonDuLieu = new ReportDataSource
                {
                    Name = "dtsDanhSachSanPham",
                    Value = danhSachXuat
                };

                ReportParameter[] thamSo = new ReportParameter[]
                {
                    new ReportParameter("prmTong", tongSP.ToString())
                };

                // Hiển thị form báo cáo
                var formBaoCao = new BaoCaoForm(xuatNguonDuLieu, thamSo, "danhsachsanpham");
                formBaoCao.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất phiếu in: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        // Xử lý sự kiện chọn ảnh
        private void ChonAnh_btn_Click(object sender, EventArgs e)
        {
            try
            {
                using (OpenFileDialog hopThoai = new OpenFileDialog())
                {
                    hopThoai.Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp";
                    if (hopThoai.ShowDialog() == DialogResult.OK)
                    {
                        HinhAnh_pb.Image = Image.FromFile(hopThoai.FileName);
                        DuongDanAnh_txt.Text = hopThoai.FileName;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tải ảnh: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
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

        
    }
}
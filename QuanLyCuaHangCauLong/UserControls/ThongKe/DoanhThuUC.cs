using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.UserControls.SanPham;
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
using QuanLyCuaHangCauLong.Common;
using QuanLyCuaHangCauLong.UserControls.Dashboard;
using Microsoft.Reporting.WinForms;
using QuanLyCuaHangCauLong.Forms;

namespace QuanLyCuaHangCauLong.UserControls.ThongKe
{
    public partial class DoanhThuUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly ThongKeDoanhThu thongKeDoanhThu;
        private string tenNV;

        // Constructor khởi tạo UserControl
        public DoanhThuUC(string tenNV)
        {
            this.tenNV = tenNV;

            InitializeComponent();
            thongKeDoanhThu = new ThongKeDoanhThu(chuoiKetNoi);

            // Nạp danh sách dữ liệu cho các ComboBox
            NapDanhSachLoaiNhanVien();
            NapDanhSachLoaiSanPham();
            NapDanhSachThuongHieu();

            // Thiết lập giá trị mặc định cho DateTimePicker
            NgayBatDau_dtp.Value = DateTime.Today.AddMonths(-1);
            NgayKetThuc_dtp.Value = DateTime.Today;

            // Thiết lập danh sách loại thời gian
            LoaiThoiGian_cb.Items.AddRange(new string[] { "Ngày", "Tháng", "Năm" });
            LoaiThoiGian_cb.SelectedIndex = 0; // Chọn "Ngày" mặc định

            // Load dữ liệu thống kê tổng quan cho 2 cards
            var thongKe = thongKeDoanhThu.LayThongKeTongQuan();
            DoanhThu_lbl.Text = thongKe.DoanhThu.ToString("N0") + " VNĐ";
            HoaDon_lbl.Text = thongKe.SoHoaDon.ToString("N0");
        }

        // Xử lý sự kiện khi UserControl được tải
        private void DoanhThuUC_Load(object sender, EventArgs e)
        {
            // Vô hiệu hóa nút xuất báo cáo
            XuatBaoCao_btn.Enabled = false;
        }

        // Nạp danh sách nhân viên vào ComboBox
        private void NapDanhSachLoaiNhanVien()
        {
            try
            {
                using (SqlConnection ketNoi = new SqlConnection(chuoiKetNoi))
                {
                    ketNoi.Open();
                    string truyVan = "SELECT MaNV, TenNV FROM NhanVien";
                    using (SqlCommand lenh = new SqlCommand(truyVan, ketNoi))
                    {
                        using (SqlDataReader doc = lenh.ExecuteReader())
                        {
                            var bangDuLieu = new DataTable();
                            bangDuLieu.Load(doc);

                            // Thêm dòng "Tất cả"
                            DataRow tatCa = bangDuLieu.NewRow();
                            tatCa["MaNV"] = "";
                            tatCa["TenNV"] = "Tất cả";
                            bangDuLieu.Rows.InsertAt(tatCa, 0);

                            // Gán DataSource cho LoaiNhanVien_cb
                            LoaiNhanVien_cb.DataSource = bangDuLieu;
                            LoaiNhanVien_cb.DisplayMember = "TenNV";
                            LoaiNhanVien_cb.ValueMember = "MaNV";
                            LoaiNhanVien_cb.SelectedIndex = 0; // Chọn "Tất cả" mặc định
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp danh sách nhân viên: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                            // Thêm dòng "Tất cả"
                            DataRow tatCa = bangDuLieu.NewRow();
                            tatCa["MaLoaiSP"] = "";
                            tatCa["TenLoaiSP"] = "Tất cả";
                            bangDuLieu.Rows.InsertAt(tatCa, 0);

                            // Gán DataSource cho LoaiSanPham_cb
                            LoaiSanPham_cb.DataSource = bangDuLieu;
                            LoaiSanPham_cb.DisplayMember = "TenLoaiSP";
                            LoaiSanPham_cb.ValueMember = "MaLoaiSP";
                            LoaiSanPham_cb.SelectedIndex = 0; // Chọn "Tất cả" mặc định
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp danh sách loại sản phẩm: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
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

                            // Thêm dòng "Tất cả"
                            DataRow tatCa = bangDuLieu.NewRow();
                            tatCa["MaTH"] = "";
                            tatCa["TenTH"] = "Tất cả";
                            bangDuLieu.Rows.InsertAt(tatCa, 0);

                            // Gán DataSource cho LoaiThuongHieu_cb
                            LoaiThuongHieu_cb.DataSource = bangDuLieu;
                            LoaiThuongHieu_cb.DisplayMember = "TenTH";
                            LoaiThuongHieu_cb.ValueMember = "MaTH";
                            LoaiThuongHieu_cb.SelectedIndex = 0; // Chọn "Tất cả" mặc định
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi nạp danh sách thương hiệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Hiển thị danh sách thống kê doanh thu
        private void HienThiDanhSach()
        {
            try
            {
                // Lấy thông tin lọc
                DateTime ngayBatDau = NgayBatDau_dtp.Value;
                DateTime ngayKetThuc = NgayKetThuc_dtp.Value;
                string loaiThoiGian = LoaiThoiGian_cb.SelectedItem?.ToString() ?? "Ngày";
                string maNhanVien = LoaiNhanVien_cb.SelectedValue?.ToString();
                string maLoaiSanPham = LoaiSanPham_cb.SelectedValue?.ToString();
                string maThuongHieu = LoaiThuongHieu_cb.SelectedValue?.ToString();

                // Xử lý giá trị "Tất cả"
                if (maNhanVien == "") maNhanVien = null;
                if (maLoaiSanPham == "") maLoaiSanPham = null;
                if (maThuongHieu == "") maThuongHieu = null;

                // Lấy dữ liệu thống kê
                DataTable ketQua = thongKeDoanhThu.LayDuLieuDoanhThu(ngayBatDau, ngayKetThuc, loaiThoiGian, maNhanVien, maLoaiSanPham, maThuongHieu);

                // Xử lý giá trị NULL trong cột ThoiGian
                foreach (DataRow row in ketQua.Rows)
                {
                    if (row["ThoiGian"] == DBNull.Value || row["ThoiGian"] == null)
                    {
                        row["ThoiGian"] = "N/A";
                    }
                }

                // Cấu hình DataGridView
                DanhSach_dgv.AutoGenerateColumns = false;
                DanhSach_dgv.DataSource = ketQua;

                // Gán DataPropertyName cho các cột
                DanhSach_dgv.Columns["ThoiGian"].DataPropertyName = "ThoiGian";
                DanhSach_dgv.Columns["TenNV"].DataPropertyName = "TenNV";
                DanhSach_dgv.Columns["TenLoaiSP"].DataPropertyName = "TenLoaiSP";
                DanhSach_dgv.Columns["TenTH"].DataPropertyName = "TenTH";
                DanhSach_dgv.Columns["TongTien"].DataPropertyName = "TongTien";

                // Định dạng cột TongTien
                DanhSach_dgv.Columns["TongTien"].DefaultCellStyle.Format = "N0";
                DanhSach_dgv.Columns["TongTien"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;

                // Đặt header cho cột thời gian theo loại được chọn
                switch (loaiThoiGian.ToUpper())
                {
                    case "NGÀY":
                        DanhSach_dgv.Columns["ThoiGian"].HeaderText = "Ngày";
                        break;
                    case "THÁNG":
                        DanhSach_dgv.Columns["ThoiGian"].HeaderText = "Tháng/Năm";
                        break;
                    case "NĂM":
                        DanhSach_dgv.Columns["ThoiGian"].HeaderText = "Năm";
                        break;
                }

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

        // Xử lý sự kiện thống kê doanh thu
        private void ThongKe_btn_Click(object sender, EventArgs e)
        {
            try
            {
                HienThiDanhSach();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thống kê: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện làm mới giao diện
        private void TaiLai_btn_Click(object sender, EventArgs e)
        {
            // Đặt lại các điều khiển
            NgayBatDau_dtp.Value = DateTime.Today.AddMonths(-1);
            NgayKetThuc_dtp.Value = DateTime.Today;
            LoaiThoiGian_cb.SelectedIndex = 0;
            LoaiNhanVien_cb.SelectedIndex = 0;
            LoaiSanPham_cb.SelectedIndex = 0;
            LoaiThuongHieu_cb.SelectedIndex = 0;
            DanhSach_dgv.DataSource = null;
            XuatBaoCao_btn.Enabled = false;
        }

        // Xử lý sự kiện xuất báo cáo
        private void XuatBaoCao_btn_Click(object sender, EventArgs e)
        {
            try
            {
                // Lấy thông tin báo cáo
                DateTime ngayBatDau = NgayBatDau_dtp.Value;
                DateTime ngayKetThuc = NgayKetThuc_dtp.Value;
                DateTime ngayLap = DateTime.Today;
                string tenNhanVien = LoaiNhanVien_cb.Text ?? "Tất cả";
                string loaiSanPham = LoaiSanPham_cb.Text ?? "Tất cả";
                string thuongHieu = LoaiThuongHieu_cb.Text ?? "Tất cả";

                // Tính tổng tiền
                decimal tongTien = 0;
                foreach (DataGridViewRow row in DanhSach_dgv.Rows)
                {
                    if (!row.IsNewRow && row.Cells["TongTien"].Value != null)
                        tongTien += Convert.ToDecimal(row.Cells["TongTien"].Value);
                }

                // Tạo danh sách báo cáo
                var danhSachBaoCao = new List<BaoCaoDoanhThu>();
                foreach (DataGridViewRow row in DanhSach_dgv.Rows)
                {
                    if (!row.IsNewRow)
                    {
                        danhSachBaoCao.Add(new BaoCaoDoanhThu
                        {
                            thoiGian = row.Cells["ThoiGian"].Value?.ToString() ?? "N/A",
                            tenNV = row.Cells["TenNV"].Value?.ToString() ?? "N/A",
                            tenLoaiSP = row.Cells["TenLoaiSP"].Value?.ToString() ?? "N/A",
                            tenTH = row.Cells["TenTH"].Value?.ToString() ?? "N/A",
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
                    new ReportParameter("prmNgayBatDau", ngayBatDau.ToString("dd/MM/yyyy")),
                    new ReportParameter("prmNgayKetThuc", ngayKetThuc.ToString("dd/MM/yyyy")),
                    new ReportParameter("prmNguoiLap", tenNV),
                    new ReportParameter("prmNgayLap", ngayLap.ToString("dd/MM/yyyy")),
                    new ReportParameter("prmNhanVien", tenNhanVien),
                    new ReportParameter("prmLoaiSP", loaiSanPham),
                    new ReportParameter("prmThuongHieu", thuongHieu),
                    new ReportParameter("prmTongDoanhThu", tongTien.ToString("N0"))
                };

                // Hiển thị form báo cáo
                var formBaoCao = new BaoCaoForm(baoCaoNguonDuLieu, thamSo, "doanhthu");
                formBaoCao.ShowDialog();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi xuất báo cáo: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
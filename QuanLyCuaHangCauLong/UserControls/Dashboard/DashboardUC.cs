using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.DataVisualization.Charting;
using QuanLyCuaHangCauLong.Common;
using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.UserControls.QuanLy;

namespace QuanLyCuaHangCauLong.UserControls.Dashboard
{
    public partial class DashboardUC : UserControl
    {
        // Biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.Dashboard dashboard;

        // Constructor khởi tạo UserControl
        public DashboardUC()
        {
            InitializeComponent();
            dashboard = new Data.Dashboard(chuoiKetNoi);

            // Thiết lập giá trị mặc định cho các điều khiển
            NgayBatDau_dtp.Value = DateTime.Today.AddMonths(-1);
            NgayKetThuc_dtp.Value = DateTime.Today;

            LoaiThoiGian_cb.Items.AddRange(new string[] { "Ngày", "Tháng", "Năm" });
            LoaiThoiGian_cb.SelectedIndex = 0;

            ConfigureCharts();

            TaiDuLieu();
        }

        // Xử lý sự kiện khi UserControl được tải
        private void DashboardUC_Load(object sender, EventArgs e)
        {
            // Hiện tại không có xử lý cụ thể trong sự kiện Load
        }

        // Cấu hình các biểu đồ
        private void ConfigureCharts()
        {
            try
            {
                // Cấu hình biểu đồ DoanhThu_chart
                DoanhThu_chart.Series.Clear();
                DoanhThu_chart.Series.Add("DoanhThu");
                DoanhThu_chart.Series["DoanhThu"].ChartType = SeriesChartType.Spline;
                DoanhThu_chart.Series["DoanhThu"].Color = Color.Blue;
                DoanhThu_chart.ChartAreas[0].AxisX.Title = "Thời gian";
                DoanhThu_chart.ChartAreas[0].AxisY.Title = "Doanh thu (VNĐ)";
                DoanhThu_chart.ChartAreas[0].AxisX.Interval = 1;
                DoanhThu_chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                DoanhThu_chart.ChartAreas[0].AxisY.LabelStyle.Format = "{N0}";
                DoanhThu_chart.Titles.Clear();
                DoanhThu_chart.Titles.Add("Doanh Thu Theo Thời Gian");

                // Cấu hình biểu đồ HoaDon_chart
                HoaDon_chart.Series.Clear();
                HoaDon_chart.Series.Add("SoHoaDon");
                HoaDon_chart.Series["SoHoaDon"].ChartType = SeriesChartType.Column;
                HoaDon_chart.Series["SoHoaDon"].Color = Color.Green;
                HoaDon_chart.ChartAreas[0].AxisX.Title = "Thời gian";
                HoaDon_chart.ChartAreas[0].AxisY.Title = "Số hóa đơn";
                HoaDon_chart.ChartAreas[0].AxisX.Interval = 1;
                HoaDon_chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                HoaDon_chart.Titles.Clear();
                HoaDon_chart.Titles.Add("Số Hóa Đơn Theo Thời Gian");

                // Cấu hình biểu đồ LoaiSP_chart
                LoaiSP_chart.Series.Clear();
                LoaiSP_chart.Series.Add("SoSanPham");
                LoaiSP_chart.Series["SoSanPham"].ChartType = SeriesChartType.Pie;
                LoaiSP_chart.Series["SoSanPham"].Color = Color.Orange;
                LoaiSP_chart.ChartAreas[0].AxisX.Title = "Loại sản phẩm";
                LoaiSP_chart.ChartAreas[0].AxisY.Title = "Số sản phẩm";
                LoaiSP_chart.ChartAreas[0].AxisX.Interval = 1;
                LoaiSP_chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                LoaiSP_chart.Titles.Clear();
                LoaiSP_chart.Titles.Add("Số Sản Phẩm Theo Loại");

                // Cấu hình biểu đồ ThuongHieu_chart
                ThuongHieu_chart.Series.Clear();
                ThuongHieu_chart.Series.Add("SoSanPham");
                ThuongHieu_chart.Series["SoSanPham"].ChartType = SeriesChartType.Pie;
                ThuongHieu_chart.Series["SoSanPham"].Color = Color.Purple;
                ThuongHieu_chart.ChartAreas[0].AxisX.Title = "Thương hiệu";
                ThuongHieu_chart.ChartAreas[0].AxisY.Title = "Số sản phẩm";
                ThuongHieu_chart.ChartAreas[0].AxisX.Interval = 1;
                ThuongHieu_chart.ChartAreas[0].AxisX.MajorGrid.Enabled = false;
                ThuongHieu_chart.Titles.Clear();
                ThuongHieu_chart.Titles.Add("Số Sản Phẩm Theo Thương Hiệu");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi cấu hình biểu đồ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tải dữ liệu cho các điều khiển và biểu đồ
        private void TaiDuLieu()
        {
            try
            {
                DateTime ngayBatDau = NgayBatDau_dtp.Value;
                DateTime ngayKetThuc = NgayKetThuc_dtp.Value;
                string loaiThoiGian = LoaiThoiGian_cb.SelectedItem?.ToString() ?? "Ngày";

                // Load dữ liệu cho các thẻ thông tin (cards)
                var thongKe = dashboard.LayThongKeTongQuan(ngayBatDau, ngayKetThuc);
                DoanhThu_lbl.Text = thongKe.DoanhThu.ToString("N0") + " VNĐ";
                HoaDon_lbl.Text = thongKe.SoHoaDon.ToString("N0");
                KhachHangDangKy_lbl.Text = thongKe.SoKhachHangMoi.ToString("N0");
                SanPhamBanRa_lbl.Text = thongKe.SoSanPhamBan.ToString("N0");

                // Load dữ liệu cho DataGridView Top 10 sản phẩm bán chạy
                TopSanPham_dgv.AutoGenerateColumns = false;
                TopSanPham_dgv.DataSource = dashboard.LayTop10SanPhamBanChay(ngayBatDau, ngayKetThuc);
                TopSanPham_dgv.Columns["MaSP"].DataPropertyName = "MaSP";
                TopSanPham_dgv.Columns["TenSP"].DataPropertyName = "TenSP";
                TopSanPham_dgv.Columns["SoLuongBan"].DataPropertyName = "SoLuongBan";
                TopSanPham_dgv.Columns["DoanhThu"].DataPropertyName = "DoanhThu";

                // Load dữ liệu cho DataGridView Top 10 hóa đơn giá trị cao
                TopHoaDon_dgv.AutoGenerateColumns = false;
                TopHoaDon_dgv.DataSource = dashboard.LayTop10HoaDonGiaTriCao(ngayBatDau, ngayKetThuc);
                TopHoaDon_dgv.Columns["MaHD"].DataPropertyName = "MaHD";
                TopHoaDon_dgv.Columns["NgayLap"].DataPropertyName = "NgayLap";
                TopHoaDon_dgv.Columns["TenKH"].DataPropertyName = "TenKH";
                TopHoaDon_dgv.Columns["TenNV"].DataPropertyName = "TenNV";
                TopHoaDon_dgv.Columns["TongTien"].DataPropertyName = "TongTien";

                // Load dữ liệu cho DataGridView sản phẩm tồn kho thấp
                SanPhamTonKhoThap_dgv.AutoGenerateColumns = false;
                SanPhamTonKhoThap_dgv.DataSource = dashboard.LaySanPhamTonKhoThap();
                SanPhamTonKhoThap_dgv.Columns["MaSP_TK"].DataPropertyName = "MaSP";
                SanPhamTonKhoThap_dgv.Columns["TenSP_TK"].DataPropertyName = "TenSP";
                SanPhamTonKhoThap_dgv.Columns["SoLuongSP_TK"].DataPropertyName = "SoLuongSP";
                SanPhamTonKhoThap_dgv.Columns["TenLoaiSP_TK"].DataPropertyName = "TenLoaiSP";
                SanPhamTonKhoThap_dgv.Columns["TenTH_TK"].DataPropertyName = "TenTH";

                // Load dữ liệu cho DataGridView Top 10 khách hàng giao dịch nhiều
                TopKhachHang_dgv.AutoGenerateColumns = false;
                TopKhachHang_dgv.DataSource = dashboard.LayTop10KhachHangGiaoDichNhieu(ngayBatDau, ngayKetThuc);
                TopKhachHang_dgv.Columns["MaKH_GD"].DataPropertyName = "MaKH";
                TopKhachHang_dgv.Columns["TenKH_GD"].DataPropertyName = "TenKH";
                TopKhachHang_dgv.Columns["SDT_GD"].DataPropertyName = "SDT";
                TopKhachHang_dgv.Columns["DiemTichLuy_GD"].DataPropertyName = "DiemTichLuy";
                TopKhachHang_dgv.Columns["SoGD"].DataPropertyName = "SoGiaoDich";
                TopKhachHang_dgv.Columns["TongGiaTriGiaoDich"].DataPropertyName = "TongGiaTriGiaoDich";

                // Load dữ liệu cho các biểu đồ
                LoadChartData(ngayBatDau, ngayKetThuc, loaiThoiGian);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Tải dữ liệu cho các biểu đồ
        private void LoadChartData(DateTime startDate, DateTime endDate, string timeType)
        {
            try
            {
                // Xóa dữ liệu cũ trên các biểu đồ
                DoanhThu_chart.Series["DoanhThu"].Points.Clear();
                HoaDon_chart.Series["SoHoaDon"].Points.Clear();
                LoaiSP_chart.Series["SoSanPham"].Points.Clear();
                ThuongHieu_chart.Series["SoSanPham"].Points.Clear();

                // Load dữ liệu cho biểu đồ doanh thu
                DataTable dtDoanhThu = dashboard.LayDuLieuBieuDoDoanhThu(startDate, endDate, timeType);
                foreach (DataRow row in dtDoanhThu.Rows)
                {
                    string label;
                    if (timeType == "Ngày")
                    {
                        label = ((DateTime)row["ThoiGian"]).ToString("dd/MM");
                    }
                    else
                    {
                        label = row["ThoiGian"].ToString();
                    }
                    DoanhThu_chart.Series["DoanhThu"].Points.AddXY(label, row["DoanhThu"]);
                }

                // Load dữ liệu cho biểu đồ số hóa đơn
                DataTable dtSoHoaDon = dashboard.LayDuLieuBieuDoSoHoaDon(startDate, endDate, timeType);
                foreach (DataRow row in dtSoHoaDon.Rows)
                {
                    string label;
                    if (timeType == "Ngày")
                    {
                        label = ((DateTime)row["ThoiGian"]).ToString("dd/MM");
                    }
                    else
                    {
                        label = row["ThoiGian"].ToString();
                    }
                    HoaDon_chart.Series["SoHoaDon"].Points.AddXY(label, row["SoHoaDon"]);
                }

                // Load dữ liệu cho biểu đồ loại sản phẩm
                DataTable dtLoaiSanPham = dashboard.LayDuLieuBieuDoLoaiSanPham();
                foreach (DataRow row in dtLoaiSanPham.Rows)
                {
                    string tenLoaiSP = row["TenLoaiSP"].ToString();
                    if (tenLoaiSP.Length > 15)
                    {
                        tenLoaiSP = tenLoaiSP.Substring(0, 12) + "...";
                    }
                    LoaiSP_chart.Series["SoSanPham"].Points.AddXY(tenLoaiSP, row["SoSanPham"]);
                }

                // Load dữ liệu cho biểu đồ thương hiệu
                DataTable dtThuongHieu = dashboard.LayDuLieuBieuDoThuongHieu();
                foreach (DataRow row in dtThuongHieu.Rows)
                {
                    string tenTH = row["TenTH"].ToString();
                    if (tenTH.Length > 15)
                    {
                        tenTH = tenTH.Substring(0, 12) + "...";
                    }
                    ThuongHieu_chart.Series["SoSanPham"].Points.AddXY(tenTH, row["SoSanPham"]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải dữ liệu biểu đồ: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Xử lý sự kiện click nút Xem
        private void Xem_btn_Click(object sender, EventArgs e)
        {
            TaiDuLieu();
        }
    }
}
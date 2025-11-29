using Microsoft.Reporting.WinForms;
using QuanLyCuaHangCauLong.Common;
using QuanLyCuaHangCauLong.Data;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Windows.Forms;
using System.Xml;

namespace QuanLyCuaHangCauLong.Forms
{
    public partial class ThanhToanForm : Form
    {
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.KhachHang khachHang;
        private readonly Data.ChinhSachDiemTichLuy chinhSachDiemTichLuy;
        private readonly Data.HoaDon hoaDon;
        private readonly decimal tongTien;
        private readonly DataTable sanPhamDaChon;
        private decimal tienKhachDua;
        private string maNV;
        private string tenNV;

        public bool ThanhToanThanhCong { get; set; } = false;
        public ThanhToanForm(decimal tongTien, DataTable sanPhamDaChon, string maNV, string tenNV)
        {
            InitializeComponent();
            khachHang = new Data.KhachHang(chuoiKetNoi);
            chinhSachDiemTichLuy = new Data.ChinhSachDiemTichLuy(chuoiKetNoi);
            hoaDon = new Data.HoaDon(chuoiKetNoi);
            this.tongTien = tongTien;
            this.sanPhamDaChon = sanPhamDaChon;
            this.maNV = maNV;
            this.tenNV = tenNV;
            tienKhachDua = 0;
        }

        private void ThanhToanForm_Load_1(object sender, EventArgs e)
        {
            TienPhaiThu_lbl.Text = tongTien.ToString("N0");
            TienKhachDua_lbl.Text = "0";
            TienTraKhach_lbl.Text = "0";
            TienGiamTheoDiem_lbl.Text = "0";
            LoaiKhachHang_cb.Items.AddRange(new string[] { "Lẻ", "Đã có tài khoản" });
            LoaiKhachHang_cb.SelectedIndex = 0;
            CapNhatTrangThaiControls();
        }

        private void CapNhatTrangThaiControls()
        {
            bool isKhachLe = LoaiKhachHang_cb.SelectedItem?.ToString() == "Lẻ";
            TimKiem_txt.Enabled = !isKhachLe;
            DanhSachKhachHang_dgv.Enabled = !isKhachLe;
            DanhSachDiemTichLuy_dgv.Enabled = !isKhachLe;

            if (isKhachLe)
            {
                MaKH_txt.Text = "KL005";
                TenKH_txt.Text = "lẻ";
                SDT_txt.Text = "0";
                DiemTichLuy_txt.Text = "0";
                TimKiem_txt.Text = "";
                DanhSachKhachHang_dgv.DataSource = null;
                DanhSachDiemTichLuy_dgv.DataSource = null;
                TienGiamTheoDiem_lbl.Text = "0";
            }
            else
            {
                HienThiDanhSachKhachHang();
            }
            CapNhatTienTraKhach();
        }

        private void HienThiDanhSachKhachHang(string tuKhoa = null)
        {
            try
            {
                string loaiKhachHang = null;
                if (LoaiKhachHang_cb.SelectedItem.ToString() == "Lẻ") { loaiKhachHang = LoaiKhachHang_cb.SelectedItem?.ToString(); }
                if (LoaiKhachHang_cb.SelectedItem.ToString() == "Đã có tài khoản") { loaiKhachHang = LoaiKhachHang_cb.SelectedItem?.ToString(); }

                DanhSachKhachHang_dgv.AutoGenerateColumns = false;
                DanhSachKhachHang_dgv.DataSource = khachHang.LayDanhSachKhachHang(loaiKhachHang, tuKhoa, true);

                DanhSachKhachHang_dgv.Columns["MaKH"].DataPropertyName = "MaKH";
                DanhSachKhachHang_dgv.Columns["TenKH"].DataPropertyName = "TenKH";
                DanhSachKhachHang_dgv.Columns["SDT"].DataPropertyName = "SDT";
                DanhSachKhachHang_dgv.Columns["DiaChi"].DataPropertyName = "DiaChi";
                DanhSachKhachHang_dgv.Columns["DiemTichLuy"].DataPropertyName = "DiemTichLuy";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách khách hàng", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HienThiDanhSachChinhSachDiemTichLuy(int diemTichLuy)
        {
            try
            {
                DanhSachDiemTichLuy_dgv.AutoGenerateColumns = false;
                DanhSachDiemTichLuy_dgv.DataSource = chinhSachDiemTichLuy.LayDanhSachChinhSachDiemTichLuy(diemTichLuy);

                DanhSachDiemTichLuy_dgv.Columns["MaCSDTL"].DataPropertyName = "MaCSDTL";
                DanhSachDiemTichLuy_dgv.Columns["DiemToiThieu"].DataPropertyName = "DiemToiThieu";
                DanhSachDiemTichLuy_dgv.Columns["PhanTramGiam"].DataPropertyName = "PhanTramGiam";
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách chính sách điểm tích lũy", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private string TaoMaHoaDon()
        {
            try
            {
                return Functions.TaoMaTuTang("HoaDon", "MaHD", "HD", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "HD001";
            }
        }
        private string TaoMaKhachHangLe()
        {
            try
            {
                return Functions.TaoMaTuTang("KhachHang", "MaKH", "KL", 3);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return "KL001";
            }
        }
        private void LoaiKhachHang_cb_SelectedValueChanged(object sender, EventArgs e)
        {
            CapNhatTrangThaiControls();
        }

        private void DanhSachKhachHang_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow dong = DanhSachKhachHang_dgv.Rows[e.RowIndex];
                    MaKH_txt.Text = dong.Cells["MaKH"].Value?.ToString();
                    TenKH_txt.Text = dong.Cells["TenKH"].Value?.ToString();
                    SDT_txt.Text = dong.Cells["SDT"].Value?.ToString();
                    int diemTichLuy = Convert.ToInt32(dong.Cells["DiemTichLuy"].Value);
                    DiemTichLuy_txt.Text = diemTichLuy.ToString();
                    HienThiDanhSachChinhSachDiemTichLuy(diemTichLuy);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chọn khách hàng: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DanhSachDiemTichLuy_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow dong = DanhSachDiemTichLuy_dgv.Rows[e.RowIndex];
                    decimal phanTramGiam = Convert.ToDecimal(dong.Cells["PhanTramGiam"].Value);
                    TienGiamTheoDiem_lbl.Text = (phanTramGiam*tongTien/100).ToString("N0");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chọn chính sách điểm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void TimKiem_txt_TextChange(object sender, EventArgs e)
        {
            string tuKhoa = TimKiem_txt.Text.Trim();
            HienThiDanhSachKhachHang(tuKhoa);
        }

        private void T500_btn_Click(object sender, EventArgs e) => ThemTienKhachDua(500000);
        private void T200_btn_Click(object sender, EventArgs e) => ThemTienKhachDua(200000);
        private void T100_btn_Click(object sender, EventArgs e) => ThemTienKhachDua(100000);
        private void T50_btn_Click(object sender, EventArgs e) => ThemTienKhachDua(50000);
        private void T20_btn_Click(object sender, EventArgs e) => ThemTienKhachDua(20000);
        private void T10_btn_Click(object sender, EventArgs e) => ThemTienKhachDua(10000);
        private void T5_btn_Click(object sender, EventArgs e) => ThemTienKhachDua(5000);
        private void T2_btn_Click(object sender, EventArgs e) => ThemTienKhachDua(2000);
        private void T1_btn_Click(object sender, EventArgs e) => ThemTienKhachDua(1000);

        private void ThemTienKhachDua(decimal soTien)
        {
            try
            {
                tienKhachDua += soTien;
                TienKhachDua_lbl.Text = tienKhachDua.ToString("N0");
                CapNhatTienTraKhach();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thêm số tiền: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void CapNhatTienTraKhach()
        {
            try
            {
                decimal tienPhaiThu = tongTien;
                decimal tienGiamTheoDiem = Convert.ToDecimal(TienGiamTheoDiem_lbl.Text);
                decimal tienCoThueVAT = Convert.ToDecimal(ThueVAT_lbl.Text)*tienPhaiThu/100;
                decimal tienTraKhach = Convert.ToDecimal(TienKhachDua_lbl.Text) - tienPhaiThu + tienGiamTheoDiem - tienCoThueVAT;
                TienTraKhach_lbl.Text = tienTraKhach.ToString("N0");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi tính tiền trả khách: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ThanhToan_btn_Click(object sender, EventArgs e)
        {
            try
            {
                decimal tienTraKhach = Convert.ToDecimal(TienTraKhach_lbl.Text);
                if(tienTraKhach < 0)
                {
                    MessageBox.Show("Số tiền khách đưa không đủ!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                string maKH = MaKH_txt.Text;
                if (LoaiKhachHang_cb.SelectedItem?.ToString() == "Lẻ")
                {
                    maKH = TaoMaKhachHangLe();
                    khachHang.ThemKhachHang(maKH, null, null, null, null, null, null, null);
                }

                string maCSDTL = null;
                if (DanhSachDiemTichLuy_dgv.SelectedRows.Count > 0)
                {
                    maCSDTL = DanhSachDiemTichLuy_dgv.SelectedRows[0].Cells["MaCSDTL"].Value?.ToString();
                }

                string maHD = TaoMaHoaDon();

                decimal tienHD = tongTien - Convert.ToDecimal(TienGiamTheoDiem_lbl.Text);
                hoaDon.ThemHoaDon(maHD, maNV, maKH, DateTime.Now, tienHD, maCSDTL, sanPhamDaChon);

                // Thêm điểm tích luỹ cho khách hàng đã có tài khoản
                if (LoaiKhachHang_cb.SelectedItem?.ToString() == "Đã có tài khoản")
                {
                    int themDiemTichLuy = (int)((tongTien - Convert.ToDecimal(TienGiamTheoDiem_lbl.Text))/ 1000);
                    khachHang.CapNhatDiemTichLuy(maKH, themDiemTichLuy);
                }

                ThanhToanThanhCong = true;
                MessageBox.Show("Thanh toán thành công! Mã hóa đơn: " + maHD, "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);


                // Chuẩn bị dữ liệu cho phiếu in
                string ngayLap = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"); // Định dạng ngày giờ
                string tenKH = TenKH_txt.Text; // Lấy tên khách hàng từ TextBox

                var danhSachXuat = new List<XuatChiTietHoaDon>();
                foreach (DataRow row in sanPhamDaChon.Rows)
                {
                    danhSachXuat.Add(new XuatChiTietHoaDon
                    {
                        tenSP = row["TenSP"].ToString() ?? "N/A",
                        soLuong = Convert.ToInt32(row["SoLuong"]),
                        donGia = Convert.ToDecimal(row["DonGia"]),
                        thanhTien = Convert.ToDecimal(row["ThanhTien"])
                    });
                }

                ReportDataSource xuatNguonDuLieu = new ReportDataSource
                {
                    Name = "dstDanhSachXuat",
                    Value = danhSachXuat
                };

                ReportParameter[] thamSo = new ReportParameter[]
                {
                    new ReportParameter("prmSoHoaDon", maHD),
                    new ReportParameter("prmNhanVien", tenNV),
                    new ReportParameter("prmNgay", ngayLap),
                    new ReportParameter("prmKhachHang", tenKH),
                    new ReportParameter("prmTongThanhToan", tienHD.ToString("N0"))
                };

                var formBaoCao = new BaoCaoForm(xuatNguonDuLieu, thamSo, "hoadon");
                formBaoCao.ShowDialog();

                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi thanh toán: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void HuyBo_btn_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void NhapSoTien_txt_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
            if (e.KeyChar == (char)Keys.Enter)
            {
                try
                {
                    if (decimal.TryParse(NhapSoTien_txt.Text, out decimal soTien))
                    {
                        // Định dạng số tiền theo N0 (có dấu phân cách hàng nghìn, không có phần thập phân)
                        TienKhachDua_lbl.Text = soTien.ToString("N0");
                        CapNhatTienTraKhach();
                    }
                    else
                    {
                        MessageBox.Show("Vui lòng nhập số tiền hợp lệ!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Lỗi khi nhập số tiền: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }
        private bool isFormatting = false; // Biến để tránh vòng lặp vô hạn trong TextChanged
        private void NhapSoTien_txt_TextChange(object sender, EventArgs e)
        {
            // Tránh vòng lặp vô hạn khi định dạng
            if (isFormatting) return;
            try
            {
                isFormatting = true;

                // Lấy giá trị hiện tại, loại bỏ dấu phẩy
                string input = NhapSoTien_txt.Text.Replace(",", "");

                // Kiểm tra nếu input là số hợp lệ
                if (decimal.TryParse(input, out decimal soTien) && soTien >= 0)
                {
                    // Định dạng lại giá trị theo N0
                    NhapSoTien_txt.Text = soTien.ToString("N0");
                    // Đặt con trỏ ở cuối TextBox
                    NhapSoTien_txt.SelectionStart = NhapSoTien_txt.Text.Length;
                }
                else if (!string.IsNullOrWhiteSpace(input))
                {
                    // Nếu input không hợp lệ, giữ nguyên giá trị trước đó hoặc xóa
                    NhapSoTien_txt.Text = "";
                }
            }
            catch
            {
                // Nếu có lỗi, giữ nguyên giá trị hoặc xóa
                NhapSoTien_txt.Text = "";
            }
            finally
            {
                isFormatting = false;
            }
        }
    }
}
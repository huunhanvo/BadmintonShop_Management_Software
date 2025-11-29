using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using QuanLyCuaHangCauLong.Data;
using QuanLyCuaHangCauLong.Forms;
using System.Globalization;

namespace QuanLyCuaHangCauLong.UserControls.BanHang
{
    public partial class BanHangUC : UserControl
    {
        // Khai báo biến và thuộc tính
        private readonly string chuoiKetNoi = ConfigurationManager.ConnectionStrings["QuanLyCuaHangCauLong"].ConnectionString;
        private readonly Data.SanPham sanPham;
        private DataTable sanPhamDaChon;
        private string maNV;
        private string tenNV;


        // Constructor
        public BanHangUC(string maNV, string tenNV)
        {
            InitializeComponent();
            sanPham = new Data.SanPham(chuoiKetNoi);
            KhoiTaoSanPhamDaChon();
            this.maNV = maNV;
            this.tenNV = tenNV;
        }

        // Event Load
        private void BanHangUC_Load(object sender, EventArgs e)
        {
            HienThiDanhSachSanPham();
            Xoa_btn.Enabled = false;
            TinhTien_lbl.Text = "0";
        }

        // Phương thức khởi tạo và hiển thị dữ liệu
        private void KhoiTaoSanPhamDaChon()
        {
            sanPhamDaChon = new DataTable();
            sanPhamDaChon.Columns.Add("MaSP", typeof(string));
            sanPhamDaChon.Columns.Add("TenSP", typeof(string));
            sanPhamDaChon.Columns.Add("SoLuong", typeof(int));
            sanPhamDaChon.Columns.Add("DonGia", typeof(decimal));
            sanPhamDaChon.Columns.Add("ThanhTien", typeof(decimal));

            DanhSachSanPhamDaChon_dgv.AutoGenerateColumns = false;
            DanhSachSanPhamDaChon_dgv.DataSource = sanPhamDaChon;

            DanhSachSanPhamDaChon_dgv.Columns["TenSP_DaChon"].DataPropertyName = "TenSP";
            DanhSachSanPhamDaChon_dgv.Columns["SoLuong_DaChon"].DataPropertyName = "SoLuong";
            DanhSachSanPhamDaChon_dgv.Columns["DonGia_DaChon"].DataPropertyName = "ThanhTien";
        }

        private void HienThiDanhSachSanPham(string loaiSanPham = null, string tuKhoa = null)
        {
            try
            {
                DanhSachSanPham_dgv.AutoGenerateColumns = false;
                DanhSachSanPham_dgv.DataSource = sanPham.LayDanhSachSanPham(loaiSanPham, null, tuKhoa, true);

                DanhSachSanPham_dgv.Columns["HinhAnhHienThi"].DataPropertyName = "HinhAnhHienThi";
                DanhSachSanPham_dgv.Columns["TenSP"].DataPropertyName = "TenSP";
                DanhSachSanPham_dgv.Columns["TenLoaiSP"].DataPropertyName = "TenLoaiSP";
                DanhSachSanPham_dgv.Columns["TenTH"].DataPropertyName = "TenTH";
                DanhSachSanPham_dgv.Columns["DonGiaSP"].DataPropertyName = "DonGiaSP";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Lỗi hiển thị danh sách sản phẩm", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Phương thức xử lý nghiệp vụ
        private void CapNhatTongTien()
        {
            decimal tongTien = sanPhamDaChon.AsEnumerable().Sum(r => r.Field<decimal>("ThanhTien"));
            TinhTien_lbl.Text = tongTien.ToString("N0");
        }

        // Event handlers cho DataGridView
        private void DanhSachSanPham_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                try
                {
                    DataGridViewRow dong = DanhSachSanPham_dgv.Rows[e.RowIndex];
                    DataRowView rowView = dong.DataBoundItem as DataRowView;
                    if (rowView != null)
                    {
                        DataRow row = rowView.Row;
                        string maSP = row["MaSP"].ToString();
                        string tenSP = row["TenSP"].ToString();
                        decimal donGia = Convert.ToDecimal(row["DonGiaSP"]);
                        int soLuongTon = Convert.ToInt32(row["SoLuongSP"]);

                        var existingRow = sanPhamDaChon.AsEnumerable()
                            .FirstOrDefault(r => r.Field<string>("MaSP") == maSP);

                        if (existingRow != null)
                        {
                            int soLuongHienTai = existingRow.Field<int>("SoLuong");
                            if (soLuongHienTai + 1 > soLuongTon)
                            {
                                MessageBox.Show("Số lượng vượt quá tồn kho!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            existingRow.SetField("SoLuong", soLuongHienTai + 1);
                            existingRow.SetField("ThanhTien", (soLuongHienTai + 1) * donGia);
                        }
                        else
                        {
                            if (soLuongTon < 1)
                            {
                                MessageBox.Show("Sản phẩm đã hết hàng!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                return;
                            }
                            DataRow newRow = sanPhamDaChon.NewRow();
                            newRow["MaSP"] = maSP;
                            newRow["TenSP"] = tenSP;
                            newRow["SoLuong"] = 1;
                            newRow["DonGia"] = donGia;
                            newRow["ThanhTien"] = donGia;
                            sanPhamDaChon.Rows.Add(newRow);
                        }

                        CapNhatTongTien();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Lỗi khi chọn sản phẩm: " + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void DanhSachSanPhamDaChon_dgv_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                Xoa_btn.Enabled = true;
            }
        }

        // Event handlers cho TextBox
        private void TiemKiem_txt_TextChange(object sender, EventArgs e)
        {
            string tuKhoa = TiemKiem_txt.Text.Trim();
            HienThiDanhSachSanPham(null, string.IsNullOrEmpty(tuKhoa) ? null : tuKhoa);
        }

        // Event handlers cho các nút lọc sản phẩm
        private void TatCaSP_btn_Click(object sender, EventArgs e)
        {
            HienThiDanhSachSanPham();
        }

        private void Vot_btn_Click(object sender, EventArgs e)
        {
            HienThiDanhSachSanPham("LSP01");
        }

        private void Balo_btn_Click(object sender, EventArgs e)
        {
            HienThiDanhSachSanPham("LSP02");
        }

        private void PhuKien_btn_Click(object sender, EventArgs e)
        {
            HienThiDanhSachSanPham("LSP03");
        }

        // Event handlers cho các nút thao tác
        private void Xoa_btn_Click(object sender, EventArgs e)
        {
            try
            {
                if (DanhSachSanPhamDaChon_dgv.SelectedRows.Count > 0)
                {
                    var selectedRow = DanhSachSanPhamDaChon_dgv.SelectedRows[0];
                    DanhSachSanPhamDaChon_dgv.Rows.Remove(selectedRow);
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

        private void Huy_btn_Click(object sender, EventArgs e)
        {
            sanPhamDaChon.Clear();
            Xoa_btn.Enabled = false;
            TinhTien_lbl.Text = "0";
        }

        private void TinhTien_btn_Click(object sender, EventArgs e)
        {
            if (decimal.TryParse(TinhTien_lbl.Text, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal tongTien) && tongTien > 0)
            {
                ThanhToanForm thanhToanForm = new ThanhToanForm(tongTien, sanPhamDaChon, maNV, tenNV);
                thanhToanForm.ShowDialog();
                // Kiểm tra nếu thanh toán thành công thì xóa dữ liệu
                if (thanhToanForm.ThanhToanThanhCong)
                {
                    sanPhamDaChon.Clear();
                    Xoa_btn.Enabled = false;
                    TinhTien_lbl.Text = "0";
                }
            }
            else
            {
                MessageBox.Show("Vui lòng chọn ít nhất một sản phẩm để thanh toán!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}
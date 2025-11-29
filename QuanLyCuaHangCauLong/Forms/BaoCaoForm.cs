using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Reporting.WinForms;

namespace QuanLyCuaHangCauLong.Forms
{
    public partial class BaoCaoForm : Form
    {
        private readonly ReportDataSource baoCaoNguonDuLieu;
        private readonly ReportParameter[] thamSo;
        private string baoCao;
        public BaoCaoForm(ReportDataSource baoCaoNguonDuLieu, ReportParameter[] thamSo, string baoCao)
        {
            InitializeComponent();
            this.baoCaoNguonDuLieu = baoCaoNguonDuLieu;
            this.thamSo = thamSo;
            this.baoCao = baoCao;
        }

        private void BaoCaoForm_Load(object sender, EventArgs e)
        {
            try
            {
                reportViewer1.ProcessingMode = ProcessingMode.Local;
                reportViewer1.LocalReport.DataSources.Clear();
                reportViewer1.LocalReport.DataSources.Add(baoCaoNguonDuLieu);
                string loaiBaoCao = "";
                switch (baoCao)
                {
                    case "doanhthu":
                        loaiBaoCao = "QuanLyCuaHangCauLong.Reports.DoanhThuReport.rdlc";
                        break;
                    case "khachhang":
                        loaiBaoCao = "QuanLyCuaHangCauLong.Reports.KhachHangReport.rdlc";
                        break;
                    case "nhanvien":
                        loaiBaoCao = "QuanLyCuaHangCauLong.Reports.NhanVienReport.rdlc";
                        break;
                    case "hoadon":
                        loaiBaoCao = "QuanLyCuaHangCauLong.Prints.HoaDonPrint.rdlc";
                        break;
                    case "phieunhap":
                        loaiBaoCao = "QuanLyCuaHangCauLong.Prints.PhieuNhapPrint.rdlc";
                        break;
                    case "danhsachsanpham":
                        loaiBaoCao = "QuanLyCuaHangCauLong.Prints.DanhSachSanPhamPrint.rdlc";
                        break;
                    case "danhsachkhachhang":
                        loaiBaoCao = "QuanLyCuaHangCauLong.Prints.DanhSachKhachHangPrint.rdlc";
                        break;
                    case "danhsachnhanvien":
                        loaiBaoCao = "QuanLyCuaHangCauLong.Prints.DanhSachNhanVienPrint.rdlc";
                        break;
                }
                reportViewer1.LocalReport.ReportEmbeddedResource = loaiBaoCao;
                reportViewer1.LocalReport.SetParameters(thamSo);
                reportViewer1.RefreshReport();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi tải báo cáo/phiếu in: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            this.reportViewer1.RefreshReport();
        }
    }
}

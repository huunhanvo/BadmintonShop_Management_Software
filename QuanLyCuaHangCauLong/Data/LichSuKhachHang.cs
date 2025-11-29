using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace QuanLyCuaHangCauLong.Data
{
    public class LichSuKhachHang
    {
        private readonly string chuoiKetNoi;
        public LichSuKhachHang(string chuoiKetNoi)
        {
            this.chuoiKetNoi = chuoiKetNoi;
        }

    }
}

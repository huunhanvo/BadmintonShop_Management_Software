-- Tạo cơ sở dữ liệu
CREATE DATABASE QuanLyCuaHangCauLong;
GO

USE QuanLyCuaHangCauLong;
GO

-- Tạo bảng LoaiSanPham
CREATE TABLE LoaiSanPham (
    MaLoaiSP varchar(10) PRIMARY KEY,
    TenLoaiSP varchar(100) NOT NULL
);

-- Tạo bảng ThuongHieu
CREATE TABLE ThuongHieu (
    MaTH varchar(10) PRIMARY KEY,
    TenTH varchar(30) NOT NULL
);

-- Tạo bảng SanPham
CREATE TABLE SanPham (
    MaSP varchar(10) PRIMARY KEY,
    TenSP nvarchar(100) NOT NULL,
    DonGiaSP decimal(18,2) NOT NULL,
    SoLuongSP int NOT NULL,
    HinhAnh varchar(200),
    TrangThai bit NOT NULL DEFAULT 1, -- 1: Hiện, 0: Ẩn
    MaLoaiSP varchar(10) NOT NULL,
    MaTH varchar(10) NOT NULL,
    FOREIGN KEY (MaLoaiSP) REFERENCES LoaiSanPham(MaLoaiSP),
    FOREIGN KEY (MaTH) REFERENCES ThuongHieu(MaTH)
);

-- Tạo bảng NhanVien
CREATE TABLE NhanVien (
    MaNV varchar(10) PRIMARY KEY,
    TenNV nvarchar(30) NOT NULL,
    NgaySinh date NOT NULL,
    DiaChi nvarchar(100),
    SDT varchar(10),
    TrangThai bit NOT NULL DEFAULT 1, -- 1: Đang làm, 0: Đã nghỉ
    VaiTro nvarchar(20) NOT NULL,
    TenDangNhap varchar(20) NOT NULL,
    MatKhau varchar(256) NOT NULL,
    CoTheDangNhap bit NOT NULL,
    CONSTRAINT UQ_TenDangNhap UNIQUE (TenDangNhap)
);

-- Tạo bảng KhachHang
CREATE TABLE KhachHang (
    MaKH varchar(10) PRIMARY KEY,
    TenKH nvarchar(50) NOT NULL,
    DiaChi nvarchar(50),
    SDT varchar(10),
    Email varchar(50),
    NgayTao datetime NOT NULL DEFAULT GETDATE(),
    DiemTichLuy int NOT NULL DEFAULT 0,
    TrangThai bit NOT NULL DEFAULT 1, -- 1: Đang giao dịch, 0: Không còn giao dịch
);

-- Tạo bảng NhaCungCap
CREATE TABLE NhaCungCap (
    MaNCC varchar(10) PRIMARY KEY,
    TenNCC nvarchar(100) NOT NULL,
    DiaChi nvarchar(100),
    SDT varchar(10),
);

-- Tạo bảng KhuyenMai
CREATE TABLE KhuyenMai (
    MaKM varchar(10) PRIMARY KEY,
    TenKM nvarchar(50) NOT NULL,
    NoiDung nvarchar(100),
    PhanTramKM float NOT NULL,
    NgayBD date NOT NULL,
    NgayKT date NOT NULL
);

-- Tạo bảng ChinhSachDiemTichLuy
CREATE TABLE ChinhSachDiemTichLuy (
    MaCSDTL varchar(10) PRIMARY KEY,
    DiemToiThieu int NOT NULL,
    PhanTramGiam float NOT NULL
);

-- Tạo bảng HoaDon
CREATE TABLE HoaDon (
    MaHD varchar(10) PRIMARY KEY,
    MaNV varchar(10),
    MaKH varchar(10) NULL, -- Cho phép NULL cho khách lẻ
    MaKM varchar(10),
    MaCSDTL varchar(10),
    NgayLap datetime NOT NULL DEFAULT GETDATE(),
    TongTien decimal(18,2) NOT NULL,
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV),
    FOREIGN KEY (MaKH) REFERENCES KhachHang(MaKH),
    FOREIGN KEY (MaKM) REFERENCES KhuyenMai(MaKM),
    FOREIGN KEY (MaCSDTL) REFERENCES ChinhSachDiemTichLuy(MaCSDTL)
);

-- Tạo bảng PhieuNhap
CREATE TABLE PhieuNhap (
    MaPN varchar(10) PRIMARY KEY,
    MaNCC varchar(10),
    MaNV varchar(10),
    ThoiGianNhap datetime NOT NULL DEFAULT GETDATE(),
    TongTien decimal(18,2) NOT NULL,
    FOREIGN KEY (MaNCC) REFERENCES NhaCungCap(MaNCC),
    FOREIGN KEY (MaNV) REFERENCES NhanVien(MaNV)
);

-- Tạo bảng ChiTietHoaDon
CREATE TABLE ChiTietHoaDon (
    MaHD varchar(10),
    MaSP varchar(10),
    SoLuong int NOT NULL,
    DonGia decimal(18,2) NOT NULL,
    PRIMARY KEY (MaHD, MaSP),
    FOREIGN KEY (MaHD) REFERENCES HoaDon(MaHD),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- Tạo bảng ChiTietPhieuNhap
CREATE TABLE ChiTietPhieuNhap (
    MaPN varchar(10),
    MaSP varchar(10),
    SoLuong int NOT NULL,
    DonGia decimal(18,2) NOT NULL,
    PRIMARY KEY (MaPN, MaSP),
    FOREIGN KEY (MaPN) REFERENCES PhieuNhap(MaPN),
    FOREIGN KEY (MaSP) REFERENCES SanPham(MaSP)
);

-- Thêm dữ liệu mẫu
INSERT INTO LoaiSanPham (MaLoaiSP, TenLoaiSP) VALUES
('LSP01', N'Vợt'),
('LSP02', N'Balo'),
('LSP03', N'Phụ kiện');

INSERT INTO ThuongHieu (MaTH, TenTH) VALUES
('TH01', 'Yonex'),
('TH02', 'Lining'),
('TH03', 'Victor'),
('TH04', 'Mizuno');

INSERT INTO ChinhSachDiemTichLuy (MaCSDTL, DiemToiThieu, PhanTramGiam) VALUES
('CS01', 1000, 5.0);

-- Thêm dữ liệu mẫu cho SanPham
INSERT INTO SanPham (MaSP, TenSP, DonGiaSP, SoLuongSP, HinhAnh, TrangThai, MaLoaiSP, MaTH) VALUES
('SP01', N'Vợt Yonex Astrox 99', 2500000.00, 10, 'astrox99.jpg', 1, 'LSP01', 'TH01'),
('SP02', N'Balo Lining Pro', 1200000.00, 5, 'liningpro.jpg', 1, 'LSP02', 'TH02'),
('SP03', N'Quấn cán Victor', 50000.00, 50, 'victorwrap.jpg', 1, 'LSP03', 'TH03');

-- Thêm dữ liệu mẫu cho NhanVien
INSERT INTO NhanVien (MaNV, TenNV, NgaySinh, DiaChi, SDT, TrangThai, VaiTro, TenDangNhap, MatKhau, CoTheDangNhap) VALUES
('NV01', N'Võ Hữu Nhân', '2004-02-17', N'123 Lê Lợi, Quận 1', '0901234567', 1, 'Admin', 'admin01', '123', 1),
('NV02', N'Lý Như Ngọc', '2004-08-04', N'456 Lê Lợi, Quận 1', '0912345678', 1, 'Admin', 'admin02', '456', 1),
('NV03', N'Trần Thị Bình', '1995-05-10', N'456 Nguyễn Huệ, Quận 3', '0917345679', 1, 'ThuNgan', 'thungan01', 'hashed_password_2', 1),
('NV04', N'Lê Văn Cường', '1998-03-15', N'789 Trần Hưng Đạo, Quận 5', '0923456789', 1, 'NhanVienThuong', 'nhanvien01', '123', 0);

-- Thêm dữ liệu mẫu cho KhachHang
INSERT INTO KhachHang (MaKH, TenKH, DiaChi, SDT, Email, NgayTao, DiemTichLuy, TrangThai) VALUES
('KH01', N'Phạm Thị Dung', N'101 Hai Bà Trưng, Quận 1', '0934567890', 'dung.pham@email.com', '2025-01-01 10:00:00', 500, 1),
('KH02', N'Hoàng Văn Em', N'202 Phạm Ngũ Lão, Quận 3', '0945678901', 'em.hoang@email.com', '2025-02-01 12:00:00', 1200, 1);
namespace Nhom9.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Nhom9DB : DbContext
    {
        public Nhom9DB()
            : base("name=Nhom9DB")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }

        public virtual DbSet<ChiTietHoaDon> ChiTietHoaDons { get; set; }
        public virtual DbSet<DanhMuc> DanhMucs { get; set; }
        public virtual DbSet<HoaDon> HoaDons { get; set; }
        public virtual DbSet<KichCo> KichCoes { get; set; }
        public virtual DbSet<SanPham> SanPhams { get; set; }
        public virtual DbSet<SanPhamChiTiet> SanPhamChiTiets { get; set; }
        public virtual DbSet<TaiKhoanNguoiDung> TaiKhoanNguoiDungs { get; set; }
        public virtual DbSet<TaiKhoanQuanTri> TaiKhoanQuanTris { get; set; }

        public DbSet<DanhGia> DanhGias { get; set; }

        public DbSet<TinTuc> TinTucs { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChiTietHoaDon>()
                .Property(e => e.GiaMua)
                .HasPrecision(19, 4);

            modelBuilder.Entity<HoaDon>()
                .Property(e => e.SoDienThoaiNhan)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<SanPham>()
                .Property(e => e.Gia)
                .HasPrecision(19, 4);

            modelBuilder.Entity<SanPham>()
                .Property(e => e.MaMau)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanNguoiDung>()
                .Property(e => e.TenDangNhap)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanNguoiDung>()
                .Property(e => e.MatKhau)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanNguoiDung>()
                .Property(e => e.SoDienThoai)
                .IsFixedLength()
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanNguoiDung>()
                .Property(e => e.Email)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanQuanTri>()
                .Property(e => e.TenDangNhap)
                .IsUnicode(false);

            modelBuilder.Entity<TaiKhoanQuanTri>()
                .Property(e => e.MatKhau)
                .IsUnicode(false);
        }
    }
}

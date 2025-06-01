namespace Nhom9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddDanhGiaTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.DanhGias",
                c => new
                {
                    MaDanhGia = c.Int(nullable: false, identity: true),
                    MaSP = c.Int(nullable: false),
                    SoSao = c.Int(nullable: false),
                    HoTen = c.String(nullable: false, maxLength: 100),
                    NoiDung = c.String(nullable: false),
                    NgayDanhGia = c.DateTime(nullable: false),
                })
                .PrimaryKey(t => t.MaDanhGia)
                .ForeignKey("dbo.SanPham", t => t.MaSP, cascadeDelete: true)
                .Index(t => t.MaSP);

        }
    }
}

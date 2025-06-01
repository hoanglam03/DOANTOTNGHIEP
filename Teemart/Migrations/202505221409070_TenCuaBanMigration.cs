namespace Nhom9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class TenCuaBanMigration : DbMigration
    {
        public override void Up()
        {
            //DropTable("dbo.News");
           // DropTable("dbo.TinTucs");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.TinTucs",
                c => new
                    {
                        MaTin = c.Int(nullable: false, identity: true),
                        TieuDe = c.String(nullable: false, maxLength: 255),
                        NoiDung = c.String(nullable: false),
                        HinhAnh = c.String(),
                        NgayDang = c.DateTime(nullable: false),
                        NguoiDang = c.String(),
                    })
                .PrimaryKey(t => t.MaTin);
            
            CreateTable(
                "dbo.News",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 255),
                        Content = c.String(nullable: false),
                        ShortDescription = c.String(maxLength: 500),
                        ImageUrl = c.String(),
                        CreatedDate = c.DateTime(nullable: false),
                        UpdatedDate = c.DateTime(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
    }
}

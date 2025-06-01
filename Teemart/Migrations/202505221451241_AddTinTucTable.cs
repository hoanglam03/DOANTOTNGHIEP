namespace Nhom9.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTinTucTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TinTucs",
                c => new
                    {
                        MaTin = c.Int(nullable: false, identity: true),
                        TieuDe = c.String(nullable: false, maxLength: 255),
                        NoiDung = c.String(nullable: false),
                        MoTaNgan = c.String(maxLength: 500),
                        HinhAnh = c.String(),
                        NgayDang = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.MaTin);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TinTucs");
        }
    }
}

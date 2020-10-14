namespace NCKH.HTMLSteg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitDb : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Menus",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Link = c.String(),
                        Order = c.Int(nullable: false),
                        Type = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ReturnPages",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        HTML = c.String(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        FullName = c.String(),
                        Username = c.String(),
                        Password = c.String(),
                        Message = c.String(),
                        UserRole = c.String(maxLength: 50),
                        XMLKeyID = c.Int(),
                        ReturnPageID = c.Int(),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ReturnPages", t => t.ReturnPageID)
                .ForeignKey("dbo.XMLKeys", t => t.XMLKeyID)
                .Index(t => t.XMLKeyID)
                .Index(t => t.ReturnPageID);
            
            CreateTable(
                "dbo.XMLKeys",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        KeyName = c.String(nullable: false, maxLength: 255),
                        FilePath = c.String(nullable: false, maxLength: 255),
                        Status = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Users", "XMLKeyID", "dbo.XMLKeys");
            DropForeignKey("dbo.Users", "ReturnPageID", "dbo.ReturnPages");
            DropIndex("dbo.Users", new[] { "ReturnPageID" });
            DropIndex("dbo.Users", new[] { "XMLKeyID" });
            DropTable("dbo.XMLKeys");
            DropTable("dbo.Users");
            DropTable("dbo.ReturnPages");
            DropTable("dbo.Menus");
        }
    }
}

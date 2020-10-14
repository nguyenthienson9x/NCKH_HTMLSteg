namespace NCKH.HTMLSteg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class IsPrivateFor2Tbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReturnPages", "isPrivate", c => c.Boolean(nullable: false));
            AddColumn("dbo.XMLKeys", "isPrivate", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.XMLKeys", "isPrivate");
            DropColumn("dbo.ReturnPages", "isPrivate");
        }
    }
}

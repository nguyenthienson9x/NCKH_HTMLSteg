namespace NCKH.HTMLSteg.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNAmeHTML : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReturnPages", "Name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReturnPages", "Name");
        }
    }
}

namespace SettlementMaster.App.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Supervisor : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Supervisor", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Supervisor");
        }
    }
}

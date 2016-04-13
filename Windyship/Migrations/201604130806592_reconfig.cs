namespace Windyship.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reconfig : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.LocationToes", "ShipmentId", c => c.Int(nullable: false));
            CreateIndex("dbo.LocationToes", "ShipmentId");
        }
        
        public override void Down()
        {
        }
    }
}

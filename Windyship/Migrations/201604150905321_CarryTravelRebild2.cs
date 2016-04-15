namespace Windyship.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CarryTravelRebild2 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TravelFroms", "CarryTraveId", c => c.Int(nullable: false));
            AlterColumn("dbo.TravelToes", "CarryTraveId", c => c.Int(nullable: false));
            CreateIndex("dbo.TravelFroms", "CarryTraveId");
            CreateIndex("dbo.TravelToes", "CarryTraveId");
        }
        
        public override void Down()
        {

        }
    }
}

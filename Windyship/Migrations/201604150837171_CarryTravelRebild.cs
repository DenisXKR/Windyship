namespace Windyship.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CarryTravelRebild : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TravelFroms", "CarryTraveId", c => c.Int(nullable: false));
            AddColumn("dbo.TravelToes", "CarryTraveId", c => c.Int(nullable: false));
            DropColumn("dbo.TravelFroms", "TravelId");
            DropColumn("dbo.TravelToes", "TravelId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TravelToes", "TravelId", c => c.Int(nullable: false));
            AddColumn("dbo.TravelFroms", "TravelId", c => c.Int(nullable: false));
            DropColumn("dbo.TravelToes", "CarryTraveId");
            DropColumn("dbo.TravelFroms", "CarryTraveId");
        }
    }
}

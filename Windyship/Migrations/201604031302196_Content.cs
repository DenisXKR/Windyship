namespace Windyship.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Content : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Contents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Language = c.Int(nullable: false),
                        ContentPart = c.Int(nullable: false),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Users", "CodeLastSentTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "CodeLastSentTime");
            DropTable("dbo.Contents");
        }
    }
}

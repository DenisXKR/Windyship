namespace Windyship.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 100),
                        SecondName = c.String(maxLength: 100),
                        MiddleName = c.String(maxLength: 100),
                        FacebookId = c.String(maxLength: 100),
                        TwitterId = c.String(maxLength: 100),
                        Email = c.String(maxLength: 100),
                        EmailChecked = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        Avatar = c.Binary(),
                        AvatarMimeType = c.String(),
                        AvatarAddedUtc = c.DateTime(),
                        Phone = c.String(maxLength: 50),
                        PhoneChecked = c.Boolean(nullable: false),
                        Role = c.Int(nullable: false),
                        PhoneCode = c.String(maxLength: 10),
                        EmailCode = c.String(maxLength: 10),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
        }
        
        public override void Down()
        {
            DropTable("dbo.Users");
        }
    }
}

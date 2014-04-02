namespace SampleApp.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HomePages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        Alias = c.String(nullable: false, maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TextPages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Text = c.String(),
                        SortOrder = c.Int(nullable: false),
                        Alias = c.String(nullable: false, maxLength: 100),
                        TextPage_Id = c.Int(),
                        HomePage_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.TextPages", t => t.TextPage_Id)
                .ForeignKey("dbo.HomePages", t => t.HomePage_Id)
                .Index(t => t.TextPage_Id)
                .Index(t => t.HomePage_Id);
            
            CreateTable(
                "dbo.ContentPathCacheEntries",
                c => new
                    {
                        Hash = c.Long(nullable: false),
                        Id = c.Int(nullable: false, identity: true),
                        ContentId = c.Int(nullable: false),
                        ContentType = c.String(nullable: false, maxLength: 200),
                        Path = c.String(nullable: false),
                        Parent_Hash = c.Long(),
                        Parent_Id = c.Int(),
                    })
                .PrimaryKey(t => new { t.Hash, t.Id })
                .ForeignKey("dbo.ContentPathCacheEntries", t => new { t.Parent_Hash, t.Parent_Id })
                .Index(t => new { t.Parent_Hash, t.Parent_Id });
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(nullable: false, maxLength: 20),
                        PasswordHash = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.ContentPathCacheEntries", new[] { "Parent_Hash", "Parent_Id" });
            DropIndex("dbo.TextPages", new[] { "HomePage_Id" });
            DropIndex("dbo.TextPages", new[] { "TextPage_Id" });
            DropForeignKey("dbo.ContentPathCacheEntries", new[] { "Parent_Hash", "Parent_Id" }, "dbo.ContentPathCacheEntries");
            DropForeignKey("dbo.TextPages", "HomePage_Id", "dbo.HomePages");
            DropForeignKey("dbo.TextPages", "TextPage_Id", "dbo.TextPages");
            DropTable("dbo.Users");
            DropTable("dbo.ContentPathCacheEntries");
            DropTable("dbo.TextPages");
            DropTable("dbo.HomePages");
        }
    }
}

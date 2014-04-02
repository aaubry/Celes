namespace SampleApp.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddSortingToCache : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ContentPathCacheEntries", "SortOrder", c => c.Int(nullable: false, defaultValue: 0));
			Sql("update dbo.ContentPathCacheEntries set SortOrder = Id");
        }
        
        public override void Down()
        {
            DropColumn("dbo.ContentPathCacheEntries", "SortOrder");
        }
    }
}

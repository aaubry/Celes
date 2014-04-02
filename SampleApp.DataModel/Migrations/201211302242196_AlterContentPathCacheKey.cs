namespace SampleApp.DataModel.Migrations
{
	using System.Data.Entity.Migrations;
    
    public partial class AlterContentPathCacheKey : DbMigration
    {
        public override void Up()
        {
			DropForeignKey("ContentPathCacheEntries", "FK_dbo.ContentPathCacheEntries_dbo.ContentPathCacheEntries_Parent_Hash_Parent_Id");
			DropPrimaryKey("ContentPathCacheEntries", "PK_dbo.ContentPathCacheEntries");
			DropIndex("ContentPathCacheEntries", "IX_Parent_Hash_Parent_Id");
			AddPrimaryKey("ContentPathCacheEntries", "Id");
			AddForeignKey("ContentPathCacheEntries", "Parent_Id", "ContentPathCacheEntries", "Id");
			DropColumn("ContentPathCacheEntries", "Parent_Hash");
        }
        
        public override void Down()
        {
        }
    }
}

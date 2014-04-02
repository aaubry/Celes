namespace SampleApp.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;

	public partial class AddContentPathHashIndex : DbMigration
	{
		public override void Up()
		{
			CreateIndex("ContentPathCacheEntries", "Hash");
		}
        
        public override void Down()
        {
        }
    }
}

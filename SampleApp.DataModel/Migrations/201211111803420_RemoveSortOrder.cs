namespace SampleApp.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveSortOrder : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.TextPages", "SortOrder");
        }
        
        public override void Down()
        {
            AddColumn("dbo.TextPages", "SortOrder", c => c.Int(nullable: false));
        }
    }
}

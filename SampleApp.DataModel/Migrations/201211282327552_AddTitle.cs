namespace SampleApp.DataModel.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTitle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TextPages", "Title", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.TextPages", "Title");
        }
    }
}

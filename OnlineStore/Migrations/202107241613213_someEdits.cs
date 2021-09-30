namespace OnlineStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someEdits : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Reviews", "ApplicationUserId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reviews", "ApplicationUserId", c => c.Int(nullable: false));
        }
    }
}

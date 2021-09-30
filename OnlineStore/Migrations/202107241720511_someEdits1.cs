namespace OnlineStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class someEdits1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Reviews", name: "ApplicationUser_Id", newName: "user_Id");
            RenameIndex(table: "dbo.Reviews", name: "IX_ApplicationUser_Id", newName: "IX_user_Id");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.Reviews", name: "IX_user_Id", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.Reviews", name: "user_Id", newName: "ApplicationUser_Id");
        }
    }
}

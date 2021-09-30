namespace OnlineStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTwoPropertiesToOrderModel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "IsActive", c => c.Boolean(nullable: false));
            AddColumn("dbo.Orders", "Deliverd", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Deliverd");
            DropColumn("dbo.Orders", "IsActive");
        }
    }
}

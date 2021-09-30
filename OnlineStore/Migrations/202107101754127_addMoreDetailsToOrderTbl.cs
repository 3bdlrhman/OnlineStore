namespace OnlineStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addMoreDetailsToOrderTbl : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "FullName", c => c.String());
            AddColumn("dbo.Orders", "MobileNumber", c => c.String());
            AddColumn("dbo.Orders", "City", c => c.String());
            AddColumn("dbo.Orders", "Address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "Address");
            DropColumn("dbo.Orders", "City");
            DropColumn("dbo.Orders", "MobileNumber");
            DropColumn("dbo.Orders", "FullName");
        }
    }
}

namespace OnlineStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class shippingsModelDetails : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Shippings", "FullName", c => c.String());
            AddColumn("dbo.Shippings", "Landmark", c => c.String());
            AddColumn("dbo.Shippings", "MobileNumber", c => c.String());
            AddColumn("dbo.Shippings", "City", c => c.String());
            AlterColumn("dbo.Shippings", "PaymentMethod", c => c.String());
            DropColumn("dbo.Shippings", "Address");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Shippings", "Address", c => c.String());
            AlterColumn("dbo.Shippings", "PaymentMethod", c => c.String(nullable: false));
            DropColumn("dbo.Shippings", "City");
            DropColumn("dbo.Shippings", "MobileNumber");
            DropColumn("dbo.Shippings", "Landmark");
            DropColumn("dbo.Shippings", "FullName");
        }
    }
}

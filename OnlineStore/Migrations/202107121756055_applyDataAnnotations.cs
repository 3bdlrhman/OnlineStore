namespace OnlineStore.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class applyDataAnnotations : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Categories", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Products", "img", c => c.String(nullable: false));
            AlterColumn("dbo.Shippings", "PaymentMethod", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Shippings", "PaymentMethod", c => c.String());
            AlterColumn("dbo.Products", "img", c => c.String());
            AlterColumn("dbo.Products", "Name", c => c.String());
            AlterColumn("dbo.Categories", "Name", c => c.String());
        }
    }
}

namespace RedWallet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class decimalupdate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Address", "LatestBalance", c => c.Decimal(nullable: false, precision: 16, scale: 8));
            AlterColumn("dbo.Transaction", "TotalAmount", c => c.Decimal(nullable: false, precision: 16, scale: 8));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transaction", "TotalAmount", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.Address", "LatestBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}

namespace RedWallet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class decimalupdate1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Wallet", "LatestBalance", c => c.Decimal(nullable: false, precision: 16, scale: 8));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Wallet", "LatestBalance", c => c.Decimal(nullable: false, precision: 8, scale: 8));
        }
    }
}

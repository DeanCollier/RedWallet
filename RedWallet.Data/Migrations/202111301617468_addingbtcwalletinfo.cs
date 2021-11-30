namespace RedWallet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addingbtcwalletinfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Wallet", "LatestBalance", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Wallet", "NextReceiveChild", c => c.Int(nullable: false));
            AddColumn("dbo.Wallet", "NextChangeChild", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Wallet", "NextChangeChild");
            DropColumn("dbo.Wallet", "NextReceiveChild");
            DropColumn("dbo.Wallet", "LatestBalance");
        }
    }
}

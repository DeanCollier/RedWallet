namespace RedWallet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class xpubAddresses : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Wallet", "Xpub", c => c.String(nullable: false));
            AddColumn("dbo.Wallet", "XpubIteration", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Wallet", "XpubIteration");
            DropColumn("dbo.Wallet", "Xpub");
        }
    }
}

namespace RedWallet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class balanceupdate : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Wallet", "XpubIteration");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Wallet", "XpubIteration", c => c.Int(nullable: false));
        }
    }
}

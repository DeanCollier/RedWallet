namespace RedWallet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class requests2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Request", "RequestAddress", c => c.String(nullable: false));
            AddColumn("dbo.Send", "TransactionHash", c => c.String(nullable: false));
            DropColumn("dbo.Request", "BTCPaymentRequest");
            DropColumn("dbo.Send", "BTCTransaction");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Send", "BTCTransaction", c => c.String(nullable: false));
            AddColumn("dbo.Request", "BTCPaymentRequest", c => c.String(nullable: false));
            DropColumn("dbo.Send", "TransactionHash");
            DropColumn("dbo.Request", "RequestAddress");
        }
    }
}

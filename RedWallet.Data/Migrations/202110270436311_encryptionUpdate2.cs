namespace RedWallet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class encryptionUpdate2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Wallet", "EncryptedSecret", c => c.String(nullable: false));
            DropColumn("dbo.Wallet", "EncryptedPrivateKey");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Wallet", "EncryptedPrivateKey", c => c.String(nullable: false));
            DropColumn("dbo.Wallet", "EncryptedSecret");
        }
    }
}

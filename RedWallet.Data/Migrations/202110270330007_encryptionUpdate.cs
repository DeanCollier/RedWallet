namespace RedWallet.Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class encryptionUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Wallet", "EncryptedPrivateKey", c => c.String(nullable: false));
            DropColumn("dbo.Wallet", "PassphraseHash");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Wallet", "PassphraseHash", c => c.String(nullable: false));
            DropColumn("dbo.Wallet", "EncryptedPrivateKey");
        }
    }
}

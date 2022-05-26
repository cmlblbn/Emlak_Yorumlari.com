namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class last_update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Embedding_Analyse", "trained_path", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Embedding_Analyse", "trained_path");
        }
    }
}

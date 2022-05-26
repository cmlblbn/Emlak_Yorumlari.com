namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class last_update_2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Embedding_Analyse", "nontrained_path", c => c.String(maxLength: 1000));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Embedding_Analyse", "nontrained_path");
        }
    }
}

namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateDatabaseEmbeddingAnalyse : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Embedding_Analyse", "kl_divergenceValue", c => c.Single(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Embedding_Analyse", "kl_divergenceValue");
        }
    }
}

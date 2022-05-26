namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mlops : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Crobjob_Parameter",
                c => new
                    {
                        cronjob_id = c.Int(nullable: false, identity: true),
                        batch_size = c.Int(nullable: false),
                        epoch = c.Int(nullable: false),
                        maxlen = c.Int(nullable: false),
                        createedOn = c.DateTime(nullable: false),
                        isActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.cronjob_id)
                .Index(t => t.cronjob_id, unique: true);
            
            CreateTable(
                "dbo.Embedding_Analyse",
                c => new
                    {
                        analyse_id = c.Int(nullable: false, identity: true),
                        lastAnalyseDate = c.DateTime(nullable: false),
                        isActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.analyse_id)
                .Index(t => t.analyse_id, unique: true);
            
            CreateTable(
                "dbo.Embeddings",
                c => new
                    {
                        embedding_id = c.Int(nullable: false, identity: true),
                        text = c.String(maxLength: 10000),
                        prediction_sentiment = c.Int(nullable: false),
                        actual_sentiment = c.Int(nullable: false),
                        isTrained = c.Boolean(nullable: false),
                        createdOn = c.DateTime(nullable: false),
                        isActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.embedding_id)
                .Index(t => t.embedding_id, unique: true);
            
            CreateTable(
                "dbo.Models",
                c => new
                    {
                        model_id = c.Int(nullable: false, identity: true),
                        modelName = c.String(),
                        type = c.String(),
                        Accuracy = c.Single(nullable: false),
                        loss = c.Single(nullable: false),
                        batch_size = c.Int(nullable: false),
                        epoch = c.Int(nullable: false),
                        maxlen = c.Int(nullable: false),
                        createedOn = c.DateTime(nullable: false),
                        isActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.model_id)
                .Index(t => t.model_id, unique: true);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Models", new[] { "model_id" });
            DropIndex("dbo.Embeddings", new[] { "embedding_id" });
            DropIndex("dbo.Embedding_Analyse", new[] { "analyse_id" });
            DropIndex("dbo.Crobjob_Parameter", new[] { "cronjob_id" });
            DropTable("dbo.Models");
            DropTable("dbo.Embeddings");
            DropTable("dbo.Embedding_Analyse");
            DropTable("dbo.Crobjob_Parameter");
        }
    }
}

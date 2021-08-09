namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class commentandsurveyLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Comment_Log",
                c => new
                    {
                        user_id = c.Int(nullable: false),
                        place_id = c.Int(nullable: false),
                        text = c.String(maxLength: 10000),
                        toxic_type = c.Int(nullable: false),
                        createdOn = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.user_id, t.place_id })
                .ForeignKey("dbo.Place", t => t.place_id, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.user_id, cascadeDelete: true)
                .Index(t => t.user_id)
                .Index(t => t.place_id);
            
            CreateTable(
                "dbo.Survey_Log",
                c => new
                    {
                        user_id = c.Int(nullable: false),
                        question_id = c.Int(nullable: false),
                        place_id = c.Int(nullable: false),
                        score = c.Single(nullable: false),
                        toxic_type = c.Int(nullable: false),
                        createdOn = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.user_id, t.question_id, t.place_id })
                .ForeignKey("dbo.Place", t => t.place_id, cascadeDelete: true)
                .ForeignKey("dbo.Question_Definition", t => t.question_id, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.user_id, cascadeDelete: true)
                .Index(t => t.user_id)
                .Index(t => t.question_id)
                .Index(t => t.place_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Survey_Log", "user_id", "dbo.User");
            DropForeignKey("dbo.Survey_Log", "question_id", "dbo.Question_Definition");
            DropForeignKey("dbo.Survey_Log", "place_id", "dbo.Place");
            DropForeignKey("dbo.Comment_Log", "user_id", "dbo.User");
            DropForeignKey("dbo.Comment_Log", "place_id", "dbo.Place");
            DropIndex("dbo.Survey_Log", new[] { "place_id" });
            DropIndex("dbo.Survey_Log", new[] { "question_id" });
            DropIndex("dbo.Survey_Log", new[] { "user_id" });
            DropIndex("dbo.Comment_Log", new[] { "place_id" });
            DropIndex("dbo.Comment_Log", new[] { "user_id" });
            DropTable("dbo.Survey_Log");
            DropTable("dbo.Comment_Log");
        }
    }
}

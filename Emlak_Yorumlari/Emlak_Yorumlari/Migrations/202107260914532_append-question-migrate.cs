namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class appendquestionmigrate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Combobox_Answer",
                c => new
                    {
                        question_answer_id = c.Int(nullable: false, identity: true),
                        question_id = c.Int(nullable: false),
                        question_answer = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.question_answer_id)
                .ForeignKey("dbo.Question_Definition", t => t.question_id, cascadeDelete: true)
                .Index(t => t.question_id);
            
            CreateTable(
                "dbo.Question_Type",
                c => new
                    {
                        question_type_id = c.Int(nullable: false, identity: true),
                        question_type = c.String(),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.question_type_id);
            
            AddColumn("dbo.Question_Definition", "question_type_id", c => c.Int(nullable: false));
            CreateIndex("dbo.Question_Definition", "question_type_id");
            AddForeignKey("dbo.Question_Definition", "question_type_id", "dbo.Question_Type", "question_type_id", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Question_Definition", "question_type_id", "dbo.Question_Type");
            DropForeignKey("dbo.Combobox_Answer", "question_id", "dbo.Question_Definition");
            DropIndex("dbo.Combobox_Answer", new[] { "question_id" });
            DropIndex("dbo.Question_Definition", new[] { "question_type_id" });
            DropColumn("dbo.Question_Definition", "question_type_id");
            DropTable("dbo.Question_Type");
            DropTable("dbo.Combobox_Answer");
        }
    }
}

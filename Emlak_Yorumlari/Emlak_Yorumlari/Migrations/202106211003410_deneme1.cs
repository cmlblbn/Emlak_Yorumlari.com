namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deneme1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Adress_Description",
                c => new
                    {
                        adress_desc_id = c.Int(nullable: false, identity: true),
                        adress_name = c.String(maxLength: 50),
                        adress_type_id = c.Int(nullable: false),
                        parent_id = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.adress_desc_id)
                .ForeignKey("dbo.Adress_Type", t => t.adress_type_id, cascadeDelete: true)
                .Index(t => t.adress_desc_id, unique: true)
                .Index(t => t.adress_type_id);
            
            CreateTable(
                "dbo.Adress_Type",
                c => new
                    {
                        adress_type_id = c.Int(nullable: false, identity: true),
                        adress_type = c.String(maxLength: 10),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.adress_type_id);
            
            CreateTable(
                "dbo.Place",
                c => new
                    {
                        place_id = c.Int(nullable: false, identity: true),
                        user_id = c.Int(nullable: false),
                        adress_desc_id = c.Int(nullable: false),
                        placeName = c.String(maxLength: 100),
                        placeImage = c.Binary(),
                        createdOn = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.place_id)
                .ForeignKey("dbo.Adress_Description", t => t.adress_desc_id, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.user_id, cascadeDelete: true)
                .Index(t => t.place_id, unique: true)
                .Index(t => t.user_id)
                .Index(t => t.adress_desc_id);
            
            CreateTable(
                "dbo.Comment",
                c => new
                    {
                        user_id = c.Int(nullable: false),
                        place_id = c.Int(nullable: false),
                        text = c.String(maxLength: 10000),
                        createdOn = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => new { t.user_id, t.place_id })
                .ForeignKey("dbo.Place", t => t.place_id, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.user_id, cascadeDelete: true)
                .Index(t => t.user_id)
                .Index(t => t.place_id);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        user_id = c.Int(nullable: false, identity: true),
                        username = c.String(nullable: false, maxLength: 25),
                        email = c.String(nullable: false, maxLength: 60),
                        password = c.String(nullable: false, maxLength: 60),
                        profileImage = c.Binary(),
                        createOn = c.DateTime(nullable: false),
                        IsAdmin = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.user_id)
                .Index(t => t.user_id, unique: true)
                .Index(t => t.username, unique: true)
                .Index(t => t.email, unique: true);
            
            CreateTable(
                "dbo.Survey",
                c => new
                    {
                        user_id = c.Int(nullable: false),
                        question_id = c.Int(nullable: false),
                        place_id = c.Int(nullable: false),
                        score = c.Single(nullable: false),
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
            
            CreateTable(
                "dbo.Question_Definition",
                c => new
                    {
                        question_id = c.Int(nullable: false, identity: true),
                        question_name = c.String(maxLength: 50),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.question_id)
                .Index(t => t.question_id, unique: true);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Survey", "user_id", "dbo.User");
            DropForeignKey("dbo.Survey", "question_id", "dbo.Question_Definition");
            DropForeignKey("dbo.Survey", "place_id", "dbo.Place");
            DropForeignKey("dbo.Place", "user_id", "dbo.User");
            DropForeignKey("dbo.Comment", "user_id", "dbo.User");
            DropForeignKey("dbo.Comment", "place_id", "dbo.Place");
            DropForeignKey("dbo.Place", "adress_desc_id", "dbo.Adress_Description");
            DropForeignKey("dbo.Adress_Description", "adress_type_id", "dbo.Adress_Type");
            DropIndex("dbo.Question_Definition", new[] { "question_id" });
            DropIndex("dbo.Survey", new[] { "place_id" });
            DropIndex("dbo.Survey", new[] { "question_id" });
            DropIndex("dbo.Survey", new[] { "user_id" });
            DropIndex("dbo.User", new[] { "email" });
            DropIndex("dbo.User", new[] { "username" });
            DropIndex("dbo.User", new[] { "user_id" });
            DropIndex("dbo.Comment", new[] { "place_id" });
            DropIndex("dbo.Comment", new[] { "user_id" });
            DropIndex("dbo.Place", new[] { "adress_desc_id" });
            DropIndex("dbo.Place", new[] { "user_id" });
            DropIndex("dbo.Place", new[] { "place_id" });
            DropIndex("dbo.Adress_Description", new[] { "adress_type_id" });
            DropIndex("dbo.Adress_Description", new[] { "adress_desc_id" });
            DropTable("dbo.Question_Definition");
            DropTable("dbo.Survey");
            DropTable("dbo.User");
            DropTable("dbo.Comment");
            DropTable("dbo.Place");
            DropTable("dbo.Adress_Type");
            DropTable("dbo.Adress_Description");
        }
    }
}

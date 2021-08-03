namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class statisticApp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Place_Statistics",
                c => new
                    {
                        statistic_id = c.Int(nullable: false, identity: true),
                        place_id = c.Int(nullable: false),
                        male_count = c.Int(nullable: false),
                        female_count = c.Int(nullable: false),
                        otherSex_count = c.Int(nullable: false),
                        primarySchool_count = c.Int(nullable: false),
                        middleSchool_count = c.Int(nullable: false),
                        highSchool_count = c.Int(nullable: false),
                        degree_count = c.Int(nullable: false),
                        masterDegree_count = c.Int(nullable: false),
                        married_count = c.Int(nullable: false),
                        single_count = c.Int(nullable: false),
                        divorced_count = c.Int(nullable: false),
                        widow_count = c.Int(nullable: false),
                        average_rent = c.Int(nullable: false),
                        age_lower_18 = c.Int(nullable: false),
                        age_between_18_34 = c.Int(nullable: false),
                        age_between_34_55 = c.Int(nullable: false),
                        age_upper_55 = c.Int(nullable: false),
                        createdOn = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.statistic_id)
                .ForeignKey("dbo.Place", t => t.place_id, cascadeDelete: true)
                .Index(t => t.place_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Place_Statistics", "place_id", "dbo.Place");
            DropIndex("dbo.Place_Statistics", new[] { "place_id" });
            DropTable("dbo.Place_Statistics");
        }
    }
}

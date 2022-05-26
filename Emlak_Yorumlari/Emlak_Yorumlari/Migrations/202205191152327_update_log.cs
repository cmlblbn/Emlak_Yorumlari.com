namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_log : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Comment_Log", "isLabeled", c => c.Boolean());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Comment_Log", "isLabeled");
        }
    }
}

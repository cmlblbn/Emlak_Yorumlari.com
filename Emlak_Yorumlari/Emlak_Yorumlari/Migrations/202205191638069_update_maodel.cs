namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update_maodel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Models", "createdOn", c => c.DateTime(nullable: false));
            DropColumn("dbo.Models", "createedOn");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Models", "createedOn", c => c.DateTime(nullable: false));
            DropColumn("dbo.Models", "createdOn");
        }
    }
}

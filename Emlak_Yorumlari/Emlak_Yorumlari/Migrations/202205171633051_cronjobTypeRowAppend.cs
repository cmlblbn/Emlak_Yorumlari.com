namespace Emlak_Yorumlari.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cronjobTypeRowAppend : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Crobjob_Parameter", "type", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Crobjob_Parameter", "type");
        }
    }
}

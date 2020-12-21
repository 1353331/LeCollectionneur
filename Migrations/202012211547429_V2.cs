namespace LeCollectionneur.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class V2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Utilisateurs", "DarkMode", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Utilisateurs", "DarkMode");
        }
    }
}

namespace LeCollectionneur.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class utilisateursDarkMode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Utilisateurs", "DarkMode", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Utilisateurs", "DarkMode");
            DropColumn("dbo.Utilisateurs", "EstActif");
            DropColumn("dbo.Utilisateurs", "Role");
        }
    }
}

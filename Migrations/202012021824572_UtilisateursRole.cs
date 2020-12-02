namespace LeCollectionneur.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UtilisateursRole : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Utilisateurs", "Role", c => c.String(unicode: false));
            AddColumn("dbo.Utilisateurs", "estActif", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Utilisateurs", "estActif");
            DropColumn("dbo.Utilisateurs", "Role");
        }
    }
}

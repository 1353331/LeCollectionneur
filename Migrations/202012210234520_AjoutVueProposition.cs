namespace LeCollectionneur.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AjoutVueProposition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Propositions", "estVue", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Propositions", "estVue");
        }
    }
}

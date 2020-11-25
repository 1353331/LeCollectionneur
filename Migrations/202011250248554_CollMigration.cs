namespace LeCollectionneur.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CollMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Annonces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Titre = c.String(unicode: false),
                        DatePublication = c.DateTime(nullable: false, precision: 0),
                        Description = c.String(unicode: false),
                        Montant = c.Double(nullable: false),
                        Annonceur_Id = c.Int(),
                        EtatAnnonce_Id = c.Int(),
                        Type_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Utilisateurs", t => t.Annonceur_Id)
                .ForeignKey("dbo.EtatsAnnonce", t => t.EtatAnnonce_Id)
                .ForeignKey("dbo.TypesAnnonce", t => t.Type_Id)
                .Index(t => t.Annonceur_Id)
                .Index(t => t.EtatAnnonce_Id)
                .Index(t => t.Type_Id);
            
            CreateTable(
                "dbo.Utilisateurs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NomUtilisateur = c.String(unicode: false),
                        MotDePasse = c.String(unicode: false),
                        Courriel = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Collections",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(unicode: false),
                        DateCreation = c.DateTime(nullable: false, precision: 0),
                        Utilisateur_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Utilisateurs", t => t.Utilisateur_Id)
                .Index(t => t.Utilisateur_Id);
            
            CreateTable(
                "dbo.Items",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(unicode: false),
                        DateSortie = c.DateTime(precision: 0),
                        CheminImage = c.String(unicode: false),
                        Manufacturier = c.String(unicode: false),
                        Description = c.String(unicode: false),
                        Condition_Id = c.Int(),
                        Type_Id = c.Int(),
                        Collection_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Conditions", t => t.Condition_Id)
                .ForeignKey("dbo.TypesItem", t => t.Type_Id)
                .ForeignKey("dbo.Collections", t => t.Collection_Id)
                .Index(t => t.Condition_Id)
                .Index(t => t.Type_Id)
                .Index(t => t.Collection_Id);
            
            CreateTable(
                "dbo.Conditions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Propositions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Montant = c.Double(nullable: false),
                        DateProposition = c.DateTime(nullable: false, precision: 0),
                        AnnonceLiee_Id = c.Int(),
                        EtatProposition_Id = c.Int(),
                        Proposeur_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Annonces", t => t.AnnonceLiee_Id)
                .ForeignKey("dbo.EtatsProposition", t => t.EtatProposition_Id)
                .ForeignKey("dbo.Utilisateurs", t => t.Proposeur_Id)
                .Index(t => t.AnnonceLiee_Id)
                .Index(t => t.EtatProposition_Id)
                .Index(t => t.Proposeur_Id);
            
            CreateTable(
                "dbo.EtatsProposition",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TypesItem",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.EtatsAnnonce",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TypesAnnonce",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Nom = c.String(unicode: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Conversations",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Utilisateur1_Id = c.Int(),
                        Utilisateur2_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Utilisateurs", t => t.Utilisateur1_Id)
                .ForeignKey("dbo.Utilisateurs", t => t.Utilisateur2_Id)
                .Index(t => t.Utilisateur1_Id)
                .Index(t => t.Utilisateur2_Id);
            
            CreateTable(
                "dbo.Messages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(unicode: false),
                        Date = c.DateTime(nullable: false, precision: 0),
                        item = c.Boolean(nullable: false),
                        image = c.Boolean(nullable: false),
                        emoji = c.Boolean(nullable: false),
                        vue = c.Boolean(nullable: false),
                        utilisateur_Id = c.Int(),
                        Conversation_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Utilisateurs", t => t.utilisateur_Id)
                .ForeignKey("dbo.Conversations", t => t.Conversation_Id)
                .Index(t => t.utilisateur_Id)
                .Index(t => t.Conversation_Id);
            
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, precision: 0),
                        PropositionTrx_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Propositions", t => t.PropositionTrx_Id)
                .Index(t => t.PropositionTrx_Id);
            
            CreateTable(
                "dbo.ItemAnnonces",
                c => new
                    {
                        Item_Id = c.Int(nullable: false),
                        Annonce_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Item_Id, t.Annonce_Id })
                .ForeignKey("dbo.Items", t => t.Item_Id, cascadeDelete: true)
                .ForeignKey("dbo.Annonces", t => t.Annonce_Id, cascadeDelete: true)
                .Index(t => t.Item_Id)
                .Index(t => t.Annonce_Id);
            
            CreateTable(
                "dbo.PropositionItems",
                c => new
                    {
                        Proposition_Id = c.Int(nullable: false),
                        Item_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Proposition_Id, t.Item_Id })
                .ForeignKey("dbo.Propositions", t => t.Proposition_Id, cascadeDelete: true)
                .ForeignKey("dbo.Items", t => t.Item_Id, cascadeDelete: true)
                .Index(t => t.Proposition_Id)
                .Index(t => t.Item_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "PropositionTrx_Id", "dbo.Propositions");
            DropForeignKey("dbo.Conversations", "Utilisateur2_Id", "dbo.Utilisateurs");
            DropForeignKey("dbo.Conversations", "Utilisateur1_Id", "dbo.Utilisateurs");
            DropForeignKey("dbo.Messages", "Conversation_Id", "dbo.Conversations");
            DropForeignKey("dbo.Messages", "utilisateur_Id", "dbo.Utilisateurs");
            DropForeignKey("dbo.Annonces", "Type_Id", "dbo.TypesAnnonce");
            DropForeignKey("dbo.Annonces", "EtatAnnonce_Id", "dbo.EtatsAnnonce");
            DropForeignKey("dbo.Annonces", "Annonceur_Id", "dbo.Utilisateurs");
            DropForeignKey("dbo.Collections", "Utilisateur_Id", "dbo.Utilisateurs");
            DropForeignKey("dbo.Items", "Collection_Id", "dbo.Collections");
            DropForeignKey("dbo.Items", "Type_Id", "dbo.TypesItem");
            DropForeignKey("dbo.Propositions", "Proposeur_Id", "dbo.Utilisateurs");
            DropForeignKey("dbo.PropositionItems", "Item_Id", "dbo.Items");
            DropForeignKey("dbo.PropositionItems", "Proposition_Id", "dbo.Propositions");
            DropForeignKey("dbo.Propositions", "EtatProposition_Id", "dbo.EtatsProposition");
            DropForeignKey("dbo.Propositions", "AnnonceLiee_Id", "dbo.Annonces");
            DropForeignKey("dbo.Items", "Condition_Id", "dbo.Conditions");
            DropForeignKey("dbo.ItemAnnonces", "Annonce_Id", "dbo.Annonces");
            DropForeignKey("dbo.ItemAnnonces", "Item_Id", "dbo.Items");
            DropIndex("dbo.PropositionItems", new[] { "Item_Id" });
            DropIndex("dbo.PropositionItems", new[] { "Proposition_Id" });
            DropIndex("dbo.ItemAnnonces", new[] { "Annonce_Id" });
            DropIndex("dbo.ItemAnnonces", new[] { "Item_Id" });
            DropIndex("dbo.Transactions", new[] { "PropositionTrx_Id" });
            DropIndex("dbo.Messages", new[] { "Conversation_Id" });
            DropIndex("dbo.Messages", new[] { "utilisateur_Id" });
            DropIndex("dbo.Conversations", new[] { "Utilisateur2_Id" });
            DropIndex("dbo.Conversations", new[] { "Utilisateur1_Id" });
            DropIndex("dbo.Propositions", new[] { "Proposeur_Id" });
            DropIndex("dbo.Propositions", new[] { "EtatProposition_Id" });
            DropIndex("dbo.Propositions", new[] { "AnnonceLiee_Id" });
            DropIndex("dbo.Items", new[] { "Collection_Id" });
            DropIndex("dbo.Items", new[] { "Type_Id" });
            DropIndex("dbo.Items", new[] { "Condition_Id" });
            DropIndex("dbo.Collections", new[] { "Utilisateur_Id" });
            DropIndex("dbo.Annonces", new[] { "Type_Id" });
            DropIndex("dbo.Annonces", new[] { "EtatAnnonce_Id" });
            DropIndex("dbo.Annonces", new[] { "Annonceur_Id" });
            DropTable("dbo.PropositionItems");
            DropTable("dbo.ItemAnnonces");
            DropTable("dbo.Transactions");
            DropTable("dbo.Messages");
            DropTable("dbo.Conversations");
            DropTable("dbo.TypesAnnonce");
            DropTable("dbo.EtatsAnnonce");
            DropTable("dbo.TypesItem");
            DropTable("dbo.EtatsProposition");
            DropTable("dbo.Propositions");
            DropTable("dbo.Conditions");
            DropTable("dbo.Items");
            DropTable("dbo.Collections");
            DropTable("dbo.Utilisateurs");
            DropTable("dbo.Annonces");
        }
    }
}

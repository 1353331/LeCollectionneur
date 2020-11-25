using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Modeles;
using MySql.Data.EntityFramework;

namespace LeCollectionneur.EF
{
	[DbConfigurationType(typeof(MySqlEFConfiguration))]
	public class Context : DbContext
	{
		public DbSet<Annonce> Annonces { get; set; }
		public DbSet<Collection> Collections { get; set; }
		public DbSet<Conversation> Conversations { get; set; }
		public DbSet<Condition> Conditions { get; set; }
		public DbSet<EtatAnnonce> EtatsAnnonceDB { get; set; }
		public DbSet<EtatProposition> EtatsPropositionDB { get; set; }
		public DbSet<Item> Items { get; set; }
		public DbSet<Message> Messages { get; set; }
		public DbSet<Proposition> Propositions { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<TypeAnnonce> TypesAnnonce { get; set; }
		public DbSet<TypeItem> TypesItem { get; set; }
		public DbSet<Utilisateur> Utilisateurs { get; set; }


		public Context() : base("name=connexionLeCollectionneur")
		{
			
		}
	}
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Outils.Enumerations;

namespace LeCollectionneur.Modeles
{
	[Table("Propositions")]
	public class Proposition
	{

		public int Id { get; set; }
		public Annonce AnnonceLiee { get; set; }
		public Utilisateur Proposeur { get; set; }
		public double Montant { get; set; }
		public DateTime DateProposition{ get; set; }
		public EtatProposition EtatProposition{ get; set; }

		public ObservableCollection<Item> ItemsProposes { get; set; }

		public Proposition()
		{
			ItemsProposes = new ObservableCollection<Item>();
			EtatProposition = new EtatProposition(EtatsProposition.EnAttente);
		}

		public Proposition(Proposition prop)
		{
			Id = prop.Id;
			AnnonceLiee = prop.AnnonceLiee;
			Proposeur = prop.Proposeur;
			Montant = prop.Montant;
			DateProposition = prop.DateProposition;
			EtatProposition = prop.EtatProposition;
			ItemsProposes = prop.ItemsProposes;
		}

		public Proposition(DataRow dr)
		{
			AnnonceADO annonceADO = new AnnonceADO();
			UtilisateurADO utilisateurADO = new UtilisateurADO();

			Id = (int)dr["id"];
			AnnonceLiee = annonceADO.RecupererUn((int)dr["AnnonceLiee_Id"]);
			Proposeur = utilisateurADO.RechercherUtilisateurById((int)dr["Proposeur_Id"]);
			Montant = (double)dr["montant"];
			DateProposition = (DateTime)dr["dateProposition"];
			EtatProposition = new EtatProposition((string)dr["etatProposition"]);

			ItemsProposes = new ObservableCollection<Item>();
		}
	}
}

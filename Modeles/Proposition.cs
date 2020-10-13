using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
	public class Proposition
	{

		public int Id { get; set; }
		public Annonce AnnonceLiee { get; set; }
		public Utilisateur Proposeur { get; set; }
		public double Montant { get; set; }
		public DateTime DateProposition{ get; set; }
		public string EtatProposition{ get; set; }

		public ObservableCollection<Item> ItemsProposes { get; set; }

		public Proposition()
		{
			ItemsProposes = new ObservableCollection<Item>();
			EtatProposition = "En attente";
		}

		public Proposition(DataRow dr)
		{
			AnnonceADO annonceADO = new AnnonceADO();
			UtilisateurADO utilisateurADO = new UtilisateurADO();

			Id = (int)dr["id"];
			AnnonceLiee = annonceADO.RecupererUn((int)dr["idAnnonce"]);
			Proposeur = utilisateurADO.RechercherUtilisateurById((int)dr["idUtilisateur"]);
			Montant = (double)dr["montant"];
			DateProposition = (DateTime)dr["dateProposition"];
			EtatProposition = (string)dr["etatProposition"];

			ItemsProposes = new ObservableCollection<Item>();
		}
	}
}

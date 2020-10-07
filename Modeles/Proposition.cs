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
		//public Utilisateur Proposeur { get; set; }
		public decimal Montant { get; set; }
		public DateTime DateProposition{ get; set; }
		public string EtatProposition{ get; set; }

		//public ObservableCollection<Item> ItemsProposes { get; set; }

		public Proposition()
		{
			//ItemsProposes = new ObservableCollection<Item>();		
		}

		public Proposition(DataRow dr)
		{
			Id = (int)dr["id"];
			//AnnonceLiee
			//Proposeur
			Montant = (decimal)dr["montant"];
			DateProposition = (DateTime)dr["dateProposition"];
			EtatProposition = (string)dr["etatProposition"];

			//ItemsProposes = new ObservableCollection<Item>();
		}
	}
}

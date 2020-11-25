using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
	[Table("EtatsAnnonce")]
	public class EtatAnnonce
	{
		public int Id { get; set; }
		public string Nom { get; set; }

		public EtatAnnonce()
		{

		}

		public EtatAnnonce(string nom)
		{
			Nom = nom;
		}
	}
}

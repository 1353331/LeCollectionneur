using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
	[Table("EtatsProposition")]
	public class EtatProposition
	{
		public int Id { get; set; }
		public string Nom { get; set; }

		public EtatProposition()
		{

		}
		public EtatProposition(string nom)
		{
			Nom = nom;
		}
	}
}

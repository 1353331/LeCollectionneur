using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
	[Table("TypesAnnonce")]
	public class TypeAnnonce
	{
		public int Id { get; set; }
		public string Nom { get; set; }

		public TypeAnnonce()
		{

		}
		public TypeAnnonce(string nom)
		{
			Nom = nom;
		}
	}
}

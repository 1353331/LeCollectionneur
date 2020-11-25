using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
	[Table("Conditions")]
	public class Condition
	{
		public int Id { get; set; }
		public string Nom { get; set; }

		public Condition()
		{

		}

		public Condition(string nom)
		{
			Nom = nom;
		}
	}
}

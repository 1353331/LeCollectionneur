using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
	[Table("TypesItem")]
	public class TypeItem
	{
		public int Id { get; set; }
		public string Nom { get; set; }

		public TypeItem()
		{

		}
		public TypeItem(string nom)
		{
			Nom = nom;
		}
	}
}

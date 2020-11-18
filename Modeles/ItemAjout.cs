using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
	public class ItemAjout : Item
	{
		public bool EstAjoute { get; set; }

		public ItemAjout(Item item)
		{
         Id = item.Id;
         Nom = item.Nom;
         Manufacturier = item.Manufacturier;
         CheminImage = item.CheminImage;
         Description = item.Description;
         DateSortie = item.DateSortie;
         // Reste à get le Type, Condition et Manufacturier.
         Type = item.Type;
         Condition = item.Condition;
			BmImage = item.BmImage;
			EstAjoute = false;
      }

		public static ObservableCollection<ItemAjout> ModifierItemsEnItemsAjout(IEnumerable<Item> lstItems)
		{
			ObservableCollection<ItemAjout> lstRetour = new ObservableCollection<ItemAjout>();

			foreach (Item item in lstItems)
			{

				lstRetour.Add(new ItemAjout(item));
			}

			return lstRetour;
		}
	}
}

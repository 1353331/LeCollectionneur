using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Modeles;

namespace LeCollectionneur.Outils.Messages
{
	public class EnvoyerItemsMessage
	{
		public ObservableCollection<Item> Items { get; set; }

		public ObservableCollection<Item> ConvertirIListEnObservColl(IList ListeDItem)
        {
			ObservableCollection<Item> CollectionDItems = new ObservableCollection<Item>();
			foreach(Item i in ListeDItem)
            {
				CollectionDItems.Add(i);
            }
			return CollectionDItems;
        }
	}
}

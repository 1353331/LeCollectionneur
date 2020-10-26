using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Modeles;

namespace LeCollectionneur.VuesModeles
{
	public class ModalDetailsItem_VM : INotifyPropertyChanged
	{
		private Item _itemDetails;

		public Item ItemDetails
		{
			get { return _itemDetails; }
			set 
			{ 
				_itemDetails = value;
				OnPropertyChanged("ItemDetails");
			}
		}


		public ModalDetailsItem_VM(Item item)
		{
			ItemDetails = item;
		}

		#region OnPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string nomPropriete)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
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

			if(item.BmImage is null)
            {
				ImageItem = new BitmapImage();
				ImageItem.BeginInit();
				ImageItem.UriSource = new Uri("pack://application:,,,/LeCollectionneur;component/images/noimage.png", UriKind.Absolute);
				ImageItem.EndInit();
			}
			else
            {
				ImageItem = item.BmImage;
            }
		}

		private BitmapImage _imageItem;
		public BitmapImage ImageItem
		{
			get
			{
				return _imageItem;
			}
			set
			{
				_imageItem = value;
				OnPropertyChanged("ImageItem");
			}
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

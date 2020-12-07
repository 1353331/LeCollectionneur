using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Messages;

namespace LeCollectionneur.VuesModeles
{
	public class ModalAjoutItem_VM : INotifyPropertyChanged
	{
		public List<Item> ItemsAjoutes { get; set; }

		private ItemAjout _itemSelectionne;

		public ItemAjout ItemSelectionne
		{
			get { return _itemSelectionne; }
			set 
			{ 
				_itemSelectionne = value;
				if (_itemSelectionne != null)
				{
					if (_itemSelectionne.BmImage is null)
					{
						ImageItem = new BitmapImage();
						ImageItem.BeginInit();
						ImageItem.UriSource = new Uri("pack://application:,,,/LeCollectionneur;component/images/noimage.png", UriKind.Absolute);
						ImageItem.EndInit();
					}
					else
					{
						ImageItem = _itemSelectionne.BmImage;
					}
				}
				cmdAjouter_Item = new Commande(cmdAjouter, boutonAjouterActif);
				OnPropertyChanged("ItemSelectionne");
			}
		}

		public ObservableCollection<Collection> lstCollections { get; set; }

		private Collection _collectionSelectionnee;

		public Collection CollectionSelectionnee
		{
			get { return _collectionSelectionnee; }
			set 
			{ 
				_collectionSelectionnee = value;
				ItemsCollectionSelectionnee = ItemAjout.ModifierItemsEnItemsAjout(CollectionSelectionnee.ItemsCollection);
				foreach (Item item in ItemsAjoutes)
				{
					foreach (ItemAjout itemColl in ItemsCollectionSelectionnee)
					{
						if (item.Id == itemColl.Id)
						{
							itemColl.EstAjoute = true;
							break;
						}
					}
				}
			}
		}

		private ObservableCollection<ItemAjout> _itemsCollectionSelectionnee;

		public ObservableCollection<ItemAjout> ItemsCollectionSelectionnee
		{
			get { return _itemsCollectionSelectionnee; }
			set 
			{ 
				_itemsCollectionSelectionnee = value;
				OnPropertyChanged("ItemsCollectionSelectionnee");
			}
		}

		private ICommand _cmdAjouter_Item;

		public ICommand cmdAjouter_Item
		{
			get { return _cmdAjouter_Item; }
			set 
			{
				_cmdAjouter_Item = value;
				OnPropertyChanged("cmdAjouter_Item");
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

		public ModalAjoutItem_VM(IEnumerable<Item> items)
		{
			cmdAjouter_Item = new Commande(cmdAjouter, boutonAjouterActif);
			lstCollections = new CollectionADO().Recuperer(UtilisateurADO.utilisateur.Id);
			ItemsAjoutes = new List<Item>(items);
		}

		private void cmdAjouter(object param)
		{
			//Envoie l'évènement d'ajout d'item avec l'item sélectionné pour qu'il soit reçu par la fenêtre à laquelle celle-ci appartient
			EvenementSysteme.Publier<EnvoyerItemMessage>(new EnvoyerItemMessage() { Item = ItemSelectionne });
			ItemSelectionne.EstAjoute = true;
			ItemsAjoutes.Add(ItemSelectionne);
			ItemSelectionne = null;
			CollectionSelectionnee = CollectionSelectionnee;
		}

		private bool boutonAjouterActif()
		{
			return !(ItemSelectionne is null) && !ItemSelectionne.EstAjoute;
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

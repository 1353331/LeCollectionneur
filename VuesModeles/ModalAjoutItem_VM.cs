using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Messages;

namespace LeCollectionneur.VuesModeles
{
	public class ModalAjoutItem_VM : INotifyPropertyChanged
	{
		private Item _itemSelectionne;

		public Item ItemSelectionne
		{
			get { return _itemSelectionne; }
			set 
			{ 
				_itemSelectionne = value;
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
				ItemsCollectionSelectionnee = _collectionSelectionnee.ItemsCollection;
			}
		}

		private ObservableCollection<Item> _itemsCollectionSelectionnee;

		public ObservableCollection<Item> ItemsCollectionSelectionnee
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


		public ModalAjoutItem_VM()
		{
			cmdAjouter_Item = new Commande(cmdAjouter, boutonAjouterActif);
			lstCollections = new CollectionADO().Recuperer(UtilisateurADO.utilisateur.Id);
		}

		private void cmdAjouter(object param)
		{
			//Envoie l'évènement d'ajout d'item avec l'item sélectionné pour qu'il soit reçu par ModalNouvelleProposition_VM
			EvenementSysteme.Publier<EnvoyerItemMessage>(new EnvoyerItemMessage() { Item = ItemSelectionne });

			ItemSelectionne = null;
		}

		private bool boutonAjouterActif()
		{
			return !(ItemSelectionne is null);
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

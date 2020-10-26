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
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.Outils.Messages;

namespace LeCollectionneur.VuesModeles
{
	public class ModalNouvelleProposition_VM : INotifyPropertyChanged
	{
		#region Commandes

		public ICommand cmdAnnuler_Proposition { get; set; }
		public ICommand cmdDetails_Item { get; set; }
		public ICommand cmdProposer_Proposition { get; set; }
		public ICommand cmdAjouterItem_Proposition { get; set; }
		public ICommand cmdSupprimerItem_Proposition { get; set; }

		#endregion

		#region Propriétés

		private Proposition nouvelleProposition;

		public double MontantDemande { get; set; }


		private double _montantProposition;

		public double MontantProposition
		{
			get { return _montantProposition; }
			set 
			{
				_montantProposition = Math.Round(value, 2);
				nouvelleProposition.Montant = _montantProposition;
			}
		}

		private ObservableCollection<Item> _itemsProposition;
		public ObservableCollection<Item> ItemsProposition
		{
			get { return _itemsProposition; }
			set
			{
				_itemsProposition = value;
				nouvelleProposition.ItemsProposes = value;
				OnPropertyChanged("ItemsProposition");
			}
		}

		private Item _itemSelectionne;
		public Item ItemSelectionne
		{
			get { return _itemSelectionne; }
			set
			{
				_itemSelectionne = value;
				OnPropertyChanged("ItemSelectionne");
			}
		}

		public ObservableCollection<Item> ItemsAnnonce { get; set; }
		#endregion

		public ModalNouvelleProposition_VM(Annonce annonce)
		{
			// Initialiser les commandes
			cmdAnnuler_Proposition = new Commande(cmdAnnuler);
			cmdDetails_Item = new Commande(cmdDetailsItem);
			cmdProposer_Proposition = new Commande(cmdProposer);
			cmdAjouterItem_Proposition = new Commande(cmdAjouterItem);
			cmdSupprimerItem_Proposition = new Commande(cmdSupprimerItem);

			//Initialiser la nouvelle proposition avec les informations qui ne changeront pas
			nouvelleProposition = new Proposition();
			nouvelleProposition.AnnonceLiee = annonce;
			nouvelleProposition.Proposeur = UtilisateurADO.utilisateur;

			ItemsProposition = new ObservableCollection<Item>();
		
			MontantDemande = annonce.Montant;
			ItemsAnnonce = annonce.ListeItems;

			//Abonnement à l'évènement Ajout d'un item à une proposition
			EvenementSysteme.Abonnement<EnvoyerItemMessage>(ajouterItemMessage);
		}

		#region Implémentation Commandes

		private void cmdAnnuler(object param)
		{
			//Pour obtenir l'interface de la fenêtre, il faut la passer en paramètre lors de l'envoi de la commande (Voir le XAML du bouton, CommandParameter={...})
			IFenetreFermeable fenetre = param as IFenetreFermeable;
			if (fenetre != null)
			{
				//On confirme que l'utilisateur veut bien fermer la fenêtre
				MessageBoxResult resultat = MessageBox.Show("Voulez-vous vraiment annuler votre proposition?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
				if (resultat == MessageBoxResult.Yes)
				{
					fenetre.Fermer();
				}
				
			}
		}

		private void cmdDetailsItem(object param)
		{
			// On recoit en paramètre l'item à détailler en premier et l'interface de la vue en deuxième.
			// On passe l'item à l'interface pour ouvrir une modal de détails
			Item itemDetails = ((object[])param)[0] as Item;
			IOuvreModalAvecParametre<Item> interfaceV = ((object[])param)[1] as IOuvreModalAvecParametre<Item>;
			interfaceV.OuvrirModal(itemDetails);
		}

		private void cmdProposer(object param)
		{

			PropositionADO propADO = new PropositionADO();
			nouvelleProposition.DateProposition = DateTime.Now;

			propADO.Ajouter(nouvelleProposition);

			//Pour obtenir l'interface de la fenêtre, il faut la passer en paramètre lors de l'envoi de la commande (Voir le XAML du bouton, CommandParameter={...})
			IFenetreFermeable fenetre = param as IFenetreFermeable;
			if (fenetre != null)
			{
				//On confirme que l'utilisateur veut bien fermer la fenêtre
				MessageBox.Show($"La proposition sur l'annonce {nouvelleProposition.AnnonceLiee.Titre} a été envoyée. Vous pouvez la consulter dans l'onglet Propositions.", "Envoi réussi", MessageBoxButton.OK, MessageBoxImage.Information);
				fenetre.Fermer();
			}
		}

		private void cmdAjouterItem(object param)
		{
			IOuvreModal fenetre = param as IOuvreModal;
			fenetre.OuvrirModal();
		}

		private void cmdSupprimerItem(object param)
		{
			if (ItemSelectionne == null)
				return;

			MessageBoxResult resultat = MessageBox.Show($"Voulez-vous vraiment supprimer {ItemSelectionne.Nom} de la proposition?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
			if (resultat == MessageBoxResult.Yes)
			{
				ItemsProposition.Remove(ItemSelectionne);
				ItemSelectionne = null;
			}
		}

		#endregion

		private bool itemEstDansProposition(Item item)
		{
			foreach (Item i in ItemsProposition)
			{
				if (item.Id == i.Id)
				{
					return true;
				}
			}

			return false;
		}

		private void ajouterItemMessage(EnvoyerItemMessage msg)
		{
			if (!itemEstDansProposition(msg.Item))
			{
				ItemsProposition.Add(msg.Item);
			}
			else
			{
				MessageBox.Show($"Cet objet existe déjà dans la proposition.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}

		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string nomPropriete)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
		}
	}
}

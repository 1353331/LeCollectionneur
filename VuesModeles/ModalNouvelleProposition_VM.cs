﻿using System;
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
			// Voir comment on va passer les informations de l'utilisateur au travers du projet
			//nouvelleProposition.Proposeur = Utilisateur;

			ItemsProposition = new ObservableCollection<Item>();
		
			MontantDemande = annonce.Montant;
			ItemsAnnonce = annonce.ListeItems;
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
			//On reçoit l'item qui est sur la ligne du bouton Détails, puis on le passe à la fenêtre de détails d'un item
			Item itemDetails = param as Item;
			//TODO: Afficher la fenêtre modale de détails d'items

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
			//TODO: Modifier pour ajouter un Item à partir d'une fenêtre modale
			ItemADO itemADO = new ItemADO();
			Item item = itemADO.RecupererUn(8);

			if (!itemEstDansProposition(item))
			{
				ItemsProposition.Add(item);
			}
			else
			{
				MessageBox.Show($"Cet objet existe déjà dans la proposition.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
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
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string nomPropriete)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
		}
	}
}
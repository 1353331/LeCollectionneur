using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Enumerations;
using LeCollectionneur.Outils.Interfaces;

namespace LeCollectionneur.VuesModeles
{
	public class Transactions_VM : INotifyPropertyChanged
	{
		public ICommand cmdDetails_Item { get; set; }

		#region Propriétés
		TransactionADO transactionADO = new TransactionADO();

		private ObservableCollection<Transaction> _TransactionsAffichees;

		public ObservableCollection<Transaction> TransactionsAffichees
		{
			get { return _TransactionsAffichees; }
			set
			{
				_TransactionsAffichees = value;
				OnPropertyChanged("TransactionsAffichees");
			}
		}

		private Transaction _TransactionSelectionnee;

		public Transaction TransactionSelectionnee
		{
			get { return _TransactionSelectionnee; }
			set
			{
				_TransactionSelectionnee = value;

				if (TransactionSelectionnee != null)
				{
					// Si l'annonceur est l'utilisateur connecté, on met les informations de son annonce à gauche. Sinon, on met les informations de sa proposition à gauche
					if (TransactionSelectionnee.PropositionTrx.AnnonceLiee.Annonceur.Id == UtilisateurADO.utilisateur.Id)
					{
						ItemsGauche = TransactionSelectionnee.PropositionTrx.AnnonceLiee.ListeItems;
						MontantGauche = TransactionSelectionnee.PropositionTrx.AnnonceLiee.Montant;
						TitreGauche = "Mon annonce";

						ItemsDroite = TransactionSelectionnee.PropositionTrx.ItemsProposes;
						MontantDroite = TransactionSelectionnee.PropositionTrx.Montant;
						TitreDroite = $"Proposition de {TransactionSelectionnee­.PropositionTrx.Proposeur.NomUtilisateur}";
					}
					else
					{
						ItemsGauche = TransactionSelectionnee.PropositionTrx.ItemsProposes;
						MontantGauche = TransactionSelectionnee.PropositionTrx.Montant;
						TitreGauche = "Ma proposition";

						ItemsDroite = TransactionSelectionnee.PropositionTrx.AnnonceLiee.ListeItems;
						MontantDroite = TransactionSelectionnee.PropositionTrx.AnnonceLiee.Montant;
						TitreDroite = $"Annonce de {TransactionSelectionnee.PropositionTrx.AnnonceLiee.Annonceur.NomUtilisateur}";
					}
				}

				OnPropertyChanged("TransactionSelectionnee");
			}
		}

		private string _dateTransaction;

		public string DateTransaction
		{
			get 
			{
				if (TransactionSelectionnee != null)
				{
					return TransactionSelectionnee.Date.ToString("yyyy-MM-dd");
				}
				return "Indisponible" ; 
			}
			set { _dateTransaction = value; }
		}


		private ObservableCollection<Item> _itemsGauche;

		public ObservableCollection<Item> ItemsGauche
		{
			get { return _itemsGauche; }
			set
			{
				_itemsGauche = value;
				OnPropertyChanged("ItemsGauche");
			}
		}

		private ObservableCollection<Item> _itemsDroite;

		public ObservableCollection<Item> ItemsDroite
		{
			get { return _itemsDroite; }
			set
			{
				_itemsDroite = value;
				OnPropertyChanged("ItemsDroite");
			}
		}

		private double _montantGauche;

		public double MontantGauche
		{
			get { return _montantGauche; }
			set
			{
				_montantGauche = value;
				OnPropertyChanged("MontantGauche");
			}
		}

		private double _montantDroite;

		public double MontantDroite
		{
			get { return _montantDroite; }
			set
			{
				_montantDroite = value;
				OnPropertyChanged("MontantDroite");
			}
		}

		private string _titreGauche;

		public string TitreGauche
		{
			get { return _titreGauche; }
			set
			{
				_titreGauche = value;
				OnPropertyChanged("TitreGauche");
			}
		}

		private string _titreDroite;

		public string TitreDroite
		{
			get { return _titreDroite; }
			set
			{
				_titreDroite = value;
				OnPropertyChanged("TitreDroite");
			}
		}

		#endregion
		public Transactions_VM()
		{
			TransactionsAffichees = transactionADO.RecupererToutParUtilisateur(UtilisateurADO.utilisateur.Id);

			cmdDetails_Item = new Commande(cmdDetailsItem);
		}



		#region Implémentation des commandes

		private void cmdDetailsItem(object param)
		{
			// On recoit en paramètre l'item à détailler en premier et l'interface de la vue en deuxième.
			// On passe l'item à l'interface pour ouvrir une modal de détails
			Item itemDetails = ((object[])param)[0] as Item;
			IOuvreModalAvecParametre<Item> interfaceV = ((object[])param)[1] as IOuvreModalAvecParametre<Item>;
			interfaceV.OuvrirModal(itemDetails);
		}

		#endregion

		#region NotifyPropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string nomPropriete)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
		}
		#endregion
	}
}

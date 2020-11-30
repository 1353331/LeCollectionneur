using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using LeCollectionneur.EF;
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

		private bool _controlesActifs;

		public bool ControlesActifs
		{
			get { return _controlesActifs; }
			set
			{
				_controlesActifs = value;
				OnPropertyChanged("ControlesActifs");
			}
		}

		private Visibility _visibiliteSpinner;

		public Visibility VisibiliteSpinner
		{
			get { return _visibiliteSpinner; }
			set
			{
				_visibiliteSpinner = value;

				OnPropertyChanged("VisibiliteSpinner");
			}
		}

		private bool _premiereLigne = true;

		private bool _filtreAnnonceur;

		public bool FiltreAnnonceur
		{
			get { return _filtreAnnonceur; }
			set 
			{
				_premiereLigne = true;
				_filtreAnnonceur = value;
				OnPropertyChanged("FiltreAnnonceur");
				if (TransactionsAfficheesView != null)
				{
					TransactionsAfficheesView.Refresh();
				}
				
			}
		}

		private bool _filtreProposeur;

		public bool FiltreProposeur
		{
			get { return _filtreProposeur; }
			set 
			{
				_premiereLigne = true;
				_filtreProposeur = value;
				OnPropertyChanged("FiltreProposeur");
				if (TransactionsAfficheesView != null)
				{
					TransactionsAfficheesView.Refresh();
				}
			}
		}

		private ICollectionView _transactionsAfficheesView;

		public ICollectionView TransactionsAfficheesView
		{
			get { return _transactionsAfficheesView; }
			set
			{
				_transactionsAfficheesView = value;
				OnPropertyChanged("TransactionsAfficheesView");

			}
		}

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
				else
				{
					ItemsGauche = new ObservableCollection<Item>();
					MontantGauche = 0;
					TitreGauche = "";

					ItemsDroite = new ObservableCollection<Item>();
					MontantDroite = 0;
					TitreDroite = "";
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
				return "Indisponible";
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
			chargerTransactions();
			cmdDetails_Item = new Commande(cmdDetailsItem);
		}

		#region Async

		private readonly BackgroundWorker worker = new BackgroundWorker();

		private void chargerTransactions()
		{
			TransactionsAffichees = new ObservableCollection<Transaction>();
			ControlesActifs = false;
			VisibiliteSpinner = Visibility.Visible;
			if (!worker.IsBusy)
			{
				worker.DoWork += Worker_DoWork;
				worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
				worker.RunWorkerAsync();
			}
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			using (Context context = new Context())
			{
				List<Transaction> lstTransactions = OutilEF.ctx.Transactions.Include("PropositionTrx.Proposeur")
																					 .Include("PropositionTrx.AnnonceLiee.ListeItems.Condition")
																					 .Include("PropositionTrx.AnnonceLiee.ListeItems.Type")
																					 .Include("PropositionTrx.AnnonceLiee.Type")
																					 .Include("PropositionTrx.AnnonceLiee.EtatAnnonce")
																					 .Include("PropositionTrx.ItemsProposes.Type")
																					 .Include("PropositionTrx.ItemsProposes.Condition")
																					 .Include("PropositionTrx.EtatProposition")
																					 .Include("PropositionTrx.AnnonceLiee.Annonceur")
																					 .Where(t => (t.PropositionTrx.AnnonceLiee.Annonceur.Id == UtilisateurADO.utilisateur.Id) || (t.PropositionTrx.Proposeur.Id == UtilisateurADO.utilisateur.Id))
																					 .ToList();

				ObservableCollection<Transaction> ocTransactions = changerListTransactionsEnOcTransaction(lstTransactions);

				TransactionsAffichees = ocTransactions;
				TransactionsAfficheesView = CollectionViewSource.GetDefaultView(TransactionsAffichees);
				TransactionsAfficheesView.Filter += new Predicate<object>(filtreTransactionsAffichees);
			}
		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			VisibiliteSpinner = Visibility.Collapsed;
			FiltreAnnonceur = true;
			FiltreProposeur = true;
			ControlesActifs = true;
			if (TransactionsAffichees.Count > 0)
			{
				TransactionSelectionnee = TransactionsAffichees.First();
			}
		}

		private ObservableCollection<Transaction> changerListTransactionsEnOcTransaction(List<Transaction> lstTransactions)
		{
			ObservableCollection<Transaction> ocTransactions = new ObservableCollection<Transaction>();
			foreach (Transaction trx in lstTransactions)
			{
				foreach (Item item in trx.PropositionTrx.ItemsProposes)
				{
					if (!String.IsNullOrWhiteSpace(item.CheminImage))
					{
						item.BmImage = Fichier.TransformerBitmapEnBitmapImage(Fichier.RecupererImageServeur(item.CheminImage));
						if (item.BmImage is null)
						{
							ItemADO gestionItem = new ItemADO();
							item.CheminImage = null;
							gestionItem.EnleverCheminImage(item.Id);
						}
					}
				}

				foreach (Item item in trx.PropositionTrx.AnnonceLiee.ListeItems)
				{
					if (!String.IsNullOrWhiteSpace(item.CheminImage))
					{
						item.BmImage = Fichier.TransformerBitmapEnBitmapImage(Fichier.RecupererImageServeur(item.CheminImage));
						if (item.BmImage is null)
						{
							ItemADO gestionItem = new ItemADO();
							item.CheminImage = null;
							gestionItem.EnleverCheminImage(item.Id);
						}
					}
				}

				trx.Role = trx.PropositionTrx.Proposeur.Id == UtilisateurADO.utilisateur.Id ? "Proposeur" : "Annonceur";

				ocTransactions.Add(trx);
			}

			return ocTransactions;
		}

		#endregion

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

		private bool filtreTransactionsAffichees(object obj)
		{
			Transaction trx = obj as Transaction;
			bool estAffiche = (FiltreAnnonceur && trx.Role == "Annonceur") || (FiltreProposeur && trx.Role == "Proposeur");

			if (TransactionSelectionnee is null && estAffiche && _premiereLigne)
			{
				TransactionSelectionnee = trx;
				_premiereLigne = !_premiereLigne;
			}

			return estAffiche;
		}

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

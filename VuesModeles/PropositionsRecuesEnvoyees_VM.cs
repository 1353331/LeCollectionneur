using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using LeCollectionneur.EF;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Enumerations;
using LeCollectionneur.Outils.Interfaces;

namespace LeCollectionneur.VuesModeles
{
	public class PropositionsRecuesEnvoyees_VM : INotifyPropertyChanged
	{
		#region Commandes
		private ICommand _cmdAccepterProposition;

		public ICommand cmdAccepterProposition
		{
			get { return _cmdAccepterProposition; }
			set
			{
				_cmdAccepterProposition = value;
				OnPropertyChanged("cmdAccepterProposition");
			}
		}

		private ICommand _cmdRefuserProposition;

		public ICommand cmdRefuserProposition
		{
			get { return _cmdRefuserProposition; }
			set
			{
				_cmdRefuserProposition = value;
				OnPropertyChanged("cmdRefuserProposition");
			}
		}

		private ICommand _cmdAnnulerProposition;

		public ICommand cmdAnnulerProposition
		{
			get { return _cmdAnnulerProposition; }
			set
			{
				_cmdAnnulerProposition = value;
				OnPropertyChanged("cmdAnnulerProposition");
			}
		}

		public ICommand cmdDetails_Item { get; set; }

		public ICommand cmdPropositionsRecues { get; set; }
		public ICommand cmdPropositionsEnvoyees { get; set; }

		public ICommand cmdEnvoyerMessage { get; set; }
		public ICommand cmdTransactions { get; set; }
		public ICommand cmdRafraichirPropositions { get; set; }
		#endregion

		#region Propriétés Transactions
		private bool _premiereLigneTransactions = true;

		private bool _filtreAnnonceur;

		public bool FiltreAnnonceur
		{
			get { return _filtreAnnonceur; }
			set
			{
				_premiereLigneTransactions = true;
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
				_premiereLigneTransactions = true;
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
						TitreMontantGauche = "Montant demandé: ";
						VisibiliteItemsVideGauche = Visibility.Hidden;

						ItemsDroite = TransactionSelectionnee.PropositionTrx.ItemsProposes;
						MontantDroite = TransactionSelectionnee.PropositionTrx.Montant;
						TitreDroite = $"Proposition de {TransactionSelectionnee­.PropositionTrx.Proposeur.NomUtilisateur}";
						TitreMontantDroite = "Montant proposé: ";
						VisibiliteItemsVideDroite = TransactionSelectionnee.PropositionTrx.ItemsProposes.Count == 0 ? Visibility.Visible : Visibility.Hidden;
					}
					else
					{
						ItemsGauche = TransactionSelectionnee.PropositionTrx.ItemsProposes;
						MontantGauche = TransactionSelectionnee.PropositionTrx.Montant;
						TitreGauche = "Ma proposition";
						TitreMontantGauche = "Montant proposé: ";
						VisibiliteItemsVideGauche = TransactionSelectionnee.PropositionTrx.ItemsProposes.Count == 0 ? Visibility.Visible : Visibility.Hidden;

						ItemsDroite = TransactionSelectionnee.PropositionTrx.AnnonceLiee.ListeItems;
						MontantDroite = TransactionSelectionnee.PropositionTrx.AnnonceLiee.Montant;
						TitreDroite = $"Annonce de {TransactionSelectionnee.PropositionTrx.AnnonceLiee.Annonceur.NomUtilisateur}";
						TitreMontantDroite = "Montant demandé: ";
						VisibiliteItemsVideDroite = Visibility.Hidden;
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
		#endregion

		#region Propriétés Propositions

		private PropositionADO propADO = new PropositionADO();

		private ObservableCollection<Proposition> _propositionsAffichees;

		public ObservableCollection<Proposition> PropositionsAffichees
		{
			get { return _propositionsAffichees; }
			set
			{
				_propositionsAffichees = value;
				OnPropertyChanged("PropositionsAffichees");
			}
		}

		private Proposition _propositionSelectionnee;

		public Proposition PropositionSelectionnee
		{
			get { return _propositionSelectionnee; }
			set
			{
				_propositionSelectionnee = value;
				changementVisibiliteCommandes();
				if (PropositionSelectionnee != null)
				{
					if (RecuesSelectionnees)
					{
						ItemsGauche = PropositionSelectionnee.AnnonceLiee.ListeItems;
						MontantGauche = PropositionSelectionnee.AnnonceLiee.Montant;
						TitreGauche = "Mon Annonce";
						TitreMontantGauche = "Montant demandé: ";
						VisibiliteItemsVideGauche = Visibility.Hidden;

						ItemsDroite = PropositionSelectionnee.ItemsProposes;
						MontantDroite = PropositionSelectionnee.Montant;
						TitreDroite = $"Proposition de {PropositionSelectionnee.Proposeur.NomUtilisateur}";
						TitreMontantDroite = "Montant proposé: ";
						VisibiliteItemsVideDroite = PropositionSelectionnee.ItemsProposes.Count == 0 ? Visibility.Visible : Visibility.Hidden;
					}
					else
					{
						ItemsGauche = PropositionSelectionnee.ItemsProposes;
						MontantGauche = PropositionSelectionnee.Montant;
						TitreGauche = "Ma Proposition";
						TitreMontantGauche = "Montant proposé: ";
						VisibiliteItemsVideGauche = PropositionSelectionnee.ItemsProposes.Count == 0 ? Visibility.Visible : Visibility.Hidden;

						ItemsDroite = PropositionSelectionnee.AnnonceLiee.ListeItems;
						MontantDroite = PropositionSelectionnee.AnnonceLiee.Montant;
						TitreDroite = $"Annonce de {PropositionSelectionnee.AnnonceLiee.Annonceur.NomUtilisateur}";
						TitreMontantDroite = "Montant demandé: ";
						VisibiliteItemsVideDroite = Visibility.Hidden;
					}
					VisibiliteMessage = Visibility.Visible;
					_dernierePropositionSelectionnee = PropositionSelectionnee;
				}
				else
				{
					ItemsGauche = new ObservableCollection<Item>();
					ItemsDroite = new ObservableCollection<Item>();
					MontantDroite = 0;
					MontantGauche = 0;
					VisibiliteMessage = Visibility.Hidden;
					VisibiliteItemsVideGauche = Visibility.Hidden;
					VisibiliteItemsVideDroite = Visibility.Hidden;

					TitreGauche = "";
					TitreMontantGauche = "";
					TitreDroite = "";
					TitreMontantDroite = "";
				}

				OnPropertyChanged("PropositionSelectionnee");
			}
		}

		private Proposition _dernierePropositionSelectionnee;

		private bool _recuesSelectionnees;

		private bool RecuesSelectionnees
		{
			get { return _recuesSelectionnees; }
			set
			{
				_recuesSelectionnees = value;
				EnvoyeesSelectionnees = !value;
			}
		}

		private bool EnvoyeesSelectionnees { get; set; }
		#endregion

		#region Propriétés Communes
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

		private Visibility _visibiliteMessage;

		public Visibility VisibiliteMessage
		{
			get { return _visibiliteMessage; }
			set
			{
				_visibiliteMessage = value;
				OnPropertyChanged("VisibiliteMessage");
			}
		}

		private Visibility _visibiliteItemsVideGauche;

		public Visibility VisibiliteItemsVideGauche
		{
			get { return _visibiliteItemsVideGauche; }
			set
			{
				_visibiliteItemsVideGauche = value;
				OnPropertyChanged("VisibiliteItemsVideGauche");
			}
		}

		private Visibility _visibiliteItemsVideDroite;

		public Visibility VisibiliteItemsVideDroite
		{
			get { return _visibiliteItemsVideDroite; }
			set 
			{ 
				_visibiliteItemsVideDroite = value;
				OnPropertyChanged("VisibiliteItemsVideDroite");
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

		private string _titreMontantGauche;

		public string TitreMontantGauche
		{
			get { return _titreMontantGauche; }
			set 
			{ 
				_titreMontantGauche = value;
				OnPropertyChanged("TitreMontantGauche");
			}
		}

		private string _titreMontantDroite;

		public string TitreMontantDroite
		{
			get { return _titreMontantDroite; }
			set
			{
				_titreMontantDroite = value;
				OnPropertyChanged("TitreMontantDroite");
			}
		}

		#endregion

		public PropositionsRecuesEnvoyees_VM()
		{
			_recuesSelectionnees = true;
			ControlesActifs = true;
			VisibiliteMessage = Visibility.Hidden;
			VisibiliteItemsVideGauche = Visibility.Collapsed;
			VisibiliteItemsVideDroite = Visibility.Collapsed;
			chargerPropositionsRecues();
			changementVisibiliteCommandes();
			cmdPropositionsRecues = new Commande(cmdRecues);
			cmdPropositionsEnvoyees = new Commande(cmdEnvoyees);
			cmdDetails_Item = new Commande(cmdDetailsItem);
			cmdEnvoyerMessage = new Commande(cmdEnvMessage);
			cmdTransactions = new Commande(cmdHistoriqueTransactions);
			cmdRafraichirPropositions = new Commande(cmdRafraichir);
		}

		private BackgroundWorker worker = new BackgroundWorker();

		#region Async Transactions

		private void chargerTransactions()
		{
			TransactionsAffichees = new ObservableCollection<Transaction>();
			ControlesActifs = false;
			VisibiliteSpinner = Visibility.Visible;
			if (worker != null && !worker.IsBusy)
			{
				worker = new BackgroundWorker();
				worker.DoWork += Worker_DoWorkTransactions;
				worker.RunWorkerCompleted += Worker_RunWorkerCompletedTransactions;
				worker.RunWorkerAsync();
			}
		}

		private void Worker_DoWorkTransactions(object sender, DoWorkEventArgs e)
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
																					 .OrderByDescending(t => t.Date)
																					 .ToList();

				ObservableCollection<Transaction> ocTransactions = changerListTransactionsEnOcTransaction(lstTransactions);

				TransactionsAffichees = ocTransactions;
				TransactionsAfficheesView = CollectionViewSource.GetDefaultView(TransactionsAffichees);
				TransactionsAfficheesView.Filter += new Predicate<object>(filtreTransactionsAffichees);
			}
		}

		private void Worker_RunWorkerCompletedTransactions(object sender, RunWorkerCompletedEventArgs e)
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

		private bool filtreTransactionsAffichees(object obj)
		{
			Transaction trx = obj as Transaction;
			bool estAffiche = (FiltreAnnonceur && trx.Role == "Annonceur") || (FiltreProposeur && trx.Role == "Proposeur");

			if (TransactionSelectionnee is null && estAffiche && _premiereLigneTransactions)
			{
				TransactionSelectionnee = trx;
				_premiereLigneTransactions = !_premiereLigneTransactions;
			}

			return estAffiche;
		}
		#endregion

		#region Async Propositions

		private void chargerPropositionsRecues()
		{
			PropositionsAffichees = new ObservableCollection<Proposition>();
			ControlesActifs = false;
			VisibiliteSpinner = Visibility.Visible;
			if (worker != null && !worker.IsBusy)
			{
				worker = new BackgroundWorker();
				worker.DoWork += Worker_DoWorkRecues;
				worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
				worker.RunWorkerAsync();
			}
		}


		private void chargerPropositionsEnvoyees()
		{
			PropositionsAffichees = new ObservableCollection<Proposition>();
			ControlesActifs = false;
			VisibiliteSpinner = Visibility.Visible;
			if (worker != null && !worker.IsBusy)
			{
				worker = new BackgroundWorker();
				worker.DoWork += Worker_DoWorkEnvoyees;
				worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
				worker.RunWorkerAsync();
			}

		}

		private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			ControlesActifs = true;
			VisibiliteSpinner = Visibility.Collapsed;
			if (PropositionsAffichees.Count > 0)
			{
				PropositionSelectionnee = PropositionsAffichees.First();
			}
		}

		private void Worker_DoWorkRecues(object sender, DoWorkEventArgs e)
		{
			using (Context context = new Context())
			{
				List<Proposition> lstPropositions = context.Propositions.Include("Proposeur")
																					 .Include("AnnonceLiee.ListeItems.Condition")
																					 .Include("AnnonceLiee.ListeItems.Type")
																					 .Include("AnnonceLiee.Type")
																					 .Include("AnnonceLiee.EtatAnnonce")
																					 .Include("ItemsProposes.Type")
																					 .Include("ItemsProposes.Condition")
																					 .Include("EtatProposition")
																					 .Include("AnnonceLiee.Annonceur")
																					 .AsNoTracking()
																					 .Where(p => (p.AnnonceLiee.Annonceur.Id == UtilisateurADO.utilisateur.Id) && (p.EtatProposition.Nom != EtatsProposition.Acceptee))
																					 .OrderByDescending(p => p.DateProposition)
																					 .ToList();

				ObservableCollection<Proposition> ocPropositions = changerListPropositionsEnOCProposition(lstPropositions);

				if (RecuesSelectionnees)
				{
					PropositionsAffichees = ocPropositions;
				}
			}
		}

		private void Worker_DoWorkEnvoyees(object sender, DoWorkEventArgs e)
		{
			using (Context context = new Context())
			{
				List<Proposition> lstPropositions = context.Propositions.Include("Proposeur")
																					.Include("EtatProposition")
																					.Include("AnnonceLiee.ListeItems.Condition")
																					.Include("AnnonceLiee.ListeItems.Type")
																					.Include("AnnonceLiee.Type")
																					.Include("AnnonceLiee.EtatAnnonce")
																					.Include("AnnonceLiee.Annonceur")
																					.Include("ItemsProposes.Type")
																					.Include("ItemsProposes.Condition")
																					.AsNoTracking()
																					.Where(p => (p.Proposeur.Id == UtilisateurADO.utilisateur.Id) && (p.EtatProposition.Nom != EtatsProposition.Acceptee))
																					.OrderByDescending(p => p.DateProposition)
																					.ToList();

				ObservableCollection<Proposition> ocPropositions = changerListPropositionsEnOCProposition(lstPropositions);
				if (!RecuesSelectionnees)
				{
					PropositionsAffichees = ocPropositions;
				}
			}
		}
		#endregion

		#region Implémentation des commandes
		private void cmdAccepter(object param)
		{
			if (PropositionSelectionnee != null)
			{
				//On modifie la propriété sélectionnée en BD, puis on reload les propositions reçues
				Transaction nouvelleTransaction = new Transaction(PropositionSelectionnee);
				nouvelleTransaction.EffectuerTransaction();
				if (PropositionSelectionnee.ItemsProposes.Count > 0)
				{
					MessageBox.Show($"La transaction a été effectuée. Vous pouvez retrouver vos nouveaux items dans la nouvelle collection \"{PropositionSelectionnee.AnnonceLiee.Type.Nom}: {PropositionSelectionnee.AnnonceLiee.Titre}\"", "Échange réussie", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				else
				{
					MessageBox.Show($"La transaction a été effectuée. Vos items ont été transférés à l'utilisateur \" {PropositionSelectionnee.Proposeur.NomUtilisateur} \"", "Échange réussie", MessageBoxButton.OK, MessageBoxImage.Information);
				}

				chargerPropositionsRecues();
			}
		}

		private void cmdRefuser(object param)
		{
			if (PropositionSelectionnee != null)
			{
				PropositionSelectionnee.EtatProposition = new EtatProposition(EtatsProposition.Refusee);
				propADO.Modifier(PropositionSelectionnee);

				chargerPropositionsRecues();
			}
		}

		private void cmdAnnuler(object param)
		{
			if (PropositionSelectionnee != null)
			{
				PropositionSelectionnee.EtatProposition = new EtatProposition(EtatsProposition.Annulee);
				propADO.Modifier(PropositionSelectionnee);
				chargerPropositionsEnvoyees();
			}
		}

		private void cmdEnvoyees(object param)
		{
			RecuesSelectionnees = false;
			PropositionSelectionnee = null;
			TransactionSelectionnee = null;
			chargerPropositionsEnvoyees();
		}

		private void cmdRecues(object param)
		{
			RecuesSelectionnees = true;
			PropositionSelectionnee = null;
			TransactionSelectionnee = null;
			chargerPropositionsRecues();
		}

		private void cmdHistoriqueTransactions(object param)
		{
			PropositionSelectionnee = null;
			TransactionSelectionnee = null;
			TransactionsAfficheesView = null;
			_premiereLigneTransactions = true;
			chargerTransactions();
		}

		private void cmdDetailsItem(object param)
		{
			// On recoit en paramètre l'item à détailler en premier et l'interface de la vue en deuxième.
			// On passe l'item à l'interface pour ouvrir une modal de détails
			Item itemDetails = ((object[])param)[0] as Item;
			IOuvreModalAvecParametre<Item> interfaceV = ((object[])param)[1] as IOuvreModalAvecParametre<Item>;
			interfaceV.OuvrirModal(itemDetails);
		}

		private void cmdEnvMessage(object param)
		{
			if (UnePropositionSelectionnee())
			{
				IOuvreModalAvecParametre<Utilisateur> fenetre = param as IOuvreModalAvecParametre<Utilisateur>;
				if (RecuesSelectionnees)
				{
					fenetre.OuvrirModal(PropositionSelectionnee.Proposeur);
				}
				else
				{
					fenetre.OuvrirModal(PropositionSelectionnee.AnnonceLiee.Annonceur);
				}
			}
		}

		private void cmdRafraichir(object obj)
		{
			if (RecuesSelectionnees)
			{
				chargerPropositionsRecues();
			}
			else
			{
				chargerPropositionsEnvoyees();
			}
		}
		#endregion

		#region Méthodes Privées

		private bool UnePropositionSelectionnee()
		{
			return !(PropositionSelectionnee is null);
		}

		private bool BoutonsActifs()
		{
			return (UnePropositionSelectionnee() && PropositionSelectionnee.EtatProposition.Nom == EtatsProposition.EnAttente);
		}

		private void changementVisibiliteCommandes()
		{
			cmdAccepterProposition = new Commande(cmdAccepter, BoutonsActifs);
			cmdRefuserProposition = new Commande(cmdRefuser, BoutonsActifs);
			cmdAnnulerProposition = new Commande(cmdAnnuler, BoutonsActifs);
		}

		private ObservableCollection<Proposition> changerListPropositionsEnOCProposition(List<Proposition> lstPropositions)
		{
			ObservableCollection<Proposition> ocPropositions = new ObservableCollection<Proposition>();
			foreach (Proposition proposition in lstPropositions)
			{
				foreach (Item item in proposition.ItemsProposes)
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

				foreach (Item item in proposition.AnnonceLiee.ListeItems)
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
				ocPropositions.Add(proposition);
			}

			return ocPropositions;
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

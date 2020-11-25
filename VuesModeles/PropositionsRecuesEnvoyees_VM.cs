using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LeCollectionneur.EF;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Enumerations;
using LeCollectionneur.Outils.Interfaces;

namespace LeCollectionneur.VuesModeles
{
	public class PropositionsRecuesEnvoyees_VM : INotifyPropertyChanged
	{
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

		#region Propriétés
		PropositionADO propADO = new PropositionADO();
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

						ItemsDroite = PropositionSelectionnee.ItemsProposes;
						MontantDroite = PropositionSelectionnee.Montant;
					}
					else
					{
						ItemsGauche = PropositionSelectionnee.ItemsProposes;
						MontantGauche = PropositionSelectionnee.Montant;

						ItemsDroite = PropositionSelectionnee.AnnonceLiee.ListeItems;
						MontantDroite = PropositionSelectionnee.AnnonceLiee.Montant;
					}
				}
				else
				{
					ItemsGauche = new ObservableCollection<Item>();
					ItemsDroite = new ObservableCollection<Item>();
					MontantDroite = 0;
					MontantGauche = 0;
				}

				OnPropertyChanged("PropositionSelectionnee");
			}
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

		public PropositionsRecuesEnvoyees_VM()
		{
			_recuesSelectionnees = true;
			ControlesActifs = true;
			chargerPropositionsRecues();
			//PropositionsAffichees = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
			changementVisibiliteCommandes();
			cmdPropositionsRecues = new Commande(cmdRecues);
			cmdPropositionsEnvoyees = new Commande(cmdEnvoyees);
			cmdDetails_Item = new Commande(cmdDetailsItem);
			cmdEnvoyerMessage = new Commande(cmdEnvMessage);
		}

		private readonly BackgroundWorker worker = new BackgroundWorker();

		private void chargerPropositionsRecues()
		{
			PropositionsAffichees = new ObservableCollection<Proposition>();
			ControlesActifs = false;
			VisibiliteSpinner = Visibility.Visible;
			if (!worker.IsBusy)
			{
				worker.DoWork += Worker_DoWork;
				worker.RunWorkerCompleted += WorkerEnvoyees_RunWorkerCompleted;
				worker.RunWorkerAsync();
			}
		}


		private void chargerPropositionsEnvoyees()
		{
			PropositionsAffichees = new ObservableCollection<Proposition>();
			ControlesActifs = false;
			VisibiliteSpinner = Visibility.Visible;
			if (!worker.IsBusy)
			{
				worker.DoWork += Worker_DoWork2;
				worker.RunWorkerCompleted += WorkerEnvoyees_RunWorkerCompleted;
				worker.RunWorkerAsync();
			}

		}

		private void WorkerEnvoyees_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			ControlesActifs = true;
			VisibiliteSpinner = Visibility.Collapsed;
			if (PropositionsAffichees.Count > 0)
			{
				PropositionSelectionnee = PropositionsAffichees.First();
			}
		}

		private void Worker_DoWork(object sender, DoWorkEventArgs e)
		{
			List<Proposition> lstPropositions = OutilEF.ctx.Propositions.Include("Proposeur")
																					.Include("AnnonceLiee.ListeItems.Condition")
																					.Include("AnnonceLiee.ListeItems.Type")
																					.Include("AnnonceLiee.Type")
																					.Include("AnnonceLiee.EtatAnnonce")
																					.Include("ItemsProposes.Type")
																					.Include("ItemsProposes.Condition")
																					.Include("EtatProposition")
																					.Include("AnnonceLiee.Annonceur")
																					.Where(p => p.AnnonceLiee.Annonceur.Id == UtilisateurADO.utilisateur.Id)
																					.ToList();

			ObservableCollection<Proposition> ocPropositions = changerListPropositionsEnOCProposition(lstPropositions);

			if (RecuesSelectionnees)
			{
				PropositionsAffichees = ocPropositions;
			}
		}

		private void Worker_DoWork2(object sender, DoWorkEventArgs e)
		{
			List<Proposition> lstPropositions = OutilEF.ctx.Propositions.Include("Proposeur")
																					.Include("EtatProposition")
																					.Include("AnnonceLiee.ListeItems.Condition")
																					.Include("AnnonceLiee.ListeItems.Type")
																					.Include("AnnonceLiee.Type")
																					.Include("AnnonceLiee.EtatAnnonce")
																					.Include("AnnonceLiee.Annonceur")
																					.Include("ItemsProposes.Type")
																					.Include("ItemsProposes.Condition")
																					.Where(p => p.Proposeur.Id == UtilisateurADO.utilisateur.Id)
																					.ToList();

			ObservableCollection<Proposition> ocPropositions = changerListPropositionsEnOCProposition(lstPropositions);
			if (!RecuesSelectionnees)
			{
				PropositionsAffichees = ocPropositions;
			}
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

		#region Implémentation des commandes
		private void cmdAccepter(object param)
		{
			if (PropositionSelectionnee != null)
			{
				//On modifie la propriété sélectionnée en BD, puis on reload les propositions reçues
				Transaction nouvelleTransaction = new Transaction(PropositionSelectionnee);
				nouvelleTransaction.EffectuerTransaction();
				MessageBox.Show($"La transaction a été effectuée. Vous pouvez retrouver vos nouveaux items dans la nouvelle collection \"{PropositionSelectionnee.AnnonceLiee.Type.Nom}: {PropositionSelectionnee.AnnonceLiee.Titre}\"", "Échange réussie", MessageBoxButton.OK, MessageBoxImage.Information);

				chargerPropositionsRecues();
				//PropositionsAffichees = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
			}
		}

		private void cmdRefuser(object param)
		{
			if (PropositionSelectionnee != null)
			{
				PropositionSelectionnee.EtatProposition = new EtatProposition(EtatsProposition.Refusee);
				propADO.Modifier(PropositionSelectionnee);

				chargerPropositionsRecues();
				//PropositionsAffichees = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
			}
		}

		private void cmdAnnuler(object param)
		{
			if (PropositionSelectionnee != null)
			{
				PropositionSelectionnee.EtatProposition = new EtatProposition(EtatsProposition.Annulee);
				propADO.Modifier(PropositionSelectionnee);
				chargerPropositionsEnvoyees();
				//PropositionsAffichees = propADO.RecupererPropositionsEnvoyees(UtilisateurADO.utilisateur.Id);
			}
		}

		private void cmdEnvoyees(object param)
		{
			RecuesSelectionnees = false;
			PropositionSelectionnee = null;
			//PropositionsAffichees = propADO.RecupererPropositionsEnvoyees(UtilisateurADO.utilisateur.Id);
			chargerPropositionsEnvoyees();
		}

		private void cmdRecues(object param)
		{
			RecuesSelectionnees = true;
			PropositionSelectionnee = null;
			//PropositionsAffichees = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
			chargerPropositionsRecues();
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
		#endregion

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

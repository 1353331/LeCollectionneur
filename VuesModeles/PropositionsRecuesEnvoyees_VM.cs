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

			PropositionsAffichees = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
			changementVisibiliteCommandes();
			cmdPropositionsRecues = new Commande(cmdRecues);
			cmdPropositionsEnvoyees = new Commande(cmdEnvoyees);
			cmdDetails_Item = new Commande(cmdDetailsItem);
			cmdEnvoyerMessage = new Commande(cmdEnvMessage);
		}

		

		#region Implémentation des commandes
		private void cmdAccepter(object param)
		{
			if (PropositionSelectionnee != null)
			{
				//On modifie la propriété sélectionnée en BD, puis on reload les propositions reçues
				Transaction nouvelleTransaction = new Transaction(PropositionSelectionnee);
				nouvelleTransaction.EffectuerTransaction();

				PropositionsAffichees = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
			}
		}

		private void cmdRefuser(object param)
		{
			if (PropositionSelectionnee != null)
			{
				PropositionSelectionnee.EtatProposition = EtatsProposition.Refusee;
				propADO.Modifier(PropositionSelectionnee);

				PropositionsAffichees = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
			}
		}

		private void cmdAnnuler(object param)
		{
			if (PropositionSelectionnee != null)
			{
				PropositionSelectionnee.EtatProposition = EtatsProposition.Annulee;
				propADO.Modifier(PropositionSelectionnee);

				PropositionsAffichees = propADO.RecupererPropositionsEnvoyees(UtilisateurADO.utilisateur.Id);
			}
		}

		private void cmdEnvoyees(object param)
		{
			RecuesSelectionnees = false;
			PropositionSelectionnee = null;
			PropositionsAffichees = propADO.RecupererPropositionsEnvoyees(UtilisateurADO.utilisateur.Id);
		}

		private void cmdRecues(object param)
		{
			RecuesSelectionnees = true;
			PropositionSelectionnee = null;
			PropositionsAffichees = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
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
			return (UnePropositionSelectionnee() && PropositionSelectionnee.EtatProposition == EtatsProposition.EnAttente);
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

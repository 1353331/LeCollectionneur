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

		public ICommand cmdChangementContexteRecuesEnvoyees { get; set; }

		public ICommand cmdEnvoyerMessage { get; set; }

		#region Propriétés
		PropositionADO propADO = new PropositionADO();

		private ObservableCollection<Proposition> _propositionsRecues;

		public ObservableCollection<Proposition> PropositionsRecues
		{
			get { return _propositionsRecues; }
			set
			{
				_propositionsRecues = value;
				OnPropertyChanged("PropositionsRecues");
			}
		}

		private ObservableCollection<Proposition> _propositionsEnvoyees;

		public ObservableCollection<Proposition> PropositionsEnvoyees
		{
			get { return _propositionsEnvoyees; }
			set
			{
				_propositionsEnvoyees = value;
				OnPropertyChanged("PropositionsEnvoyees");
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
				OnPropertyChanged("PropositionSelectionnee");
			}
		}
		#endregion

		public PropositionsRecuesEnvoyees_VM()
		{
			PropositionsEnvoyees = propADO.RecupererPropositionsEnvoyees(UtilisateurADO.utilisateur.Id);
			PropositionsRecues = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);

			changementVisibiliteCommandes();
			cmdChangementContexteRecuesEnvoyees = new Commande(cmdChangement);
			cmdDetails_Item = new Commande(cmdDetailsItem);
			cmdEnvoyerMessage = new Commande(cmdMessage);
		}

		#region Implémentation des commandes
		private void cmdAccepter(object param)
		{
			if (PropositionSelectionnee != null)
			{
				//On modifie la propriété sélectionnée en BD, puis on reload les propositions reçues
				PropositionSelectionnee.EtatProposition = "Acceptée";
				propADO.Modifier(PropositionSelectionnee);

				PropositionsRecues = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
			}
		}

		private void cmdRefuser(object param)
		{
			if (PropositionSelectionnee != null)
			{
				PropositionSelectionnee.EtatProposition = "Refusée";
				propADO.Modifier(PropositionSelectionnee);

				PropositionsRecues = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);
			}
		}

		private void cmdAnnuler(object param)
		{
			if (PropositionSelectionnee != null)
			{
				PropositionSelectionnee.EtatProposition = "Annulée";
				propADO.Modifier(PropositionSelectionnee);

				PropositionsEnvoyees = propADO.RecupererPropositionsEnvoyees(UtilisateurADO.utilisateur.Id);
			}
		}

		private void cmdChangement(object param)
		{
			PropositionSelectionnee = null;
		}

		private void cmdDetailsItem(object param)
		{
			// On recoit en paramètre l'item à détailler en premier et l'interface de la vue en deuxième.
			// On passe l'item à l'interface pour ouvrir une modal de détails
			Item itemDetails = ((object[])param)[0] as Item;
			IOuvreModalAvecParametre<Item> interfaceV = ((object[])param)[1] as IOuvreModalAvecParametre<Item>;
			interfaceV.OuvrirModal(itemDetails);
		}

		private void cmdMessage(object param)
		{
			MessageBox.Show("La fonction d'envoi de message n'est pas encore implémentée.", "Inexistant", MessageBoxButton.OK, MessageBoxImage.Error);
		}
		#endregion

		public bool UnePropositionSelectionnee()
		{
			return !(PropositionSelectionnee is null);
		}

		private void changementVisibiliteCommandes()
		{
			cmdAccepterProposition = new Commande(cmdAccepter, UnePropositionSelectionnee);
			cmdRefuserProposition = new Commande(cmdRefuser, UnePropositionSelectionnee);
			cmdAnnulerProposition = new Commande(cmdAnnuler, UnePropositionSelectionnee);
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

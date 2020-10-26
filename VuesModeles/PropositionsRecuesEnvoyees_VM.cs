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

namespace LeCollectionneur.VuesModeles
{
	public class PropositionsRecuesEnvoyees_VM : INotifyPropertyChanged
	{
		public ICommand cmdAccepterProposition { get; set; }
		public ICommand cmdRefuserProposition { get; set; }
		public ICommand cmdAnnulerProposition { get; set; }
		public ICommand cmdChangementContexteRecuesEnvoyees { get; set; }

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
				OnPropertyChanged("PropositionSelectionnee");
			}
		}
		#endregion

		public PropositionsRecuesEnvoyees_VM()
		{
			PropositionsEnvoyees = propADO.RecupererPropositionsEnvoyees(UtilisateurADO.utilisateur.Id);
			PropositionsRecues = propADO.RecupererPropositionsRecues(UtilisateurADO.utilisateur.Id);

			cmdAccepterProposition = new Commande(cmdAccepter);
			cmdRefuserProposition = new Commande(cmdRefuser);
			cmdAnnulerProposition = new Commande(cmdAnnuler);
			cmdChangementContexteRecuesEnvoyees = new Commande(cmdChangement);
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

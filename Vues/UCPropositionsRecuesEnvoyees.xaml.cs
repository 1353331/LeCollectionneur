using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.VuesModeles;

namespace LeCollectionneur.Vues
{
	/// <summary>
	/// Logique d'interaction pour UCProposition.xaml
	/// </summary>
	public partial class UCPropositionsRecuesEnvoyees : UserControl, IOuvreModalAvecParametre<Item>, IOuvreModalAvecParametre<Utilisateur>, IOuvreModalAvecParametre<Proposition>
	{
		public UCPropositionsRecuesEnvoyees()
		{
			InitializeComponent();
			DataContext = new PropositionsRecuesEnvoyees_VM();
		}

		private void radPropositionsRecues_Checked(object sender, RoutedEventArgs e)
		{
			// Vérifier si les contrôles sont initialisés (pour le loading initial de la page)
			if (btnRefuserProposition != null)
			{
				btnRefuserProposition.Visibility = Visibility.Visible;
				btnAccepterProposition.Visibility = Visibility.Visible;

				lblEtatProposition.Visibility = Visibility.Hidden;
				txbEtatProposition.Visibility = Visibility.Hidden;
				btnAnnulerProposition.Visibility = Visibility.Hidden;
				btnProposerDeNouveau.Visibility = Visibility.Hidden;

				dpPropositions.Visibility = Visibility.Visible;
				dpTransactions.Visibility = Visibility.Collapsed;

				gbTypePropositions.Visibility = Visibility.Visible;
				gbRoleTransactions.Visibility = Visibility.Collapsed;
			}
		}

		private void radPropositionsEnvoyees_Checked(object sender, RoutedEventArgs e)
		{
			btnRefuserProposition.Visibility = Visibility.Hidden;
			btnAccepterProposition.Visibility = Visibility.Hidden;

			lblEtatProposition.Visibility = Visibility.Visible;
			txbEtatProposition.Visibility = Visibility.Visible;
			btnAnnulerProposition.Visibility = Visibility.Visible;
			btnProposerDeNouveau.Visibility = Visibility.Visible;

			dpPropositions.Visibility = Visibility.Visible;
			dpTransactions.Visibility = Visibility.Collapsed;
		}
		private void btnTransactions_Click(object sender, RoutedEventArgs e)
		{
			btnRefuserProposition.Visibility = Visibility.Hidden;
			btnAccepterProposition.Visibility = Visibility.Hidden;
			lblEtatProposition.Visibility = Visibility.Hidden;
			txbEtatProposition.Visibility = Visibility.Hidden;
			btnAnnulerProposition.Visibility = Visibility.Hidden;
			btnProposerDeNouveau.Visibility = Visibility.Hidden;
			dpPropositions.Visibility = Visibility.Hidden;
			gbTypePropositions.Visibility = Visibility.Hidden;

			dpTransactions.Visibility = Visibility.Visible;
			gbRoleTransactions.Visibility = Visibility.Visible;
			btnTransactions.Visibility = Visibility.Collapsed;
			btnPropositions.Visibility = Visibility.Visible;
		}

		private void btnPropositions_Click(object sender, RoutedEventArgs e)
		{
			btnRefuserProposition.Visibility = Visibility.Visible;
			btnAccepterProposition.Visibility = Visibility.Visible;
			lblEtatProposition.Visibility = Visibility.Hidden;
			txbEtatProposition.Visibility = Visibility.Hidden;
			btnAnnulerProposition.Visibility = Visibility.Hidden;
			btnProposerDeNouveau.Visibility = Visibility.Hidden;

			dpPropositions.Visibility = Visibility.Visible;
			dpTransactions.Visibility = Visibility.Collapsed;

			gbTypePropositions.Visibility = Visibility.Visible;
			gbRoleTransactions.Visibility = Visibility.Collapsed;

			btnTransactions.Visibility = Visibility.Visible;
			btnPropositions.Visibility = Visibility.Collapsed;

			radPropositionsEnvoyees.IsChecked = false;
			radPropositionsRecues.IsChecked = true;
		}

		public void OuvrirModal(Item itemSelectionne)
		{
			ModalDetailsItem modalDetails = new ModalDetailsItem(itemSelectionne);
			modalDetails.Owner = Window.GetWindow(this);
			modalDetails.ShowDialog();
		}

		public void OuvrirModal(Utilisateur destinataire)
		{
			ModalEnvoyerMessage modalMessage = new ModalEnvoyerMessage(destinataire);
			modalMessage.Owner = Window.GetWindow(this);
			modalMessage.ShowDialog();
		}

		public void OuvrirModal(Proposition prop)
		{
			ModalNouvelleProposition viewProp = new ModalNouvelleProposition(prop);
			viewProp.Owner = Window.GetWindow(this);
			viewProp.ShowDialog();
		}
	}
}

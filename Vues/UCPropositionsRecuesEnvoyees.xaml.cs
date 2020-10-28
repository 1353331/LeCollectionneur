﻿using System;
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
	public partial class UCPropositionsRecuesEnvoyees : UserControl, IOuvreModalAvecParametre<Item>
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
				btnMessageProposition.Visibility = Visibility.Visible;
				dgRecues.Visibility = Visibility.Visible;

				lblEtatProposition.Visibility = Visibility.Hidden;
				txbEtatProposition.Visibility = Visibility.Hidden;
				btnMessageOffre.Visibility = Visibility.Hidden;
				btnAnnulerProposition.Visibility = Visibility.Hidden;
				dgEnvoyees.Visibility = Visibility.Hidden;
			}
		}

		private void radPropositionsEnvoyees_Checked(object sender, RoutedEventArgs e)
		{
			btnRefuserProposition.Visibility = Visibility.Hidden;
			btnAccepterProposition.Visibility = Visibility.Hidden;
			btnMessageProposition.Visibility = Visibility.Hidden;
			dgRecues.Visibility = Visibility.Hidden;

			lblEtatProposition.Visibility = Visibility.Visible;
			txbEtatProposition.Visibility = Visibility.Visible;
			btnMessageOffre.Visibility = Visibility.Visible;
			btnAnnulerProposition.Visibility = Visibility.Visible;
			dgEnvoyees.Visibility = Visibility.Visible;
		}

		public void OuvrirModal(Item itemSelectionne)
		{
			ModalDetailsItem modalDetails = new ModalDetailsItem(itemSelectionne);
			modalDetails.Owner = Window.GetWindow(this);
			modalDetails.ShowDialog();
		}
	}
}

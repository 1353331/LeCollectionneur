using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Vues;
using System;
using System.Collections.Generic;
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

namespace LeCollectionneur
{
	/// <summary>
	/// Logique d'interaction pour MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		
		public MainWindow()
		{
			InitializeComponent();
			BdBase MaBD = new BdBase();
			Window login = new Login();
			login.ShowDialog();
			if (UtilisateurADO.utilisateur != null)
				presenteurContenu.Content = new UCConversation();
			else
				this.Close();
		}

		private void btnCollections_Click(object sender, RoutedEventArgs e)
		{
			presenteurContenu.Content = new UCCollection();

		}

		private void btnAnnonces_Click(object sender, RoutedEventArgs e)
		{
			presenteurContenu.Content = new UCAnnonce();
		}

		private void btnPropositions_Click(object sender, RoutedEventArgs e)
		{
			presenteurContenu.Content = new UCPropositionsRecuesEnvoyees();
		}

		private void btnConversations_Click(object sender, RoutedEventArgs e)
		{
			presenteurContenu.Content = new UCConversation();
		}

		private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnParametres_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}

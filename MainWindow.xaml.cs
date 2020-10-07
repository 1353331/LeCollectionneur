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
		}

		private void btnCollections_Click(object sender, RoutedEventArgs e)
		{
			presenteurContenu.Content = new UCCollection();
		}

		private void btnAnnonces_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnPropositions_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnConversations_Click(object sender, RoutedEventArgs e)
		{

		}

		private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
		{

		}
	}
}

using LeCollectionneur.Modeles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour ContexteUtilisateur.xaml
    /// </summary>
    public partial class UCContexteUtilisateur : UserControl
    {
        public UCContexteUtilisateur()
        {
            InitializeComponent();
			presenteurContenu.Content = new UCCollection();
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
			UtilisateurADO gestionUser = new UtilisateurADO();
			gestionUser.Deconnection();
			// Restart de l'application permet de se login par MainWindow, ce qui permettra de se connecter en tant qu'admin si besoin.
			Process.Start(Application.ResourceAssembly.Location);
			Application.Current.Shutdown();
		}

		private void btnParametres_Click(object sender, RoutedEventArgs e)
		{
			presenteurContenu.Content = new UCParametre();
		}
	}
}

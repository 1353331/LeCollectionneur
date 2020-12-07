using LeCollectionneur.Modeles;
using LeCollectionneur.Outils.Messages;
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
		public static bool onConversation = false;
        public UCContexteUtilisateur()
        {
            InitializeComponent();
			presenteurContenu.Content = new UCCollection();
			modifierBackgroundBoutons(btnCollections);
        }

		private void btnCollections_Click(object sender, RoutedEventArgs e)
		{
			onConversation = false;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage()) ;
			presenteurContenu.Content = new UCCollection();
			modifierBackgroundBoutons(sender);
		}

		private void btnAnnonces_Click(object sender, RoutedEventArgs e)
		{
			onConversation = false;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage());
			presenteurContenu.Content = new UCAnnonce();
			modifierBackgroundBoutons(sender);
		}

		private void btnPropositions_Click(object sender, RoutedEventArgs e)
		{
			onConversation = false;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage());
			presenteurContenu.Content = new UCPropositionsRecuesEnvoyees();
			modifierBackgroundBoutons(sender);
		}

		private void btnConversations_Click(object sender, RoutedEventArgs e)
		{
			onConversation = true;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage());
			presenteurContenu.Content = new UCConversation();
			modifierBackgroundBoutons(sender);
		}

		private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
		{
			onConversation = false;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage());
			UtilisateurADO gestionUser = new UtilisateurADO();
			gestionUser.Deconnection();
			// Restart de l'application permet de se login par MainWindow, ce qui permettra de se connecter en tant qu'admin si besoin.
			Process.Start(Application.ResourceAssembly.Location);
			Application.Current.Shutdown();
		}

		private void btnParametres_Click(object sender, RoutedEventArgs e)
		{
			onConversation = false;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage());
			presenteurContenu.Content = new UCParametre();
			modifierBackgroundBoutons(sender);
		}

		private void modifierBackgroundBoutons(object sender)
		{
			Grid laGrid=(Grid)this.Content;
			Grid deuxiemeGrid = (Grid)laGrid.Children[0];
			foreach (Control control in deuxiemeGrid.Children )
			{
				if (control is Button)
				{
					Button bouton = control as Button;


					if (bouton != sender)
					{
						bouton.IsEnabled = true;
						bouton.ClearValue(BackgroundProperty);
					}
					else
					{
						bouton.IsEnabled = false;
						bouton.Background = new SolidColorBrush(Colors.Beige);
					}
				}
			}
		}

		
	}
}

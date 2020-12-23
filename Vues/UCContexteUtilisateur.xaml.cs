using LeCollectionneur.EF;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils.Messages;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
using System.Windows.Threading;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour ContexteUtilisateur.xaml
    /// </summary>
    public partial class UCContexteUtilisateur : UserControl
    {
			public static bool onConversation = false;
		public int pageAide = 19;

        public UCContexteUtilisateur()
        {
            InitializeComponent();
				presenteurContenu.Content = new UCCollection();
				modifierBackgroundBoutons(btnCollections);
				rafraichirNotifications();
		  }

		  ~UCContexteUtilisateur()
		  {
				timer.Stop();
		  }

		private void btnCollections_Click(object sender, RoutedEventArgs e)
		{
			onConversation = false;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage()) ;
			presenteurContenu.Content = new UCCollection();
			modifierBackgroundBoutons(sender);
			pageAide = 19;
		}

		private void btnAnnonces_Click(object sender, RoutedEventArgs e)
		{
			onConversation = false;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage());
			presenteurContenu.Content = new UCAnnonce();
			modifierBackgroundBoutons(sender);
			pageAide = 13;
		}

		private void btnPropositions_Click(object sender, RoutedEventArgs e)
		{
			onConversation = false;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage());
			presenteurContenu.Content = new UCPropositionsRecuesEnvoyees();
			modifierBackgroundBoutons(sender);
			pageAide = 21;
		}

		private void btnConversations_Click(object sender, RoutedEventArgs e)
		{
			onConversation = true;
			EvenementSysteme.Publier<EnvoyerThreadPropositionsMessage>(new EnvoyerThreadPropositionsMessage());
			presenteurContenu.Content = new UCConversation();
			modifierBackgroundBoutons(sender);
			pageAide = 8;
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
			pageAide = 12;
		}

		private void btnAide_Click(object sender, RoutedEventArgs e)
		{
			string fileName = System.IO.Path.GetFullPath("GuideUtilisateur.pdf");
			Process process = new Process();
			process.StartInfo.FileName = fileName;
			process.StartInfo.Arguments = "/A \"page=" + pageAide + "\" \"" + fileName + "\"";

			process.Start();
		}

		private void modifierBackgroundBoutons(object sender)
		{
			Grid laGrid=(Grid)this.Content;
			Grid deuxiemeGrid = (Grid)laGrid.Children[0];
			foreach (Object control in deuxiemeGrid.Children )
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


		private void btnChangerTheme_Click(object sender, RoutedEventArgs e)
		{
			UtilisateurADO.utilisateur.DarkMode = !(UtilisateurADO.utilisateur.DarkMode);
			UtilisateurADO.sauvegarderPreferenceTheme();
			((App)Application.Current).ChangementTheme();
		}

		#region Notifications
		DispatcherTimer timer = new DispatcherTimer();
		BackgroundWorker workerNotifications = new BackgroundWorker();
		private int nbPropositionNotif = 0;
		private int nbMessagesNotif = 0;

		private void rafraichirNotifications()
		{
			timer.Interval = new TimeSpan(0, 0, 1);
			timer.Tick += new EventHandler(rafraichirNotificationsThread);
			timer.Start();
		}

		private void rafraichirNotificationsThread(object sender, EventArgs e)
		{
			if (!workerNotifications.IsBusy)
			{
				workerNotifications = new BackgroundWorker();
				workerNotifications.RunWorkerCompleted += WorkerNotifications_RunWorkerCompleted;
				workerNotifications.DoWork += WorkerNotifications_DoWork;
				workerNotifications.RunWorkerAsync();
			}
		}

		private void WorkerNotifications_DoWork(object sender, DoWorkEventArgs e)
		{
			using (Context context = new Context())
			{
				nbPropositionNotif = context.Propositions.Include("AnnonceLiee.Annonceur")
																						.AsNoTracking()
																						.Where(p => !p.estVue && p.AnnonceLiee.Annonceur.Id == UtilisateurADO.utilisateur.Id)
																						.Count();

				List<Modeles.Conversation> conversations = context.Conversations.Include("Utilisateur1")
																						  .Include("Utilisateur2")
																						  .Include("ListMessage.Utilisateur")
																						  .AsNoTracking()
																						  .Where(c => c.Utilisateur1.Id == UtilisateurADO.utilisateur.Id || c.Utilisateur2.Id == UtilisateurADO.utilisateur.Id)
																						  .ToList();

				nbMessagesNotif = 0;

				foreach (Modeles.Conversation convo in conversations)
				{
					nbMessagesNotif += convo.ListMessage.Where(m => !m.vue && m.utilisateur.Id != UtilisateurADO.utilisateur.Id).Count();
				}
			}
		}

		private void WorkerNotifications_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
		{
			if (nbPropositionNotif > 0)
			{
				txbNotifProp.Text = nbPropositionNotif.ToString();
				brdNotifProp.Visibility = Visibility.Visible;
			}
			else
			{
				brdNotifProp.Visibility = Visibility.Hidden;
			}

			if (nbMessagesNotif > 0)
			{
				txbNotifMessages.Text = nbMessagesNotif.ToString();
				brdNotifMessages.Visibility = Visibility.Visible;
			}
			else
			{
				brdNotifMessages.Visibility = Visibility.Hidden;
			}
		}
		#endregion
	}
}

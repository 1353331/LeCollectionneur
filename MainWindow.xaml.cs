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
				if (!UtilisateurADO.admin)
				{
					
					this.Title = "Collectionneur - " + UtilisateurADO.utilisateur.NomUtilisateur;
					presenteurContenu.Content = new UCContexteUtilisateur();
				}
				else
					presenteurContenu.Content = new UCContexteAdmin();
			else
				this.Close();
		}
		

		
	}
}

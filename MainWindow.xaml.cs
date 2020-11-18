using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Vues;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
	public partial class MainWindow : Window , INotifyPropertyChanged
	{
		private string _nomUtilisateur;
		public string NomUtilisateur
		{
			get { return _nomUtilisateur; }
			set { _nomUtilisateur = value; OnPropertyChanged("NomUtilisateur"); }
		}
		public MainWindow()
		{
			InitializeComponent();
			BdBase MaBD = new BdBase();
			Window login = new Login();
			DataContext = this;
		login.ShowDialog();
			if (UtilisateurADO.utilisateur != null)
				if (!UtilisateurADO.admin)
				{
					
					NomUtilisateur = $"Le Collectionneur - { UtilisateurADO.utilisateur.NomUtilisateur}";
					presenteurContenu.Content = new UCContexteUtilisateur();
				}
				else
					presenteurContenu.Content = new UCContexteAdmin();
			else
				this.Close();
		}



		#region PropertyChanged
		public event PropertyChangedEventHandler PropertyChanged;
		private void OnPropertyChanged(string nomPropriete)
		{
			if (PropertyChanged != null)
				PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
		}
		#endregion

	}
}

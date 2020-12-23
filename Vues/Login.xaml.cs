using LeCollectionneur.EF;
using LeCollectionneur.Modeles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour Login.xaml
    /// </summary>
    public partial class Login : Window
    {
        public Login()
        {
            InitializeComponent();

            OutilEF outilEF = new OutilEF();
      }

        private void ButtonInscription(object sender, RoutedEventArgs e)
        {
            Inscription inscription = new Inscription();
            UtilisateurADO.connectionProf = false;
            
            inscription.ShowDialog();
            if (UtilisateurADO.utilisateur != null)
                this.Close();

        }

        private void btnConnectionProf_Click(object sender, RoutedEventArgs e)
        {
            UtilisateurADO temp = new UtilisateurADO();
            temp.connectionParId(1);
            UtilisateurADO.connectionProf = true;
            this.Close();
        }

        private void btnConnection_Click(object sender, RoutedEventArgs e)
        {
            UtilisateurADO temp = new UtilisateurADO();
            UtilisateurADO.connectionProf = false;
            if (temp.Connection(User.Text, MDP.Password))
                this.Close();
            else if (UtilisateurADO.utilisateur==null)
                MessageBox.Show("Champs invalide");
            else if (UtilisateurADO.utilisateur!=null)
            {
                MessageBox.Show("Votre compte a été désactivé par un administrateur");
                UtilisateurADO.utilisateur = null;
            }

        }

        private void btnConnectionAdmin_Click(object sender, RoutedEventArgs e)
        {
            UtilisateurADO temp = new UtilisateurADO();
            temp.Connection("admin","admin");
            this.Close();
        }

		private void btnConnectionProf2_Click(object sender, RoutedEventArgs e)
		{
            UtilisateurADO temp = new UtilisateurADO();
            temp.connectionParId(2);
            UtilisateurADO.connectionProf = true;
         this.Close();
      }


        private void btnAide_Click(object sender, RoutedEventArgs e)
        {
            int pageAide = 3;

            string fileName = System.IO.Path.GetFullPath("GuideUtilisateur.pdf");
            Process process = new Process();
            process.StartInfo.FileName = fileName;
            process.StartInfo.Arguments = "/A \"page=" + pageAide + "\" \"" + fileName + "\"";

            process.Start();
      }
    }
}

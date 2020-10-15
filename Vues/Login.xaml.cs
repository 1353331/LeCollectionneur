using LeCollectionneur.Modeles;
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
using System.Windows.Shapes;

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
        }

        private void ButtonInscription(object sender, RoutedEventArgs e)
        {
            Inscription inscription = new Inscription();
            inscription.ShowDialog();
        }

        private void btnConnectionProf_Click(object sender, RoutedEventArgs e)
        {
            UtilisateurADO temp = new UtilisateurADO();
            temp.Connection("collectionneur1","collectionneur");
            this.Close();
        }

        private void btnConnection_Click(object sender, RoutedEventArgs e)
        {
            UtilisateurADO temp = new UtilisateurADO();
            if (temp.Connection(User.Text, MDP.Text))
                this.Close();
            else
                MessageBox.Show("Champs invalide");

        }
    }
}

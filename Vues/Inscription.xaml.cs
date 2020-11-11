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
    /// Logique d'interaction pour Inscription.xaml
    /// </summary>
    public partial class Inscription : Window
    {
        public Inscription()
        {
            InitializeComponent();
        }

        private void btnIncription_Click(object sender, RoutedEventArgs e)
        {
            UtilisateurADO user = new UtilisateurADO();
            if (user.CreerCompte(tbNomUtilisateur.Text, tbMP.Text, tbMPC.Text, tbCourriel.Text))
                this.Close();
            
            else
                MessageBox.Show("Erreur un champ est Invalide");
            
            

        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}

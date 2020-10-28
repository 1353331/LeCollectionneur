using LeCollectionneur.Modeles;
using System;
using System.Collections.Generic;
using System.Data;
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
    /// Logique d'interaction pour ajouterConversation.xaml
    /// </summary>
    public partial class ajouterConversation : Window
    {
        public ajouterConversation()
        {
            InitializeComponent();
            miseAJour();
        }

        private void btnRecherche_Click(object sender, RoutedEventArgs e)
        {
            miseAJour(UtilisateurADO.getAllUtilisateur(txbRecherche.Text));
        }
        private void miseAJour()
        {
            lstUser.Items.Clear();
            var temp = UtilisateurADO.getAllUtilisateur();
            foreach (var item in temp)
            {
                lstUser.Items.Add(new Utilisateur(item.Id, item.NomUtilisateur, item.Courriel));
            }
        }
        private void miseAJour(List<Utilisateur> temp)
        {
            lstUser.Items.Clear();
            foreach (var item in temp)
            {
                lstUser.Items.Add(new Utilisateur(item.Id,item.NomUtilisateur,item.Courriel));
            }
        }
        private void btnAjoutConversation_Click(object sender, RoutedEventArgs e)
        {
            var utlisateur = (Utilisateur)lstUser.SelectedItem;
            ConversationADO.ChercherIdConversation(UtilisateurADO.utilisateur, utlisateur);
            this.Close();   
        }
    }
}

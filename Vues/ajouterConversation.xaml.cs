using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
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
            var AllUser = UtilisateurADO.getAllUtilisateur();
            var AllConvo = new ConversationADO().RecupererConversationUtilisateur();
            var e = UtilisateurADO.utilisateur.Id;
            foreach (var user in AllUser)
            {
                if(!ConversationADO.HasConversation(user.Id))
                    lstUser.Items.Add(new Utilisateur(user.Id, user.NomUtilisateur, user.Courriel));
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
        
        
        //SELECT * FROM `conversations` WHERE (Utilisateur1_Id = 1 OR Utilisateur2_Id =1) AND NOT(Utilisateur1_Id = 2 )AND NOT(Utilisateur2_Id = 3)
    }
}

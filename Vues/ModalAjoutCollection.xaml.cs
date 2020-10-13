using LeCollectionneur.Modeles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Logique d'interaction pour ModalAjoutCollection.xaml
    /// </summary>
    public partial class ModalAjoutCollection : Window
    {
        CollectionADO gestionnaireCollections = new CollectionADO();
        public Utilisateur UtilisateurConnecte { get; set; }
        public ModalAjoutCollection( Utilisateur utilisateur)
        {
            InitializeComponent();
            // L'utilisateur est passé en paramètre, on peut lui ajouter une collection.
            UtilisateurConnecte = utilisateur;
            this.ResizeMode = ResizeMode.NoResize;
            
        }

        private void btnAjouterCollection_Click(object sender, RoutedEventArgs e)
        {
            if (txtNomCollection.Text.Trim().Length >0 && txtNomCollection.Text.Trim().Length<51)
            {
                Collection neoCollection = new Collection();
                neoCollection.Nom = Echappement(txtNomCollection.Text.Trim());
                gestionnaireCollections.Ajouter(neoCollection, UtilisateurConnecte.Id);             
                this.Close();
            }
            else
            {
                // Ajouter un message d'erreur dans la fenêtre.
                lblErreur.Content = "La longueur du nom doit être entre 1 et 50 caractères.";
            }
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private string Echappement(string entree)
        {
            string modif="";
            for (int i = 0; i < entree.Length; i++)
            {
                if (entree[i] == '\'' || entree[i]==';')
                {
                    modif += "\\";
                }
                modif += entree[i];
            }
            return modif;
        }
    }
}

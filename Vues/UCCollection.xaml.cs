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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour UCCollection.xaml
    /// </summary>
    public partial class UCCollection : UserControl
    {
        private CollectionADO GestionCollections;
        public UCCollection()
        {
            InitializeComponent();
            
        }

        private void verifierChiffre(object sender, TextChangedEventArgs e)
        {
            // Effacer les caractères qui ne seront pas des chiffres si la string n'est pas à 0 de longueur.
            if(txtQuantiteItem.Text.Length>0)
            {
                if (txtQuantiteItem.Text[txtQuantiteItem.Text.Length - 1] < '0' || txtQuantiteItem.Text[txtQuantiteItem.Text.Length - 1] > '9')
                {
                    txtQuantiteItem.Text = txtQuantiteItem.Text.Remove(txtQuantiteItem.Text.Length - 1, 1);
                    // Remettre l'insertion à la dernière position de la string.
                    txtQuantiteItem.CaretIndex = txtQuantiteItem.Text.Length;
                }
            }
        }
    }
}

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
using LeCollectionneur.Modeles;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour ModalAjoutAnnonce.xaml
    /// </summary>
    public partial class ModalAjoutAnnonce : Window
    {
        public ModalAjoutAnnonce()
        {
            InitializeComponent();
 
        }

        private void ListView_SelectItem(object sender, MouseButtonEventArgs e)
        {

        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void btnPublier_Click(object sender, RoutedEventArgs e)
        {
            Annonce NeoAnnonce = new Annonce();
            NeoAnnonce.Titre = txtTitre.Text;
            NeoAnnonce.Montant = Convert.ToDouble(txtMontant.Text);
            NeoAnnonce.DatePublication = DateTime.Now.Date;
            NeoAnnonce.Type = cmbType.Text;
            NeoAnnonce.Description = txtDescription.Text;
            AnnonceADO annonceADO = new AnnonceADO();
            annonceADO.Ajouter(NeoAnnonce);

            this.Close();
        }
    }
}

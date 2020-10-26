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
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.VuesModeles;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour ModalAjoutAnnonce.xaml
    /// </summary>
    public partial class ModalAjoutAnnonce : Window, IFenetreFermeable, IOuvreModal
    {
        public ModalAjoutAnnonce()
        {
            InitializeComponent();
            DataContext = new ModalAjoutAnnonce_VM();
        }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Fermer()
        {
            this.Close();
        }

        public void OuvrirModal()
        {
            ModalAjoutItemAnnonce viewProp = new ModalAjoutItemAnnonce();
            viewProp.Owner = Window.GetWindow(this);
            viewProp.ShowDialog();
        }
    }
}

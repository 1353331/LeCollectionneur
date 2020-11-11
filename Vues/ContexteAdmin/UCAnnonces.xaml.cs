using LeCollectionneur.Modeles;
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.VuesModeles.ContexteAdmin;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeCollectionneur.Vues.ContexteAdmin
{
    /// <summary>
    /// Logique d'interaction pour Annonces.xaml
    /// </summary>
    public partial class UCAnnonces : UserControl, IOuvreModalAvecParametre<Item>
    {
        public UCAnnonces()
        {
            InitializeComponent();
            DataContext = new AnnonceAdmin_VM();
        }

        public void OuvrirModal(Item item)
        {
            ModalDetailsItem viewProp = new ModalDetailsItem(item);
            viewProp.Owner = Window.GetWindow(this);
            viewProp.Show();
        }

    }
}

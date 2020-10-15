using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.VuesModeles;
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
    /// Logique d'interaction pour ModalDeplacementItem.xaml
    /// </summary>
    public partial class ModalDeplacementItem : Window, IFenetreFermeable
    {
        public ModalDeplacementItem(ObservableCollection<Collection> CollectionsDisponibles, Item itemADeplacer)
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            DataContext = new DeplacementItem_VM(CollectionsDisponibles,itemADeplacer);
        }

        public void Fermer()
        {
            this.Close();
        }
    }
}

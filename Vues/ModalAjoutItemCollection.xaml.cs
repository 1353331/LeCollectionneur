using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.VuesModeles;
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
    /// Logique d'interaction pour ModalAjoutItemCollection.xaml
    /// </summary>
    public partial class ModalAjoutItemCollection : Window, IFenetreFermeable
    {
        
        public ModalAjoutItemCollection(Collection CollectionItem)
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            DataContext = new AjoutItemCollection_VM(CollectionItem);
        }
        //Implémentation de l'interface IFenetreFermeable, qui permet de fermer une fenêtre à partir du ViewModel et de respecter le modèle MVVM
        public void Fermer()
        {
            this.Close();
        }
    }
}

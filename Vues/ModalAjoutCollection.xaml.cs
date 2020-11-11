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
    /// Logique d'interaction pour ModalAjoutCollection.xaml
    /// </summary>
    public partial class ModalAjoutCollection : Window, IFenetreFermeable
    {
       
        public ModalAjoutCollection()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
            DataContext = new AjoutCollection_VM();
            
        }

        //Implémentation de l'interface IFenetreFermeable, qui permet de fermer une fenêtre à partir du ViewModel et de respecter le modèle MVVM
        public void Fermer()
        {
            this.Close();
        }
    }
}

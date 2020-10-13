using LeCollectionneur.Modeles;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour UCCollection.xaml
    /// </summary>
    public partial class UCCollection : UserControl
    {
        
        public UCCollection()
        {
            InitializeComponent();
            DataContext = new Collection_VM();
        }

        
    }
}

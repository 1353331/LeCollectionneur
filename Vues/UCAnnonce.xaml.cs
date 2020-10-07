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

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour UCAnnonce.xaml
    /// </summary>
    public partial class UCAnnonce : UserControl
    {
        public UCAnnonce()
        {
            InitializeComponent();
        }

        private void ListView_SelectItem(object sender, MouseButtonEventArgs e)
        {

        }

        private void ListView_SelectAnnonce(object sender, MouseButtonEventArgs e)
        {

        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            btnPropAnnonce.Visibility = Visibility.Hidden;
            btnModAnnonce.Visibility = Visibility.Visible;

        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            btnPropAnnonce.Visibility = Visibility.Visible;
            btnModAnnonce.Visibility = Visibility.Hidden;
        }

        private void btnAjoutAnnonce_Click(object sender, RoutedEventArgs e)
        {
            ModalAjoutAnnonce viewAjoutAnnonce = new ModalAjoutAnnonce();
            viewAjoutAnnonce.Show();
        }
    }
}

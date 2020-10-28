using LeCollectionneur.Modeles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    /// Logique d'interaction pour ContexteAdmin.xaml
    /// </summary>
    public partial class UCContexteAdmin : UserControl
    {
        public UCContexteAdmin()
        {
            InitializeComponent();
        }

        private void btnUtilisateurs_Click(object sender, RoutedEventArgs e)
        {
            presenteurContenu.Content = new ContexteAdmin.UCUtilisateurs();
        }

        private void btnAnnonces_Click(object sender, RoutedEventArgs e)
        {
            presenteurContenu.Content = new ContexteAdmin.UCAnnonces();
        }

        private void btnDeconnexion_Click(object sender, RoutedEventArgs e)
        {
            UtilisateurADO gestionUser = new UtilisateurADO();
            gestionUser.Deconnection();
            // Restart de l'application permet de se login par MainWindow, ce qui permettra de se connecter en tant qu'admin si besoin.
            Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }
    }
}

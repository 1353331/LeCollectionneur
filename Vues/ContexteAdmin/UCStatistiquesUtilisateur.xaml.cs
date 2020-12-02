using LeCollectionneur.EF;
using LeCollectionneur.Modeles;
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
    /// Logique d'interaction pour UCStatistiquesUtilisateur.xaml
    /// </summary>
    public partial class UCStatistiquesUtilisateur : UserControl
    {
      
		
        public UCStatistiquesUtilisateur(Utilisateur utilisateur)
        {
            InitializeComponent();
			DataContext = new StatistiquesUtilisateur_VM(utilisateur);
		}
		
    }
}

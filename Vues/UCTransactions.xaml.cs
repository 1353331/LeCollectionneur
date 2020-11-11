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
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.VuesModeles;

namespace LeCollectionneur.Vues
{
	/// <summary>
	/// Logique d'interaction pour UCTransactions.xaml
	/// </summary>
	public partial class UCTransactions : UserControl, IOuvreModalAvecParametre<Item>
	{
		public UCTransactions()
		{
			InitializeComponent();
			DataContext = new Transactions_VM();
		}

		public void OuvrirModal(Item itemSelectionne)
		{
			ModalDetailsItem modalDetails = new ModalDetailsItem(itemSelectionne);
			modalDetails.Owner = Window.GetWindow(this);
			modalDetails.ShowDialog();
		}
	}
}

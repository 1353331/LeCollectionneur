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
using LeCollectionneur.VuesModeles;

namespace LeCollectionneur.Vues
{
	/// <summary>
	/// Logique d'interaction pour ModalAjoutItemProposition.xaml
	/// </summary>
	public partial class ModalAjoutItemProposition : Window
	{
		public ModalAjoutItemProposition(IEnumerable<Item> itemsAjoutes)
		{
			InitializeComponent();
			DataContext = new ModalAjoutItem_VM(itemsAjoutes);
		}
	}
}

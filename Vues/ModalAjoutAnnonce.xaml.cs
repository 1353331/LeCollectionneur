using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.VuesModeles;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour ModalAjoutAnnonce.xaml
    /// </summary>
    public partial class ModalAjoutAnnonce : Window, IFenetreFermeable, IOuvreModalAvecParametre<IEnumerable<Item>>, IOuvreModalAvecParametre<Item>
    {
        private static readonly Regex _regexMontant = new Regex("[^0-9.]+");
        private static bool texteMontantCorrect(string texte)
        {
            return !_regexMontant.IsMatch(texte);
        }

        public ModalAjoutAnnonce()
        {
            InitializeComponent();
            ModalAjoutAnnonce_VM VM = new ModalAjoutAnnonce_VM();
            DataContext = VM;
            Closing += VM.cmdFermer;   
      }

        private void btnAnnuler_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public void Fermer()
        {
            this.Close();
        }

        // Pour empêcher l'entrée de caractères autre que des nombres
        private void tbxMontant_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !texteMontantCorrect(e.Text);
        }

        public void OuvrirModal(Item item)
        {
            ModalDetailsItem viewProp = new ModalDetailsItem(item);
            viewProp.Owner = Window.GetWindow(this);
            viewProp.Show();
        }

		  public void OuvrirModal(IEnumerable<Item> itemsAjoutes)
		  {
            ModalAjoutItemAnnonce viewProp = new ModalAjoutItemAnnonce(itemsAjoutes);
            viewProp.Owner = Window.GetWindow(this);
            viewProp.ShowDialog();
        }
	}
}

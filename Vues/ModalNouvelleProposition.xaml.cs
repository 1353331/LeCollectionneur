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
	/// Logique d'interaction pour ModalNouvelleProposition.xaml
	/// </summary>
	public partial class ModalNouvelleProposition : Window, IFenetreFermeable, IOuvreModalAvecParametre<IEnumerable<Item>>, IOuvreModalAvecParametre<Item>, IOuvreModalAvecParametre<Utilisateur>
	{
		private static readonly Regex _regexMontant = new Regex("[^0-9.]+");
		private static bool texteMontantCorrect(string texte)
		{
			return !_regexMontant.IsMatch(texte);
		}
		public ModalNouvelleProposition(Annonce annonceLiee)
		{
			InitializeComponent();
			//Passer le paramètre au ViewModel
		   ModalNouvelleProposition_VM VM = new ModalNouvelleProposition_VM(annonceLiee);
			DataContext = VM;
			Closing += VM.cmdFermer;
		}

		//Implémentation de l'interface IFenetreFermeable, qui permet de fermer une fenêtre à partir du ViewModel et de respecter le modèle MVVM
		public void Fermer()
		{
			this.Close();
		}

		// Pour empêcher l'entrée de caractères autre que des nombres
		private void tbxMontantProposition_PreviewTextInput(object sender, TextCompositionEventArgs e)
		{
			e.Handled = !texteMontantCorrect(e.Text);
		}

		public void OuvrirModal(IEnumerable<Item> itemsAjoutes)
		{
			ModalAjoutItemAnnonce viewProp = new ModalAjoutItemAnnonce(itemsAjoutes);
			viewProp.Owner = Window.GetWindow(this);
			viewProp.ShowDialog();
		}

		public void OuvrirModal(Item itemSelectionne)
		{
			ModalDetailsItem modalDetails = new ModalDetailsItem(itemSelectionne);
			modalDetails.Owner = Window.GetWindow(this);
			modalDetails.ShowDialog();
		}

		public void OuvrirModal(Utilisateur destinataire)
		{
			ModalEnvoyerMessage modalMessage = new ModalEnvoyerMessage(destinataire);
			modalMessage.Owner = Window.GetWindow(this);
			modalMessage.ShowDialog();
		}
	}
}

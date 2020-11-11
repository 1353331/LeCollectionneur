using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Interfaces;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour UCAnnonce.xaml
    /// </summary>
    public partial class UCAnnonce : UserControl, IOuvreModal, IOuvreModalAvecChoixEtParam<Annonce>, IOuvreModalAvecParametre<Item>, IOuvreModalAvecParametre<Utilisateur>
   {
        public UCAnnonce()
        {
            InitializeComponent();
            DataContext = new Annonce_VM();
        }

        public void OuvrirModal()
        {
            ModalAjoutAnnonce viewProp = new ModalAjoutAnnonce();
            viewProp.Owner = Window.GetWindow(this);
            viewProp.ShowDialog();
        }

        public void OuvrirModal(Annonce annonce, string nom)
        {
            switch (nom)
            {
                case "proposition":
                    ModalNouvelleProposition viewProp = new ModalNouvelleProposition(annonce);
                    viewProp.Owner = Window.GetWindow(this);
                    viewProp.Show();
                    break;

                case "modifier":
                    ModalModifierAnnonce viewMod = new ModalModifierAnnonce(annonce);
                    viewMod.Owner = Window.GetWindow(this);
                    viewMod.ShowDialog();
                    break;
            }
        }

        public void OuvrirModal(Item item)
        {
            ModalDetailsItem viewProp = new ModalDetailsItem(item);
            viewProp.Owner = Window.GetWindow(this);
            viewProp.Show();
        }

        public void OuvrirModal(Utilisateur destinataire)
        {
           ModalEnvoyerMessage modalMessage = new ModalEnvoyerMessage(destinataire);
           modalMessage.Owner = Window.GetWindow(this);
           modalMessage.ShowDialog();
        }
   }
}

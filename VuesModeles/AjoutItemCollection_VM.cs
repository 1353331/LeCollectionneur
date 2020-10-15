using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LeCollectionneur.VuesModeles
{
    class AjoutItemCollection_VM : INotifyPropertyChanged
    {
        public ObservableCollection<string> ConditionsPossibles { get; set; }
        public ObservableCollection<string> TypesPossibles { get; set; }
        public Collection CollectionAjoutItem { get; set; }
        private Item _itemAjout;
        public Item ItemAjout
        {
            get
            {
                return _itemAjout;
            }
            set
            {
                _itemAjout = value;
                OnPropertyChanged("ItemAjout");
            }
        }
        public string LblErreur { get; set; }
        private string _nomFichier;
        public string NomFichier 
        { get
            {
                return _nomFichier;
            }
            set
            {
                _nomFichier = value;
                OnPropertyChanged("NomFichier");
            }
        }
        private ItemADO gestionnaireItem = new ItemADO();
        #region Commandes
        // Collection
        private ICommand _cmdAjouterItem;
        public ICommand cmdAjouterItem
        {
            get
            {
                return _cmdAjouterItem;
            }
            set
            {
                _cmdAjouterItem = value;
                OnPropertyChanged("cmdAjouterItem");
            }
        }
        private void cmdAjouter_Item(object param)
        {
            IFenetreFermeable fenetre = param as IFenetreFermeable;
            
            if (ItemAjout.Nom.Length>0 &&ItemAjout.Type.Length>0&&ItemAjout.Condition.Length>0)
            {
                if (NomFichier.Length > 0)
                    Fichier.TeleverserFichierFTP(1, NomFichier);
                gestionnaireItem.Ajouter(ItemAjout, CollectionAjoutItem);
                fenetre.Fermer();
            }

        }
        private ICommand _cmdAjouterImage;
        public ICommand cmdAjouterImage
        {
            get
            {
                return _cmdAjouterImage;
            }
            set
            {
                _cmdAjouterImage = value;
                OnPropertyChanged("cmdjouterImage");
            }
        }
        private void cmdAjouter_Image(object param)
        {
            NomFichier=Fichier.ImporterFichier();
        }

        private ICommand _cmdAnnuler;
        public ICommand cmdAnnuler
        {
            get
            {
                return _cmdAnnuler;
            }
            set
            {
                _cmdAnnuler = value;
                OnPropertyChanged("cmdAnnuler");
            }
        }
        private void cmd_Annuler(object param)
        {
            IFenetreFermeable fenetre = param as IFenetreFermeable;
            fenetre.Fermer();
        }
        #endregion
        public AjoutItemCollection_VM(Collection laCollection)
        {
            LblErreur = "";
            cmdAjouterItem = new Commande(cmdAjouter_Item);
            cmdAnnuler = new Commande(cmd_Annuler);
            cmdAjouterImage = new Commande(cmdAjouter_Image);
            CollectionAjoutItem = laCollection;
            ItemAjout = new Item();
            ConditionsPossibles = gestionnaireItem.ConditionsPossibles;
            TypesPossibles = gestionnaireItem.TypesPossibles;
            NomFichier = "";
        }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        #endregion

    }
}

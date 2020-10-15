using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LeCollectionneur.VuesModeles
{
    class DeplacementItem_VM : INotifyPropertyChanged
    {
        #region Propriétés
        public ItemADO gestionnaireItems = new ItemADO(); 
        public Item ItemDeplace { get; set; }
        public ObservableCollection<Collection> LstCollections { get; set; }
        private Collection _collectionSelectionnee;
        public Collection CollectionSelectionnee
        {
            get
            {
                return _collectionSelectionnee;
            }
            set
            {
                _collectionSelectionnee = value;
                OnPropertyChanged("CollectionSelectionnee");
                cmdDeplacerItem = new Commande(cmdDeplacer_Item, UneCollectionSelectionnee);
            }
        }  
        #endregion

        #region Commandes
        // Collection
        private ICommand _cmdDeplacerItem;
        public ICommand cmdDeplacerItem
        {
            get
            {
                return _cmdDeplacerItem;
            }
            set
            {
                _cmdDeplacerItem = value;
                OnPropertyChanged("cmdDeplacerItem");
            }
        }
        private void cmdDeplacer_Item(object param)
        {
            IFenetreFermeable fenetre = param as IFenetreFermeable;
            gestionnaireItems.TransfererItem(CollectionSelectionnee, ItemDeplace);
            fenetre.Fermer();
            MessageBox.Show($"L'item: {ItemDeplace.Nom} a été déplacé dans la collection: {CollectionSelectionnee.Nom}");

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
        #region Constructeur
        public DeplacementItem_VM(ObservableCollection<Collection> collectionsDispo,Item itemDeplacement)
        {
            LstCollections = collectionsDispo;
            ItemDeplace = itemDeplacement;
            cmdDeplacerItem = new Commande(cmdDeplacer_Item, UneCollectionSelectionnee);
            cmdAnnuler = new Commande(cmd_Annuler);
        }
        #endregion
        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        #endregion
        #region Méthodes
        public bool UneCollectionSelectionnee()
        {
            return !(CollectionSelectionnee is null);
        }
        #endregion

    }
}

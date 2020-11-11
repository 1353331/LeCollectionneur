using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Messages;
using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace LeCollectionneur.VuesModeles
{
    class AjoutItemAnnonce_VM : INotifyPropertyChanged
    {
        //Variables pour l'item sélectionné
        private Item _itemSelectionne;
        public Item ItemSelectionne
        {
            get { return _itemSelectionne; }
            set
            {
                _itemSelectionne = value;
                OnPropertyChanged("ItemSelectionne");

                //Puisqu'il y a un item de sélectionné, alors on veut pouvoir exécuter la commande d'ajout d'un item
                //cmdAjouter_Item = new Commande(cmdAjouter, UnItemSelectionne);
            }
        }

        //Les items sélectionnés dans la liste des items de la nouvelle annonce
        private IList _itemsSelectionnes;
        public IList ItemsSelectionnes
        {
            get { return _itemsSelectionnes; }
            set
            {
                _itemsSelectionnes = value;
                cmdAjouter_Item = new Commande(cmdAjouter, UnItemSelectionne);
                OnPropertyChanged("ItemsSelectionnes");
            }
        }

        //Liste des collections de l'utilisateur connecté
        public ObservableCollection<Collection> lstCollections { get; set; }

        //Variable de la collection sélectionnée
        private Collection _collectionSelectionnee;
        public Collection CollectionSelectionnee
        {
            get { return _collectionSelectionnee; }
            set
            {
                ItemsSelectionnes = null;
                _collectionSelectionnee = value;
                ItemsCollectionSelectionnee = _collectionSelectionnee.ItemsCollection;
            }
        }

        //Variable pour l'item collection sélectionné
        private ObservableCollection<Item> _itemsCollectionSelectionnee;
        public ObservableCollection<Item> ItemsCollectionSelectionnee
        {
            get { return _itemsCollectionSelectionnee; }
            set
            {
                _itemsCollectionSelectionnee = value;
                cmdAjouter_Item = new Commande(cmdAjouter, UnItemSelectionne);
                OnPropertyChanged("ItemsCollectionSelectionnee");
            }
        }

        //Variable pour la commande d'ajout d'un item
        private ICommand _cmdAjouter_Item;
        public ICommand cmdAjouter_Item
        {
            get { return _cmdAjouter_Item; }
            set
            {
                _cmdAjouter_Item = value;
                OnPropertyChanged("cmdAjouter_Item");
            }
        }

        //Constructeur
        public AjoutItemAnnonce_VM()
        {
            cmdAjouter_Item = new Commande(cmdAjouter, UnItemSelectionne);

            lstCollections = new CollectionADO().Recuperer(UtilisateurADO.utilisateur.Id);
        }

        //Méthode pour savoir si un item est sélectionné ou non
        public bool UnItemSelectionne()
        {
            if (ItemsSelectionnes is null)
            {
                return false;
            }
            if (ItemsSelectionnes.Count == 0 )
            {
                return false;
            }
            return true;
        }

        //Méthode pour l'ajout d'un item
        private void cmdAjouter(object param)
        {
            //Envoie l'évènement d'ajout d'item avec l'item sélectionné pour qu'il soit reçu par ModalNouvelleAnnonce_VM
            //EvenementSysteme.Publier<EnvoyerItemMessage>(new EnvoyerItemMessage() { Item = ItemSelectionne });
            EnvoyerItemsMessage EIM = new EnvoyerItemsMessage();
            EvenementSysteme.Publier<EnvoyerItemsMessage>(new EnvoyerItemsMessage() { Items = EIM.ConvertirIListEnObservColl(ItemsSelectionnes) });
        }

        #region OnPropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        #endregion

    }
}

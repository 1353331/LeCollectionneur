using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Vues;
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
    class Collection_VM :INotifyPropertyChanged
    {
        #region Propriétés
        private CollectionADO gestionnaireCollections = new CollectionADO();
        private ItemADO gestionnaireItems = new ItemADO();
        private ObservableCollection<Collection> _mesCollections;
        public ObservableCollection<Collection> MesCollections
        {
            get { return _mesCollections; }
            set
            {
                // Hardcode de l'utilisateur 1 pour l'instant.
                _mesCollections = gestionnaireCollections.Recuperer(1);
                OnPropertyChanged("MesCollections");
            }
        }

        private Collection _collectionSelectionnee;
        public Collection CollectionSelectionnee
        {
            get { return _collectionSelectionnee; }
            set
            {   // Changer la liste des items.
                _collectionSelectionnee = value;
                if (_collectionSelectionnee is null)
                    ItemSelectionne = null;
               cmdSupprimerCollection=new Commande(cmdSupprimer_Collection,UneCollectionSelectionnee);
               cmdModifierCollection=new Commande(cmdModifier_Collection,UneCollectionSelectionnee);
                cmdAjouterItem= new Commande(cmdAjouter_Item,UneCollectionSelectionnee);     
                OnPropertyChanged("CollectionSelectionnee");
               

            }
        }

        private Item _itemSelectionne;

        

        public Item ItemSelectionne
        {
            get { return _itemSelectionne; }
            set
            {
                // Changer les champs de l'item sélectionné
                _itemSelectionne = value;

                cmdSupprimerItem = new Commande(cmdSupprimer_Item,UnItemSelectionne);
                 cmdDeplacerItem= new Commande(cmdSupprimer_Item, UnItemSelectionne);
                cmdModifierItem = new Commande(cmdModifier_Item, UnItemSelectionne);
                OnPropertyChanged("ItemSelectionne");
            }
        }

        public ObservableCollection<string> ConditionsPossibles { get; set; }
        public ObservableCollection<string> TypesPossibles { get; set; }

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
                private void OnPropertyChanged(string nomPropriete)
                {
                    if (PropertyChanged != null)
                        PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
                }
        #endregion
        #endregion

        #region Commandes
        // Collection
        private ICommand _cmdAjouterCollection;
        public ICommand cmdAjouterCollection {
            get
            {
                return _cmdAjouterCollection;
            }
            set
            {
                _cmdAjouterCollection = value;
                OnPropertyChanged("cmdAjouterCollection");
            }
        }
        private void cmdAjouter_Collection(object param)
        {
            int avantModale = MesCollections.Count();
            // Ajouter_Collection permet de créer une nouvelle collection (ouvre un pop-up avec un champ nom et créer/annuler comme choix)
            Window modale = new ModalAjoutCollection(new Utilisateur(1,"",""));
            
            modale.ShowDialog();
            initialisationEcran(false);
            if (avantModale < MesCollections.Count())
                CollectionSelectionnee = MesCollections[MesCollections.Count() - 1];
            
        }
        private ICommand _cmdModifierCollection;
        public ICommand cmdModifierCollection
        {
            get
            {
                return _cmdModifierCollection;
            }
            set
            {
                _cmdModifierCollection = value;
                OnPropertyChanged("cmdModifierCollection");
            }
        }
        private void cmdModifier_Collection(object param)
        {
            // Modifier_Collection permet de modifier le nom de la collection dans un pop-up.
        }
        private ICommand _cmdSupprimerCollection;
        public ICommand cmdSupprimerCollection
        {
            get
            {
                return _cmdSupprimerCollection;
            }
            set
            {
                _cmdSupprimerCollection = value;
                OnPropertyChanged("cmdSupprimerCollection");
            }
        }
        private void cmdSupprimer_Collection(object param)
        {
            //Supprimer_Collection permet de supprimer cette collection de l'utilisateur en question, cela entraine également la suppression des objets de la collection
            // TODO: Pop-up de confirmation

            gestionnaireCollections.Supprimer(CollectionSelectionnee.Id);
            initialisationEcran(false);
        }
        // Item
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
            //Ajouter_Item permet d'ajouter un item à cette collection.
            // Un pop-up est ouvert, on peut rechercher l'item, s'il n'existe pas, on l'ajoute en BD.
        }
        private ICommand _cmdModifierItem;
        public ICommand cmdModifierItem
        {
            get
            {
                return _cmdModifierItem;
            }
            set
            {
                _cmdModifierItem = value;
                OnPropertyChanged("cmdModifierItem");
            }
        }
        private void cmdModifier_Item(object param)
        {
            // cmdModifier_Item permet de modifier la quantite et la condition de l'item
        }
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
            // Deplacer_Item permet de changer l'item de Collection
        }
        private ICommand _cmdSupprimerItem;
        public ICommand cmdSupprimerItem
        {
            get
            {
                return _cmdSupprimerItem;
            }
            set
            {
                _cmdSupprimerItem = value;
                OnPropertyChanged("cmdSupprimerItem");
            }
        }
        private void cmdSupprimer_Item(object param)
        {
            // Supprimer_Item enlève l'Item de la collection courante.
            gestionnaireItems.Supprimer(ItemSelectionne.Id);
            CollectionSelectionnee.ItemsCollection.Remove(ItemSelectionne);
            ItemSelectionne = null;
            //initialisationEcran(false); Rend la collection Selectionnee null.
            CollectionSelectionnee = ReselectionnerCollection();
            

        }
        #endregion

        #region Constructeurs
        public Collection_VM()
        {
            cmdAjouterCollection = new Commande(cmdAjouter_Collection,()=> { return true; });
            cmdModifierCollection = new Commande(cmdModifier_Collection, UneCollectionSelectionnee);
            cmdSupprimerCollection = new Commande(cmdSupprimer_Collection, UneCollectionSelectionnee);
            cmdAjouterItem = new Commande(cmdAjouter_Item, UneCollectionSelectionnee);
            cmdModifierItem = new Commande(cmdModifier_Item,UnItemSelectionne);
            cmdDeplacerItem = new Commande(cmdDeplacer_Item, UnItemSelectionne);
            cmdSupprimerItem = new Commande(cmdSupprimer_Item, UnItemSelectionne);
            initialisationEcran();
        }

        private void initialisationEcran(bool estPremierChargement = true)
        {
            // Récupère toutes les collections de l'utilisateur courant et les rempli avec leurs items respectifs.
            MesCollections = new ObservableCollection<Collection>();
            // Récupérer tous les états possibles d'items.
            if (estPremierChargement)
            {
                ConditionsPossibles = gestionnaireItems.RecupererListeConditions();
                TypesPossibles = gestionnaireItems.RecupererListeTypes();
            }
            
        }
        #endregion

        #region Méthodes
        public bool UnItemSelectionne()
        {
            return !(ItemSelectionne is null);
        }
        public bool UneCollectionSelectionnee()
        {
            return !(CollectionSelectionnee is null);
        }

        public Collection ReselectionnerCollection()
        {
            foreach (Collection c in MesCollections)
            {
                if (c.Id == CollectionSelectionnee.Id)
                    return c;
            }
            return null;
        }
        #endregion
    }
}

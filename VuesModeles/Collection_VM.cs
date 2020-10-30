using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Vues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;

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
                UtilisateurADO.collection = gestionnaireCollections.Recuperer(UtilisateurADO.utilisateur.Id);
                _mesCollections = UtilisateurADO.collection;
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
                if (_itemSelectionne is null)
                    ViderChamps();
                else
                    RemplirChamps();
                cmdSupprimerItem = new Commande(cmdSupprimer_Item,UnItemSelectionne);
                 cmdDeplacerItem= new Commande(cmdDeplacer_Item, UnItemSelectionne);
                cmdModifierItem = new Commande(cmdModifier_Item, UnItemSelectionne);
                cmdAjouterImage = new Commande(cmdAjouter_Image, UnItemSelectionne);
                NomFichier = null;
                ChangerImageItem();
                OnPropertyChanged("ItemSelectionne");
            }
        }
        private string _nom;
        public string Nom 
        {
            get { return _nom; }
            set 
            {
                _nom = value;
                OnPropertyChanged("Nom");
            } 
        }
        private string _condition;
        public string Condition
        {
            get { return _condition; }
            set
            {
                _condition = value;
                OnPropertyChanged("Condition");
            }
        }
        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
               if (string.IsNullOrWhiteSpace(value))
                  _description = "";
               else
                  _description = value;

                OnPropertyChanged("Description");
            }
        }
        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                OnPropertyChanged("Type");
            }
        }
        private DateTime? _dateSortie;
        public DateTime? DateSortie
        {
            get { return _dateSortie; }
            set
            {
                _dateSortie = value;
                OnPropertyChanged("DateSortie");
            }
        }
        private string _manufacturier;
        public string Manufacturier
        {
            get { return _manufacturier; }
            set
            {
               if (string.IsNullOrWhiteSpace(value))
                  _manufacturier = "";
               else
                  _manufacturier = value;

                OnPropertyChanged("Manufacturier");
            }
        }
        private string _nomFichier;
        public string NomFichier
        {
            get
            {
                return _nomFichier;
            }
            set
            {
                _nomFichier = value;
                NomCourtFichier = Path.GetFileName(NomFichier);
                OnPropertyChanged("NomFichier");
            }
        }
        private string _nomCourtFichier;
        public string NomCourtFichier
        {
            get
            {
                return _nomCourtFichier;
            }
            set
            {
                _nomCourtFichier = value;
                OnPropertyChanged("NomCourtFichier");
            }
        }
        private BitmapImage _imageItemSelectionne;
        public BitmapImage ImageItemSelectionne
        {
            get
            {
                return _imageItemSelectionne;
            }
            set
            {
                _imageItemSelectionne = value;
                OnPropertyChanged("ImageItemSelectionne");
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
            Window modale = new ModalAjoutCollection();
            
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
            MessageBoxResult resultat;
            resultat=MessageBox.Show($"Voulez vous vraiment supprimer la collection: {CollectionSelectionnee.Nom}?\n Cela entrainera la suppression de ses {CollectionSelectionnee.ItemsCollection.Count} items. ", $"Attention", MessageBoxButton.YesNo);
            if (resultat==MessageBoxResult.Yes)
            {
                gestionnaireCollections.Supprimer(CollectionSelectionnee.Id);
                initialisationEcran(false);
            }
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
            int avantModale = CollectionSelectionnee.ItemsCollection.Count;
            int idCollection = CollectionSelectionnee.Id;
            Window modale = new ModalAjoutItemCollection(CollectionSelectionnee);
            modale.ShowDialog();
            initialisationEcran(false);
            
            CollectionSelectionnee = ReselectionnerCollection(true,idCollection);
            if (avantModale<CollectionSelectionnee.ItemsCollection.Count)
                ItemSelectionne = CollectionSelectionnee.ItemsCollection[CollectionSelectionnee.ItemsCollection.Count - 1]; 
           
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
                OnPropertyChanged("cmdAjouterImage");
            }
        }
        private void cmdAjouter_Image(object param)
        {
            NomFichier = Fichier.ImporterFichier();
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
            // cmdModifier_Item permet de modifier les informations de l'ItemSelectionne
            if (Nom.Trim().Length>0)
            {
                MessageBoxResult resultat = MessageBox.Show($"Appliquer les modifications à l'item: {ItemSelectionne.Nom}?", "Attention", MessageBoxButton.YesNo);
                 // TODO: Validation des entrées
                if (resultat==MessageBoxResult.Yes)
                {   
                    int index=CollectionSelectionnee.ItemsCollection.IndexOf(ItemSelectionne);
                    int idCollection = CollectionSelectionnee.Id;
                
                    ItemSelectionne.Nom = Validateur.Echappement(Nom.Trim());
                    ItemSelectionne.DateSortie = DateSortie.GetValueOrDefault();
                    ItemSelectionne.Type = Type;
                    ItemSelectionne.Condition = Condition;
                    ItemSelectionne.Description = Validateur.Echappement(Description.Trim());
                    ItemSelectionne.Manufacturier = Validateur.Echappement(Manufacturier.Trim());
                    if (!(NomFichier is null)&&NomFichier.Trim().Length>0)
                    {
                        Fichier.TeleverserFichierFTP(ItemSelectionne.Id,NomFichier);
                        gestionnaireItems.AjouterCheminImage(ItemSelectionne.Id);
                    }
                    gestionnaireItems.Modifier(ItemSelectionne);
                    initialisationEcran(false);
                    CollectionSelectionnee=ReselectionnerCollection(true,idCollection);
                    ItemSelectionne = CollectionSelectionnee.ItemsCollection[index];
                
                
                
                }
                RemplirChamps();
            }
            else
            {
                MessageBox.Show("Vous devez au moins fournir un nom à l'item pour pouvoir le modifier.");
            }
           
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
            if (MesCollections.Count>1)
            {
                Window modale = new ModalDeplacementItem(gestionnaireCollections.RecupererToutesSaufUne(UtilisateurADO.utilisateur.Id, CollectionSelectionnee.Id),ItemSelectionne);
                modale.ShowDialog();
                initialisationEcran(false);
            }
            else
            {
                MessageBox.Show("Vous devez avoir plus d'une collection pour déplacer un item.");
            }
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
            MessageBoxResult resultat = MessageBox.Show($"Voulez vous vraiment supprimer l'item: {ItemSelectionne.Nom}?", "Attention", MessageBoxButton.YesNo);
            if (resultat==MessageBoxResult.Yes)
            {
                // Supprimer_Item enlève l'Item de la collection courante.
                gestionnaireItems.Supprimer(ItemSelectionne.Id);
                CollectionSelectionnee.ItemsCollection.Remove(ItemSelectionne);
                ItemSelectionne = null;
                CollectionSelectionnee = ReselectionnerCollection();
             }
            

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
            cmdAjouterImage = new Commande(cmdAjouter_Image, UnItemSelectionne);
            initialisationEcran();
        }

        private void initialisationEcran(bool estPremierChargement = true)
        {
            // Récupère toutes les collections de l'utilisateur courant et les rempli avec leurs items respectifs.

            MesCollections = new ObservableCollection<Collection>();
            ChangerImageItem();
            // Récupérer tous les états possibles d'items.
            if (estPremierChargement)
            {
                ConditionsPossibles = gestionnaireItems.ConditionsPossibles;
                TypesPossibles = gestionnaireItems.TypesPossibles;
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

        public Collection ReselectionnerCollection(bool reinitialisationEcran=false, int? id=null)
        {
            foreach (Collection c in MesCollections)
            {
                if (!reinitialisationEcran)
                {
                    if (c.Id == CollectionSelectionnee.Id)
                        return c;
                }
                else
                {
                     if (c.Id == id)
                         return c;
                }
                       
            }
            return null;
        }

        private void ViderChamps()
        {
            Nom = null;
            DateSortie = null;
            Manufacturier = null;
            Condition = null;
            Type = null;
            Description = null;
            NomFichier = null;
        }

        private void RemplirChamps()
        {
            Nom = ItemSelectionne.Nom;
            DateSortie = ItemSelectionne.DateSortie;
            Manufacturier = ItemSelectionne.Manufacturier;
            Condition = ItemSelectionne.Condition;
            Type = ItemSelectionne.Type;
            Description = ItemSelectionne.Description;
        }
        private void ChangerImageItem()
        {
            if (!(ItemSelectionne is null) && !(ItemSelectionne.BmImage==null))
            {
                ImageItemSelectionne = ItemSelectionne.BmImage;
            }
            else
            {
                ImageItemSelectionne = new BitmapImage();
                ImageItemSelectionne.BeginInit();
                ImageItemSelectionne.UriSource = new Uri("pack://application:,,,/LeCollectionneur;component/images/noimage.png",UriKind.Absolute);
                
                ImageItemSelectionne.EndInit();
            }
               
            
        }
        #endregion
    }
}

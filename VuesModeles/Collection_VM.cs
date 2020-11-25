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
        #region Constantes
        private const string TITRE_MODIF = "Modification d'un item";
        private const string TITRE_AJOUT = "Ajouter un item";
        private const string CONTENU_BTN_AJOUT = "Ajouter";
        private const string CONTENU_BTN_MODIF = "Appliquer les modifications";
        #endregion

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
               cmdSupprimerCollection=new Commande(cmdSupprimer_Collection,CollectionEstSupprimable);
               cmdModifierCollection=new Commande(cmdModifier_Collection,UneCollectionSelectionnee);
                cmdToggleAjouterItem= new Commande(cmdToggleAjouter_Item,UneCollectionSelectionnee); 
                cmdDeplacerItem= new Commande(cmdDeplacer_Item, ()=> { return MesCollections.Count > 1; });    
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
                {
                    RemplirChamps();
                    ChangerContexte();
                }
                cmdSupprimerItem = new Commande(cmdSupprimer_Item,ItemEstSupprimable);
                 
                cmdModifierItem = new Commande(cmdModifier_Item, UnItemSelectionne);
                cmdAjouterImage = new Commande(cmdAjouter_Image, UnItemSelectionne);
                NomFichier = null;
                ChangerImageItem();
                OnPropertyChanged("ItemSelectionne");
            }
        }
        private string _titreContexte;
        public string TitreContexte
        {
            get { return _titreContexte; }
            set
            {
                _titreContexte = value;
                OnPropertyChanged("TitreContexte");
            }
        }

        private string _texteBoutonContexte;
        public string TexteBoutonContexte
        {
            get { return _texteBoutonContexte; }
            set
            {
                _texteBoutonContexte = value;
                OnPropertyChanged("TexteBoutonContexte");
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
            Collection col =new Collection( CollectionSelectionnee.Id);
            // Modifier_Collection permet de modifier le nom de la collection dans un pop-up.
            Window modale = new ModalModifierCollection(CollectionSelectionnee);

            modale.ShowDialog();
            initialisationEcran(false);
            CollectionSelectionnee = ReselectionnerCollection(col);

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
        private ICommand _cmdOperationItem;
        public ICommand CmdOperationItem
        {
            get { return _cmdOperationItem; }
            set
            {
                _cmdOperationItem = value;
                OnPropertyChanged("CmdOperationItem");
            }
        }
        private ICommand _cmdToggleAjouterItem;
        public ICommand cmdToggleAjouterItem
        {
            get
            {
                return _cmdToggleAjouterItem;
            }
            set
            {
                _cmdToggleAjouterItem = value;
                OnPropertyChanged("cmdToggleAjouterItem");
            }
        }
        private void cmdToggleAjouter_Item(object param)
        {

            
            ItemSelectionne = null;
            ChangerContexte(true);
           
        }

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
            Item neoItem = new Item();
            int avantTentative = CollectionSelectionnee.ItemsCollection.Count;
            bool estInsere = false;
           
            try
            {
                if (!(Nom is null) && Nom.Trim().Length > 0 && !(Type is null) && Type.Length > 0 && !(Condition is null) && Condition.Length > 0)
                {

                    if (Nom.Trim().Length > 60)
                    {
                        throw new Exception("Le nom de l'item doit contenir un maximum de 60 caractères.");                       
                    }
                    Nom = Validateur.Echappement(Nom.Trim());
                    neoItem = new Item();
                    neoItem.Nom = Nom;
                    neoItem.Type = new TypeItem(Type);
                    neoItem.Condition = new Modeles.Condition(Condition);
                    if (!(Description is null) && Description.Trim().Length > 0)
                    {
                        Description = Validateur.Echappement(Description.Trim());
                        neoItem.Description = Description;
                    }


                    if (!(Manufacturier is null) && Manufacturier.Trim().Length > 0)
                    {
                        if (!(Manufacturier.Trim().Length > 50))
                        {
                            Manufacturier = Validateur.Echappement(Manufacturier.Trim());
                            neoItem.Manufacturier = Manufacturier;
                        }
                        else
                        {
                            throw new Exception("Le nom du producteur doit avoir un maximum de 50 caractères.");
                        }
                    }
                    


                    int id = gestionnaireItems.AjouterAvecRetourId(neoItem, CollectionSelectionnee);
                    neoItem.Id = id;
                    if (!(NomFichier is null)&&NomFichier.Length > 0)
                    {
                        Fichier.TeleverserFichierFTP(id, NomFichier);
                        gestionnaireItems.AjouterCheminImage(id);
                    }
                    estInsere = true;
                }
                else
                {
                    throw new Exception("Veuillez au minimum choisir un nom, un type et une condition.");   
                }
            }
            catch(Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
           finally
            {
                if (estInsere)
                {
                    // Recharger l'écran
                    Collection laCol  = CollectionSelectionnee;
                    initialisationEcran(false);
                    CollectionSelectionnee = ReselectionnerCollection(laCol); 
                    ItemSelectionne =TrouverItem(CollectionSelectionnee,neoItem) ;
                }
                 
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
                    Collection laCol = CollectionSelectionnee;
                
                    ItemSelectionne.Nom = Validateur.Echappement(Nom.Trim());
                    ItemSelectionne.DateSortie = DateSortie.GetValueOrDefault();
                    ItemSelectionne.Type = new TypeItem(Type);
                    ItemSelectionne.Condition = new Modeles.Condition(Condition);
                    ItemSelectionne.Description = Validateur.Echappement(Description.Trim());
                    ItemSelectionne.Manufacturier = Validateur.Echappement(Manufacturier.Trim());
                    if (!(NomFichier is null)&&NomFichier.Trim().Length>0)
                    {
                        Fichier.TeleverserFichierFTP(ItemSelectionne.Id,NomFichier);
                        gestionnaireItems.AjouterCheminImage(ItemSelectionne.Id);
                    }
                    gestionnaireItems.Modifier(ItemSelectionne);
                    initialisationEcran(false);
                    CollectionSelectionnee=ReselectionnerCollection(laCol);
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
            
                Window modale = new ModalDeplacementItem(gestionnaireCollections.RecupererToutesSaufUne(UtilisateurADO.utilisateur.Id, CollectionSelectionnee.Id),ItemSelectionne);
                modale.ShowDialog();
                if (CollectionSelectionnee.ItemsCollection.Count!=gestionnaireCollections.RecupererUn(CollectionSelectionnee.Id).ItemsCollection.Count)
                {
                    Item itemDeplace = ItemSelectionne;
                    initialisationEcran(false);
                    CollectionSelectionnee = ReselectionnerCollection(new Collection(gestionnaireItems.RecupererIdCollection(itemDeplace)));
                    ItemSelectionne = TrouverItem(CollectionSelectionnee, itemDeplace);
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
                CollectionSelectionnee = ReselectionnerCollection(CollectionSelectionnee);
             }
            

        }
        #endregion

        #region Constructeurs
        public Collection_VM()
        {
            cmdAjouterCollection = new Commande(cmdAjouter_Collection,()=> { return true; });
            cmdModifierCollection = new Commande(cmdModifier_Collection, UneCollectionSelectionnee);
            cmdSupprimerCollection = new Commande(cmdSupprimer_Collection, CollectionEstSupprimable);
            cmdToggleAjouterItem = new Commande(cmdToggleAjouter_Item, UneCollectionSelectionnee);
            cmdDeplacerItem = new Commande(cmdDeplacer_Item, ()=>{return MesCollections.Count > 1; });
            cmdSupprimerItem = new Commande(cmdSupprimer_Item, ItemEstSupprimable);
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
                ChangerContexte();
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

        public bool ItemEstSupprimable()
        {
            if (ItemSelectionne is null)
            {
                return false;
            }
            return (!gestionnaireItems.EstDansAnnonce(ItemSelectionne) && !gestionnaireItems.EstDansProposition(ItemSelectionne));    
        }
        public bool ItemParamEstSupprimable(Item item)
        {
            if (item is null)
            {
                return false;
            }
            return (!gestionnaireItems.EstDansAnnonce(item) && !gestionnaireItems.EstDansProposition(item));
        }
        public bool CollectionEstSupprimable()
        {
            if (CollectionSelectionnee is null)
            {
                return false;
            }
            foreach (Item item in CollectionSelectionnee.ItemsCollection)
            {
                if (!ItemParamEstSupprimable(item))
                {
                    return false;
                }
            }
            return true;
        }

        public Item TrouverItem(Collection collectionAParcourir,Item itemRecherche)
        {
           return collectionAParcourir.ItemsCollection[collectionAParcourir.ItemsCollection.IndexOf(itemRecherche)];
        }
        public Collection ReselectionnerCollection(Collection collectionRecherche)
        {     
            return MesCollections[MesCollections.IndexOf(collectionRecherche)];
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
            Condition = ItemSelectionne.Condition.Nom;
            Type = ItemSelectionne.Type.Nom;
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

        private void ChangerContexte(bool estAjout=false)
        {
            if (!estAjout)
            {
                // Si c'est n'est pas un ajout, on affiche le contexte de mise à jour de l'item.
                TitreContexte = TITRE_MODIF;
                TexteBoutonContexte = CONTENU_BTN_MODIF;
                CmdOperationItem = new Commande(cmdModifier_Item,UnItemSelectionne);
            }
            else
            {
                TitreContexte = TITRE_AJOUT;
                TexteBoutonContexte = CONTENU_BTN_AJOUT;
                CmdOperationItem = new Commande(cmdAjouter_Item, UneCollectionSelectionnee);
                cmdAjouterImage = new Commande(cmdAjouter_Image, ()=> { return true; });
            }
        }
        #endregion
    }
}

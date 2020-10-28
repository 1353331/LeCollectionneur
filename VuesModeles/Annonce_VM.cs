using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.Vues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Drawing.Text;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

namespace LeCollectionneur.VuesModeles
{
    class Annonce_VM : INotifyPropertyChanged
    {
        private const string FILTRE_NULL = "Aucune sélection";
        private const string PROPOSER = "Proposer une offre";
        private const string MODIFIER = "Modifier mon annonce";
        private const string NOM_MODAL_PROPOSITION = "proposition";
        private const string NOM_MODAL_MODIFICATION = "modifier";

        public ICommand cmdFiltrer_Annonce { get; set; }
        public ICommand cmdAjouterAnnonce_Annonce { get; set; }
        public ICommand cmdFiltrerMesAnnonces_Annonce { get; set; }

        public Annonce_VM()
        {
            cmdFiltrer_Annonce = new Commande(cmdFiltrer);
            cmdProposerOuModifier_Annonce = new Commande(cmdProposer, UneAnnonceSelectionnee);
            cmdAjouterAnnonce_Annonce = new Commande(cmdAjouterAnnonce);
            cmdFiltrerMesAnnonces_Annonce = new Commande(cmdFiltrerMesAnnonces);
            cmdDetails_Annonce = new Commande(cmdDetails);

            ProposerOuModifier = PROPOSER;

            initAnnonces();
            initFiltre();
        }

        private AnnonceADO annonceADO = new AnnonceADO();

        #region Partie Annonce
        private ObservableCollection<Annonce> _lesAnnonces;
        public ObservableCollection<Annonce> LesAnnonces 
        { 
            get { return _lesAnnonces; }
            set
            {
                _lesAnnonces = value;
                OnPropertyChanged("LesAnnonces");
            } 
        }

        private Annonce _annonceSelectionnee;
        public Annonce AnnonceSelectionnee 
        {   get { return _annonceSelectionnee; } 
            set
            {
                _annonceSelectionnee = value;
                if (_annonceSelectionnee == null)
                    return;

                Annonceur = _annonceSelectionnee.Annonceur;
                Titre = _annonceSelectionnee.Titre;
                DatePublication = _annonceSelectionnee.DatePublication;
                Type = _annonceSelectionnee.Type;
                Description = _annonceSelectionnee.Description;
                Montant = _annonceSelectionnee.Montant;
                LesItems = _annonceSelectionnee.ListeItems;

                if (EstMonAnnonce())
                {
                    ProposerOuModifier = MODIFIER;
                    cmdProposerOuModifier_Annonce = new Commande(cmdModifier, UneAnnonceSelectionnee);
                }
                else
                {
                    ProposerOuModifier = PROPOSER;
                    cmdProposerOuModifier_Annonce = new Commande(cmdProposer, UneAnnonceSelectionnee);
                }

                OnPropertyChanged("AnnonceSelectionnee");
            } 
        }

        private Utilisateur _annonceur;
        public Utilisateur Annonceur
        {
            get { return _annonceur; }
            set
            {
                _annonceur = value;
                OnPropertyChanged("Annonceur");
            }
        }

        private string _titre;
        public string Titre
        {
            get { return _titre; }
            set
            {
                _titre = value;
                OnPropertyChanged("Titre");
            }
        }

        private DateTime _datePublication;
        public DateTime DatePublication
        {
            get { return _datePublication; }
            set
            {
                _datePublication = value;
                OnPropertyChanged("DatePublication");
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

        private string _description;
        public string Description
        {
            get { return _description; }
            set
            {
                _description = value;
                OnPropertyChanged("Description");
            }
        }

        private ObservableCollection<Item> _lesItems;
        public ObservableCollection<Item> LesItems
        {
            get { return _lesItems; }
            set
            {
                _lesItems = value;
                OnPropertyChanged("LesItems");
            }
        }

        private Item _itemSelectionne;
        public Item ItemSelectionne
        {
            get { return _itemSelectionne; }
            set
            {
                _itemSelectionne = value;
                if (_itemSelectionne == null)
                    return;
                OnPropertyChanged("ItemSelectionne");
            }
        }

        private double _montant;
        public double Montant
        {
            get { return _montant; }
            set
            {
                _montant = value;
                OnPropertyChanged("Montant");
            }
        }

        private ICommand _cmdProposerOuModifier_Annonce;
        public ICommand cmdProposerOuModifier_Annonce
        {
            get { return _cmdProposerOuModifier_Annonce; }
            set
            {
                _cmdProposerOuModifier_Annonce = value;
                OnPropertyChanged("cmdProposerOuModifier_Annonce");
            }
        }

        private ICommand _cmdDetails_Annonce;
        public ICommand cmdDetails_Annonce
        {
            get { return _cmdDetails_Annonce; }
            set
            {
                _cmdDetails_Annonce = value;
                OnPropertyChanged("cmdDetails_Annonce");
            }
        }

        private string _proposerOuModifier;
        public string ProposerOuModifier
        {
            get { return _proposerOuModifier; }
            set
            {
                _proposerOuModifier = value;
                OnPropertyChanged("ProposerOuModifier");
            }
        }
        #endregion

        #region Partie Filtres
        private ObservableCollection<string> _lesTypesAnnonce;
        public ObservableCollection<string> LesTypesAnnonce
        {
            get { return _lesTypesAnnonce; }
            set
            {
                _lesTypesAnnonce = value;
                OnPropertyChanged("LesTypesAnnonce");
            }
        }

        private string _typeAnnonceFiltre;
        public string TypeAnnonceFiltre
        {
            get { return _typeAnnonceFiltre; }
            set
            {
                _typeAnnonceFiltre = value;
                OnPropertyChanged("TypeAnnonceFiltre");
            }
        }

        private ObservableCollection<string> _lesTypesItems;
        public ObservableCollection<string> LesTypesItems
        {
            get { return _lesTypesItems; }
            set
            {
                _lesTypesItems = value;
                OnPropertyChanged("LesTypesItems");
            }
        }

        private string _typeItemFiltre;
        public string TypeItemFiltre
        {
            get { return _typeItemFiltre; }
            set
            {
                _typeItemFiltre = value;
                OnPropertyChanged("TypeItemFiltre");
            }
        }


        private DateTime _dateDebutFiltre;
        public DateTime DateDebutFiltre
        {
            get { return _dateDebutFiltre; }
            set
            {
                _dateDebutFiltre = value;
                OnPropertyChanged("DateDebutFiltre");
            }
        }

        private DateTime _dateFinFiltre;
        public DateTime DateFinFiltre
        {
            get { return _dateFinFiltre; }
            set
            {
                _dateFinFiltre = value;
                OnPropertyChanged("DateFinFiltre");
            }
        }

        private string _rechercheTextuelle;
        public string RechercheTextuelle
        {
            get { return _rechercheTextuelle; }
            set
            {
                _rechercheTextuelle = value;
                OnPropertyChanged("RechercheTextuel");
            }
        }

        private bool _filtrerParNomAnnonceur;
        public bool FiltrerParNomAnnonceur
        {
            get { return _filtrerParNomAnnonceur; }
            set
            {
                _filtrerParNomAnnonceur = value;
                OnPropertyChanged("FiltrerParNomAnnonceur");
            }
        }

        private bool _filtrerParNomItem;
        public bool FiltrerParNomItem
        {
            get { return _filtrerParNomItem; }
            set
            {
                _filtrerParNomItem = value;
                OnPropertyChanged("FiltrerParNomItem");
            }
        }

        private bool _filtrerParTitreAnnonce;
        public bool FiltrerParTitreAnnonce
        {
            get { return _filtrerParTitreAnnonce; }
            set
            {
                _filtrerParTitreAnnonce = value;
                OnPropertyChanged("FiltrerParTitreAnnonce");
            }
        }

        private bool _filtrerParMesAnnonces;
        public bool FiltrerParMesAnnonces
        {
            get { return _filtrerParMesAnnonces; }
            set
            {
                _filtrerParMesAnnonces = value;
                OnPropertyChanged("FiltrerParMesAnnonces");
            }
        }
        #endregion

        #region Méthodes
        private void initAnnonces()
        {
            LesAnnonces = new ObservableCollection<Annonce>();
            LesAnnonces = annonceADO.Recuperer();
        }

        private void cmdAjouterAnnonce(object param)
        {
            IOuvreModal fenetre = param as IOuvreModal;
            fenetre.OuvrirModal();
            LesAnnonces = annonceADO.Recuperer();
        }

        private void cmdProposer(object param)
        {
            //Pour obtenir l'interface de la fenêtre, il faut la passer en paramètre lors de l'envoi de la commande (Voir le XAML du bouton, CommandParameter={...})
            IOuvreModalAvecChoixEtParam<Annonce> modal = param as IOuvreModalAvecChoixEtParam<Annonce>;
            string nomModal = NOM_MODAL_PROPOSITION;
            if (modal != null)
            {
                modal.OuvrirModal(AnnonceSelectionnee, nomModal);
            }
        }

        private void cmdModifier(object param)
        {

        }

        private void cmdDetails(object param)
        {
            IOuvreModalAvecParametre<Item> modal = param as IOuvreModalAvecParametre<Item>;
            if(modal != null)
            {
                modal.OuvrirModal(ItemSelectionne);
            }
        }

        public bool UneAnnonceSelectionnee()
        {
            return !(AnnonceSelectionnee is null);
        }

        public bool EstMonAnnonce()
        {
            UtilisateurADO Ud = new UtilisateurADO();
            Utilisateur UtilisateurConnecte = Ud.RetourUtilisateurActif();

            if (AnnonceSelectionnee.Annonceur.Id == UtilisateurConnecte.Id)
                return true;

            return false;
        }
        #endregion

        #region Méthodes de filtres
        private void initFiltre()
        {
            LesTypesAnnonce = new ObservableCollection<string>();
            LesTypesItems = new ObservableCollection<string>();

            LesTypesAnnonce = annonceADO.RecupererTypes();
            LesTypesAnnonce.Add(FILTRE_NULL);

            LesTypesItems = annonceADO.RecupererTypesItem();
            LesTypesItems.Add(FILTRE_NULL);

            DateDebutFiltre = new DateTime(0001, 01, 01);
            DateFinFiltre = DateTime.Now;
        }

        private void cmdFiltrer(object param)
        {
            LesAnnonces = annonceADO.Recuperer();


            if (TypeAnnonceFiltre != null && TypeAnnonceFiltre != FILTRE_NULL)
                LesAnnonces = FiltrerParTypeAnnonce();

            if (TypeItemFiltre != null && TypeItemFiltre != FILTRE_NULL)
                LesAnnonces = FiltrerParTypeItem();

            if (DateDebutFiltre != null && DateFinFiltre != null)
                LesAnnonces = FiltrerParDate();

            if (RechercheTextuelle != null && RechercheTextuelle != "" && (FiltrerParNomAnnonceur || FiltrerParTitreAnnonce || FiltrerParNomItem))
                LesAnnonces = FiltrerParRechercheTextuelle();
        }

        private ObservableCollection<Annonce> FiltrerParTypeAnnonce()
        {
            ObservableCollection<Annonce> LesAnnoncesFiltrees = new ObservableCollection<Annonce>(LesAnnonces.Where(a => a.Type == TypeAnnonceFiltre).ToList());
            return LesAnnoncesFiltrees;
        }

        private ObservableCollection<Annonce> FiltrerParTypeItem()
        {
            //Dans le premier Where on va regarder dans chacune des annonces leur liste d'items
            ObservableCollection<Annonce> LesAnnoncesFiltrees = new ObservableCollection<Annonce>(LesAnnonces.Where(a => 
                //Ici dans le 2e Where, on regarde si les items ont le type d'item spécifié par l'utilisateur
                //Puis, on garde seulement les items avec le type spécifié (la fonction ToList())
                //Si la liste a au moins un item à l'intérieur ( plus grand que 0 ), alors cela veut dire que l'annonce a un item avec le type d'item spécifié
                a.ListeItems.Where(i => i.Type == TypeItemFiltre).ToList().Count() > 0
            ).ToList()); //On garde seulement les annonces ayant un item du type d'item spécifié

            return LesAnnoncesFiltrees;
        }

        private ObservableCollection<Annonce> FiltrerParDate()
        {
            ObservableCollection<Annonce> LesAnnoncesFiltrees = new ObservableCollection<Annonce>(LesAnnonces.Where(a => a.DatePublication >= DateDebutFiltre && a.DatePublication <= DateFinFiltre).ToList());
            return LesAnnoncesFiltrees;
        }

        private ObservableCollection<Annonce> FiltrerParRechercheTextuelle()
        {
            ObservableCollection<Annonce> LesAnnoncesFiltrees = new ObservableCollection<Annonce>(LesAnnonces.Where(a => FiltrerSelonChoix(a)).ToList());
            return LesAnnoncesFiltrees;
        }

        private void cmdFiltrerMesAnnonces(object param)
        {
            if (FiltrerParMesAnnonces)
            {
                LesAnnonces = annonceADO.RecupererParUtilisateurConnecte();
            }
            else
            {
                LesAnnonces = annonceADO.Recuperer();
            }
        }

        private bool FiltrerSelonChoix(Annonce a)
        {
            if (FiltrerParNomAnnonceur)
            {
                if(a.Annonceur.NomUtilisateur.Contains(RechercheTextuelle) ||
                   a.Annonceur.NomUtilisateur.StartsWith(RechercheTextuelle, System.StringComparison.CurrentCultureIgnoreCase) ||
                   a.Annonceur.NomUtilisateur.EndsWith(RechercheTextuelle, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            if (FiltrerParTitreAnnonce)
            {
                if (a.Titre.Contains(RechercheTextuelle) ||
                    a.Titre.StartsWith(RechercheTextuelle, System.StringComparison.CurrentCultureIgnoreCase) ||
                    a.Titre.EndsWith(RechercheTextuelle, System.StringComparison.CurrentCultureIgnoreCase))
                {
                    return true;
                }
            }

            if(FiltrerParNomItem)
            {
                if (a.ListeItems.Where(i => i.Nom.Contains(RechercheTextuelle) ||
                    i.Nom.StartsWith(RechercheTextuelle, System.StringComparison.CurrentCultureIgnoreCase) ||
                    i.Nom.EndsWith(RechercheTextuelle, System.StringComparison.CurrentCultureIgnoreCase)).ToList().Count() > 0)
                {
                    return true;
                }
            }
            return false;
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        
    }
}

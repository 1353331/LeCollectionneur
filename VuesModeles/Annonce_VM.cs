using LeCollectionneur.EF;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Enumerations;
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
using System.Windows;
using System.Windows.Annotations;
using System.Windows.Input;
using System.Windows.Markup;

namespace LeCollectionneur.VuesModeles
{
    class Annonce_VM : INotifyPropertyChanged
    {
        #region Variable Annonce_VM

        private const string FILTRE_NULL = "Aucune sélection";
        private const string PROPOSER = "Proposer une offre";
        private const string MODIFIER = "Modifier mon annonce";
        private const string NOM_MODAL_PROPOSITION = "proposition";
        private const string NOM_MODAL_MODIFICATION = "modifier";

        private readonly BackgroundWorker worker = new BackgroundWorker();
        private readonly BackgroundWorker worker_update = new BackgroundWorker();
        private readonly BackgroundWorker worker_filtre = new BackgroundWorker();
        private AnnonceADO annonceADO = new AnnonceADO();

        private bool onInitialise = false;
        private bool onAjouteAnnonce = false;
        private bool onModifieAnnonce = false;
        private int idAnnonceMod;
        private bool onSupprimeAnnonce = false;
        private bool onFiltre = false;

        public ICommand cmdAjouterAnnonce_Annonce { get; set; }
        public ICommand cmdFiltrerMesAnnonces_Annonce { get; set; }
        public ICommand cmdEnvoyerMessage_Annonce{ get; set; }
        public ICommand cmdAfficherTout_Annonce { get; set; }

        #endregion

        #region Contructeur

        public Annonce_VM()
        {
            //Initialisation des commandes présentes dans l'onglet Annonce
            cmdFiltrer_Annonce = new Commande(cmdFiltrer, UneColonneCochee);
            cmdProposerOuModifier_Annonce = new Commande(cmdProposer, UneAnnonceSelectionnee);
            cmdAjouterAnnonce_Annonce = new Commande(cmdAjouterAnnonce);
            cmdFiltrerMesAnnonces_Annonce = new Commande(cmdFiltrerMesAnnonces);
            cmdDetails_Annonce = new Commande(cmdDetails);
            cmdEnvoyerMessage_Annonce = new Commande(cmdEnvMessage);
            cmdAfficherTout_Annonce = new Commande(cmdAfficherTout);
            cmdSupprimer_Annonce = new Commande(cmdSupprimer);

            ProposerOuModifier = PROPOSER;

            //On indique que l'on initialise (Pour le filtres, cela est nécessaire, sinon boucle infinie)
            onInitialise = true;

            //On initialise la liste des annonces
            LesAnnonces = new ObservableCollection<Annonce>();

            //On charge l'onglet annonce
            chargerAnnonces();
        }

        #endregion

        #region Variables de la liste des annonces

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

                //Affiche les informations de l'annonce dans la partie de droite de l'écran
                Annonceur = _annonceSelectionnee.Annonceur;
                Titre = _annonceSelectionnee.Titre;
                DatePublication = _annonceSelectionnee.DatePublication;
                Type = _annonceSelectionnee.Type.Nom;
                Description = _annonceSelectionnee.Description;
                Montant = _annonceSelectionnee.Montant;

                LesItems = _annonceSelectionnee.ListeItems;

                //Connaitre si c'est une annonce à l'utilisateur connecté
                if (EstMonAnnonce())
                {
                    //Si l'annonce appartient à l'utilisateur connecté, il peut la modifier
                    ProposerOuModifier = MODIFIER;
                    cmdProposerOuModifier_Annonce = new Commande(cmdModifier, UneAnnonceSelectionnee);                    
                }
                else
                {
                    //L'annonce n'appartient pas à l'utilisateur connecté, alors il peut proposer une offre
                    ProposerOuModifier = PROPOSER;
                    cmdProposerOuModifier_Annonce = new Commande(cmdProposer, UneAnnonceSelectionnee);
                }

                OnPropertyChanged("AnnonceSelectionnee");
            } 
        }

        private Visibility _visibiliteSpinner;
        public Visibility VisibiliteSpinner
        {
            get { return _visibiliteSpinner; }
            set
            {
                _visibiliteSpinner = value;
                OnPropertyChanged("VisibiliteSpinner");
            }
        }

        #endregion

        #region Variables pour une annonce sélectionnée

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

        private ICommand _cmdSupprimer_Annonce;
        public ICommand cmdSupprimer_Annonce
        {
            get { return _cmdSupprimer_Annonce; }
            set
            {
                _cmdSupprimer_Annonce = value;
                OnPropertyChanged("cmdSupprimer_Annonce");
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

        #region Variables pour les filtres

        private ICommand _cmdFiltrer_Annonce;
        public ICommand cmdFiltrer_Annonce
        {
            get { return _cmdFiltrer_Annonce; }
            set
            {
                _cmdFiltrer_Annonce = value;
                OnPropertyChanged("cmdFiltrer_Annonce");
            }
        }

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

                //Si on initialise, on ne veut pas appliquer la commande de filtrage
                if (!onInitialise)
                {
                    cmdFiltrer_Annonce.Execute(null);
                }
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

                //Si on initialise, on ne veut pas appliquer la commande de filtrage
                if (!onInitialise)
                {
                    cmdFiltrer_Annonce.Execute(null);
                }
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

                //Si on initialise, on ne veut pas appliquer la commande de filtrage
                if (!onInitialise)
                {
                    cmdFiltrer_Annonce.Execute(null);
                }
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

                //Si on initialise, on ne veut pas appliquer la commande de filtrage
                if (!onInitialise)
                {
                    cmdFiltrer_Annonce.Execute(null);
                }
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
                //Si on initialise, on ne veut pas appliquer la commande de filtrage
                if (!onInitialise)
                {
                    cmdFiltrer_Annonce.Execute(null);
                }
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
                cmdFiltrer_Annonce = new Commande(cmdFiltrer, UneColonneCochee);

                //Si on initialise, on ne veut pas appliquer la commande de filtrage
                if (!onInitialise)
                {
                    cmdFiltrer_Annonce.Execute(null);
                }
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
                cmdFiltrer_Annonce = new Commande(cmdFiltrer, UneColonneCochee);

                //Si on initialise, on ne veut pas appliquer la commande de filtrage
                if (!onInitialise)
                {
                    cmdFiltrer_Annonce.Execute(null);
                }
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
                cmdFiltrer_Annonce = new Commande(cmdFiltrer, UneColonneCochee);

                //Si on initialise, on ne veut pas appliquer la commande de filtrage
                if (!onInitialise)
                {
                    cmdFiltrer_Annonce.Execute(null);
                }
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

        #region Méthodes pour les annonces

        private void chargerAnnonces()
        {
            //On affiche le spinner de chargement
            VisibiliteSpinner = Visibility.Visible;

            //On s'assure que le worker n'est pas occupé
            if (!worker.IsBusy)
            {
                //Ce qu'il doit faire
                worker.DoWork += Worker_DoWork;

                //Ce qu'il doit faire lorsqu'il a fini sa tâche
                worker.RunWorkerCompleted += WorkerEnvoyees_RunWorkerCompleted;

                //On fait la tâche en Async
                worker.RunWorkerAsync();
            }
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            using (Context context = new Context())
            {
                //On va récupérer tous les annonces et tous leurs informations
                List<Annonce> lstAnnonces = context.Annonces.Include("Annonceur")
                                                                .Include("EtatAnnonce")
                                                                .Include("Type")
                                                                .Include("ListeItems.Type")
                                                                .Include("ListeItems.Condition")
                                                                .AsNoTracking()
                                                                .Where(a => a.EtatAnnonce.Id == 2)
                                                                .ToList();

                //On change la liste d'annonces en ObservableCollection
                ObservableCollection<Annonce> ocAnnonces = changerListAnnoncesEnOCAnnonces(lstAnnonces);

                //On initialise LesAnnonces
                LesAnnonces = ocAnnonces;

                //On va récupérer les types d'annonce
                List<TypeAnnonce> lstTypesAnnonce = context.TypesAnnonce.ToList();

                //On transforme la liste des types d'annonce en OC de string
                ObservableCollection<string> ocTypesAnnonce = new ObservableCollection<string>();
                foreach (TypeAnnonce ta in lstTypesAnnonce)
                {
                    ocTypesAnnonce.Add(ta.Nom);
                }

                //On initialise LesTypesAnnonce
                LesTypesAnnonce = ocTypesAnnonce;

                //On va récupérer tous les types items de présents dans les items des annonces
                ObservableCollection<string> ocTypesItem = new ObservableCollection<string>();
                foreach (Annonce a in LesAnnonces)
                {
                    foreach (Item i in a.ListeItems)
                    {
                        if (!ocTypesItem.Contains(i.Type.Nom))
                        {
                            ocTypesItem.Add(i.Type.Nom);
                        }
                    }
                }

                //On initialise LesTypesItems
                LesTypesItems = ocTypesItem;
            }
        }

        private void WorkerEnvoyees_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //On sélectionne la première annonce de présente dans la OC
            if (LesAnnonces.Count > 0)
            {
                AnnonceSelectionnee = LesAnnonces.First();  
            }

            //On ajoute les filtres null
            LesTypesAnnonce.Add(FILTRE_NULL);
            LesTypesItems.Add(FILTRE_NULL);

            //On sélectionne le filtre null (le dernier ajouté)
            TypeAnnonceFiltre = LesTypesAnnonce.Last();
            TypeItemFiltre = LesTypesItems.Last();

            //On initialise les filtres de date
            DateDebutFiltre = PlusVieilleAnnonce();
            DateFinFiltre = DateTime.Now;

            //Une case de cochée par défaut
            FiltrerParTitreAnnonce = true;

            //On a terminé d'initialiser
            onInitialise = false;

            //On enlève le spinner de chargement
            VisibiliteSpinner = Visibility.Collapsed;

            //Ce qu'il doit faire
            worker.DoWork -= Worker_DoWork;

            //Ce qu'il doit faire lorsqu'il a fini sa tâche
            worker.RunWorkerCompleted -= WorkerEnvoyees_RunWorkerCompleted;
        }


        private void UpdateAnnonce()
        {
            //On affiche le spinner de chargement
            VisibiliteSpinner = Visibility.Visible;
            
            //On s'assure que le worker n'est pas occupé
            if (!worker_update.IsBusy)
            {
                //Ce qu'il doit faire
                worker_update.DoWork += Worker_DoWork_Update;

                //Ce qu'il doit faire lorsqu'il a fini sa tâche
                worker_update.RunWorkerCompleted += WorkerEnvoyees_RunWorkerCompleted_Update;

                //On fait la tâche en Async
                worker_update.RunWorkerAsync();
            }
        }

        private void Worker_DoWork_Update(object sender, DoWorkEventArgs e)
        {
            using (Context context = new Context())
            {
                //On va récupérer tous les annonces et tous leurs informations
                List<Annonce> lstAnnonces = context.Annonces.Include("Annonceur")
                                                                .Include("EtatAnnonce")
                                                                .Include("Type")
                                                                .Include("ListeItems.Type")
                                                                .Include("ListeItems.Condition")
                                                                .AsNoTracking()
                                                                .Where(a => a.EtatAnnonce.Id == 2)
                                                                .ToList();

                //On change la liste d'annonces en ObservableCollection
                ObservableCollection<Annonce> ocAnnonces = changerListAnnoncesEnOCAnnonces(lstAnnonces);

                //On update LesAnnonces
                LesAnnonces = ocAnnonces;
            }
        }

        private void WorkerEnvoyees_RunWorkerCompleted_Update(object sender, RunWorkerCompletedEventArgs e)
        {
            if (onSupprimeAnnonce)
            {
                //On sélectionne la première annonce de présente dans la OC
                if (LesAnnonces.Count > 0)
                {
                    AnnonceSelectionnee = LesAnnonces.First();
                }
                if (FiltrerParMesAnnonces)
                    FiltrerParMesAnnonces = false;
            }
            if (onAjouteAnnonce)
            {
                AnnonceSelectionnee = LesAnnonces.Last();
            }
            if (onModifieAnnonce)
            {
                AnnonceSelectionnee = LesAnnonces.Single(a => a.Id == idAnnonceMod);
            }
            if (onFiltre)
            {
                FiltrerAnnonce();
            }

            onSupprimeAnnonce = false;
            onAjouteAnnonce = false;
            onModifieAnnonce = false;

            //On enlève le spinner de chargement
            VisibiliteSpinner = Visibility.Collapsed;

            //Ce qu'il doit faire
            worker_update.DoWork -= Worker_DoWork_Update;

            //Ce qu'il doit faire lorsqu'il a fini sa tâche
            worker_update.RunWorkerCompleted -= WorkerEnvoyees_RunWorkerCompleted_Update;
        }

        private ObservableCollection<Annonce> changerListAnnoncesEnOCAnnonces(List<Annonce> lstAnnonces)
        {
            ObservableCollection<Annonce> ocAnnonces = new ObservableCollection<Annonce>();
            foreach (Annonce annonce in lstAnnonces)
            {
                foreach (Item item in annonce.ListeItems)
                {
                    if (!String.IsNullOrWhiteSpace(item.CheminImage))
                    {
                        item.BmImage = Fichier.TransformerBitmapEnBitmapImage(Fichier.RecupererImageServeur(item.CheminImage));
                        if (item.BmImage is null)
                        {
                            ItemADO gestionItem = new ItemADO();
                            item.CheminImage = null;
                            gestionItem.EnleverCheminImage(item.Id);
                        }
                    }
                }
                ocAnnonces.Add(annonce);
            }

            return ocAnnonces;
        }

        private void cmdAjouterAnnonce(object param)
        {
            IOuvreModal fenetre = param as IOuvreModal;
            fenetre.OuvrirModal();

            onAjouteAnnonce = true;
            UpdateAnnonce();
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
            //Pour obtenir l'interface de la fenêtre, il faut la passer en paramètre lors de l'envoi de la commande (Voir le XAML du bouton, CommandParameter={...})
            IOuvreModalAvecChoixEtParam<Annonce> modal = param as IOuvreModalAvecChoixEtParam<Annonce>;
            string nomModal = NOM_MODAL_MODIFICATION;
            if (modal != null)
            {
                Annonce AnnonceTEMP = AnnonceSelectionnee;
                modal.OuvrirModal(AnnonceSelectionnee, nomModal);

                if(annonceADO.RecupererUn(AnnonceTEMP.Id).EtatAnnonce.Nom == EtatsAnnonce.Annulee)
                {
                    onSupprimeAnnonce = true;
                }
                else
                {
                    onModifieAnnonce = true;
                }
                
                idAnnonceMod = AnnonceTEMP.Id;
                UpdateAnnonce();
            }
        }

        private void cmdSupprimer(object param)
        {
            if (AnnonceSelectionnee != null)
            {
                onSupprimeAnnonce = true;
                //On affiche le message de confirmation
                MessageBoxResult resultat = MessageBox.Show($"Voulez-vous vraiment supprimer votre annonce?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

                //Si l'utilisateur veut bel et bien supprimer son annonce, alors on supprime l'annonce
                if (resultat == MessageBoxResult.Yes)
                {
                    AnnonceSelectionnee.EtatAnnonce = new EtatAnnonce(EtatsAnnonce.Annulee);
                    annonceADO.Modifier(AnnonceSelectionnee);
                }
                UpdateAnnonce();
            }
        }

        private void cmdDetails(object param)
        {
            IOuvreModalAvecParametre<Item> modal = param as IOuvreModalAvecParametre<Item>;
            if(modal != null)
            {
                modal.OuvrirModal(ItemSelectionne);
            }
        }

        private void cmdEnvMessage(object param)
        {
            if (UneAnnonceSelectionnee() && AnnonceSelectionnee.Annonceur.NomUtilisateur != UtilisateurADO.utilisateur.NomUtilisateur)
            {
               IOuvreModalAvecParametre<Utilisateur> fenetre = param as IOuvreModalAvecParametre<Utilisateur>;
               fenetre.OuvrirModal(AnnonceSelectionnee.Annonceur);
            }
        }

        public bool UneAnnonceSelectionnee()
        {
            return !(AnnonceSelectionnee is null);
        }

        public bool EstMonAnnonce()
        {
            //Vérifie si l'annonce appartient à l'utilisateur connecté
            if (AnnonceSelectionnee.Annonceur.Id == UtilisateurADO.utilisateur.Id)
                return true;

            return false;
        }
        #endregion

        #region Méthodes pour les filtres

        private void cmdFiltrer(object param)
        {
            onFiltre = true;
            UpdateAnnonce();
        }

        private void FiltrerAnnonce()
        {
            //On affiche le spinner de chargement
            VisibiliteSpinner = Visibility.Visible;

            //On s'assure que le worker n'est pas occupé
            if (!worker_filtre.IsBusy)
            {
                //Ce qu'il doit faire
                worker_filtre.DoWork += Worker_DoWork_Filtrer;

                //Ce qu'il doit faire lorsqu'il a fini sa tâche
                worker_filtre.RunWorkerCompleted += WorkerEnvoyees_RunWorkerCompleted_Filtrer;

                //On fait la tâche en Async
                worker_filtre.RunWorkerAsync();
            }
        }

        private void Worker_DoWork_Filtrer(object sender, DoWorkEventArgs e)
        {
            using (Context context = new Context())
            {
                if (TypeAnnonceFiltre != null && TypeAnnonceFiltre != FILTRE_NULL)
                    LesAnnonces = FiltrerParTypeAnnonce();

                if (TypeItemFiltre != null && TypeItemFiltre != FILTRE_NULL)
                    LesAnnonces = FiltrerParTypeItem();

                if (DateDebutFiltre != null && DateFinFiltre != null)
                    LesAnnonces = FiltrerParDate();

                if (RechercheTextuelle != null && RechercheTextuelle != "" && (FiltrerParNomAnnonceur || FiltrerParTitreAnnonce || FiltrerParNomItem))
                    LesAnnonces = FiltrerParRechercheTextuelle();

                if (FiltrerParMesAnnonces)
                    LesAnnonces = FiltrerMesAnnonces();
            }
        }

        private void WorkerEnvoyees_RunWorkerCompleted_Filtrer(object sender, RunWorkerCompletedEventArgs e)
        {
            onFiltre = false;

            //On enlève le spinner de chargement
            VisibiliteSpinner = Visibility.Collapsed;

            //Ce qu'il doit faire
            worker_filtre.DoWork -= Worker_DoWork_Filtrer;

            //Ce qu'il doit faire lorsqu'il a fini sa tâche
            worker_filtre.RunWorkerCompleted -= WorkerEnvoyees_RunWorkerCompleted_Filtrer;
        }

        private ObservableCollection<Annonce> FiltrerParTypeAnnonce()
        {
            ObservableCollection<Annonce> LesAnnoncesFiltrees = new ObservableCollection<Annonce>(LesAnnonces.Where(a => a.Type.Nom == TypeAnnonceFiltre).ToList());
            return LesAnnoncesFiltrees;
        }

        private ObservableCollection<Annonce> FiltrerMesAnnonces()
        {
            ObservableCollection<Annonce> LesAnnoncesFiltrees = new ObservableCollection<Annonce>(LesAnnonces.Where(a => a.Annonceur.Id == UtilisateurADO.utilisateur.Id).ToList());
            return LesAnnoncesFiltrees;
        }

        private ObservableCollection<Annonce> FiltrerParTypeItem()
        {
            for (int i = 0; i < LesAnnonces.Count; i++)
            {
                LesAnnonces[i].RecupererItems();
            }
            
            //Dans le premier Where on va regarder dans chacune des annonces leur liste d'items
            ObservableCollection<Annonce> LesAnnoncesFiltrees = new ObservableCollection<Annonce>(LesAnnonces.Where(a => 
                //Ici dans le 2e Where, on regarde si les items ont le type d'item spécifié par l'utilisateur
                //Puis, on garde seulement les items avec le type spécifié (la fonction ToList())
                //Si la liste a au moins un item à l'intérieur ( plus grand que 0 ), alors cela veut dire que l'annonce a un item avec le type d'item spécifié
                a.ListeItems.Where(i => i.Type.Nom == TypeItemFiltre).ToList().Count() > 0
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
                onFiltre = true;
                UpdateAnnonce();
            }
            else
            {
                UpdateAnnonce();
            }
        }

        private void cmdAfficherTout(object param)
        {
            onInitialise = true;
            chargerAnnonces();
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

        private bool UneColonneCochee()
        {
            if (FiltrerParTitreAnnonce || FiltrerParNomItem || FiltrerParNomAnnonceur)
            {
                return true;
            }

            return false;
        }

        private DateTime PlusVieilleAnnonce()
        {
           ObservableCollection<Annonce> temp =  new ObservableCollection<Annonce>(LesAnnonces.OrderBy(a => a.DatePublication));
           if(temp.Count > 0)
                return temp.First().DatePublication;


           return new DateTime(0001, 01, 01);
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        #endregion#
    }
}

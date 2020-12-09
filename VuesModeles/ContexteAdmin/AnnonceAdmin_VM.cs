using LeCollectionneur.EF;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Enumerations;
using LeCollectionneur.Outils.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LeCollectionneur.VuesModeles.ContexteAdmin
{
    class AnnonceAdmin_VM : INotifyPropertyChanged
    {
        private readonly BackgroundWorker worker = new BackgroundWorker();
        public ICommand cmdSupprimer_Annonce { get; set; }

        public AnnonceAdmin_VM()
        {
            cmdSupprimer_Annonce = new Commande(cmdSupprimer);
            cmdDetails_Annonce = new Commande(cmdDetails);
            chargerAnnonces();
        }

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

        private AnnonceADO annonceADO = new AnnonceADO();

        private Annonce _annonceSelectionnee;
        public Annonce AnnonceSelectionnee
        {
            get { return _annonceSelectionnee; }
            set
            {
                _annonceSelectionnee = value;
                if (_annonceSelectionnee == null)
                    return;

                Annonceur = _annonceSelectionnee.Annonceur;
                Titre = _annonceSelectionnee.Titre;
                DatePublication = _annonceSelectionnee.DatePublication;
                Type = _annonceSelectionnee.Type.Nom;
                Description = _annonceSelectionnee.Description;
                Montant = _annonceSelectionnee.Montant;

                LesItems = _annonceSelectionnee.ListeItems;

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
            }
        }

        private void WorkerEnvoyees_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            //On sélectionne la première annonce de présente dans la OC
            if (LesAnnonces.Count > 0)
            {
                AnnonceSelectionnee = LesAnnonces.First();
            }

            //On enlève le spinner de chargement
            VisibiliteSpinner = Visibility.Collapsed;

            //Ce qu'il doit faire
            worker.DoWork -= Worker_DoWork;

            //Ce qu'il doit faire lorsqu'il a fini sa tâche
            worker.RunWorkerCompleted -= WorkerEnvoyees_RunWorkerCompleted;
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

        private void cmdSupprimer(object param)
        {
            if (AnnonceSelectionnee != null)
            {
                AnnonceSelectionnee.EtatAnnonce = new EtatAnnonce(EtatsAnnonce.Annulee);
                annonceADO.Modifier(AnnonceSelectionnee);
                chargerAnnonces();
            }
        }

        private void cmdDetails(object param)
        {
            IOuvreModalAvecParametre<Item> modal = param as IOuvreModalAvecParametre<Item>;
            if (modal != null)
            {
                modal.OuvrirModal(ItemSelectionne);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
    }
}

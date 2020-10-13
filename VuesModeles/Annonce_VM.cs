using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Vues;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Markup;

namespace LeCollectionneur.VuesModeles
{
    class Annonce_VM : INotifyPropertyChanged
    {
        private const string FILTRE_NULL = "Aucun";

        public ICommand cmdAjouter_Annonce { get; set; }
        public ICommand cmdFiltrer_Annonce { get; set; }
        public ICommand cmdProposer_Annonce { get; set; }

        public Annonce_VM()
        {
            cmdAjouter_Annonce = new Commande(cmdAjouter);
            cmdFiltrer_Annonce = new Commande(cmdFiltrer);
            cmdProposer_Annonce = new Commande(cmdProposer);
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

        // TODO: LISTE ITEMS

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
        #endregion

        #region Méthodes
        private void initAnnonces()
        {
            LesAnnonces = new ObservableCollection<Annonce>();
            LesAnnonces = annonceADO.Recuperer();
        }

        private void initFiltre()
        {
            LesTypesAnnonce = new ObservableCollection<string>();
            LesTypesAnnonce = annonceADO.RecupererTypes();
            LesTypesAnnonce.Add(FILTRE_NULL);

        }

        private void cmdAjouter(object param)
        {
            ModalAjoutAnnonce fenetreAjout = new ModalAjoutAnnonce();
            fenetreAjout.ShowDialog();
            
            LesAnnonces = annonceADO.Recuperer();

        }

        private void cmdFiltrer(object param)
        {
            LesAnnonces = annonceADO.Recuperer();

            if(TypeAnnonceFiltre == FILTRE_NULL)
                return;

            ObservableCollection<Annonce> LesAnnoncesFiltrees = new ObservableCollection<Annonce>(LesAnnonces.Where(a => a.Type == TypeAnnonceFiltre).ToList());
            LesAnnonces = LesAnnoncesFiltrees;
        }

        private void cmdProposer(object param)
        {
            //Pour obtenir l'interface de la fenêtre, il faut la passer en paramètre lors de l'envoi de la commande (Voir le XAML du bouton, CommandParameter={...})
            IOuvreFenetreNouvellePropositionModalVM modal = param as IOuvreFenetreNouvellePropositionModalVM;
            if (modal != null)
            {
               modal.OuvrirModal(AnnonceSelectionnee);
            }
      }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        #endregion
    }
}

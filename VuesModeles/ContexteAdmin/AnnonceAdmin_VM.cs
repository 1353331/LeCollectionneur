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

namespace LeCollectionneur.VuesModeles.ContexteAdmin
{
    class AnnonceAdmin_VM : INotifyPropertyChanged
    {
        public ICommand cmdSupprimer_Annonce { get; set; }

        public AnnonceAdmin_VM()
        {
            cmdSupprimer_Annonce = new Commande(cmdSupprimer);
            cmdDetails_Annonce = new Commande(cmdDetails);
            initAnnonces();
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
                Type = _annonceSelectionnee.Type;
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

        private void initAnnonces()
        {
            LesAnnonces = new ObservableCollection<Annonce>();
            LesAnnonces = annonceADO.Recuperer();
        }

        private void cmdSupprimer(object param)
        {
            annonceADO.Supprimer(AnnonceSelectionnee);
            LesAnnonces = annonceADO.Recuperer();
            AnnonceSelectionnee = null;
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

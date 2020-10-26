using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.Outils.Messages;
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
    class ModalAjoutAnnonce_VM : INotifyPropertyChanged
    {
        public ICommand cmdAjouter_Annonce { get; set; }
        public ICommand cmdAjouterItem_Annonce { get; set; }

        public ModalAjoutAnnonce_VM()
        {
            cmdAjouter_Annonce = new Commande(cmdAjouter);
            cmdAjouterItem_Annonce = new Commande(cmdAjouterItem);
            InitNouvelleAnnonce();

            //Abonnement à l'évènement Ajout d'un item à une annonce
            EvenementSysteme.Abonnement<EnvoyerItemMessage>(ajouterItemMessage);
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

        private Annonce _nouvelleAnnonce { get; set; }
        public Annonce NouvelleAnnonce
        {
            get { return _nouvelleAnnonce; }
            set
            {
                _nouvelleAnnonce = value;
                OnPropertyChanged("NouvelleAnnonce");
            }
        }

        private string _titre { get; set; }
        public string Titre
        {
            get { return _titre; }
            set
            {
                _titre = value;
                OnPropertyChanged("Titre");
            }
        }

        private ObservableCollection<Item> _lesItems { get; set; }
        public ObservableCollection<Item> LesItems
        {
            get { return _lesItems; }
            set
            {
                _lesItems = value;
                OnPropertyChanged("LesItems");
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


        #region Méthodes
        private void InitNouvelleAnnonce()
        {
            NouvelleAnnonce = new Annonce();
            LesTypesAnnonce = new ObservableCollection<string>();
            AnnonceADO annonceADO = new AnnonceADO();
            LesTypesAnnonce = annonceADO.RecupererTypes();
            LesItems = new ObservableCollection<Item>();
        }

        private void cmdAjouter(object param)
        {
           
            NouvelleAnnonce.Titre = Titre;
            NouvelleAnnonce.Montant = Math.Round(Montant, 2);
            NouvelleAnnonce.Type = Type;
            NouvelleAnnonce.Description = Description;
            NouvelleAnnonce.ListeItems = LesItems;

            AnnonceADO annonceADO = new AnnonceADO();
            annonceADO.Ajouter(NouvelleAnnonce);

            IFenetreFermeable fenetre = param as IFenetreFermeable;
            if (fenetre != null)
            {
                //On confirme que l'utilisateur veut bien fermer la fenêtre
                fenetre.Fermer();
            }
        }

        private void cmdAjouterItem(object param)
        {
            IOuvreModal fenetre = param as IOuvreModal;
            fenetre.OuvrirModal();
        }

        private bool itemEstDansProposition(Item item)
        {
            foreach (Item i in LesItems)
            {
                if (item.Id == i.Id)
                {
                    return true;
                }
            }

            return false;
        }

        private void ajouterItemMessage(EnvoyerItemMessage msg)
        {
            if (!itemEstDansProposition(msg.Item))
            {
                LesItems.Add(msg.Item);
            }
            else
            {
                MessageBox.Show($"Cet objet existe déjà dans l'annonce.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
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

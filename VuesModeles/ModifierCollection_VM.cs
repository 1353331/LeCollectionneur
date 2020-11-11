using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LeCollectionneur.VuesModeles
{
    class ModifierCollection_VM : INotifyPropertyChanged
    {

        #region Propriétés
        private string _nomCollection;
        public string NomCollection 
        {
            get { return _nomCollection; }
            set 
            {
                _nomCollection = value;
                OnPropertyChanged("NomCollection");
                cmdModifierCollection = new Commande(cmdModifier_Collection, EstModifiee);
            }
        }
        private DateTime _dateCreation;
        public DateTime DateCreation 
        { 
            get { return _dateCreation; }
            set 
            {
                _dateCreation = value;
                OnPropertyChanged("DateCreation");
                cmdModifierCollection = new Commande(cmdModifier_Collection, EstModifiee);
            } 
        }
        private Collection _collectionAModifier;
        public Collection CollectionAModifier
        {
            get { return _collectionAModifier; }
            set {
                    _collectionAModifier = value;
                    OnPropertyChanged("CollectionAModifier");
                }
        }
        private CollectionADO gestionnaireCollections = new CollectionADO();
        #endregion

        #region Commandes
        // Collection
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
            IFenetreFermeable fenetre = param as IFenetreFermeable;
            if (NomCollection.Trim().Length > 0 && NomCollection.Trim().Length < 51)
            {
                
                CollectionAModifier.Nom = Validateur.Echappement(NomCollection.Trim());
                CollectionAModifier.DateCreation = DateCreation;
                gestionnaireCollections.Modifier(CollectionAModifier);
                // Fermer la fenêtre après modification.
                fenetre.Fermer();
            }
            else
            {
                // Ajouter un message d'erreur dans la fenêtre.
                MessageBox.Show("La longueur du nom doit être entre 1 et 50 caractères.");
            }

        }
        private ICommand _cmdAnnuler;
        public ICommand cmdAnnuler
        {
            get
            {
                return _cmdAnnuler;
            }
            set
            {
                _cmdAnnuler = value;
                OnPropertyChanged("cmdAnnuler");
            }
        }
        private void cmd_Annuler(object param)
        {
            IFenetreFermeable fenetre = param as IFenetreFermeable;
            fenetre.Fermer();
        }
        #endregion

        #region Constructeur
        public ModifierCollection_VM(Collection colAModifier)
        {
            CollectionAModifier = colAModifier;
            cmdModifierCollection = new Commande(cmdModifier_Collection,EstModifiee);
            cmdAnnuler = new Commande(cmd_Annuler);
            NomCollection = CollectionAModifier.Nom;
            DateCreation = CollectionAModifier.DateCreation;
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        #endregion

        #region Méthodes

        private bool EstModifiee()
        {
            return NomCollection != CollectionAModifier.Nom || DateCreation != CollectionAModifier.DateCreation;
        }

        #endregion
    }
}

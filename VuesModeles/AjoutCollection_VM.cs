﻿using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
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

namespace LeCollectionneur.VuesModeles
{
    class AjoutCollection_VM : INotifyPropertyChanged
    {
        #region Propriétés
        public string NomNeoCollection { get; set; }
        private CollectionADO gestionnaireCollections = new CollectionADO();
        #endregion

        #region Commandes
        // Collection
        private ICommand _cmdAjouterCollection;
        public ICommand cmdAjouterCollection
        {
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
            IFenetreFermeable fenetre = param as IFenetreFermeable;
            if (NomNeoCollection.Trim().Length > 0 && NomNeoCollection.Trim().Length < 51)
            {
                Collection neoCollection = new Collection();
                neoCollection.Nom = Validateur.Echappement(NomNeoCollection.Trim());
                gestionnaireCollections.Ajouter(neoCollection, UtilisateurADO.utilisateur.Id);
                UtilisateurADO.collection.Add(neoCollection);
                // Fermer la fenêtre après l'ajout.
                fenetre.Fermer();
            }
            else
            {
                // Ajouter un message d'erreur dans la fenêtre.
                MessageBox.Show( "La longueur du nom doit être entre 1 et 50 caractères.");
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
        public AjoutCollection_VM()
        {
            
            cmdAjouterCollection = new Commande(cmdAjouter_Collection);
            cmdAnnuler = new Commande(cmd_Annuler);
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

    }
}

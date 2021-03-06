﻿using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.Outils.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace LeCollectionneur.VuesModeles
{
    class ModalAjoutAnnonce_VM : INotifyPropertyChanged
    {
        public ICommand cmdAjouterItem_Annonce { get; set; }

        #region Constructeur
        public ModalAjoutAnnonce_VM()
        {
            //on initialise les commandes utiles au VM
            cmdAjouter_Annonce = new Commande(cmdAjouter, champsRemplis);
            cmdAjouterItem_Annonce = new Commande(cmdAjouterItem);
            cmdSupprimerItem_Annonce = new Commande(cmdSupprimerItem, presenceItem);
            cmdDetails_Annonce = new Commande(cmdDetails);

            //on initialise la nouvelle annonce et ses variables
            InitNouvelleAnnonce();

            //Abonnement à l'évènement Ajout d'un item à une annonce
            EvenementSysteme.Abonnement<EnvoyerItemsMessage>(ajouterItemsMessage);
        }
        #endregion

        #region Variables
        //Listes des types d'annonce possibles
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

        //La nouvelle annonce
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

        //Titre de l'annonce
        private string _titre { get; set; }
        public string Titre
        {
            get { return _titre; }
            set
            {
                _titre = value;
                cmdAjouter_Annonce = new Commande(cmdAjouter, champsRemplis);
                OnPropertyChanged("Titre");
            }
        }

        //Les items de la nouvelle annonce
        private ObservableCollection<Item> _lesItems { get; set; }
        public ObservableCollection<Item> LesItems
        {
            get { return _lesItems; }
            set
            {
                _lesItems = value;
                cmdAjouter_Annonce = new Commande(cmdAjouter, champsRemplis);
                OnPropertyChanged("LesItems");
            }
        }

        private bool _typeAnnonce;
        public bool TypeAnnonce
        {
            get { return _typeAnnonce; }
            set
            {
                _typeAnnonce = value;
                if (_typeAnnonce)
                {
                    Type = LesTypesAnnonce[0];
                }
                else
                {
                    Type = LesTypesAnnonce[1];
                }
                cmdAjouter_Annonce = new Commande(cmdAjouter, champsRemplis);
                OnPropertyChanged("TypeAnnonce");
            }
        }

        //Le type de la nouvelle annonce
        private string _type;
        public string Type
        {
            get { return _type; }
            set
            {
                _type = value;
                cmdAjouter_Annonce = new Commande(cmdAjouter, champsRemplis);
                OnPropertyChanged("Type");
            }
        }

        //La description de la nouvelle annonce
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

        //Le montant de la nouvelle annonce 
        private double _montant;
        public double Montant
        {
            get { return _montant; }
            set
            {
                _montant = Math.Round(value, 2);
                cmdAjouter_Annonce = new Commande(cmdAjouter, champsRemplis);
                OnPropertyChanged("Montant");
            }
        }

        //L'item sélectionné dans la liste des items de la nouvelle annonce
        private Item _itemSelectionne;
        public Item ItemSelectionne
        {
            get { return _itemSelectionne; }
            set
            {
                _itemSelectionne = value;
                OnPropertyChanged("ItemSelectionne");
            }
        }

        //La commande de publication de la nouvelle annonce 
        private ICommand _cmdAjouter_Annonce;
        public ICommand cmdAjouter_Annonce
        {
            get { return _cmdAjouter_Annonce; }
            set
            {
                _cmdAjouter_Annonce = value;
                OnPropertyChanged("cmdAjouter_Annonce");
            }
        }

        private ICommand _cmdSupprimerItem_Annonce;
        public ICommand cmdSupprimerItem_Annonce 
        {
            get { return _cmdSupprimerItem_Annonce; }
            set 
            {
                _cmdSupprimerItem_Annonce = value;
                OnPropertyChanged("cmdSupprimerItem_Annonce");
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
        #endregion

        #region Méthodes
        private void InitNouvelleAnnonce()
        {
            NouvelleAnnonce = new Annonce();
            LesTypesAnnonce = new ObservableCollection<string>();
            AnnonceADO annonceADO = new AnnonceADO();

            //On va récupérer les types d'annonce possibles
            LesTypesAnnonce = annonceADO.RecupererTypes();
            TypeAnnonce = false;

            LesItems = new ObservableCollection<Item>();
            Montant = 0;
        }

        //Méthode d'ajout de la nouvelle annonce en BD
        private void cmdAjouter(object param)
        {
            //Ici on veut ajouter l'annonce en BD
            NouvelleAnnonce.Titre = Validateur.Echappement(Titre.Trim());
            NouvelleAnnonce.Montant = Math.Round(Montant, 2);
            NouvelleAnnonce.Type = new TypeAnnonce(Type);
            if(String.IsNullOrWhiteSpace(Description))
            {
                NouvelleAnnonce.Description = "";
            }
            else
            {
                NouvelleAnnonce.Description = Validateur.Echappement(Description.Trim());
            }
            NouvelleAnnonce.ListeItems = LesItems;

            //On ajoute l'annonce en BD
            AnnonceADO annonceADO = new AnnonceADO();
            annonceADO.Ajouter(NouvelleAnnonce);

            //Puis on ferme la fenêtre
            IFenetreFermeable fenetre = param as IFenetreFermeable;
            if (fenetre != null)
            {
                //On confirme que l'utilisateur veut bien fermer la fenêtre
                fenetre.Fermer();
            }
        }

        //Méthode d'ouverture de la fenêtre d'ajout item
        private void cmdAjouterItem(object param)
        {
            //On ouvre la fenêtre d'ajout d'items
            IOuvreModalAvecParametre<IEnumerable<Item>> fenetre = param as IOuvreModalAvecParametre<IEnumerable<Item>>;
            fenetre.OuvrirModal(LesItems);

            //On met à jour la commande d'ajout d'annonce pour vérifier si elle est exécutable
            cmdAjouter_Annonce = new Commande(cmdAjouter, champsRemplis);
        }

        //Méthode de vérificaiton de si l'item est déjà présent dans la liste d'items de la nouvelle annonce
        private bool itemEstDansProposition(Item item)
        {
            foreach (Item i in LesItems)
            {
                if (item.Id == i.Id)
                    return true;
            }
            return false;
        }

        private void ajouterItemsMessage(EnvoyerItemsMessage msg)
        {
            List<string> itemsAjoutes = new List<string>();
            List<string> itemsDejaPresents = new List<string>();
            foreach (Item i in msg.Items)
            {
                //Si l'item n'est pas déjà présent dans la liste d'item, alors on l'ajoute à la liste, sinon on affiche le message d'erreur
                if (!itemEstDansProposition(i))
                {
                    LesItems.Add(i);
                    itemsAjoutes.Add(i.Nom);
                }
                else
                {
                    itemsDejaPresents.Add(i.Nom);
                }
            }

            if (itemsDejaPresents.Any())
            {
               MessageBox.Show($"Les items \"{String.Join(", ", itemsDejaPresents)}\" existent déjà dans l'annonce.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            if (itemsAjoutes.Any())
            {
               MessageBox.Show($"Vous avez ajouté les items \"{String.Join(", ", itemsAjoutes)}\"", "Ajout réussi", MessageBoxButton.OK, MessageBoxImage.Information);
            }

            //On met à jour la commande d'ajout d'annonce pour vérifier si elle est exécutable
            cmdAjouter_Annonce = new Commande(cmdAjouter, champsRemplis);
            cmdSupprimerItem_Annonce = new Commande(cmdSupprimerItem, presenceItem);
        }

        //Méthode pour supprimer un item de la liste d'item de la nouvelle annonce
        private void cmdSupprimerItem(object param)
        {
            //on s'assure qu'il y a bien un item de sélectionné
            if (ItemSelectionne == null)
                return;

            //On affiche le message de confirmation
            MessageBoxResult resultat = MessageBox.Show($"Voulez-vous vraiment supprimer {ItemSelectionne.Nom} de l'annonce?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //Si l'utilisateur veut bel et bien supprimer l'item de la liste, alors on retire l'item de la liste
            if (resultat == MessageBoxResult.Yes)
            {
                LesItems.Remove(ItemSelectionne);
                ItemSelectionne = null;
            }

            //On met à jour la commande d'ajout d'annonce pour vérifier si elle est exécutable
            cmdAjouter_Annonce = new Commande(cmdAjouter, champsRemplis);
            cmdSupprimerItem_Annonce = new Commande(cmdSupprimerItem, presenceItem);
        }

        //Méthode qui vérifie si les champs obligatoires sont remplis ou non
        private bool champsRemplis()
        {
            if (string.IsNullOrEmpty(Titre) || string.IsNullOrEmpty(Type) || LesItems.Count == 0 || Montant < 0)
                return false;

            //Les champs obligatoires sont remplis
            return true;
        }

        private void cmdDetails(object param)
        {
            IOuvreModalAvecParametre<Item> modal = param as IOuvreModalAvecParametre<Item>;
            if (modal != null)
            {
                modal.OuvrirModal(ItemSelectionne);
            }
        }

        private bool presenceItem()
        {
            return LesItems.Count > 0;
        }

        public void cmdFermer(object sender, CancelEventArgs e)
        {
           EvenementSysteme.Desabonnement<EnvoyerItemsMessage>(ajouterItemsMessage);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
            }
        }
        #endregion
    }
}

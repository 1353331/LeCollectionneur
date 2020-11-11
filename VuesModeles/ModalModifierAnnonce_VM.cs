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
    class ModalModifierAnnonce_VM : INotifyPropertyChanged
    {
        public ICommand cmdAjouterItem_Annonce { get; set; }

        #region Constructeur
        public ModalModifierAnnonce_VM(Annonce annonceLiee)
        {
            //on initialise les commandes utiles au VM
            cmdModifier_Annonce = new Commande(cmdModifier, champsRemplis);
            cmdAjouterItem_Annonce = new Commande(cmdAjouterItem);
            cmdSupprimerItem_Annonce = new Commande(cmdSupprimerItem, presenceItem);
            cmdDetails_Annonce = new Commande(cmdDetails);

            //on initialise la nouvelle annonce et ses variables
            InitModificationAnnonce(annonceLiee);

            //Abonnement à l'évènement Ajout d'un item à une annonce
            //EvenementSysteme.Abonnement<EnvoyerItemMessage>(ajouterItemMessage);
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
        private Annonce _annonceAMod { get; set; }
        public Annonce AnnonceAMod
        {
            get { return _annonceAMod; }
            set
            {
                _annonceAMod = value;
                OnPropertyChanged("AnnonceAMod");
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
                cmdModifier_Annonce = new Commande(cmdModifier, champsRemplis);
                OnPropertyChanged("Titre");
            }
        }

        //Les items de la nouvelle annonce
        private ObservableCollection<Item> _lesItemsMod { get; set; }
        public ObservableCollection<Item> LesItemsMod
        {
            get { return _lesItemsMod; }
            set
            {
                _lesItemsMod = value;
                cmdModifier_Annonce = new Commande(cmdModifier, champsRemplis);
                OnPropertyChanged("LesItemsMod");
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
                cmdModifier_Annonce = new Commande(cmdModifier, champsRemplis);
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
                cmdModifier_Annonce = new Commande(cmdModifier, champsRemplis);
                OnPropertyChanged("Montant");
            }
        }

        //L'item sélectionné dans la liste des items de la nouvelle annonce
        private Item _itemSelectionneMod;
        public Item ItemSelectionneMod
        {
            get { return _itemSelectionneMod; }
            set
            {
                _itemSelectionneMod = value;
                OnPropertyChanged("ItemSelectionneMod");
            }
        }

        //La commande de publication de la nouvelle annonce 
        private ICommand _cmdModifier_Annonce;
        public ICommand cmdModifier_Annonce
        {
            get { return _cmdModifier_Annonce; }
            set
            {
                _cmdModifier_Annonce = value;
                OnPropertyChanged("cmdModifier_Annonce");
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
        private void InitModificationAnnonce(Annonce annonceLiee)
        {
            AnnonceAMod = annonceLiee;

            LesTypesAnnonce = new ObservableCollection<string>();
            AnnonceADO annonceADO = new AnnonceADO();

            //On va récupérer les types d'annonce possibles
            LesTypesAnnonce = annonceADO.RecupererTypes();

            LesItemsMod = new ObservableCollection<Item>();

            foreach(Item i in AnnonceAMod.ListeItems)
            {
                LesItemsMod.Add(i);
            }

            Titre = AnnonceAMod.Titre;
            Type = AnnonceAMod.Type;
            Description = AnnonceAMod.Description;
            Montant = AnnonceAMod.Montant;
        }

        //Méthode de modification de l'annonce en BD
        private void cmdModifier(object param)
        {
            //Ici on veut ajouter l'annonce en BD
            AnnonceAMod.Titre = Titre.Trim();
            AnnonceAMod.Montant = Math.Round(Montant, 2);
            AnnonceAMod.Type = Type;
            if (String.IsNullOrWhiteSpace(Description))
            {
                AnnonceAMod.Description = "";
            }
            else
            {
                AnnonceAMod.Description = Description.Trim();
            }
            AnnonceAMod.ListeItems = LesItemsMod;

            //On modifie l'annonce en BD
            AnnonceADO annonceADO = new AnnonceADO();
            annonceADO.Modifier(AnnonceAMod);

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
            IOuvreModal fenetre = param as IOuvreModal;
            fenetre.OuvrirModal();

            //On met à jour la commande d'ajout d'annonce pour vérifier si elle est exécutable
            cmdModifier_Annonce = new Commande(cmdModifier, champsRemplis);
            cmdSupprimerItem_Annonce = new Commande(cmdSupprimerItem, presenceItem);
        }

        //Méthode de vérificaiton de si l'item est déjà présent dans la liste d'items de la nouvelle annonce
        private bool itemEstDansProposition(Item item)
        {
            foreach (Item i in LesItemsMod)
            {
                if (item.Id == i.Id)
                    return true;
            }
            return false;
        }

        //Méthode d'ajout d'item dans la liste d'item de la nouvelle annonce
        private void ajouterItemMessage(EnvoyerItemMessage msg)
        {
            //Si l'item n'est pas déjà présent dans la liste d'item, alors on l'ajoute à la liste, sinon on affiche le message d'erreur
            if (!itemEstDansProposition(msg.Item))
            {
                LesItemsMod.Add(msg.Item);
                MessageBox.Show($"Vous avez ajouté l'item {msg.Item.Nom}", "Ajout réussi", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
                MessageBox.Show($"Cet objet existe déjà dans l'annonce.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);

            //On met à jour la commande d'ajout d'annonce pour vérifier si elle est exécutable
            cmdModifier_Annonce = new Commande(cmdModifier, champsRemplis);
            cmdSupprimerItem_Annonce = new Commande(cmdSupprimerItem, presenceItem);
        }

        private void ajouterItemsMessage(EnvoyerItemsMessage msg)
        {
            foreach (Item i in msg.Items)
            {
                //Si l'item n'est pas déjà présent dans la liste d'item, alors on l'ajoute à la liste, sinon on affiche le message d'erreur
                if (!itemEstDansProposition(i))
                {
                    LesItemsMod.Add(i);
                    MessageBox.Show($"Vous avez ajouté l'item {i.Nom}", "Ajout réussi", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show($"L'objet {i.Nom} existe déjà dans l'annonce.", "Attention", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }

            //On met à jour la commande d'ajout d'annonce pour vérifier si elle est exécutable
            cmdModifier_Annonce = new Commande(cmdModifier, champsRemplis);
            cmdSupprimerItem_Annonce = new Commande(cmdSupprimerItem, presenceItem);
        }

        //Méthode pour supprimer un item de la liste d'item de la nouvelle annonce
        private void cmdSupprimerItem(object param)
        {
            //on s'assure qu'il y a bien un item de sélectionné
            if (ItemSelectionneMod == null)
                return;

            //On affiche le message de confirmation
            MessageBoxResult resultat = MessageBox.Show($"Voulez-vous vraiment supprimer {ItemSelectionneMod.Nom} de l'annonce?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);

            //Si l'utilisateur veut bel et bien supprimer l'item de la liste, alors on retire l'item de la liste
            if (resultat == MessageBoxResult.Yes)
            {
                LesItemsMod.Remove(ItemSelectionneMod);
                ItemSelectionneMod = null;
            }

            //On met à jour la commande d'ajout d'annonce pour vérifier si elle est exécutable
            cmdModifier_Annonce = new Commande(cmdModifier, champsRemplis);
            cmdSupprimerItem_Annonce = new Commande(cmdSupprimerItem, presenceItem);
        }

        //Méthode qui vérifie si les champs obligatoires sont remplis ou non
        private bool champsRemplis()
        {
            if (string.IsNullOrEmpty(Titre) || string.IsNullOrEmpty(Type) || LesItemsMod.Count == 0 || Montant < 0)
                return false;

            //Les champs obligatoires sont remplis
            return true;
        }

        private void cmdDetails(object param)
        {
            IOuvreModalAvecParametre<Item> modal = param as IOuvreModalAvecParametre<Item>;
            if (modal != null)
            {
                modal.OuvrirModal(ItemSelectionneMod);
            }
        }

        private bool presenceItem()
        {
            return LesItemsMod.Count > 0;
        }

        public void cmdFermer(object sender, CancelEventArgs e)
        {
           EvenementSysteme.Desabonnement<EnvoyerItemMessage>(ajouterItemMessage);
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

using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
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

namespace LeCollectionneur.VuesModeles
{
    class AjoutItemDansConversation : INotifyPropertyChanged
    {
        
        private Conversation conversation {get;set;}
        public Boolean _cv;
        public Boolean cv
        {
            get { return _cv; }
            set
            {
                _cv = value;
                OnPropertyChanged("cv");
            }
        }
        //Les items sélectionnés dans la liste des items de la nouvelle annonce
        private Item _itemsSelectionnes;
        public Item ItemsSelectionnes
        {
            get { return _itemsSelectionnes; }
            set
            {
                _itemsSelectionnes = value;

                cv = true;
                OnPropertyChanged("ItemsSelectionnes");
            }
        }

       

        //Liste des collections de l'utilisateur connecté
        public ObservableCollection<Collection> lstCollections { get; set; }

        //Variable de la collection sélectionnée
        private Collection _collectionSelectionnee;
        public Collection CollectionSelectionnee
        {
            get { return _collectionSelectionnee; }
            set
            {
               
                _collectionSelectionnee = value;
                ItemsCollectionSelectionnee = _collectionSelectionnee.ItemsCollection;

                
                OnPropertyChanged("CollectionSelectionnee");
            }
        }

        //Variable pour l'item collection sélectionné
        private ObservableCollection<Item> _itemsCollectionSelectionnee;
        public ObservableCollection<Item> ItemsCollectionSelectionnee
        {
            get { return _itemsCollectionSelectionnee; }
            set
            {
                _itemsCollectionSelectionnee = value;

               

                OnPropertyChanged("ItemsCollectionSelectionnee");
            }
        }

        
        //Variable pour la commande d'ajout d'un item
        private ICommand _cmdEnvoyItem;
        public ICommand cmdEnvoyItem
        {
            get { return _cmdEnvoyItem; }
            set
            {
                _cmdEnvoyItem = value;
                OnPropertyChanged("cmdAjouter_Item");
            }
        }
     

        private void cmdEnvoyer_Item(object param)
        {
            if(UnItemSelectionne())
            {
                var message = new Message();
                message.Contenu = ItemsSelectionnes.Id.ToString();
                message.item = true;
                message.idUtilisateur = UtilisateurADO.utilisateur.Id;
                ConversationADO.EnvoyerMessageStatic(message, Conversation_VM.conversationStatic);
            }
            else
            {
                MessageBox.Show("Veuiller Sélectionner un Item");
            }
        }
        //Constructeur
        public AjoutItemDansConversation()
        {


            cmdEnvoyItem = new Commande(cmdEnvoyer_Item);
            lstCollections = new CollectionADO().Recuperer(UtilisateurADO.utilisateur.Id);
            
        }

        //Méthode pour savoir si un item est sélectionné ou non
        public bool UnItemSelectionne()
        {
            if (ItemsSelectionnes is null)
            {
                return false;
            }
            return true;
        }

        public bool UneCollSelectionnee()
        {
            if (CollectionSelectionnee is null)
            {
                return false;
            }
            return true;
        }

        
       

        private void cmdAjouterColl(object param)
        {
            EvenementSysteme.Publier<EnvoyerItemsMessage>(new EnvoyerItemsMessage() { Items = CollectionSelectionnee.ItemsCollection });
           
        }

       
        #region OnPropertyChanged
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

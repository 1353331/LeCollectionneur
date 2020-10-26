using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeCollectionneur.VuesModeles
{
    class Conversation_VM : INotifyPropertyChanged
    {
        #region Propriétés
        int id;
        List<Message> conversations;
        private Utilisateur _utilisateur;
        public Utilisateur utilisateur
        {
            get { return _utilisateur; }
            set
            {
                _utilisateur = value;
                OnPropertyChanged("utilisateur");
            }
        }

        private string _messageContenu;
        public string messageContenu
        {
            get { return _messageContenu; }
            set
            {
                _messageContenu = value;
                OnPropertyChanged("messageContenu");
            }
        }
        private Conversation _conversation;
        public Conversation conversation
        {
            get { return _conversation; }
            set
            {
                _conversation = value;
                OnPropertyChanged("conversation");
            }
        }
        private ConversationADO _conversationADO = new ConversationADO();
        public ConversationADO conversationADO
        {
            get { return _conversationADO; }
            set
            {
                _conversationADO = value;
                OnPropertyChanged("conversationADO");
            }
        }

       
        private ObservableCollection<Conversation> _mesConversation;
        public ObservableCollection<Conversation> MesConversation
        {
            get { return _mesConversation; }

            set
            {
                _mesConversation = value;
                OnPropertyChanged("MesConversation");
            }
        }
        private Conversation _conversationSelectionne;
        public Conversation ConversationSelectionne
        {
            get { return _conversationSelectionne; }

            set
            {
                _conversationSelectionne = value;
                if (_conversationSelectionne == null)
                    return;
                Conversation temp = _conversationSelectionne;
                ConversationADO.Convo =temp;
                id = _conversationSelectionne.Id;
               
                utilisateur = _conversationSelectionne.UserAutre;
                conversations = _conversationSelectionne.ListMessage;


                OnPropertyChanged("ConversationSelectionne");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }



        #endregion

        #region Commandes
        private ICommand _cmdEnvoyerMessage;
        public ICommand cmdEnvoyerMessage
        {
            get
            {
                return _cmdEnvoyerMessage;
            }
         
            set
            {
                _cmdEnvoyerMessage = value;
                OnPropertyChanged("cmdEnvoyerMessage");
                
            }
        }

        private void cmdEnvoyerMessage_Message(object param)
        {
            if (messageContenu == "" || messageContenu==null || ConversationSelectionne ==null)
                return;
            ConversationSelectionne.lastMessage = messageContenu;
            conversationADO.EnvoyerMessage(messageContenu,id);
            
            messageContenu = "";
            OnPropertyChanged("cmdEnvoyerMessage_Message");
        }
        #endregion

        #region Constructeur
        public Conversation_VM()
        {
            cmdEnvoyerMessage = new Commande(cmdEnvoyerMessage_Message);
            GetConversations();
        }


        #endregion
        #region Méthodes
        public void EnvoyerMessage(Message message)
        {
            conversationADO.EnvoyerMessage(message.Contenu,id);
        }
        private void GetConversations()
        {
            MesConversation = new ObservableCollection<Conversation>();

            ConversationADO conversationADO = new ConversationADO();

            var temp = conversationADO.RecupererConversationUtilisateur();
            foreach(var d in temp)
            {
                d.lastMessage = conversationADO.GetMessages(d.Id)[conversationADO.GetMessages(d.Id).Count()-1].Contenu;
                MesConversation.Add(d);
            }

            OnPropertyChanged("MesConversation");

        }
        #endregion
    }
}

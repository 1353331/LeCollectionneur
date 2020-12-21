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
using LeCollectionneur.VuesModeles;
using System.Threading;
using LeCollectionneur.Vues;
using LeCollectionneur.EF;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Security.Cryptography;

namespace LeCollectionneur.VuesModeles
{
    class Conversation_VM : INotifyPropertyChanged
    {


        #region Propriétés
        int id;
        Thread thread;
       
        private string _emojiVisible;
        public string emojiVisible
        {
            get { return _emojiVisible; }
            set
            {
                _emojiVisible = value;
                OnPropertyChanged("emojiVisible");
            }
        }

        private bool stop;
        private bool stopThread
        {
            get { return stop; }
            set
            {
                stop = value;
                OnPropertyChanged("stopThread");
                if (value) {  wait();  }

            }
        }
        private Emoji.Wpf.EmojiData.Emoji _emoji;
        public Emoji.Wpf.EmojiData.Emoji  emoji
        {
            get { return _emoji; }
            set
            {
                _emoji = value;
                messageContenu = messageContenu + value.Text;
                
                OnPropertyChanged("emoji");
            }
        }

          
        private bool threadEnCour = false;
        public Boolean _cv ;
        public Boolean cv
        {
            get { return _cv; }
            set
            {
                _cv = value;
                OnPropertyChanged("cv");
            }
        }

        public static Modeles.Conversation conversationStatic { get; set; }

        private int _idItem;
        private int idItem
        {
            get { return _idItem; }
            set
            {
                try
                {

                    _idItem = value;
                }
                catch
                {
                    
                    _idItem = Convert.ToInt32(value);
                }
                OnPropertyChanged("idItem");
            }
        }

        

        private ObservableCollection<Emoji.Wpf.EmojiData.Emoji> _emojiAll;
        public ObservableCollection<Emoji.Wpf.EmojiData.Emoji> emojiAll
        {
            get
            {
                return _emojiAll;
            }
            set
            {
                _emojiAll = value;
                OnPropertyChanged("EmojiAll");
            }
        }

        private Utilisateur utlisateurASelection;
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
        private Modeles.Conversation _conversation;
        public Modeles.Conversation conversation
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


        private ObservableCollection<Modeles.Conversation> _mesConversation;
        public ObservableCollection<Modeles.Conversation> MesConversation
        {
            get { return _mesConversation; }

            set
            {
                _mesConversation = value;
                OnPropertyChanged("MesConversation");
            }
        }
        private Modeles.Conversation _conversationSelectionne;
        public Modeles.Conversation ConversationSelectionne
        {

            get { return _conversationSelectionne; }

            set
            {
                stopThread = true;
               
                wait();
                cv = true;
                _conversationSelectionne = value;
                
                if (_conversationSelectionne == null)
                    return;


                Modeles.Conversation temp = _conversationSelectionne;
                ConversationADO.Convo = temp;
                id = _conversationSelectionne.Id;

                utilisateur = _conversationSelectionne.UserAutre;
                utlisateurASelection = utilisateur;
                listMessage = ConversationADO.chercherMessage(_conversationSelectionne.UserAutre.Id);
               new ConversationADO().MettreMessagesVus(ConversationSelectionne.Id);

                stopThread = false;
                conversationStatic = _conversationSelectionne;
               
                OnPropertyChanged("ConversationSelectionne");
            }
        }
        private ObservableCollection<Message> _listMessage;
        public ObservableCollection<Message> listMessage
        {
            get { return _listMessage; }
            set
            {
                _listMessage = value;
                OnPropertyChanged("listMessage");
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

        private ICommand _cmdImage;
        public ICommand cmdImage
        {
            get { return _cmdImage; }
            set
            {
                _cmdImage = value;
                OnPropertyChanged("cmdImage");
            }
        }
        private ICommand _cmdEmoji;
        public ICommand cmdEmoji
        {
            get { return _cmdEmoji; }
            set
            {
                _cmdEmoji = value;
                OnPropertyChanged("cmdEmoji");
            }
        }

        private ICommand _cmdEnvoyerMessage;
        public ICommand cmdEnvoyerMessage
        {
            get
            {
                
                return _cmdEnvoyerMessage;
            }

            set
            {
                stopThread = true;
                _cmdEnvoyerMessage = value;
                stopThread = false;
                OnPropertyChanged("cmdEnvoyerMessage");
            }
        }
        private ICommand _cmdEnvoyerItem;
        public ICommand cmdEnvoyerItem
        {
            get
            {
                return _cmdEnvoyerItem;
            }

            set
            {
                stopThread = true;
                _cmdEnvoyerItem = value;
                stopThread = false;
                OnPropertyChanged("cmdEnvoyerMessage");
            }
        }
        private ICommand _cmdAfficher;
        public ICommand cmdAfficher
        {
            get
            {
                return _cmdAfficher;
            }

            set
            {
                stopThread = true;
                
                _cmdAfficher = value;
                stopThread = false;
                OnPropertyChanged("cmdAfficher");
            }
        }
        private void cmdAfficher_Afficher(object param)
        {
            MessageBox.Show("e");
        }
        private void cmdEnvoyerMessage_Item(object param)
        {
            stopThread = true;

            //Ouvrir Fenetre 
            ModalEnvoyerItemConversation temp = new ModalEnvoyerItemConversation();
            temp.ShowDialog();

            stopThread = false;
            OnPropertyChanged("cmdEnvoyerMessage_Message");
        }
        private void cmdEnvoyerMessage_Message(object param)
        {
            stopThread = true;

            if (messageContenu == "" || messageContenu == null || ConversationSelectionne == null)
            {

                return;
            }
            ConversationSelectionne.lastMessage = messageContenu;
            



            conversationADO.EnvoyerMessage(transform(messageContenu), id);
            messageContenu = "";
            listMessage = ConversationADO.chercherMessage(_conversationSelectionne.UserAutre.Id);
            stopThread = false;
            OnPropertyChanged("cmdEnvoyerMessage_Message");
        }

        private ICommand _cmdAjouterConversation;
        public ICommand cmdAjouterConversation
        {
            get
            {
                return _cmdAjouterConversation;
            }
            set
            {
                _cmdAjouterConversation = value;
                OnPropertyChanged("cmdAjouterMessage");
            }
        }

        public void cmdAjouter_Conversation(object param)
        {
            stopThread = true;

            LeCollectionneur.Vues.ajouterConversation ajouterConversation = new LeCollectionneur.Vues.ajouterConversation();
            ajouterConversation.ShowDialog();
            
            GetConversations();
            stopThread = false;
            OnPropertyChanged("MesConversation");
        }

        public void cmdEmojiAjouter(object param)
        {
            if (emojiVisible == "Hidden")
                emojiVisible = "Visible";
            else
                emojiVisible = "Hidden";
            
            OnPropertyChanged("lsbEmoji");
        }
        public void cmdEnvoyImage(object param)
        {
            var e = Fichier.ImporterFichierJPGPNGConversation();
            if(e !="")
            {
            var message =new Message();
                message.Contenu = e;
                message.Date = DateTime.Now;
                message.image = true;
                message.idUtilisateur = UtilisateurADO.utilisateur.Id;
                message.emoji = false;
                message.vue = false;
                message.item = false;
            var modal = new modalEnvoyerImage(e);
            modal.ShowDialog();
            string adresseFichier = "";
            if(modal.OKButtonClicked)
                {
                    adresseFichier=Fichier.TeleverserFichierFTPConversation(e);
                    ConversationADO.EnvoyerMessageStatic2(message,ConversationSelectionne);
                }
            }
        }
        #endregion

        #region Constructeur
        public Conversation_VM()
        {
            cmdAfficher = new Commande(cmdAfficher_Afficher);
            
            cmdEnvoyerMessage = new Commande(cmdEnvoyerMessage_Message);
            cmdAjouterConversation = new Commande(cmdAjouter_Conversation);
            cmdEnvoyerItem = new Commande(cmdEnvoyerMessage_Item);
            cmdEmoji = new Commande(cmdEmojiAjouter);
            cmdImage = new Commande(cmdEnvoyImage);
            GetConversations();
            thread = new Thread(RefreshMessage);
            thread.Name = "Fillon Principal";
            stopThread = false;
            emojiVisible = "Hidden";
            
            emojiAll =Message.lstEmoji;
            thread.Start();
        }
       


        #endregion

        #region Méthodes
        private ObservableCollection<Emoji.Wpf.EmojiData.Emoji> getEmojie()
        {
            return emojiAll;
        }
        private void wait()
        {
            
            while (threadEnCour) { 
            }
            
        }
        private void RefreshMessage()
        {
            //Valide que l'on est sur conversation
            while (UCContexteUtilisateur.onConversation)
            {
                if (_conversationSelectionne != null && !stopThread)
                {
                    threadEnCour = true;
                    listMessage = ConversationADO.chercherMessage(_conversationSelectionne.UserAutre.Id);
                    
                    OnPropertyChanged("listMessage");
                    threadEnCour = false;
                }


                Thread.Sleep(2000);
            }
        }
        public void EnvoyerMessage(Message message)
        {

            stopThread = true;
            
            conversationADO.EnvoyerMessage(message.Contenu, id);
            stopThread = false;
        }
        private void GetConversations()
        {
            stopThread = true;
           
            MesConversation = new ObservableCollection<Modeles.Conversation>();

            ConversationADO conversationADO = new ConversationADO();

            var temp = conversationADO.RecupererConversationUtilisateur();
            foreach (var d in temp)
            {
                try
                {
                    d.lastMessage = conversationADO.GetMessages(d.Id)[conversationADO.GetMessages(d.Id).Count() - 1].Contenu;
                    d.date = conversationADO.GetMessages(d.Id)[conversationADO.GetMessages(d.Id).Count() - 1].Date;

                    MesConversation.Add(d);
                }
                catch
                {

                    d.lastMessage = "";
                    d.date = null;

                    MesConversation.Add(d);
                }
            }
            stopThread = false;
            OnPropertyChanged("MesConversation");

        }

       
      
        private class emor
        {

            public int id;
            public string emo;
            public emor()
            {

            }
           
        }
        private string transform(string message)
       {
            var envoyer = "";
            var tempAll = getEmojie();
            var tempAllGood = new List<emor>();
            var i = 0;
            foreach (var item in tempAll)
            {
                var e = new emor();
                e.id = i;
                e.emo = item.Text;
                tempAllGood.Add(e);
                i++;
            }
            string sub = "";
            bool z = true;
            for (int iterateur = 0; iterateur < message.Length; iterateur++)
            {
                try
                {

                    z = true;
                if(message.Length -iterateur >=2)
                    sub = message.Substring(iterateur,2);
                //Valide que les caratere sont un emoji
                foreach (var item in tempAllGood)
                {
                    if(sub == item.emo)
                    {
                        z = false;
                        if(item.id <10)
                        {
                            envoyer += "{{0" + item.id +"}}";
                            iterateur++;
                        }
                        else
                        {
                            envoyer += "{{" + item.id + "}}";
                            iterateur++;
                        }
                    }
                }
                if (z)
                    envoyer += message[iterateur];
                }
                catch 
                { 
                }
            }
            return envoyer;
       }
        
        #endregion
    }
    
}

using LeCollectionneur.Modeles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.VuesModeles
{
    class Conversation_VM : INotifyPropertyChanged
    {
        #region Propriétés
        private ConversationADO conversationADO = new ConversationADO();
        
        private ObservableCollection<Utilisateur> _mesConversation;
        public ObservableCollection<Utilisateur> MesConversation
        {
            get { return _mesConversation; }

            set
            {
                _mesConversation = value;
                OnPropertyChanged("MesCollection");
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
        #endregion

        #region Constructeur
        public Conversation_VM()
        {
            GetConversations();
        }


        #endregion
        #region Méthodes
        private void GetConversations()
        {
            MesConversation = new ObservableCollection<Utilisateur>();

            ConversationADO conversationADO = new ConversationADO();

            var temp = conversationADO.RecupererConversationUtilisateur();
            foreach(var d in temp)
            {
                MesConversation.Add(d.UserAutre);
            }
            var e =temp;
        }
        #endregion
    }
}

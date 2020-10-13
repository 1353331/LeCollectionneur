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
        #region Proprieter
        private ConversationADO conversationADO = new ConversationADO();
        
        private ObservableCollection<Conversation> _sommaireConversation;
        public ObservableCollection<Conversation> SommaireConversation
        {
            get { return _sommaireConversation; }

            set
            {
                _sommaireConversation = value;
                OnPropertyChanged("SommaireConversation");
            }
        }
        private Conversation _conversationSelectionne;
        public Conversation ConversationSelctionne
        {
            get { return _conversationSelectionne; }

            set
            {
                _conversationSelectionne = value;
                if (_conversationSelectionne == null)
                    return;

               
            }
        }

        


        #endregion
        
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
    }
}

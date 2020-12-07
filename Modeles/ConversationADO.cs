using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
    class ConversationADO
    {
        private static BdBase BD = new BdBase();
        public static Conversation Convo { get; set; }
        public static List<Message> messages {
            get { return GetMessages(); }
            set { }
        }
        //Variable pour savoir si l'utilisateur est Utilisateur1 en Bd
        public static void EnvoyerMessageStatic(Message message,Conversation convo)
        {
            //"INSERT INTO `messages` (`Date`, `Message`, `IdConversation`, `IdUtilisateur`) VALUES ( '" + message.Date.ToString("yyyy-MM-dd hh:mm:ss")+"', '"+message.Contenu+"', '"+Convo.Id+"', '"+message.idUtilisateur+"');";
            string req = "INSERT INTO `messages` (`Date`,`Message`,`Conversation_Id`,`item`,`image`,`emoji`,`utilisateur_Id`) " +
                "VALUES('"+DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss") + "' , '"+message.Contenu+"',"+convo.Id+","+true+","+false+","+false+","+UtilisateurADO.utilisateur.Id+");";
            BD.Commande(req);
        }
        public ConversationADO()
        {
            
        }
         public Conversation RetourCoversation()
        {
            return Convo;
        }
        public List<Message> GetMessages(int idConversation)
        {
            string req = "SELECT * FROM `messages` WHERE Conversation_Id = " + idConversation;
            DataSet MessageConvo = BD.Selection(req);
            List<Message> temp = new List<Message>();

            for (int i = 0; i < MessageConvo.Tables[0].Rows.Count; i++)
            {
                temp.Add(new Message(MessageConvo.Tables[0].Rows[i],true));
            }
            return temp;
        }
        public static List<Message> GetMessagesList(int idConversation)
        {
            string req = "SELECT * FROM `messages` WHERE Conversation_Id = " + idConversation;
            DataSet MessageConvo = BD.Selection(req);
            List<Message> temp = new List<Message>();

            for (int i = 0; i < MessageConvo.Tables[0].Rows.Count; i++)
            {
                temp.Add(new Message(MessageConvo.Tables[0].Rows[i],true));
            }
            return temp;
        }
        //Retourne les message de la conversation
        private static List<Message> GetMessages()
        {
            if(Convo !=null)
            {

                //1. Aller chercher l'id de la conversation
                    //1.2 Si elle n'existe pas on la créé
                Convo.Id = ChercherIdConversation(Convo.UserActif,Convo.UserAutre);
                //2. On va chercher tout les messages des utilisateurs
                string req = "SELECT * FROM `messages` WHERE Conversation_Id = " + Convo.Id;
                DataSet MessageConvo = BD.Selection(req);


                List<Message> temp = new List<Message>();
                
                for (int i = 0; i < MessageConvo.Tables[0].Rows.Count; i++)
                {
                    temp.Add(new Message(MessageConvo.Tables[0].Rows[i],true));
                }
                Convo.ListMessage = temp;

                return Convo.ListMessage;
            }
            return new List<Message>();

        }
        public static int ChercherIdConversation(Utilisateur UserActif, Utilisateur UserAutre)
        {
            // Deux possibilité d'eregistrement en bd
            string req = "SELECT id FROM `conversations` WHERE Utilisateur1_Id = " + UserActif.Id + " AND Utilisateur2_Id = " + UserAutre.Id;
            string req2 = "SELECT id FROM `conversations` WHERE Utilisateur1_Id = " + UserAutre.Id + " AND Utilisateur2_Id = " + UserActif.Id;

            DataSet Convo = BD.Selection(req);
            //Regarde si la onversation existe
            if (Convo.Tables[0].Rows.Count == 0)
            {
                //Elle n'existe pas on passe à l'autre
                Convo = BD.Selection(req2);
                if (Convo.Tables[0].Rows.Count == 0)
                    //La conversation entre les Utilisateurs n'existe pas encore
                    return CreerNouvelleConversation( UserActif, UserAutre);
                
                return (int)Convo.Tables[0].Rows[0]["id"];
            }
            return (int)Convo.Tables[0].Rows[0]["id"];

        }

        public static ObservableCollection<Message> chercherMessage(int idUtilisateurAutre)
        {
            
            //1: Trouver la conversation
            string req = "SELECT* FROM  conversations WHERE (Utilisateur1_Id = " + UtilisateurADO.utilisateur.Id + " AND Utilisateur2_Id = " + idUtilisateurAutre + ")OR (Utilisateur1_Id = " + idUtilisateurAutre + " AND Utilisateur2_Id = " + UtilisateurADO.utilisateur.Id+");";
            var t = BD.Selection(req);
            Conversation tf = new Conversation(t);
            var temp = new ObservableCollection<Message>();
            var e = GetMessagesList(tf.Id);
            foreach (var item in e)
            {
                temp.Add(item);
            }
            return temp;
        }
        public static int CreerNouvelleConversation(Utilisateur UserActif, Utilisateur UserAutre)
        {
            

                string req = "INSERT INTO `conversations` (`Utilisateur1_Id`, `Utilisateur2_Id`) VALUES (" + UserActif.Id + ", " + UserAutre.Id + ");";
                BD.Commande(req);
                req = "SELECT id FROM `conversations` WHERE Utilisateur1_Id = " + UserActif.Id + " AND Utilisateur2_Id = " + UserAutre.Id;
                DataSet temp = BD.Selection(req);
                return (int)temp.Tables[0].Rows[0]["id"];
            
        }
       

        public void EnvoyerMessage(string Contenu)
        {
            Message message = new Message(Contenu, UtilisateurADO.utilisateur,false);
            string req = "INSERT INTO `messages` (`Date`, `Message`, `Conversation_Id`, `utilisateur_Id`) VALUES ( '" + message.Date.ToString("yyyy-MM-dd hh:mm:ss")+"', '"+message.Contenu+"', '"+Convo.Id+"', '"+message.idUtilisateur+"');";
            BD.Commande(req);
            GetMessages();
        }

        public void EnvoyerMessage(string Contenu,int idConversation)
        {
            Message message = new Message(Contenu, UtilisateurADO.utilisateur,false);
            string req = "INSERT INTO `messages` (`Date`, `Message`, `Conversation_Id`, `utilisateur_Id`) VALUES ( '" + message.Date.ToString("yyyy-MM-dd hh:mm:ss") + "', '" + message.Contenu + "', '" + idConversation + "', '" + message.idUtilisateur + "');";
            BD.Commande(req);
            GetMessages();
        }

        public List<Conversation> RecupererConversationUtilisateurList()
        {
            List<Conversation> ListConversation = new List<Conversation>();
            
            string req = "SELECT * FROM  conversations WHERE Utilisateur1_Id = " + UtilisateurADO.utilisateur.Id+ " OR Utilisateur2_Id=" + UtilisateurADO.utilisateur.Id;
            
            DataSet conversation = BD.Selection(req);
            for (int i = 0; i < conversation.Tables[0].Rows.Count; i++)
            {
                ListConversation.Add(new Conversation(conversation));
            }           
            return ListConversation;
        }
        public ObservableCollection<Conversation> RecupererConversationUtilisateur()
        {
            ObservableCollection<Conversation> ListConversation = new ObservableCollection<Conversation>();

            string req = "SELECT * FROM  conversations WHERE Utilisateur1_Id = " + UtilisateurADO.utilisateur.Id + " OR Utilisateur2_Id = " + UtilisateurADO.utilisateur.Id;
            
            DataSet conversation = BD.Selection(req);
            for (int i = 0; i < conversation.Tables[0].Rows.Count; i++)
            {
                ListConversation.Add(new Conversation(conversation.Tables[0].Rows[i]));
            }
            return ListConversation;
        }
       
    }
}

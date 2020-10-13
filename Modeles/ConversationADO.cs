using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
    class ConversationADO
    {
        BdBase BD = new BdBase();
        Conversation Convo { get; set; }
        //Variable pour savoir si l'utilisateur est Utilisateur1 en Bd

        public ConversationADO()
        {
            
        }
         public Conversation RetourCoversation()
        {
            return Convo;
        }
        //Retourne les message de la conversation
        private void GetMessages()
        {
            //1. Aller chercher l'id de la conversation
                //1.2 Si elle n'existe pas on la créé
            Convo.Id = ChercherIdConversation(Convo.UserActif,Convo.UserAutre);
            //2. On va chercher tout les messages des utilisateurs
            string req = "SELECT * FROM `messages` WHERE IdConversation = " + Convo.Id;
            DataSet MessageConvo = BD.Selection(req);


            List<Message> temp = new List<Message>();
            
            for (int i = 0; i < MessageConvo.Tables[0].Rows.Count; i++)
            {
                temp.Add(new Message(MessageConvo.Tables[0].Rows[i]));
            }
            Convo.ListMessage = temp;

            

        }
        private int ChercherIdConversation(Utilisateur UserActif, Utilisateur UserAutre)
        {
            // Deux possibilité d'eregistrement en bd
           
            string req = "SELECT id FROM `conversations` WHERE IdUtilisateur1 = " + UserActif.Id + " AND IdUtilisateur2 = " + UserAutre.Id;
            string req2 = "SELECT id FROM `conversations` WHERE IdUtilisateur1 = " + UserAutre.Id + " AND IdUtilisateur2 = " + UserActif.Id;

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
       
        private int CreerNouvelleConversation(Utilisateur UserActif, Utilisateur UserAutre)
        {
            string req = "INSERT INTO `conversations` (`IdUtilisateur1`, `IdUtilisateur2`) VALUES (" + UserActif.Id + ", " + UserAutre.Id + ");";
            BD.Commande(req);
            req = "SELECT id FROM `conversations` WHERE IdUtilisateur1 = " + UserActif.Id + " AND IdUtilisateur2 = " + UserAutre.Id;
            DataSet Convo = BD.Selection(req);
            return (int)Convo.Tables[0].Rows[0]["id"];
        }

        public void EnvoyerMessage(string Contenu)
        {
            Message message = new Message(Contenu, Convo.UserActif);
            string req = "INSERT INTO `messages` (`Date`, `Message`, `IdConversation`, `IdUtilisateur`) VALUES ( '"+message.Date.ToString("yyyy-MM-dd hh:mm:ss")+"', '"+message.Contenu+"', '"+Convo.Id+"', '"+message.idUtilisateur+"');";
            BD.Commande(req);
            GetMessages();
        }

        public List<Conversation> RecupererConversationUtilisateur()
        {
            List<Conversation> ListConversation = new List<Conversation>();
            
            string req = "SELECT * FROM  conversations WHERE idUtilisateur1 = "+ UtilisateurADO.utilisateur.Id+ "OR idUtilisateur2=" + UtilisateurADO.utilisateur.Id;
            
            DataSet conversation = BD.Selection(req);
            for (int i = 0; i < conversation.Tables[0].Rows.Count; i++)
            {
                ListConversation.Add(new Conversation(conversation));
            }           
            return ListConversation;
        }
    }
}

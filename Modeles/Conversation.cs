using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
    class Conversation
    {
        #region  Variable
        
        public int Id { get; set; }
        public Utilisateur UserActif;
        public Utilisateur UserAutre;
        
        public List<Message> ListMessage{ get; set; }
        public string NomUserAutre { get; set; }
        public string lastMessage { get; set; }
        public DateTime date { get; set; }
        #endregion

        #region Constructeur
        //Contructeur Vide
        public Conversation(){ }
        //Constructeur Avec 2 Utilisateur
        public Conversation(Utilisateur UtilisateurActif,Utilisateur UtilisateurAutre)
        {
            this.UserActif = UtilisateurActif;
            this.UserAutre = UtilisateurAutre;
            this.NomUserAutre = UserAutre.NomUtilisateur;
            this.lastMessage = getLastMessage().Contenu;
            this.date = getLastMessage().Date;
        }
        //Constructeur avec DataSet
        public Conversation(DataSet data)
        {
            this.Id = (int)data.Tables[0].Rows[0]["id"];
            this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["IdUtilisateur1"]));
            this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["IdUtilisateur2"]));
            this.NomUserAutre = UserAutre.NomUtilisateur;
            this.lastMessage = getLastMessage().Contenu;
            this.date = getLastMessage().Date;
        }
        //Contructeur avec DataRow
        public Conversation(DataRow data)
        {
            this.Id = (int)data["id"];
            this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["IdUtilisateur1"]));
            this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["IdUtilisateur2"]));
            this.NomUserAutre = UserAutre.NomUtilisateur;
            this.lastMessage = getLastMessage().Contenu;
            this.date = getLastMessage().Date;
        }
        //Constructeur Avec 2 Utilisateur
        public Conversation(Utilisateur UtilisateurActif, Utilisateur UtilisateurAutre,List<Message> messages)
        {
            this.ListMessage = messages;
            this.UserActif = UtilisateurActif;
            this.UserAutre = UtilisateurAutre;
            this.NomUserAutre = UserAutre.NomUtilisateur;
            this.lastMessage = getLastMessage().Contenu;
            this.date = getLastMessage().Date;
        }
        //Constructeur avec DataSet
        public Conversation(DataSet data, List<Message> messages)
        {
            this.ListMessage = messages;
            this.Id = (int)data.Tables[0].Rows[0]["id"];
            this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["IdUtilisateur1"]));
            this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["IdUtilisateur2"]));
            this.NomUserAutre = UserAutre.NomUtilisateur;
            this.lastMessage = getLastMessage().Contenu;
            this.date = getLastMessage().Date;
        }
        //Contructeur avec DataRow
        public Conversation(DataRow data, List<Message> messages)
        {
            this.ListMessage = messages;
            this.Id = (int)data["id"];
            this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["IdUtilisateur1"]));
            this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["IdUtilisateur2"]));
            this.NomUserAutre = UserAutre.NomUtilisateur;
            this.lastMessage = getLastMessage().Contenu;
            this.date = getLastMessage().Date;
        }
        #endregion

        #region Method
        public Message getLastMessage()
        {
            ListMessage = ConversationADO.messages;
            if (ListMessage == null || ListMessage.Count ==0)
                return new Message();
            else
                return ListMessage[ListMessage.Count() - 1];
        }
        
        #endregion
    }
}

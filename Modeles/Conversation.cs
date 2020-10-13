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
        #endregion

        #region Constructeur
        //Contructeur Vide
        public Conversation(){ }
        //Constructeur Avec 2 Utilisateur
        public Conversation(Utilisateur UtilisateurActif,Utilisateur UtilisateurAutre)
        {
            this.UserActif = UtilisateurActif;
            this.UserAutre = UtilisateurAutre;  
        }
        //Constructeur avec DataSet
        public Conversation(DataSet data)
        {
            this.Id = (int)data.Tables[0].Rows[0]["id"];
            this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["IdUtilisateur1"]));
            this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["IdUtilisateur2"]));
        }

        #endregion
        #region Method
        

        #endregion
    }
}

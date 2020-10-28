using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
    class Message
    {
        #region Variable
        public string Contenu { get; set; }
        public DateTime Date { get; set; }
        public int idUtilisateur { get; set; } 
        public Utilisateur user { get; set; }
        #endregion
        
        #region Constructeur
        public Message(string Contenu,Utilisateur UtilisateurActif) 
        {
            this.Contenu = Contenu;
            this.idUtilisateur = UtilisateurActif.Id;
            this.Date = DateTime.Now;
            this.user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur);
        }
        public Message(DataSet Data)
        {
            Date =(DateTime) Data.Tables[0].Rows[0]["Date"];
            Contenu = Data.Tables[0].Rows[0]["Message"].ToString();
            idUtilisateur = (int)Data.Tables[0].Rows[0]["IdUtilisateur"];
            this.user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur);
        }
        public Message(DataRow Data)
        {
            Date = (DateTime)Data[1];
            Contenu = Data[2].ToString();
            idUtilisateur = (int)Data[4];
            this.user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur);
        }
        public Message() { }
        #endregion
        #region Method

        public int RetourIdUtilisateur()
        {
            return idUtilisateur;
        }
        #endregion
    }
}

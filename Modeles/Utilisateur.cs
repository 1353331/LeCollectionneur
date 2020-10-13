using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
    public class Utilisateur
    {
        #region Variable
        //Variable nécésaire pour le fonctionnement de l'application
        public int Id { get; set; }
        public string NomUtilisateur { get; set; }
        public string Courriel { get; set; }
        #endregion

        #region Constructeur
        //Constructeur Complet 
        public Utilisateur(int Id, string NomUtilisateur,string Courriel)
        {
            //Assignation des Champs
            this.Id = Id;
            this.NomUtilisateur = NomUtilisateur;
            this.Courriel = Courriel;
        }
        //Contrusteur par DataSet
        public Utilisateur(DataSet Compte)
        {
            this.Id = (int)Compte.Tables[0].Rows[0]["id"];
            this.NomUtilisateur = Compte.Tables[0].Rows[0]["NomUtilisateur"].ToString();
            this.Courriel = Compte.Tables[0].Rows[0]["Courriel"].ToString();
        }
        //Constructeur Vide pour Utilisateur Ado
        public Utilisateur(){ }
        #endregion
    }
}

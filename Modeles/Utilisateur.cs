using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
   [Table("Utilisateurs")]
    public class Utilisateur
    {
        #region Variable
        //Variable nécésaire pour le fonctionnement de l'application
        public int Id { get; set; }
        public string NomUtilisateur { get; set; }
        public string MotDePasse { get; set; }                                      
        public string Courriel { get; set; }
        public string Role { get; set; }
        public bool EstActif { get; set; }
        public ObservableCollection<Collection> MesCollections { get; set; }
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

            //VC Ajout des colonnes EstActif et Role
            EstActif = (bool)Compte.Tables[0].Rows[0]["EstActif"];
            Role = Compte.Tables[0].Rows[0]["Role"].ToString();
        }
        //Contrusteur par DataRow
        public Utilisateur(DataRow Compte)
        {
            this.Id = (int)Compte["id"];
            this.NomUtilisateur = Compte["NomUtilisateur"].ToString();
            this.Courriel = Compte["Courriel"].ToString();
        }
        public Utilisateur(DataRow donnees,bool versionAdmin)
        {
            Id = (int)donnees["id"];
            NomUtilisateur = (string)donnees["nomUtilisateur"];
            Role = (string)donnees["role"];
            EstActif = (bool)donnees["estActif"];
        }
        //Constructeur Vide pour Utilisateur Ado
        public Utilisateur(){ }
        #endregion
    }
}

using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeCollectionneur.Modeles
{
    class UtilisateurADO
    {
        public BdBase BD = new BdBase();
        #region Variable
        Utilisateur utilisateur;

        #endregion

        #region Constructeur
        //Constructeur Vide
        public UtilisateurADO() { }
        #endregion

        #region Method
        //Method de validation des champs entré par L'utilisateur
        public bool InfoValideConnection(string User, string MP)
        {
            //Va chercher le compte utlisant le User
            DataSet compte = GetUser(User);
            //Valide que le user existe dans la bd
            if (compte.Tables[0].Rows.Count == 0)
                //Si la fonction a GetUser n'a pas pu retourner d'info alors le compte n'existe pas
                return false;

            if (compte.Tables[0].Rows[0]["MotDePasse"].ToString() != MP)
                // Si le mot de passe n'est pas valide on retourne false
                return false;
            //Les donnée saisie sont valide 
            return true;
        }

        //Method qui retourne un compte par le User
        private DataSet GetUser(string User)
        {
            string req = "SELECT * FROM `utilisateurs` WHERE NomUtilisateur = '" + User + "'";
            return BD.Selection(req);
        }

        //Connecte L'utilisateur rentre ces informations dans un objet Utilisateur
        public void Connection(string User,string MP)
        {
            if(InfoValideConnection(User,MP))
            {
                utilisateur = new Utilisateur(GetUser(User)); 
            }
        }

        //Créé un compte en BD
        public void CreerCompte()
        {

        }


        #endregion
    }
}

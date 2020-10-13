using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeCollectionneur.Modeles
{
    class UtilisateurADO
    {
        public static BdBase BD = new BdBase();
        #region Variable
        public static Utilisateur utilisateur;

        #endregion

        #region Constructeur
        //Constructeur Vide
        public UtilisateurADO() { }
        #endregion

        #region Method
        //Retourne Un utilisateur par son Id, si l'utilisateur n'existe pas retourne un utilisateur vide
        public Utilisateur RechercherUtilisateurById(int Id)
        {
            //Va chercher le compte utlisant le User
            DataSet compte = GetUserDataSet(Id);
            //Valide que le user existe dans la bd
            if (compte.Tables[0].Rows.Count == 0)
                //Si la fonction a GetUser n'a pas pu retourner d'info alors le compte n'existe pas
                return new Utilisateur();

            return new Utilisateur(compte);
        }
        //Retourne Un utilisateur par son User, si l'utilisateur n'existe pas retourne un utilisateur vide
        public Utilisateur RechercheUtilisateurByUser(string User)
        {
            //Va chercher le compte utlisant le User
            DataSet compte = GetUserDataSet(User);
            //Valide que le user existe dans la bd
            if (compte.Tables[0].Rows.Count == 0)
                //Si la fonction a GetUser n'a pas pu retourner d'info alors le compte n'existe pas
                return new Utilisateur();

            return new Utilisateur(compte);
        }
        //Method de validation des champs entré par L'utilisateur
        public bool InfoValideConnection(string User, string MP)
        {
            //Va chercher le compte utlisant le User
            DataSet compte = GetUserDataSet(User);
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
        static public DataSet GetUserDataSet(string User)
        {
            string req = "SELECT * FROM `utilisateurs` WHERE NomUtilisateur = '" + User + "'";
            return BD.Selection(req);
        }
        //Method qui retourne un compte par l'id
        static public  DataSet GetUserDataSet(int Id)
        {
            string req = "SELECT * FROM `utilisateurs` WHERE Id =" + Id + ";";
            return BD.Selection(req);
        }


        //Connecte L'utilisateur rentre ces informations dans un objet Utilisateur
        public void Connection(string User, string MP)
        {
            if (InfoValideConnection(User, MP))
            {
                utilisateur = new Utilisateur(GetUserDataSet(User));
            }
        }

        //Créé un compte en BD, retourne false si le compte na pas pu être créé
        public bool CreerCompte(string User, string MP, string MPConfirme, string Courriel)
        {
            //1. Valide que les mot de passe sont pareil
            if (MP != MPConfirme)
                return false;
            //2. Valide que le nom d'utilisateur n'est pas prit
            if (CheckSiUserEstPrit(User))
                return false;
            //3. Valide le format du Courriel
            if (ValideCourriel(Courriel))
                return false;
            //4. On créé le compte et on se connecte
            AjoutCompte(User,MP,Courriel);
            Connection(User,MP);
            return true;
        }

        //Method qui retourne true si le nom utilisateur est déja utilisé
        private bool CheckSiUserEstPrit(string User)
        {
            //Va chercher le compte utlisant le User
            DataSet compte = GetUserDataSet(User);
            //Valide que le user existe dans la bd
            if (compte.Tables[0].Rows.Count == 0)
                //Si la fonction a GetUser n'a pas pu retourner d'info alors le compte n'existe pas
                return false;
            return true;
        }
        //Valide le courriel
        private bool ValideCourriel(string Courriel)
        {
            try
            {
                MailAddress courriel = new MailAddress(Courriel);
                return false;
            }
            catch (Exception e)
            {
                return true;
            }
        }

        //Method d'ajout de compte
        private void AjoutCompte(string User, string MP, string Courriel)
        {
            string req = "INSERT INTO `utilisateurs` (NomUtilisateur, MotDePasse, Courriel) VALUES('" + User+"','"+MP+"' ,'"+Courriel+"' )";
            BD.Commande(req);
        }

        //Method de decconection
        public void Deconnection()
        {
            utilisateur = new Utilisateur();
        }

        public Utilisateur RetourUtilisateurActif()
        {
            return utilisateur;
        }



        #endregion
    }
}

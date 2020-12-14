using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public static ObservableCollection<Collection> collection;
        public static bool admin;
        public static bool connectionProf;
        #endregion

        #region Constructeur
        //Constructeur Vide
        public UtilisateurADO() { }
        #endregion

        #region Method
        public bool connectionParId(int id)
        {
            utilisateur = RechercherUtilisateurById(id);
            
            if (utilisateur.EstActif)
            {
                chargerColletion();
                //admin = utilisateur.NomUtilisateur == "admin";
                admin = utilisateur.Role == "Administrateur";
                return true;
            }
            else
            {
                return false;
            }
            
        }
        public static List<Utilisateur> getAllUtilisateur()
        {
            var listUser = new List<Utilisateur>();
            DataSet user = BD.Selection("SELECT * FROM `utilisateurs`");

            foreach (DataRow item in user.Tables[0].Rows)
            {
                if((int)item["id"] != UtilisateurADO.utilisateur.Id)
                    listUser.Add(new Utilisateur(item));
            }
            return listUser;
        }
        public static List<Utilisateur> getAllUtilisateur(string Recherche)
        {
            var listUser = new List<Utilisateur>();
            DataSet user = BD.Selection("SELECT * FROM `utilisateurs`");

            foreach (DataRow item in user.Tables[0].Rows)
            {
                if ((int)item["id"] != UtilisateurADO.utilisateur.Id && item["NomUtilisateur"].ToString().Contains(Recherche)) 
                    listUser.Add(new Utilisateur(item));
            }
            return listUser;
        }
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
        public bool Connection(string User, string MP)
        {
            if (InfoValideConnection(User, MP))
            {
                utilisateur = new Utilisateur(GetUserDataSet(User));
                if (utilisateur.EstActif)
                {
                    chargerColletion();
                    //admin = utilisateur.NomUtilisateur == "admin";
                    admin = utilisateur.Role == "Administrateur";                       
                    return true;
                }
                
            }
            return false;
        }

        //Créé un compte en BD, retourne false si le compte na pas pu être créé
        public bool CreerCompte(string User, string MP, string MPConfirme, string Courriel)
        {
            if(User =="")
            {
                MessageBox.Show("Le nom d'utilisateurne peut pas être vide");
                return false;
            }
            if(MP == "")
            {
                MessageBox.Show("Le mot de passe ne peut pas être vide");
                return false;
            }
            //1. Valide que les mot de passe sont pareil
            if (MP != MPConfirme)
            {
                MessageBox.Show("Les mot de passe sont non équivalent");
                return false;
            }
            //2. Valide que le nom d'utilisateur n'est pas prit
            if (CheckSiUserEstPrit(User))
            {
                MessageBox.Show("Le nom d'utilisateur est prit");
                return false;
            }
            //3. Valide le format du Courriel
            if (ValideCourriel(Courriel))
            {
                MessageBox.Show("Le couriel est invalide");
                return false;
            }
            //4. On créé le compte et on se connecte
            AjoutCompte(User,MP,Courriel);
            Connection(User,MP);
            return true;
        }
        //Method qui retourne true si le nom utilisateur est déja utilisé
        public bool CheckSiUserEstPrit(string User)
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
        public bool ValideCourriel(string Courriel)
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
            string req = "INSERT INTO `utilisateurs` (NomUtilisateur, MotDePasse, Courriel,Role) VALUES('" + User+"','"+MP+"' ,'"+Courriel+"','Utilisateur' )";
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
        private void chargerColletion()
        {
            CollectionADO temp = new CollectionADO();
            collection = temp.Recuperer(utilisateur.Id);
        }

        public ObservableCollection<Utilisateur> GetAllUsers()
        {
            ObservableCollection<Utilisateur> lesUtilisateurs = new ObservableCollection<Utilisateur>();
            string req = $"SELECT id, nomUtilisateur,estActif,Role FROM Utilisateurs WHERE id!={utilisateur.Id}";
            DataSet data=BD.Selection(req);
            DataTable table = data.Tables[0];
            foreach (DataRow item in table.Rows)
            {
                lesUtilisateurs.Add(new Utilisateur(item,true));
            }
            return lesUtilisateurs;
        }

        public int CompterCollections(Utilisateur utilisateur)
        {
            string req = $"SELECT COUNT(Id) FROM Collections WHERE Utilisateur_ID = {utilisateur.Id}";
            DataSet data = BD.Selection(req);
            DataTable table = data.Tables[0];
            string retour = ((Int64)(table.Rows[0]["COUNT(Id)"])).ToString();
            return int.Parse(retour);
        }
        public int CompterItems(Utilisateur utilisateur)
        {
            string req = $"SELECT COUNT(i.Id) FROM Items i INNER JOIN Collections c ON c.Id=Collection_Id WHERE c.Utilisateur_ID = {utilisateur.Id}";
            DataSet data = BD.Selection(req);
            DataTable table = data.Tables[0];
            string retour = ((Int64)(table.Rows[0]["COUNT(i.Id)"])).ToString();
            return int.Parse(retour);
        }
        public int CompterAnnonces(Utilisateur utilisateur)
        {
            string req = $"SELECT COUNT(Id) FROM Annonces  WHERE Annonceur_ID = {utilisateur.Id}";
            DataSet data = BD.Selection(req);
            DataTable table = data.Tables[0];
            string retour = ((Int64)(table.Rows[0]["COUNT(Id)"])).ToString();
            return int.Parse(retour);
        }
        public int CompterPropositions(Utilisateur utilisateur)
        {
            string req = $"SELECT COUNT(Id) FROM Propositions  WHERE Proposeur_ID = {utilisateur.Id}";
            DataSet data = BD.Selection(req);
            DataTable table = data.Tables[0];
            string retour = ((Int64)(table.Rows[0]["COUNT(Id)"])).ToString();
            return int.Parse(retour);
        }
        public int CompterPropositionsRecues(Utilisateur utilisateur)
        {
            string req = $"SELECT COUNT(p.Id) FROM Propositions p INNER JOIN Annonces a ON a.Id=p.AnnonceLiee_Id   WHERE a.Annonceur_ID = {utilisateur.Id}";
            DataSet data = BD.Selection(req);
            DataTable table = data.Tables[0];
            string retour = ((Int64)(table.Rows[0]["COUNT(p.Id)"])).ToString();
            return int.Parse(retour);
        }
        public int CompterTransactions(Utilisateur utilisateur,bool estAnnonceur)
        {
            string retour;
            string req;
            if (!estAnnonceur)
            {
                req = $"SELECT COUNT(t.Id) FROM Transactions t INNER JOIN Propositions p ON p.id = propositionTrx_Id   WHERE p.Proposeur_ID = {utilisateur.Id}";               
            }
            else
            {
                req = $"SELECT COUNT(t.Id) FROM Transactions t INNER JOIN Propositions p ON p.id = propositionTrx_Id INNER JOIN Annonces a ON a.Id=p.AnnonceLiee_Id WHERE a.Annonceur_Id = {utilisateur.Id}";
            } 
                DataSet data = BD.Selection(req);
                DataTable table = data.Tables[0];
                retour = ((Int64)(table.Rows[0]["COUNT(t.Id)"])).ToString();
            return int.Parse(retour);
        }

        public void ModifierEstActif(Utilisateur utilisateur)
        {
            string req = $"UPDATE Utilisateurs SET EstActif={utilisateur.EstActif} WHERE Id ={utilisateur.Id}";
            BD.Commande(req);
        }
        #endregion
    }
}

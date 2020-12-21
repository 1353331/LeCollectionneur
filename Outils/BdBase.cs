using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeCollectionneur.Outils
{
    class BdBase
    {
        private string ConnexionParam;
        private MySqlConnection MaConnexion;

        private MySqlTransaction maTransaction;



        public BdBase()
        {
            ConnexionParam = ConfigurationManager.ConnectionStrings["connexionLeCollectionneur"].ConnectionString;
            //ConnexionParam = "server=" + Serveur + ";database=" + BaseDeDonnee + ";uid=" + Utilisateur + ";password=" + MotDePasse +";Convert zero datetime=true; sslmode=none";
            try
            {
                MaConnexion = new MySqlConnection(ConnexionParam);
                MaConnexion.Open();
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Impossible de se connecter :" + e.Message);
                throw;
            }
            //MessageBox.Show("Instanciation correcte");
        }

        public DataSet Selection(string req)
        {
            DataSet resultat = new DataSet();

            try
            {
                if (Ouvrir())
                {
                    MySqlDataAdapter adapteur = new MySqlDataAdapter();
                    adapteur.SelectCommand = new MySqlCommand(req, MaConnexion);
                    adapteur.Fill(resultat);
                    return resultat;
                }
            }
            catch (MySqlException e)
            {
                if (e.Message != "There is already an open DataReader associated with this Connection which must be closed first.")
                    MessageBox.Show("Erreur:" + e.Message);
                //throw;
            }
            finally
            {
                //Fermer();
            }

            return null;
        }

        public int Commande(string requete)
        {
            int nbResultat = 0;
            try
            {
                if (Ouvrir())
                {
                    MySqlCommand CMD = new MySqlCommand(requete, MaConnexion);
                    nbResultat = CMD.ExecuteNonQuery();
                }
            }
            catch (MySqlException e)
            {
                if (e.Message!="There is already an open DataReader associated with this Connection which must be closed first.")
                MessageBox.Show("Erreur dans la commande :" + e.Message);
                //throw;
            }
            finally
            {
                //Fermer();
            }
            return nbResultat;
        }


	  public int CommandeCreationAvecRetourId(string requete)
      {
         // Pour que le retour d'ID fonctionne, il faut ajouter la commande SELECT LAST_INSERT_ID(); à la fin de la requête
         // Ex: 
         // INSERT INTO Condition (Nom)
         //	VALUES
         //	('Endommagée');
         // SELECT LAST_INSERT_ID();

         int idElementInsere = 0;

         try
         {
            if (Ouvrir())
            {
               MySqlCommand cmd = new MySqlCommand(requete, MaConnexion);
               cmd.CommandType = CommandType.Text;
               idElementInsere = Convert.ToInt32(cmd.ExecuteScalar());
            }
         }
         catch (MySqlException e)
         {
            MessageBox.Show("Erreur dans la commande :" + e.Message);
            throw;
         }
         finally
         {
            //Fermer();
         }

         return idElementInsere;
      }
        private bool Ouvrir()
        {
            try
            {
                if (MaConnexion.State == System.Data.ConnectionState.Open)
                    return true;

                MaConnexion.Open();
                return true;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erreur d'ouverture :" + e.Message);
                throw;
            }
        }

        private bool Fermer()
        {
            try
            {
                if (MaConnexion.State == System.Data.ConnectionState.Closed)
                    return true;

                MaConnexion.Close();
                return true;
            }
            catch (MySqlException e)
            {
                MessageBox.Show("Erreur de fermeture :" + e.Message);
                throw;
            }
        }

        public void TransactionDebut()
        {
            try
            {
                if (Ouvrir())
                    maTransaction = MaConnexion.BeginTransaction();
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        public void TransactionFin()
        {
            try
            {
                maTransaction.Commit();
                maTransaction = null;
                MaConnexion.Close();
            }
            catch (MySqlException)
            {
                throw;
            }
        }

        public void TransactionAnnulee()
        {
            try
            {
                maTransaction.Rollback();
                maTransaction = null;
                MaConnexion.Close();
            }
            catch (MySqlException)
            {
                throw;
            }
        }
    }
}

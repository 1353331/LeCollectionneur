using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeCollectionneur.Outil
{
    class BDbase
    {
        private string ConnexionParam;
        private MySqlConnection MaConnexion;

        public BDbase()
        {
            ConnexionParam = ConfigurationManager.ConnectionStrings["lecollectionneur_db"].ConnectionString;
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
                MessageBox.Show("Erreur:" + e.Message);
                throw;
            }
            finally
            {
                Fermer();
            }

            return null;
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

    }
}

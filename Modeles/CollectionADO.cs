using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeCollectionneur.Modeles
{
    class CollectionADO
    {
        #region Propriétés
        private BdBase MaBD;
        
        public CollectionADO() { MaBD = new BdBase(); }
        #endregion

        #region Retrieve
        public ObservableCollection<Collection> Recuperer(int idUtilisateur)
        {
            ObservableCollection<Collection> lesCollections = new ObservableCollection<Collection>();
            string sel = $"select * from Collections where Utilisateur_Id = {idUtilisateur}";
            DataSet SetCollection = MaBD.Selection(sel);
            DataTable TableCollection = SetCollection.Tables[0];

            foreach (DataRow RowCollection in TableCollection.Rows)
            {
                lesCollections.Add(new Collection(RowCollection));
            }
            return lesCollections;
        }
        public ObservableCollection<Collection> RecupererToutesSaufUne(int idUtilisateur,int idPasRecupere)
        {
            ObservableCollection<Collection> lesCollections = new ObservableCollection<Collection>();
            string sel = $"select * from Collections where Utilisateur_Id = {idUtilisateur} AND NOT id={idPasRecupere}";
            DataSet SetCollection = MaBD.Selection(sel);
            DataTable TableCollection = SetCollection.Tables[0];

            foreach (DataRow RowCollection in TableCollection.Rows)
            {
                lesCollections.Add(new Collection(RowCollection));
            }
            return lesCollections;
        }
        public Collection RecupererUn(int id)
        {
            string sel = "select * from Collections where ID = " + id;
            DataSet SetCollection = MaBD.Selection(sel);
            DataTable TableCollection = SetCollection.Tables[0];

            return new Collection(TableCollection.Rows[0]);
        }
        #endregion

        #region Update
        public void Modifier(Collection c)
        {
            string req = $"update Collections set Nom = '{c.Nom}',DateCreation='{c.DateCreation.Year}-{c.DateCreation.Month}-{c.DateCreation.Day}' where id = {c.Id}";
            MaBD.Commande(req);
        }
        #endregion

        #region Delete
        public void Supprimer(int id)
        {
            string req = $"delete from Items Where Collection_Id ={id}; delete from Collections where ID = " + id;
            try
            {
                MaBD.Commande(req);
            }
            catch (Exception)
            {
                MessageBox.Show("Vous ne pouvez pas supprimer cette collection car au moins 1 item de celle-ci est inclu dans une annonce ou une proposition.", "Attention");
            }
            
            
        }
        #endregion

        #region Insert
        public void Ajouter(Collection c,int IdUtilisateur)
        {
            string req = $"insert into Collections (Utilisateur_Id,nom) values({IdUtilisateur},'{c.Nom}')";
            MaBD.Commande(req);
        }

        public int AjouterRetourId(Collection c, int IdUtilisateur)
        {
            string req = $"insert into Collections (Utilisateur_Id,nom) values({IdUtilisateur},'{c.Nom}'); SELECT LAST_INSERT_ID();";
            return MaBD.CommandeCreationAvecRetourId(req);
      }

        #endregion

    }
}

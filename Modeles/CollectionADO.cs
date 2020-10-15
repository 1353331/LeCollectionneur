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
        public ObservableCollection<Collection> Recuperer(int idUtilisateur)
        {
            ObservableCollection<Collection> lesCollections = new ObservableCollection<Collection>();
            string sel = $"select * from Collections where idUtilisateur = {idUtilisateur}";
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
            string sel = $"select * from Collections where idUtilisateur = {idUtilisateur} AND NOT id={idPasRecupere}";
            DataSet SetCollection = MaBD.Selection(sel);
            DataTable TableCollection = SetCollection.Tables[0];

            foreach (DataRow RowCollection in TableCollection.Rows)
            {
                lesCollections.Add(new Collection(RowCollection));
            }
            return lesCollections;
        }
        public void Modifier(Collection c)
        {
            string req = $"update Collections set Nom = {c.Nom} where id = {c.Id}";
            MaBD.Commande(req);
        }
        
        public Collection RecupererUn(int id)
        {
            string sel = "select * from Collections where ID = " + id;
            DataSet SetCollection = MaBD.Selection(sel);
            DataTable TableCollection = SetCollection.Tables[0];

            return new Collection(TableCollection.Rows[0]);
        }

        public void Supprimer(int id)
        {
            string req = $"delete from Items Where idCollection ={id}; delete from Collections where ID = " + id;
            try
            {
                MaBD.Commande(req);
            }
            catch (Exception)
            {
                MessageBox.Show("Vous ne pouvez pas supprimer cette collection car au moins 1 item de celle-ci est inclu dans une annonce ou une proposition.", "Attention");
            }
            
            
        }

       

        public void Ajouter(Collection c,int IdUtilisateur)
        {
            string req = $"insert into Collections (id,idUtilisateur,nom) values(NULL,{IdUtilisateur},'{c.Nom}' )";
            MaBD.Commande(req);
        }
        //public void AjouterItem(Collection c,Item i)
        //{
        //    string req = $"insert into Items values(NULL,{i.Id},{c.Id},(SELECT id from Conditions WHERE nom='{i.Condition}'),{i.Quantite})";
        //    MaBD.Commande(req);
        //}

        //public void EnleverItem(Collection c, Item i)
        //{
        //    string req = $"DELETE FROM ItemCollection WHERE idCollection={c.Id} AND idItem={i.Id}";
        //    MaBD.Commande(req);
        //}

        
    }
}

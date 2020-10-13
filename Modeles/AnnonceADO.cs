using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls.Primitives;

namespace LeCollectionneur.Modeles
{
    class AnnonceADO
    {
        #region Propriété
        private BdBase MaBD;
        #endregion

        #region Constructeur
        public AnnonceADO() { MaBD = new BdBase(); }
        #endregion

        #region Méthodes
        public ObservableCollection<Annonce> Recuperer()
        {
            ObservableCollection<Annonce> ListeAnnonces = new ObservableCollection<Annonce>();
            string sel = "SELECT a.*, ta.Nom AS typeAnnonce from annonces a LEFT JOIN typesannonce ta ON a.idTypeAnnonce = ta.Id";
            DataSet SetAnnonce = MaBD.Selection(sel);
            DataTable TableAnnonce = SetAnnonce.Tables[0];

            foreach (DataRow RowAnnonce in TableAnnonce.Rows)
            {
                ListeAnnonces.Add(new Annonce(RowAnnonce));
            }

            return ListeAnnonces;
        }

        public Annonce RecupererUn(int id)
        {
            string sel = "SELECT a.*, ta.Nom AS typeAnnonce from annonces a LEFT JOIN typesannonce ta ON a.idTypeAnnonce = ta.Id WHERE Id = " + id;
            DataSet SetAnnonce = MaBD.Selection(sel);
            DataTable TableAnnonce = SetAnnonce.Tables[0];

            return new Annonce(TableAnnonce.Rows[0]);
        }

        public ObservableCollection<string> RecupererTypes()
        {
            ObservableCollection<string> ListeTypes = new ObservableCollection<string>();
            string sel = "SELECT DISTINCT ta.Nom as TypeAnnonce from annonces a LEFT JOIN typesannonce ta ON a.idTypeAnnonce = ta.id";
            DataSet SetType = MaBD.Selection(sel);
            DataTable TableType = SetType.Tables[0];

            foreach (DataRow RowType in TableType.Rows){
                ListeTypes.Add((string)RowType["TypeAnnonce"]);
            }

            return ListeTypes;
        }

        public void Modifier(Annonce a)
        {
            string req = $"UPDATE annonces SET Id={a.Id}, Nom='{a.Titre}', IdUtilisateur={a.Annonceur.Id} , Montant={a.Montant}, Date='{a.DatePublication}', idTypeAnnonce= (Select Id from typesannonce where nom = '{a.Type}'), Description = '{a.Description}' WHERE id ={a.Id}";
            // TODO: UPDATE de la liste d'items
            MaBD.Commande(req);
        }

        public void Ajouter(Annonce a)
        {
            //Ajouter l'annonce
            string req = $"insert into Annonces values(NULL, '{a.Titre}', {a.Annonceur.Id}, {a.Montant}, '{a.DatePublication}', (Select Id from typesannonce Where Nom = '{a.Type}'), '{a.Description}')";
            MaBD.Commande(req);

            //Ajouter ses items
            //foreach(Item i in a.ListeItems)
            //{
            //    req = $"insert into itemcollectionannonce values(NULL, {a.Id}, {i.Id}, {i.Quantite})";
            //    MaBD.Commande(req);
            //}
        }

        public void Supprimer(Annonce a)
        {
            // Supprimer toutes les items de l'annonce
            string req = $"delete from itemcollectionannonce where idannonce = {a.Id}";
            MaBD.Commande(req);

            //supprimer l'annonce
            req = $"delete from annonces where id = {a.Id}";
            MaBD.Commande(req);
        }
        #endregion
    }
}

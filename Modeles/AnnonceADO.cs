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

        public ObservableCollection<Item> RecupererListeItems(int idAnnonce)
        {
            ObservableCollection<Item> ListeItems = new ObservableCollection<Item>();
            ItemADO ItemADO = new ItemADO();

            string sel = $"SELECT idItem from itemannonce where idAnnonce = {idAnnonce}";
            DataSet SetItem = MaBD.Selection(sel);
            DataTable TableItem = SetItem.Tables[0];

            foreach(DataRow RowItem in TableItem.Rows)
            {
                ListeItems.Add(ItemADO.RecupererUn((int)RowItem["idItem"]));
            }

            return ListeItems;
        }

        public ObservableCollection<Annonce> RecupererParUtilisateurConnecte()
        {
            UtilisateurADO UtilisiateurADO = new UtilisateurADO();
            Utilisateur UtilisateurConnecte = UtilisiateurADO.RetourUtilisateurActif();
            
            ObservableCollection<Annonce> ListeAnnonces = new ObservableCollection<Annonce>();
            string sel = $"SELECT a.*, ta.Nom AS typeAnnonce from annonces a LEFT JOIN typesannonce ta ON a.idTypeAnnonce = ta.Id WHERE idUtilisateur = {UtilisateurConnecte.Id}";

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
            string sel = "SELECT a.*, ta.Nom AS typeAnnonce from annonces a LEFT JOIN typesannonce ta ON a.idTypeAnnonce = ta.Id WHERE a.Id = " + id;
            DataSet SetAnnonce = MaBD.Selection(sel);
            DataTable TableAnnonce = SetAnnonce.Tables[0];

            return new Annonce(TableAnnonce.Rows[0]);
        }

        public ObservableCollection<string> RecupererTypes()
        {
            ObservableCollection<string> ListeTypes = new ObservableCollection<string>();
            string sel = "SELECT Nom as TypeAnnonce from typesannonce";
            DataSet SetType = MaBD.Selection(sel);
            DataTable TableType = SetType.Tables[0];

            foreach (DataRow RowType in TableType.Rows)
            {
                ListeTypes.Add((string)RowType["TypeAnnonce"]);
            }

            return ListeTypes;
        }

        public ObservableCollection<string> RecupererTypesItem()
        {
            ObservableCollection<string> ListeTypesItem = new ObservableCollection<string>();
            ItemADO ItemADO = new ItemADO();
            Item ItemTemp = new Item();

            string sel = "SELECT idItem FROM itemannonce";
            DataSet SetTypeItem = MaBD.Selection(sel);
            DataTable TableTypeItem = SetTypeItem.Tables[0];

            foreach(DataRow RowTypeItem in TableTypeItem.Rows)
            {
                ItemTemp = ItemADO.RecupererUn((int)RowTypeItem["idItem"]);
                if (!ListeTypesItem.Contains(ItemTemp.Type))
                {
                    ListeTypesItem.Add(ItemTemp.Type);
                }
            }

            return ListeTypesItem;
        }

        public void Modifier(Annonce a)
        {
            string req = $"UPDATE annonces SET Id={a.Id}, Nom='{a.Titre}', IdUtilisateur={a.Annonceur.Id} , Montant={a.Montant}, Date='{a.DatePublication}', idTypeAnnonce= (Select Id from typesannonce where nom = '{a.Type}'), Description = '{a.Description}' WHERE id ={a.Id}";
            MaBD.Commande(req);
            
            req = $"delete from itemannonce where idAnnonce = {a.Id}";
            MaBD.Commande(req);

            //Ajouter ses items
            foreach (Item i in a.ListeItems)
            {
                req = $"INSERT INTO `itemannonce`(`Id`, `IdAnnonce`, `IdItem`) VALUES ( null, (select Id FROM annonces where Id = {a.Id}), (select Id FROM items where Id = {i.Id}) )";
                MaBD.Commande(req);
            }
        }

        public void Ajouter(Annonce a)
        {
            //Ajouter l'annonce
            string req = $"insert into Annonces values(NULL, '{a.Titre}', {a.Annonceur.Id}, {a.Montant}, '{a.DatePublication}', (Select Id from typesannonce Where Nom = '{a.Type}'), '{a.Description}'); SELECT LAST_INSERT_ID();";
            int Id = MaBD.CommandeCreationAvecRetourId(req);

            //Ajouter ses items
            foreach (Item i in a.ListeItems)
            {
                req = $"INSERT INTO `itemannonce`(`Id`, `IdAnnonce`, `IdItem`) VALUES ( null, (select Id FROM annonces where Id = {Id}), (select Id FROM items where Id = {i.Id}) )";
                MaBD.Commande(req);
            }
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

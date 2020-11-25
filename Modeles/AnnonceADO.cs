using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Enumerations;
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
            string sel = $@"
            SELECT a.*, ta.Nom AS typeAnnonce, ea.Nom as etatAnnonce 
            FROM annonces a
            INNER JOIN EtatsAnnonce ea
            ON a.EtatAnnonce_Id = ea.Id
            LEFT JOIN typesannonce ta 
            ON a.Type_Id = ta.Id
            WHERE EtatAnnonce_Id = (Select id From etatsannonce Where Nom = '{EtatsAnnonce.Active}')
            ";
            DataSet SetAnnonce = MaBD.Selection(sel);
            DataTable TableAnnonce = SetAnnonce.Tables[0];

            foreach (DataRow RowAnnonce in TableAnnonce.Rows)
            {
                ListeAnnonces.Add(new Annonce(RowAnnonce));
            }

            Annonce.ToutesLesAnnonces = ListeAnnonces;
            return ListeAnnonces;
        }

        public ObservableCollection<Item> RecupererListeItems(int idAnnonce)
        {
            ObservableCollection<Item> ListeItems = new ObservableCollection<Item>();
            ItemADO ItemADO = new ItemADO();

            string sel = $"SELECT Item_Id from itemannonces where Annonce_Id = {idAnnonce}";
            DataSet SetItem = MaBD.Selection(sel);
            DataTable TableItem = SetItem.Tables[0];

            foreach(DataRow RowItem in TableItem.Rows)
            {
                ListeItems.Add(ItemADO.RecupererUn((int)RowItem["Item_Id"]));
            }

            return ListeItems;
        }

        public ObservableCollection<Annonce> RecupererParUtilisateurConnecte()
        {
            UtilisateurADO UtilisiateurADO = new UtilisateurADO();
            Utilisateur UtilisateurConnecte = UtilisiateurADO.RetourUtilisateurActif();
            
            ObservableCollection<Annonce> ListeAnnonces = new ObservableCollection<Annonce>();
            string sel = $@"
            SELECT a.*, ta.Nom AS typeAnnonce, ea.Nom AS etatAnnonce 
            from annonces a 
            INNER JOIN EtatsAnnonce ea
            ON a.EtatAnnonce_Id = ea.Id
            LEFT JOIN typesannonce ta 
            ON a.Type_Id = ta.Id 
            WHERE Annonceur_Id = {UtilisateurConnecte.Id}
            ";

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
            string sel = $@"
            SELECT a.*, ta.Nom AS typeAnnonce, ea.Nom AS etatAnnonce 
            FROM annonces a
            INNER JOIN EtatsAnnonce ea
            ON a.EtatAnnonce_Id = ea.Id
            LEFT JOIN typesannonce ta 
            ON a.Type_Id = ta.Id 
            WHERE a.Id = {id}";
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

            string sel = "SELECT Item_Id FROM itemannonces";
            DataSet SetTypeItem = MaBD.Selection(sel);
            DataTable TableTypeItem = SetTypeItem.Tables[0];

            foreach(DataRow RowTypeItem in TableTypeItem.Rows)
            {
                ItemTemp = ItemADO.RecupererUn((int)RowTypeItem["Item_Id"]);
                if (!ListeTypesItem.Contains(ItemTemp.Type.Nom))
                {
                    ListeTypesItem.Add(ItemTemp.Type.Nom);
                }
            }

            return ListeTypesItem;
        }

        public void Modifier(Annonce a)
        {
            string req = $"UPDATE annonces SET Titre='{a.Titre.Replace("'", @"\'")}', Annonceur_Id={a.Annonceur.Id} , Montant={a.Montant}, DatePublication='{a.DatePublication:yyyy-MM-dd HH:mm:ss}', Type_Id= (Select Id from typesannonce where nom = '{a.Type.Nom}'), Description = '{a.Description.Replace("'", @"\'")}', EtatAnnonce_Id = (SELECT Id FROM EtatsAnnonce WHERE Nom = '{a.EtatAnnonce.Nom}') WHERE id ={a.Id}";
            MaBD.Commande(req);
            
            req = $"delete from itemannonces where Annonce_Id = {a.Id}";
            MaBD.Commande(req);

            //Ajouter ses items
            foreach (Item i in a.ListeItems)
            {
                req = $"INSERT INTO `itemannonces`(`Annonce_Id`, `Item_Id`) VALUES ((select Id FROM annonces where Id = {a.Id}), (select Id FROM items where Id = {i.Id}) )";
                MaBD.Commande(req);
            }
        }

        public void AnnulerAnnoncesActivesAvecItems(IEnumerable<Item> items)
        {
            if (items.Count() > 0)
            {
               string idItems = "";

               foreach (Item item in items)
               {
                  idItems += $"{item.Id}";
                  if (item != items.Last())
                     idItems += ", ";
               }

               string requete = $@"
				      UPDATE Annonces
				      SET
				      EtatAnnonce_Id = (SELECT Id FROM EtatsAnnonce WHERE Nom = '{EtatsAnnonce.Annulee}')
				      WHERE EtatAnnonce_Id = (SELECT Id FROM EtatsAnnonce WHERE Nom = '{EtatsAnnonce.Active}')
				      AND Id IN (
								      SELECT Annonce_Id
								      FROM itemannonces
								      WHERE Item_Id IN ({idItems})
						         );
				      ";
               MaBD.Commande(requete);
            }
        }

        public void Ajouter(Annonce a)
        {
            //Ajouter l'annonce
            string req = $"insert into Annonces(Titre, Annonceur_Id, Montant, DatePublication, Type_Id, Description,EtatAnnonce_Id) values('{a.Titre}', {a.Annonceur.Id}, {a.Montant}, '{a.DatePublication}', (Select Id from typesannonce Where Nom = '{a.Type.Nom}'), '{a.Description}', (SELECT Id FROM EtatsAnnonce WHERE nom = '{a.EtatAnnonce.Nom}')); SELECT LAST_INSERT_ID();";
            int Id = MaBD.CommandeCreationAvecRetourId(req);

            //Ajouter ses items
            foreach (Item i in a.ListeItems)
            {
                req = $"INSERT INTO `itemannonces`(`Annonce_Id`, `Item_Id`) VALUES ((select Id FROM annonces where Id = {Id}), (select Id FROM items where Id = {i.Id}) )";
                MaBD.Commande(req);
            }
        }

        public void Supprimer(Annonce a)
        {
            // Supprimer tous les items de l'annonce
            string req = $"delete from itemannonces where Annonce_Id = {a.Id}";
            MaBD.Commande(req);

            //supprimer l'annonce
            req = $"delete from annonces where id = {a.Id}";
            MaBD.Commande(req);
        }
        #endregion
    }
}

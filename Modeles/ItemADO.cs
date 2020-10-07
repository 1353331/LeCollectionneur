using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeCollectionneur.Modeles
{
    class ItemADO
    {
        #region Propriétés
        private BdBase MaBD;

        public ItemADO() { MaBD = new BdBase(); }
        #endregion
        public ObservableCollection<Item> Recuperer(int idCollection)
        {
            // On recherche les Items selon la Collection entrée.
            ObservableCollection<Item> lesItems = new ObservableCollection<Item>();
            string sel = $"select i.id , i.nom as nomItem, i.description,i.dateSortie , i.cheminImage , ic.quantite , c.nom as 'condition' , t.nom as 'typeItem' , m.nom as 'manufacturier' from ItemCollection ic " +
                $" INNER JOIN Items as i on ic.idItem = i.id " +
                $" INNER JOIN Manufacturiers as m on i.idManufacturier = m.id" +
                $" INNER JOIN Conditions as c on ic.idCondition = c.id" +
                $" INNER JOIN TypesItem as t on i.idTypeItem=t.id" +
                $" WHERE ic.idCollection = {idCollection};";
            DataSet SetItem = MaBD.Selection(sel);
            DataTable TableItem = SetItem.Tables[0];

            foreach (DataRow RowItem in TableItem.Rows)
            {
                lesItems.Add(new Item(RowItem));
            }
            return lesItems;
        }
        
        public void Modifier(Item i)
        {
            string req = $"update Items set Nom = '{i.Nom}' , " +
                $"Description='{i.Description}', " +
                $"idTypeItem= (SELECT id from TypesItem WHERE nom='{i.Type}')," +
                $"idManufacturier = (SELECT id from Manufacturiers WHERE nom='{i.Manufacturier}')," +
                $"dateSortie = '{i.DateSortie.Year}-{i.DateSortie.Month}-{i.DateSortie.Day}' " +
                $"where id = {i.Id}";
            MaBD.Commande(req);
        }
        public void ModifierDansCollection(Item i, Collection c)
        {
            // On veut pouvoir modifier la Quantite et la Condition de l'item.
            string req = $"UPDATE ItemCollection SET Quantite={i.Quantite}, idCondition=(SELECT idCondition FROM Conditions WHERE nom='{i.Condition}') WHERE idCollection={c.Id} AND idItem={i.Id};";
            MaBD.Commande(req);
        }
        public Item RecupererUn(int id)
        {
            string sel = $"select i.id , i.nom as nomItem, i.description,i.dateSortie , i.cheminImage , t.nom as 'typeItem' , m.nom as 'manufacturier' from Items i " +
                $" INNER JOIN Manufacturiers as m on i.idManufacturier = m.id" +
                $" INNER JOIN TypesItem as t on i.idTypeItem=t.id" +
                $" WHERE i.id = {id};";
            DataSet SetItem = MaBD.Selection(sel);
            DataTable TableItem = SetItem.Tables[0];

            return new Item(TableItem.Rows[0],false);
        }

        public void Supprimer(int id)
        {
            // Suppression de l'item en BD (utile pour l'admin)
            string req = $"delete from ItemCollection Where idItem = {id}; delete from Items where ID = {id}";
            MaBD.Commande(req);
        }



        public void Ajouter(Item i)
        {
            string req = $"insert into Items" +
                $" values(NULL," +
                $"'{i.Nom}'," +
                (i.CheminImage is null ?"NULL," : $"'{i.CheminImage}',") +
                $"(SELECT id from TypesItem WHERE nom='{i.Type}')," +
                $"(SELECT id from Manufacturiers WHERE nom='{i.Manufacturier}')," +
                $"'{i.Description}'," +
                $"'{i.DateSortie.Year}-{i.DateSortie.Month}-{i.DateSortie.Day}');";
            MaBD.Commande(req);
        }
    }
}

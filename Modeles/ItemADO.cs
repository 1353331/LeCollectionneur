﻿using LeCollectionneur.Modeles;
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
        public ObservableCollection<string> TypesPossibles 
        { 
            get
            {
                return RecupererListeTypes();
            }
            set
            {

            }
        }
         public ObservableCollection<string> ConditionsPossibles
        {
            get
            {
                return RecupererListeConditions();
            }
            set
            {

            }
        }
        public ItemADO() { MaBD = new BdBase(); }
        #endregion

        #region Retrieve
        public ObservableCollection<Item> Recuperer(int idCollection)
        {
            // On recherche les Items selon la Collection entrée.
            ObservableCollection<Item> lesItems = new ObservableCollection<Item>();
            string sel = $"select i.id , i.nom as nomItem, i.description,i.dateSortie , i.cheminImage  , c.nom as 'condition' , t.nom as 'typeItem' , manufacturier from Items i " +
                $" INNER JOIN Conditions as c on i.idCondition = c.id" +
                $" INNER JOIN TypesItem as t on i.idTypeItem=t.id" +
                $" WHERE i.idCollection = {idCollection};";
            DataSet SetItem = MaBD.Selection(sel);
            DataTable TableItem = SetItem.Tables[0];

            foreach (DataRow RowItem in TableItem.Rows)
            {
                lesItems.Add(new Item(RowItem));
            }
            return lesItems;
        }
        public Item RecupererUn(int id)
        {
            string sel = $"select i.id, i.nom as nomItem, i.description,i.dateSortie , i.cheminImage , t.nom as 'typeItem' , manufacturier, c.Nom AS 'condition' from Items i " +
                $" INNER JOIN TypesItem as t on i.idTypeItem=t.id" +
                $" INNER JOIN Conditions as c on i.idCondition = c.id" +
                $" WHERE i.id = {id};";
            DataSet SetItem = MaBD.Selection(sel);
            DataTable TableItem = SetItem.Tables[0];

            return new Item(TableItem.Rows[0], false);
        }
        public int RecupererIdCollection(Item item)
        {
            string select = $"SELECT idCollection from Items WHERE id={item.Id}";
            DataSet SetId = MaBD.Selection(select);
            DataTable TableId = SetId.Tables[0];
            return (int)TableId.Rows[0]["idCollection"];
        }
        #endregion

        #region Update
        public void Modifier(Item i)
        {
            // Ne modifie pas le chemin de l'image.
            string req = $"update Items set Nom = '{i.Nom}' , " +
                $"Description='{i.Description}', " +
                $"idTypeItem= (SELECT id from TypesItem WHERE nom='{i.Type}')," +
                $"Manufacturier = " + (!(i.Manufacturier is null) ? $"'{i.Manufacturier}'" : "NULL") + ", " +
                $"dateSortie = "+ (i.DateSortie.HasValue&&DateTime.Compare(i.DateSortie.GetValueOrDefault(),new DateTime(1,1,1,0,0,0))!=0 ? $"'{i.DateSortie.GetValueOrDefault().Year}-{i.DateSortie.GetValueOrDefault().Month}-{i.DateSortie.GetValueOrDefault().Day}'":"NULL") +", " +
                $"idCondition = (SELECT id FROM conditions WHERE nom='{i.Condition}')"+
                $"where id = {i.Id}";
            MaBD.Commande(req);
            
        }
        public void TransfererItem(Collection cDestination, Item i)
        {
            // Transfert d'un item d'une collection à une autre en BD.
            string req = $"UPDATE Items SET idCollection={cDestination.Id} WHERE id={i.Id}";
            MaBD.Commande(req);
        }

        public void AjouterCheminImage(int id)
        {
            string req = $"UPDATE Items Set CheminImage='item{id}.jpg' WHERE id={id}";
            MaBD.Commande(req);
        }
        public void EnleverCheminImage(int id)
        {
            string req = $"UPDATE Items Set CheminImage= NULL WHERE id={id}";
            MaBD.Commande(req);
        }
        #endregion

        #region Delete
        public void Supprimer(int id)
        {
            // Suppression de l'item en BD (utile pour l'admin)
            string req = $" delete from Items where ID = {id}";
            MaBD.Commande(req);
        }
        #endregion

        #region Insert
        public void Ajouter(Item i,Collection c)
        {
            string req = $"insert into Items" +
                $" values(NULL,{c.Id},(SELECT id from Conditions WHERE nom='{i.Condition}')," +
                $"'{i.Nom}'," +
                (i.CheminImage is null ? "NULL," : $"'{i.CheminImage}',") +
                $"(SELECT id from TypesItem WHERE nom='{i.Type}')," +
                $"'{i.Description}'," +
                 (i.DateSortie.HasValue ? $"'{i.DateSortie.GetValueOrDefault().Year}-{i.DateSortie.GetValueOrDefault().Month}-{i.DateSortie.GetValueOrDefault().Day}'" : "NULL") + ", " +
                 (!(i.Manufacturier is null) ? $"'{i.Manufacturier}'" : "NULL") + "); ";

            MaBD.Commande(req);
        }
        public int AjouterAvecRetourId(Item i, Collection c)
        {
            string req = $"insert into Items" +
                $" values(NULL,{c.Id},(SELECT id from Conditions WHERE nom='{i.Condition}')," +
                $"'{i.Nom}'," +
                (i.CheminImage is null ? "NULL," : $"'{i.CheminImage}',") +
                $"(SELECT id from TypesItem WHERE nom='{i.Type}')," +
                $"'{i.Description}'," +
                (i.DateSortie.HasValue ? $"'{i.DateSortie.GetValueOrDefault().Year}-{i.DateSortie.GetValueOrDefault().Month}-{i.DateSortie.GetValueOrDefault().Day}'" : "NULL") + ", " +
                (!(i.Manufacturier is null ) ? $"'{i.Manufacturier}'" : "NULL") + "); "+
                $"SELECT LAST_INSERT_ID();";
            int id=MaBD.CommandeCreationAvecRetourId(req);
            return id;
        }
        #endregion

        #region RetrieveTablesConnexes
        private ObservableCollection<string> RecupererListeConditions()
        {
            ObservableCollection<string> lstConditions = new ObservableCollection<string>();
            string req = $"Select nom from Conditions";
            DataSet res=MaBD.Selection(req);
            DataTable tableRes = res.Tables[0];
            foreach (DataRow Row in tableRes.Rows)
            {
                lstConditions.Add((string)Row["nom"]);
            }
            return lstConditions;
        }

         private ObservableCollection<string> RecupererListeTypes()
        {
            ObservableCollection<string> lstTypes = new ObservableCollection<string>();
            string req = $"Select nom from TypesItem";
            DataSet res = MaBD.Selection(req);
            DataTable tableRes = res.Tables[0];
            foreach (DataRow Row in tableRes.Rows)
            {
                lstTypes.Add((string)Row["nom"]);
            }
            return lstTypes;
        }
        #endregion
        
    }
}

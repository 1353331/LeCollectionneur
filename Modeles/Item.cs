using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace LeCollectionneur.Modeles
{
    class Item
    {
        #region Propriétés
        public int Id { get; set; }
        public string Nom { get; set; }
        public DateTime DateSortie { get; set; }
        public String CheminImage { get; set; } 
        public string Type { get; set; }
        public string Condition { get; set; }
        public string Manufacturier { get; set; }
        public int Quantite { get; set; }
        public string Description { get; set; }
        #endregion

        #region Constructeurs
        public Item()
        {

        }

        public Item(DataRow drItem, bool estDansCollection=true)
        {
            // Le DataRow contient * de Item et * de ItemCollection,Manufacturier,TypeItem,Condition.
            Id = (int)drItem["id"];
            Nom = (String)drItem["nomItem"];
           Manufacturier = (String)drItem["manufacturier"];
            if (!drItem.IsNull("cheminImage"))
            CheminImage = (String)drItem["cheminImage"];
            Description = (String)drItem["description"];
            DateSortie = (DateTime)drItem["dateSortie"];
            // Reste à get le Type, Condition et Manufacturier.
            Type = (String)drItem["typeItem"];
            if (estDansCollection)
            {
                Condition = (String)drItem["condition"]; 
                Quantite = (int)drItem["quantite"];
            }
            
            
        }
        #endregion

        #region Méthodes

        #endregion
    }
}

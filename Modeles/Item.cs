using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LeCollectionneur.Modeles
{
    public class Item
    {
        #region Propriétés
        public int Id { get; set; }
        public string Nom { get; set; }
        public DateTime? DateSortie { get; set; }
        public String CheminImage { get; set; } 
        public string Type { get; set; }
        public string Condition { get; set; }
        public string Manufacturier { get; set; }
        //public int Quantite { get; set; }
        public string Description { get; set; }
       
        public BitmapImage BmImage { get; set; }
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
            if (!drItem.IsNull("Manufacturier"))
           Manufacturier = (String)drItem["manufacturier"];
            if (!drItem.IsNull("cheminImage"))
            CheminImage = (String)drItem["cheminImage"];
            Description = (String)drItem["description"];
            if (!drItem.IsNull("dateSortie"))
            DateSortie = (DateTime)drItem["dateSortie"];
            // Reste à get le Type, Condition et Manufacturier.
            Type = (String)drItem["typeItem"];
                Condition = (String)drItem["condition"]; 
            if (!(CheminImage is null))
            {
                BmImage = Fichier.TransformerBitmapEnBitmapImage(Fichier.RecupererImageServeur(CheminImage));
                if (BmImage is null)
                {
                    ItemADO gestionItem = new ItemADO();
                    CheminImage = null;
                    gestionItem.EnleverCheminImage(Id);
                }
                    
            }
            
        }
        #endregion

        #region Méthodes
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Item objAsItem = obj as Item;
            if (objAsItem == null)
                return false;
            return Id==objAsItem.Id;
        }
        #endregion
    }
}

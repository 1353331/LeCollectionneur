using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LeCollectionneur.Modeles
{
   [Table("Items")]
   public class Item
    {
        #region Propriétés
        public int Id { get; set; }
        public string Nom { get; set; }
        public DateTime? DateSortie { get; set; }
        public string CheminImage { get; set; } 
        public TypeItem Type { get; set; }
        public Condition Condition { get; set; }
        public string Manufacturier { get; set; }
        public ObservableCollection<Proposition> Propositions { get; set; }
        public ObservableCollection<Annonce> Annonces { get; set; }
        //public int Quantite { get; set; }
        public string Description { get; set; }


        [NotMapped]
        public Collection collectionItem { get; set; }
        [NotMapped]
        public BitmapImage BmImage { get; set; }
        #endregion

        #region Constructeurs
        public Item()
        {

        }
        public Item(int id)
        {
            Id = id;
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
            Type = new TypeItem((String)drItem["typeItem"]);
              Condition = new Condition((String)drItem["condition"]); 
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

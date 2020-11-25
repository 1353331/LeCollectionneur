using LeCollectionneur.Modeles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
   [Table("Collections")]
   public class Collection
    {
        #region Propriétés
        public int Id { get; set; }
        public string Nom { get; set; }
        public DateTime DateCreation { get; set; }
        
        
        public ObservableCollection<Item> ItemsCollection { get; set; }
        #endregion

        #region Constructeurs
        public Collection()
        {
            
        }
        public Collection(DataRow drCollection)
        {
            Id = (int)drCollection["id"];
            Nom = (String)drCollection["nom"];
            DateCreation = (DateTime)drCollection["dateCreation"];
            // Ajouter les items à la collection.
            ItemADO itemADO = new ItemADO();
            ItemsCollection=itemADO.Recuperer(Id);
        }
        public Collection(int id)
        {
            Id = id;
        }
        #endregion

        #region Méthodes
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;
            Collection objAsCollection = obj as Collection;
            if (objAsCollection == null)
                return false;
            return Id==objAsCollection.Id;
        }
        #endregion
    }
}

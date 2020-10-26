using LeCollectionneur.Modeles;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
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
        #endregion

        #region Méthodes

        #endregion
    }
}

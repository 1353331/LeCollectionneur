using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
    public class Annonce
    {
        public int Id { get; set; }
        //public Utilisateur Annonceur { get; set; }
        public string Titre { get; set; }
        public DateTime DatePublication { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        //public ObservableCollection<Item> ListeItems { get; set; }

        public Annonce()
        {
            //ListeItems = new ObservableCollection<Item>();
            //Annonceur = new Utilisateur();
        }

        public Annonce(DataRow dr)
        {
            //UtilisateurADO ud = new UtilisateurADO();
            
            Id = (int)dr["Id"];
            //Annonceur = ud.RecupererUn((int)dr["IdUtilisateur"]);
            Titre = (string)dr["Nom"];
            DatePublication = (DateTime)dr["Date"];
            Type = (string)dr["typeAnnonce"];
            Description = (string)dr["Description"];

        }


    }
}

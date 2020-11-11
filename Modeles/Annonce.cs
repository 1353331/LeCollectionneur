using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Outils.Enumerations;

namespace LeCollectionneur.Modeles
{
    public class Annonce
    {
        #region Propriétés
        public int Id { get; set; }
        public Utilisateur Annonceur { get; set; }
        public string Titre { get; set; }
        public DateTime DatePublication { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Item> ListeItems { get; set; }
        public double Montant { get; set; }
		 public string EtatAnnonce { get; set; }
		#endregion

		#region Constructeur
		public Annonce()
        {
            UtilisateurADO UtilisateurConnecte = new UtilisateurADO();
            ListeItems = new ObservableCollection<Item>();
            Annonceur = UtilisateurConnecte.RetourUtilisateurActif();
            DatePublication = DateTime.Now.Date;
            EtatAnnonce = EtatsAnnonce.Active;
        }

        public Annonce(DataRow dr)
        {
            UtilisateurADO ud = new UtilisateurADO();
            AnnonceADO annonceADO = new AnnonceADO();
            
            Id = (int)dr["Id"];
            Annonceur = ud.RechercherUtilisateurById((int)dr["IdUtilisateur"]);
            Titre = (string)dr["Nom"];
            DatePublication = (DateTime)dr["Date"];
            Type = (string)dr["typeAnnonce"];
            Description = (string)dr["Description"];
            ListeItems = annonceADO.RecupererListeItems(Id);
            Montant = (double)dr["montant"];
            EtatAnnonce = (string)dr["etatAnnonce"];
        }
        #endregion


    }
}

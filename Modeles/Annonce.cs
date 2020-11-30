using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Outils.Enumerations;

namespace LeCollectionneur.Modeles
{
    [Table("Annonces")]
    public class Annonce
    {
        public static DateTime PlusAncienneDate = DateTime.Now.Date;
        public static ObservableCollection<Annonce> ToutesLesAnnonces = new ObservableCollection<Annonce>();

        #region Propriétés
        public int Id { get; set; }
        public Utilisateur Annonceur { get; set; }
        public string Titre { get; set; }
        public DateTime DatePublication { get; set; }
        public TypeAnnonce Type { get; set; }
        public string Description { get; set; }
        public ObservableCollection<Item> ListeItems { get; set; }
        public double Montant { get; set; }
		  public EtatAnnonce EtatAnnonce { get; set; }
		#endregion

		#region Constructeur
		public Annonce()
        {
            UtilisateurADO UtilisateurConnecte = new UtilisateurADO();
            ListeItems = new ObservableCollection<Item>();
            Annonceur = UtilisateurConnecte.RetourUtilisateurActif();
            DatePublication = DateTime.Now.Date;
            EtatAnnonce = new EtatAnnonce(EtatsAnnonce.Active);
        }

        public Annonce(DataRow dr)
        {
            UtilisateurADO ud = new UtilisateurADO();
            //AnnonceADO annonceADO = new AnnonceADO();

            Id = (int)dr["Id"];
            Annonceur = ud.RechercherUtilisateurById((int)dr["Annonceur_Id"]);
            Titre = (string)dr["Titre"];
            DatePublication = (DateTime)dr["DatePublication"];
            Type = new TypeAnnonce((string)dr["typeAnnonce"]);
            Description = (string)dr["Description"];

            //ListeItems = annonceADO.RecupererListeItems(Id);
            ListeItems = new ObservableCollection<Item>();
            Montant = (double)dr["montant"];
            EtatAnnonce = new EtatAnnonce((string)dr["etatAnnonce"]);

            if(DatePublication < PlusAncienneDate)
            {
                PlusAncienneDate = DatePublication;
            }
        }

        public void RecupererItems()
        {
            AnnonceADO annonceADO = new AnnonceADO();
            ListeItems = annonceADO.RecupererListeItems(Id);
        }
        #endregion


    }
}

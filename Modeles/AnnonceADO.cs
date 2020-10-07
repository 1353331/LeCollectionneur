using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
    class AnnonceADO
    {
        private BdBase MaBD;

        public AnnonceADO() { MaBD = new BdBase(); }
        public ObservableCollection<Annonce> Recuperer()
        {
            ObservableCollection<Annonce> ListeAnnonces = new ObservableCollection<Annonce>();
            string sel = "SELECT a.*, ta.Nom AS typeAnnonce from annonces a LEFT JOIN typesannonce ta ON a.idTypeAnnonce = ta.Id";
            DataSet SetAnnonce = MaBD.Selection(sel);
            DataTable TableAnnonce = SetAnnonce.Tables[0];

            foreach (DataRow RowAnnonce in TableAnnonce.Rows)
            {
                ListeAnnonces.Add(new Annonce(RowAnnonce));
            }

            return ListeAnnonces;
        }
    }
}

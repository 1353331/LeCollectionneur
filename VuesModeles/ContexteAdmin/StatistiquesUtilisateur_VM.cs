using LeCollectionneur.EF;
using LeCollectionneur.Modeles;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.VuesModeles.ContexteAdmin
{
    class StatistiquesUtilisateur_VM : INotifyPropertyChanged
    {
		UtilisateurADO utilisateurADO = new UtilisateurADO();
        private string _txtNbCollections;
        public string TxtNbCollections
        {
            get { return _txtNbCollections; }
            set
            {
                _txtNbCollections = value;
                OnPropertyChanged("TxtNbCollections");
            }
        }
        private string _txtNbItems;
        public string TxtNbItems
        {
            get { return _txtNbItems; }
            set
            {
                _txtNbItems = value;
                OnPropertyChanged("TxtNbItems");
            }
        }

        private string _txtNbAnnonces;
        public string TxtNbAnnonces
        {
            get { return _txtNbAnnonces; }
            set
            {
                _txtNbAnnonces = value;
                OnPropertyChanged("TxtNbAnnonces");
            }
        }
        private string _txtNbPropositions;
        public string TxtNbPropositions
        {
            get { return _txtNbPropositions; }
            set
            {
                _txtNbPropositions = value;
                OnPropertyChanged("TxtNbPropositions");
            }
        }
        private string _txtNbTransactions;
        public string TxtNbTransactions
        {
            get { return _txtNbTransactions; }
            set
            {
                _txtNbTransactions = value;
                OnPropertyChanged("TxtNbTransactions");
            }
        }
        private string _titre;
        public string Titre
        {
            get { return _titre; }
            set
            {
                _titre = value;
                OnPropertyChanged("Titre");
            }
        }
        public StatistiquesUtilisateur_VM(Utilisateur utilisateur)
        {
			InitialiserChamps(utilisateur);
        }
		private void InitialiserChamps(Utilisateur utilisateur)
		{

			int nbCollections = utilisateurADO.CompterCollections(utilisateur);
			int nbItems = utilisateurADO.CompterItems(utilisateur);
			int nbAnnonces = utilisateurADO.CompterAnnonces(utilisateur);
			int nbPropositions = utilisateurADO.CompterPropositions(utilisateur);
			int nbTransactions = utilisateurADO.CompterTransactions(utilisateur);

            TxtNbCollections = $"Collections possédées: {nbCollections}";
            TxtNbItems = $"Items possédés: {nbItems}";
            TxtNbAnnonces=$"Annonces publiées: {nbAnnonces}";
            TxtNbPropositions=$"Propositions envoyées: {nbPropositions}";
            TxtNbTransactions=$"Transactions complétées: {nbTransactions}";
            Titre = $"Statistiques de {utilisateur.NomUtilisateur}";
        }


        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        #endregion

    }
}

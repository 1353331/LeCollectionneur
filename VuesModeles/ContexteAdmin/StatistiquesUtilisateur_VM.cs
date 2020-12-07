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
        private string _txtNbPropositionsRecues;
        public string TxtNbPropositionsRecues
        {
            get { return _txtNbPropositionsRecues; }
            set
            {
                _txtNbPropositionsRecues = value;
                OnPropertyChanged("TxtNbPropositionsRecues");
            }
        }

        private string _txtNbTransactionsAnnonceur;
        public string TxtNbTransactionsAnnonceur
        {
            get { return _txtNbTransactionsAnnonceur; }
            set
            {
                _txtNbTransactionsAnnonceur = value;
                OnPropertyChanged("TxtNbTransactionsAnnonceur");
            }
        }
        private string _txtNbTransactionsProposeur;
        public string TxtNbTransactionsProposeur
        {
            get { return _txtNbTransactionsProposeur; }
            set
            {
                _txtNbTransactionsProposeur = value;
                OnPropertyChanged("TxtNbTransactionsProposeur");
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
            int nbPropositionsRecues = utilisateurADO.CompterPropositionsRecues(utilisateur);
			int nbTransactionsAnnonceur = utilisateurADO.CompterTransactions(utilisateur,true);
            int nbTransactionsProposeur = utilisateurADO.CompterTransactions(utilisateur, false);
            TxtNbCollections = $"Collections possédées: {nbCollections}";
            TxtNbItems = $"Items possédés: {nbItems}";
            TxtNbAnnonces=$"Annonces publiées: {nbAnnonces}";
            TxtNbPropositions=$"Propositions envoyées: {nbPropositions}";
            TxtNbPropositionsRecues=$"Propositions reçues: {nbPropositionsRecues}";
            TxtNbTransactionsAnnonceur =$"Transactions complétées en tant qu'annonceur: {nbTransactionsAnnonceur}";
            TxtNbTransactionsProposeur=$"Transactions complétées en tant que proposeur: {nbTransactionsProposeur}";
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

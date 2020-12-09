using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Ubiety.Dns.Core.Records;

namespace LeCollectionneur.VuesModeles
{
    class Parametre_VM : INotifyPropertyChanged
    {
        #region Propriétés
        private bool _aModif;
        public bool aModif
        {
            get { return _aModif; }
            set
            {
                _aModif = value;

                OnPropertyChanged("aModif");
            }
        }


        private string _nouvNom;
        public string nouvNom
        {
            get { return _nouvNom; }
            set
            {
                _nouvNom = value;
                try
                {
                    aModif = (nouvNom != UtilisateurADO.utilisateur.NomUtilisateur || nouvCourriel != UtilisateurADO.utilisateur.Courriel || mpNouv != "" || mpNouvConf != "");
                }
                catch { aModif = false; }
                OnPropertyChanged("nouvNom");
            }
        }

        private string _nouvCourriel;
        public string nouvCourriel
        {
            get { return _nouvCourriel; }
            set
            {
                _nouvCourriel = value;
                try
                {
                    aModif = (nouvNom != UtilisateurADO.utilisateur.NomUtilisateur || nouvCourriel != UtilisateurADO.utilisateur.Courriel || mpNouv != "" || mpNouvConf != "");
                }
                catch { aModif = false; }
                OnPropertyChanged("nouvCourriel");
            }
        }

        private string _mpActuel;
        public string mpActuel
        {
            get { return _mpActuel; }
            set
            {
                _mpActuel = value;
                try
                {
                    aModif = (nouvNom != UtilisateurADO.utilisateur.NomUtilisateur || nouvCourriel != UtilisateurADO.utilisateur.Courriel || mpNouv != "" || mpNouvConf != "");
                }
                catch { aModif = false; }
                OnPropertyChanged("mpActuel");
            }
        }

        private string _mpNouv;
        public string mpNouv
        {
            get { return _mpNouv; }
            set
            {
                _mpNouv = value;
                try
                {
                    aModif = (nouvNom != UtilisateurADO.utilisateur.NomUtilisateur || nouvCourriel != UtilisateurADO.utilisateur.Courriel || mpNouv != "" || mpNouvConf != "");
                }
                catch { aModif = false; }
                OnPropertyChanged("mpNouv");
            }
        }

        private string _mpNouvConf;
        public string mpNouvConf
        {
            get { return _mpNouvConf; }
            set
            {
                _mpNouvConf = value;
                try
                {
                    aModif = (nouvNom != UtilisateurADO.utilisateur.NomUtilisateur || nouvCourriel != UtilisateurADO.utilisateur.Courriel || mpNouv != "" || mpNouvConf != "");
                }
                catch { aModif = false; }
                OnPropertyChanged("mpNouvConf");
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {

            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        #endregion

        #region Commandes
        private ICommand _cmdModifierCompte;
        public ICommand cmdModifierCompte
        {
            get { return _cmdModifierCompte; }
            set
            {
                _cmdModifierCompte = value;
                OnPropertyChanged("cmdModifierCompte");
            }
        }
        private void cmdModifierCompte_Compte(object param)
        {

            if ((mpNouv == "" || mpNouv == null) && (mpNouvConf == "" || mpNouvConf == null) && (nouvCourriel == UtilisateurADO.utilisateur.Courriel || nouvCourriel == "" || nouvCourriel == null) && (nouvNom == UtilisateurADO.utilisateur.NomUtilisateur || nouvNom == "" || nouvNom == null))
            {
                MessageBox.Show("Aucune Modifcation à faire");
            }
            //Valide que le mot de passe est valide
            else if (mpActuel != null)
            {
                if (getMP(mpActuel))
                {
                    if (validationInfo())
                        updateInfo();
                }
                else
                {
                    mpActuel = "";
                    MessageBox.Show("Mot de passe Actuel Invalide");
                }
                
            }
        }
        #endregion

        #region Contructeur
        private void updateInfo()
        {
            string req="";
            if (mpNouv!="")
                 req= "UPDATE `utilisateurs` SET `NomUtilisateur` = '"+nouvNom+"', `Courriel` = '"+nouvCourriel+ "',`MotDePasse` = '" + mpNouv + "'  WHERE `utilisateurs`.`Id` = " + UtilisateurADO.utilisateur.Id+";";
            else
                req = "UPDATE `utilisateurs` SET `NomUtilisateur` = '"+nouvNom+"', `Courriel` = '"+nouvCourriel+ "'  WHERE `utilisateurs`.`Id` = " + UtilisateurADO.utilisateur.Id+";";

            new BdBase().Commande(req);
            affichageModification();
            new UtilisateurADO().connectionParId(UtilisateurADO.utilisateur.Id);
            mpNouv = "";
            mpNouvConf = "";
            mpActuel = "";
        }
        private void affichageModification()
        {
            string temp = "";
            if (nouvNom != UtilisateurADO.utilisateur.NomUtilisateur)
                temp += "Nouveau Nom : "+nouvNom+"\n";
            if (nouvCourriel != UtilisateurADO.utilisateur.Courriel)
                temp += "Nouveau Courriel : "+nouvCourriel+"\n";
            if (mpNouv != "")
                temp += "Nouveau Mot de Passe : "+mpNouv;
            MessageBox.Show(temp);
        }
        private bool validationInfo()
        {
            string MessageErreur="";

            //Valide NouvUser
            if (nouvNom != UtilisateurADO.utilisateur.NomUtilisateur && (nouvNom == "" || new UtilisateurADO().CheckSiUserEstPrit(nouvNom)))
            {
                MessageErreur += "Erreur : Nom Utilisateur Invalide\n";
            }
            //Valide Courriel
            if (nouvCourriel != UtilisateurADO.utilisateur.Courriel && (new UtilisateurADO().ValideCourriel(nouvCourriel)))
            {
                if(MessageErreur =="")
                    MessageErreur += "Erreur : Courriel Invalide\n";
                else
                    MessageErreur += "       : Courriel Invalide\n"; 
            }
            if(mpNouv != mpNouvConf && mpNouv !="")
            {
                if (MessageErreur == "")
                    MessageErreur += "Erreur : Nouveau Mot de passe Invalide\n";
                else
                    MessageErreur += "       : Nouveau Mot de passe";
            }
            if (MessageErreur == "")
                return true;
            else
            {
                MessageBox.Show(MessageErreur);
                return false;
            }    
        }
        public Parametre_VM()
        {
            mpNouv = "";
            mpNouvConf = "";
            mpActuel = "";
            nouvCourriel = UtilisateurADO.utilisateur.Courriel;
            nouvNom = UtilisateurADO.utilisateur.NomUtilisateur;
            cmdModifierCompte = new Commande(cmdModifierCompte_Compte);
            aModif =false;
        }
        #endregion

        #region Method
        private void textChangedEventHandler(object sender)
        {
            aModif = (nouvNom == UtilisateurADO.utilisateur.NomUtilisateur && nouvCourriel == UtilisateurADO.utilisateur.Courriel && mpNouv == "" && mpNouvConf == "");        
        }
        private bool getMP(string mp)
        {
            string req = "SELECT `Id` FROM `utilisateurs` WHERE Id ="+UtilisateurADO.utilisateur.Id+" AND `MotDePasse` = '"+mp+"'";
            BdBase bd = new BdBase();
            var e = bd.Selection(req);
            return e.Tables[0].Rows.Count == 1;
        }
        #endregion
    }
}
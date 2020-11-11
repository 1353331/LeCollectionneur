using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Ubiety.Dns.Core.Records;

namespace LeCollectionneur.VuesModeles
{
    class Parametre_VM : INotifyPropertyChanged
    {
        #region Propriétés

        private string _nouvNom;
        public string nouvNom
        {
            get { return _nouvNom; }
            set
            {
                _nouvNom = value;
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

            if ((mpNouv == ""|| mpNouv==null) && (mpNouvConf == ""||mpNouvConf==null) && (nouvCourriel == UtilisateurADO.utilisateur.Courriel ||nouvCourriel =="" || nouvCourriel==null)&& (nouvNom ==UtilisateurADO.utilisateur.NomUtilisateur||nouvNom==""||nouvNom==null))
            {
                MessageBox.Show("Aucune Modifcation à faire");
            }
            //Valide que le mot de passe est valide
            else if(mpActuel!=null)
            {
                if (getMP(mpActuel))
                {
                    string deBase = "UPDATE `utilisateurs` SET";
                    string req = deBase;
                    //string req = "UPDATE `utilisateurs` SET `NomUtilisateur` = 'co', `Courriel` = 'col4@outlook.com' WHERE `utilisateurs`.`Id` = 4;";
                    
                    //Valide que les nouveau mot de passe sont valide et non vide
                    if (mpNouv == mpNouvConf && mpNouv!="")
                        req = req + " `MotDePasse` = '"+mpNouv+"',";
                    else if(mpNouvConf!=mpNouv && (mpNouv!=""||mpNouv!=null ||mpNouvConf!=""||mpNouvConf!=null))
                        MessageBox.Show("Les nouveaux mot de passe ne sont pas identique");
                    if (nouvNom != UtilisateurADO.utilisateur.NomUtilisateur&&(nouvNom != "" &&!(new UtilisateurADO().CheckSiUserEstPrit(nouvNom))))
                    { 
                            req = req + "`NomUtilisateur` = '" + nouvNom + "',";
                    }
                    else if (nouvNom != "" || nouvNom == null)
                        MessageBox.Show("Le nouveaux nom utilisateur n'est pas valide");
                        

                    if (!(new UtilisateurADO().ValideCourriel(nouvCourriel)))
                        req = req + "`Courriel` = '"+nouvCourriel+"',";
                    else if(nouvCourriel !="")
                        MessageBox.Show("Le nouveaux courriel n'est pas valide");

                    if (req != deBase)
                    {
                        if (req.Last() == ',')
                        {
                            req = req.Remove(req.Length - 1);
                        }
                        req = req + "WHERE `utilisateurs`.`Id` = " + UtilisateurADO.utilisateur.Id + "; ";

                        string t = "Les valeur Suivantes on été mit à jour:";
                        if (nouvNom != "" && !(new UtilisateurADO().CheckSiUserEstPrit(nouvNom)))
                        {
                            t = t + "\nNouveau User = " + nouvNom;
                            UtilisateurADO.utilisateur.NomUtilisateur= nouvNom;
                            
                        }
                        new BdBase().Commande(req);
                        if (!(new UtilisateurADO().ValideCourriel(nouvCourriel)))
                        {
                            t = t + "\nNouveau Courriel = " + nouvCourriel;
                            UtilisateurADO.utilisateur.Courriel = nouvCourriel;
                            
                        }
                        if (mpNouv == mpNouvConf && mpNouv != "")
                        {
                            t = t + "\nNouveau Mot De Passe = " + mpNouv;
                            mpNouv = "";
                            mpNouvConf = "";
                        }
                        
                        mpActuel = "";

                        MessageBox.Show(t);
                    }
                }
                else
                    MessageBox.Show("Votre Mot de Passe Actuel est invalide");
            }
        }
        #endregion

        #region Contructeur
        public Parametre_VM()
        {
            mpNouv = "";
            mpNouvConf = "";
            mpActuel = "";
            nouvCourriel = UtilisateurADO.utilisateur.Courriel;
            nouvNom = UtilisateurADO.utilisateur.NomUtilisateur;
            cmdModifierCompte = new Commande(cmdModifierCompte_Compte);
        }
        #endregion

        #region Method
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
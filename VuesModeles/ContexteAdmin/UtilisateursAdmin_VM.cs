using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace LeCollectionneur.VuesModeles.ContexteAdmin
{
    class UtilisateursAdmin_VM : INotifyPropertyChanged
    {
        #region Propriétés
        private ContentPresenter presenteurContenu;
        private UtilisateurADO utilisateurADO = new UtilisateurADO();
        private Utilisateur _utilisateurSelectionne;
        public Utilisateur UtilisateurSelectionne
        {
            get { return _utilisateurSelectionne; }
            set
            {
                
                _utilisateurSelectionne = value;
                OnPropertyChanged("UtilisateurSelectionne");
                presenteurContenu.Content = new Vues.ContexteAdmin.UCStatistiquesUtilisateur(_utilisateurSelectionne);
            }
        }
        private ObservableCollection<Utilisateur> _lstUtilisateurs;
        public ObservableCollection<Utilisateur> LstUtilisateurs 
        { get { return _lstUtilisateurs; }
          set
            {
                _lstUtilisateurs = value;
                OnPropertyChanged("LstUtilisateurs");
            }
        }
        #endregion

        #region Commandes

        private ICommand _cmdEstActif;
        public ICommand CmdEstActif
        {
            get
            {
                return _cmdEstActif;
            }
            set
            {
                _cmdEstActif = value;
                OnPropertyChanged("CmdEstActif");
            }
        }
        private void cmd_EstActif(object param)
        {
            if (UtilisateurSelectionne !=null)
            {
                UtilisateurSelectionne.EstActif = !UtilisateurSelectionne.EstActif;
                utilisateurADO.ModifierEstActif(UtilisateurSelectionne);
            }

        }

        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string nomPropriete)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(nomPropriete));
        }
        #endregion

        #region Constructeur
        public UtilisateursAdmin_VM(ref ContentPresenter contentPresenter)
        {
            LstUtilisateurs = utilisateurADO.GetAllUsers();
            presenteurContenu = contentPresenter;
            CmdEstActif = new Commande(cmd_EstActif);
        }
        #endregion
    }
}

using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace LeCollectionneur.VuesModeles
{
    class AjoutItemCollection_VM : INotifyPropertyChanged
    {
        #region Propriétés
        public ObservableCollection<string> ConditionsPossibles { get; set; }
        public ObservableCollection<string> TypesPossibles { get; set; }
        public Collection CollectionAjoutItem { get; set; }
        private Item _itemAjout;
        public Item ItemAjout
        {
            get
            {
                return _itemAjout;
            }
            set
            {
                _itemAjout = value;
                OnPropertyChanged("ItemAjout");
            }
        }
        public string LblErreur { get; set; }
        private string _nomFichier;
        public string NomFichier 
        { get
            {
                return _nomFichier;
            }
            set
            {
                _nomFichier = value;
                NomCourtFichier = Path.GetFileName(NomFichier);
                OnPropertyChanged("NomFichier");
            }
        }
        private string _nomCourtFichier;
        public string NomCourtFichier
        {
            get
            {
                return _nomCourtFichier;
            }
            set
            {
                _nomCourtFichier = value;
                OnPropertyChanged("NomCourtFichier");
            }
        }
        private ItemADO gestionnaireItem = new ItemADO();
        #endregion

        #region Commandes
        // Collection
        private ICommand _cmdAjouterItem;
        public ICommand cmdAjouterItem
        {
            get
            {
                return _cmdAjouterItem;
            }
            set
            {
                _cmdAjouterItem = value;
                OnPropertyChanged("cmdAjouterItem");
            }
        }
        private void cmdAjouter_Item(object param)
        {
            IFenetreFermeable fenetre = param as IFenetreFermeable;
            
            if (!(ItemAjout.Nom is null)&&ItemAjout.Nom.Trim().Length>0 && !(ItemAjout.Type is null) && ItemAjout.Type.Length>0&& !(ItemAjout.Condition is null) && ItemAjout.Condition.Length>0)
            {
                if (ItemAjout.Nom.Trim().Length>60)
                {
                    MessageBox.Show("Le nom de l'item doit contenir un maximum de 60 caractères.");
                    return;
                }
                ItemAjout.Nom = Validateur.Echappement(ItemAjout.Nom.Trim());
                if (!(ItemAjout.Description is null)&&ItemAjout.Description.Trim().Length > 0)
                    ItemAjout.Description = Validateur.Echappement(ItemAjout.Description.Trim());
                
                if (!(ItemAjout.Manufacturier is null) && ItemAjout.Manufacturier.Trim().Length > 0)
                    if (!(ItemAjout.Manufacturier.Trim().Length>50))
                    ItemAjout.Manufacturier = Validateur.Echappement(ItemAjout.Manufacturier.Trim());
                    else
                    {
                        MessageBox.Show("Le nom du producteur doit avoir un maximum de 50 caractères.");
                        return;
                    }
               
                int id =gestionnaireItem.AjouterAvecRetourId(ItemAjout, CollectionAjoutItem);
                if (NomFichier.Length > 0)
                {
                    Fichier.TeleverserFichierFTP(id, NomFichier);
                    gestionnaireItem.AjouterCheminImage(id);
                }
                   
                fenetre.Fermer();
            }
            else
            {
                MessageBox.Show("Veuillez au minimum choisir un nom, un type et une condition.");
            }
        }
        private ICommand _cmdAjouterImage;
        public ICommand cmdAjouterImage
        {
            get
            {
                return _cmdAjouterImage;
            }
            set
            {
                _cmdAjouterImage = value;
                OnPropertyChanged("cmdAjouterImage");
            }
        }
        private void cmdAjouter_Image(object param)
        {
            NomFichier=Fichier.ImporterFichier();
        }

        private ICommand _cmdAnnuler;
        public ICommand cmdAnnuler
        {
            get
            {
                return _cmdAnnuler;
            }
            set
            {
                _cmdAnnuler = value;
                OnPropertyChanged("cmdAnnuler");
            }
        }
        private void cmd_Annuler(object param)
        {
            IFenetreFermeable fenetre = param as IFenetreFermeable;
            fenetre.Fermer();
        }
        #endregion

        #region Constructeur
        public AjoutItemCollection_VM(Collection laCollection)
        {
            LblErreur = "";
            cmdAjouterItem = new Commande(cmdAjouter_Item);
            cmdAnnuler = new Commande(cmd_Annuler);
            cmdAjouterImage = new Commande(cmdAjouter_Image);
            CollectionAjoutItem = laCollection;
            ItemAjout = new Item();
            ConditionsPossibles = gestionnaireItem.ConditionsPossibles;
            TypesPossibles = gestionnaireItem.TypesPossibles;
            NomFichier = "";
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

    }
}

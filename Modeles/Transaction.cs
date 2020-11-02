using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Outils.Enumerations;

namespace LeCollectionneur.Modeles
{
	public class Transaction
	{
		public Proposition PropositionTrx { get; set; }
		public Annonce AnnonceTrx { get; set; }

		public Transaction()
		{
			PropositionTrx = new Proposition();
			AnnonceTrx = new Annonce();
		}

		public Transaction(int idAnnonce, int idProposition)
		{
			PropositionTrx = new PropositionADO().RecupererUnParId(idProposition);
			AnnonceTrx = new AnnonceADO().RecupererUn(idAnnonce);
		}

		public Transaction(Proposition proposition)
		{
			PropositionTrx = proposition;
			AnnonceTrx = proposition.AnnonceLiee;
		}

		public void EffectuerTransaction()
		{
			PropositionADO propADO = new PropositionADO();
			AnnonceADO annonceADO = new AnnonceADO();
			CollectionADO collADO = new CollectionADO();
			ItemADO iADO = new ItemADO();

			PropositionTrx.EtatProposition = EtatsProposition.Acceptee;
			propADO.Modifier(PropositionTrx);
			AnnonceTrx.EtatAnnonce = EtatsAnnonce.Terminee;
			annonceADO.Modifier(AnnonceTrx);

			if (PropositionTrx.ItemsProposes.Count() > 0)
			{
				propADO.AnnulerPropositionsActivesAvecItems(PropositionTrx.ItemsProposes);
				annonceADO.AnnulerAnnoncesActivesAvecItems(PropositionTrx.ItemsProposes);

				//Transfert Items Proposeur->Annonceur
				int idNouvelleCollectionAnnonceur = collADO.AjouterRetourId(
					new Collection() { DateCreation = DateTime.Now, Nom = $"{PropositionTrx.AnnonceLiee.Type}: {PropositionTrx.AnnonceLiee.Titre}", ItemsCollection = new ObservableCollection<Item>() },
					PropositionTrx.AnnonceLiee.Annonceur.Id
					);	

				Collection collectionAnnonceur = collADO.RecupererUn(idNouvelleCollectionAnnonceur);
				foreach (Item item in PropositionTrx.ItemsProposes)
				{
					iADO.TransfererItem(collectionAnnonceur, item);
				}
			}

			propADO.AnnulerPropositionsActivesAvecItems(PropositionTrx.AnnonceLiee.ListeItems);
			annonceADO.AnnulerAnnoncesActivesAvecItems(PropositionTrx.AnnonceLiee.ListeItems);
			//Transfert Items Annonceur->Proposeur
			int idNouvelleCollectionProposeur = collADO.AjouterRetourId(
					new Collection() { DateCreation = DateTime.Now, Nom = $"{PropositionTrx.AnnonceLiee.Type}: {PropositionTrx.AnnonceLiee.Titre}", ItemsCollection = new ObservableCollection<Item>() },
					PropositionTrx.Proposeur.Id
					) ;

			Collection collectionProposeur = collADO.RecupererUn(idNouvelleCollectionProposeur);
			foreach (Item item in PropositionTrx.AnnonceLiee.ListeItems)
			{
				iADO.TransfererItem(collectionProposeur, item);
			}

			//TODO: Créer Transaction ADO/Enregistrer la transaction en BD
		}
	}
}

﻿using System;
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
	[Table("Transactions")]
	public class Transaction
	{
		public int Id { get; set; }
		public Proposition PropositionTrx { get; set; }
		public DateTime Date { get; set; }
		[NotMapped]
		public string Role { get; set; }

		public Transaction()
		{

		}
		public Transaction(Proposition proposition)
		{
			PropositionTrx = proposition;
			Date = DateTime.Now;
			Role = PropositionTrx.Proposeur.Id == UtilisateurADO.utilisateur.Id ? "Proposeur" : "Annonceur";
		}

		public Transaction(DataRow dr)
		{
			Id = (int)dr["id"];
			PropositionTrx = new PropositionADO().RecupererUnParId((int)dr["PropositionTrx_Id"]);
			Date = (DateTime)dr["date"];
			Role = PropositionTrx.Proposeur.Id == UtilisateurADO.utilisateur.Id ? "Proposeur" : "Annonceur";
		}

		public void EffectuerTransaction()
		{
			PropositionADO propADO = new PropositionADO();
			AnnonceADO annonceADO = new AnnonceADO();
			CollectionADO collADO = new CollectionADO();
			ItemADO iADO = new ItemADO();

			PropositionTrx.EtatProposition = new EtatProposition(EtatsProposition.Acceptee);
			propADO.Modifier(PropositionTrx);
			PropositionTrx.AnnonceLiee.EtatAnnonce = new EtatAnnonce(EtatsAnnonce.Terminee);
			annonceADO.Modifier(PropositionTrx.AnnonceLiee);

			propADO.RefuserPropositionsActivesSurAnnonce(PropositionTrx.AnnonceLiee.Id);

			if (PropositionTrx.ItemsProposes.Count() > 0)
			{
				propADO.AnnulerPropositionsActivesAvecItems(PropositionTrx.ItemsProposes);
				annonceADO.AnnulerAnnoncesActivesAvecItems(PropositionTrx.ItemsProposes);

				//Transfert Items Proposeur->Annonceur
				int idNouvelleCollectionAnnonceur = collADO.AjouterRetourId(
					new Collection() { DateCreation = DateTime.Now, Nom = $"{PropositionTrx.AnnonceLiee.Type.Nom}: {PropositionTrx.AnnonceLiee.Titre.Replace("'", @"\'")}", ItemsCollection = new ObservableCollection<Item>() },
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
					new Collection() { DateCreation = DateTime.Now, Nom = $"{PropositionTrx.AnnonceLiee.Type.Nom}: {PropositionTrx.AnnonceLiee.Titre.Replace("'", @"\'")}", ItemsCollection = new ObservableCollection<Item>() },
					PropositionTrx.Proposeur.Id
					);

			Collection collectionProposeur = collADO.RecupererUn(idNouvelleCollectionProposeur);
			foreach (Item item in PropositionTrx.AnnonceLiee.ListeItems)
			{
				iADO.TransfererItem(collectionProposeur, item);
			}

			//TODO: Créer Transaction ADO/Enregistrer la transaction en BD
			new TransactionADO().Ajouter(this);
		}
	}
}

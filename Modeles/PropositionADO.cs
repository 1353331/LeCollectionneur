﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Enumerations;
using MySql.Data.MySqlClient;

namespace LeCollectionneur.Modeles
{
	public class PropositionADO
	{
		private BdBase MaBd;

		public PropositionADO()
		{
			MaBd = new BdBase();
		}

		#region CREATE

		public void Ajouter(Proposition prop)
		{
			string requete = $@"
			INSERT INTO Propositions (Id, IdAnnonce, IdUtilisateur, Montant, Date, IdEtatProposition)
			VALUES
			(NULL, {prop.AnnonceLiee.Id}, {prop.Proposeur.Id}, {prop.Montant}, '{prop.DateProposition}', (SELECT Id FROM EtatsProposition WHERE Nom = '{prop.EtatProposition}'));
			SELECT LAST_INSERT_ID();
			";
			prop.Id = MaBd.CommandeCreationAvecRetourId(requete);

			//S'il y a des objets à ajouter à la proposition, on les ajoute
			if (prop.ItemsProposes.Count > 0)
			{
				ajouterItemsAProposition(prop.Id, prop.ItemsProposes);
			}
		}

		#endregion

		#region READ

		public ObservableCollection<Proposition> RecupererTout()
		{

			ObservableCollection<Proposition> ocPropositions = new ObservableCollection<Proposition>();

			string requeteProposition = @"
			SELECT p.Id, p.idAnnonce, p.idUtilisateur, p.montant, p.date AS dateProposition, ep.Nom AS etatProposition
			FROM Propositions p
			LEFT JOIN EtatsProposition ep
			ON p.IdEtatProposition = ep.Id
			";

			DataSet setProposition = MaBd.Selection(requeteProposition);
			DataTable TableProposition = setProposition.Tables[0];

			foreach (DataRow drProposition in TableProposition.Rows)
			{
				Proposition propTemp = new Proposition(drProposition);
				propTemp.ItemsProposes = recupererItemsProposition(propTemp.Id);

				ocPropositions.Add(propTemp);
			}
			return ocPropositions;
		}

		public Proposition RecupererUnParId(int idProposition)
		{
			string requete = $@"
			SELECT p.Id, p.idAnnonce, p.idUtilisateur, p.montant, p.date AS dateProposition, ep.Nom AS etatProposition
			FROM Propositions p
			LEFT JOIN EtatsProposition ep
			ON p.IdEtatProposition = ep.Id
			WHERE p.Id = {idProposition} 
			";

			DataSet setProposition = MaBd.Selection(requete);
			DataTable TableProposition = setProposition.Tables[0];

			Proposition propTemp = new Proposition(TableProposition.Rows[0]);
			propTemp.ItemsProposes = recupererItemsProposition(propTemp.Id);

			return propTemp;
		}

		public int RecupererIdParProposition(Proposition prop)
		{
			string reqId = $@"
			SELECT p.Id
			FROM Propositions p
			LEFT JOIN EtatsProposition ep
			ON p.IdEtatProposition = ep.Id
			WHERE p.idAnnonce = {prop.AnnonceLiee.Id} AND p.idUtilisateur = {prop.Proposeur.Id} AND p.montant = {prop.Montant} AND p.date = '{prop.DateProposition}' AND ep.Nom = '{prop.EtatProposition}'
			";

			DataSet setId = MaBd.Selection(reqId);
			DataTable tableId = setId.Tables[0];

			return (int)tableId.Rows[0]["Id"];
		}

		public ObservableCollection<Proposition> RecupererPropositionsEnvoyees(int idUtilisateur)
		{
			string requete = $@"
				SELECT p.Id, p.idAnnonce, p.idUtilisateur, p.montant, p.date AS dateProposition, ep.Nom AS etatProposition
				FROM Propositions p
				LEFT JOIN EtatsProposition ep
				ON p.IdEtatProposition = ep.Id
				WHERE p.idUtilisateur = {idUtilisateur}
				ORDER BY dateProposition DESC; 
			";
			ObservableCollection<Proposition> ocPropositions = new ObservableCollection<Proposition>();
			DataSet setProposition = MaBd.Selection(requete);
			DataTable TableProposition = setProposition.Tables[0];

			foreach (DataRow drProposition in TableProposition.Rows)
			{
				Proposition propTemp = new Proposition(drProposition);
				propTemp.ItemsProposes = recupererItemsProposition(propTemp.Id);

				ocPropositions.Add(propTemp);
			}
			return ocPropositions;
		}

		public ObservableCollection<Proposition> RecupererPropositionsRecues(int idUtilisateur)
		{
			string requete = $@"
				SELECT p.Id, p.idAnnonce, p.idUtilisateur, p.montant, p.date AS dateProposition, ep.Nom AS etatProposition
				FROM Propositions p
				INNER JOIN EtatsProposition ep
				ON p.IdEtatProposition = ep.Id
				LEFT JOIN Annonces a
				ON p.idAnnonce = a.Id
				WHERE a.idUtilisateur = {idUtilisateur}
				ORDER BY dateProposition DESC;
			";
			ObservableCollection<Proposition> ocPropositions = new ObservableCollection<Proposition>();
			DataSet setProposition = MaBd.Selection(requete);
			DataTable TableProposition = setProposition.Tables[0];

			foreach (DataRow drProposition in TableProposition.Rows)
			{
				Proposition propTemp = new Proposition(drProposition);
				propTemp.ItemsProposes = recupererItemsProposition(propTemp.Id);

				ocPropositions.Add(propTemp);
			}
			return ocPropositions;
		}

		#endregion

		#region UPDATE

		public void Modifier(Proposition prop)
		{
			string requete = $@"
			UPDATE Propositions
			SET
			IdAnnonce = {prop.AnnonceLiee.Id},
			IdUtilisateur = {prop.Proposeur.Id},
			Montant = {prop.Montant},
			Date = '{prop.DateProposition:yyyy-MM-dd HH:mm:ss}',
			IdEtatProposition = (SELECT Id FROM EtatsProposition WHERE Nom = '{prop.EtatProposition}')
			WHERE Id = {prop.Id};
			";

			MaBd.Commande(requete);
		}

		public void AnnulerPropositionsActivesAvecItems(IEnumerable<Item> items)
		{
			if (items.Count() > 0)
			{
				string idItems = "";

				foreach (Item item in items)
				{
					idItems += $"{item.Id}";
					if (item != items.Last())
						idItems += ", ";
				}

				string requete = $@"
				UPDATE Propositions
				SET
				IdEtatProposition = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.Annulee}')
				WHERE IdEtatProposition = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.EnAttente}')
				AND Id IN (
								SELECT IdProposition
								FROM ItemProposition
								WHERE IdItem IN ({idItems})
						   );
				
				";
				/* Enlever la suppression des items des propositions pour l'instant
				
				DELETE FROM ItemProposition
            WHERE IdItem IN ({idItems})
            AND IdProposition IN (
                                SELECT Id
                                FROM Propositions
                                WHERE IdEtatProposition = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.Annulee}')
                             )
				*/
				MaBd.Commande(requete);
			}
		}

		public void RefuserPropositionsActivesAvecItems(IEnumerable<Item> items)
		{
			if (items.Count() > 0)
			{
				string idItems = "";

				foreach (Item item in items)
				{
					idItems += $"{item.Id}";
					if (item != items.Last())
						idItems += ", ";
				}

				string requete = $@"
				UPDATE Propositions
				SET
				IdEtatProposition = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.Refusee}')
				WHERE IdEtatProposition = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.EnAttente}')
				AND Id IN (
								SELECT IdProposition
								FROM ItemProposition
								WHERE IdItem IN ({idItems})
						   );
				
				";
				/* Enlever la suppression des items des propositions pour l'instant
				
				DELETE FROM ItemProposition
            WHERE IdItem IN ({idItems})
            AND IdProposition IN (
                                SELECT Id
                                FROM Propositions
                                WHERE IdEtatProposition = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.Annulee}')
                             )
				*/
				MaBd.Commande(requete);
			}
		}

		public void RefuserPropositionsActivesSurAnnonce(int idAnnonce)
		{
			string requete = $@"
				UPDATE Propositions
				SET
				IdEtatProposition = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.Refusee}')
				WHERE IdEtatProposition = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.EnAttente}')
						AND IdAnnonce = {idAnnonce}
				";

			MaBd.Commande(requete);
		}

		#endregion

		#region DELETE
		public void Supprimer(int IdProposition)
		{
			string requete = $@"
			DELETE FROM Propositions
			WHERE Id = {IdProposition}
			";

			MaBd.Commande(requete);
		}


		#endregion

		#region Méthodes privées

		private ObservableCollection<Item> recupererItemsProposition(int idProposition)
		{
			ObservableCollection<Item> items = new ObservableCollection<Item>();

			string reqItemsProp = $@"
         SELECT i.Id, i.Nom AS nomItem, i.Description, i.dateSortie, i.cheminImage, t.Nom AS typeItem, i.Manufacturier, c.Nom AS 'condition'
         FROM Items i
         INNER JOIN TypesItem as t ON i.idTypeItem = t.Id
         INNER JOIN Conditions c ON i.idCondition = c.Id
         LEFT JOIN ItemProposition ip ON i.Id = ip.IdItem
         WHERE ip.IdProposition = {idProposition}
         ";

			DataSet SetItems = MaBd.Selection(reqItemsProp);
			DataTable TableItems = SetItems.Tables[0];

			foreach (DataRow RowItem in TableItems.Rows)
			{
				items.Add(new Item(RowItem));
			}
			return items;
		}

		private void ajouterItemsAProposition(int idProposition, ObservableCollection<Item> lstItems)
		{
			string reqInsererItem = $@"
			INSERT INTO ItemProposition (IdProposition, idItem)
			VALUES ";

			Item dernierListe = lstItems.Last();
			foreach (Item item in lstItems)
			{
				reqInsererItem += $"({idProposition}, {item.Id})";

				if (!item.Equals(dernierListe))
				{
					reqInsererItem += ", ";
				}
			}

			MaBd.Commande(reqInsererItem);
		}

		#endregion
	}
}

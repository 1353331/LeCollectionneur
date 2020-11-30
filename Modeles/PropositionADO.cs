using System;
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
			INSERT INTO Propositions (AnnonceLiee_Id, Proposeur_Id, Montant, DateProposition, EtatProposition_Id)
			VALUES
			({prop.AnnonceLiee.Id}, {prop.Proposeur.Id}, {prop.Montant}, '{prop.DateProposition}', (SELECT Id FROM EtatsProposition WHERE Nom = '{prop.EtatProposition.Nom}'));
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
			SELECT p.Id, p.AnnonceLiee_Id, p.Proposeur_Id, p.montant, p.DateProposition, ep.Nom AS etatProposition
			FROM Propositions p
			LEFT JOIN EtatsProposition ep
			ON p.EtatProposition_Id = ep.Id
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
			SELECT p.Id, p.AnnonceLiee_Id, p.Proposeur_Id, p.montant, p.dateProposition, ep.Nom AS etatProposition
			FROM Propositions p
			LEFT JOIN EtatsProposition ep
			ON p.EtatProposition_Id = ep.Id
			WHERE p.Id = {idProposition} 
			";

			DataSet setProposition = MaBd.Selection(requete);
			DataTable TableProposition = setProposition.Tables[0];

			Proposition propTemp = new Proposition(TableProposition.Rows[0]);
			propTemp.ItemsProposes = recupererItemsProposition(propTemp.Id);

			return propTemp;
		}

		public ObservableCollection<Proposition> RecupererPropositionsEnvoyees(int idUtilisateur)
		{
			string requete = $@"
				SELECT p.Id, p.AnnonceLiee_Id, p.Proposeur_Id, p.montant, p.dateProposition, ep.Nom AS etatProposition
				FROM Propositions p
				LEFT JOIN EtatsProposition ep
				ON p.EtatProposition_Id = ep.Id
				WHERE p.Proposeur_Id = {idUtilisateur}
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
				SELECT p.Id, p.AnnonceLiee_Id, p.Proposeur_Id, p.montant, p.dateProposition, ep.Nom AS etatProposition
				FROM Propositions p
				INNER JOIN EtatsProposition ep
				ON p.EtatProposition_Id = ep.Id
				LEFT JOIN Annonces a
				ON p.AnnonceLiee_Id = a.Id
				WHERE a.Annonceur_Id = {idUtilisateur}
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
			AnnonceLiee_Id = {prop.AnnonceLiee.Id},
			Proposeur_Id = {prop.Proposeur.Id},
			Montant = {prop.Montant},
			DateProposition = '{prop.DateProposition:yyyy-MM-dd HH:mm:ss}',
			EtatProposition_Id = (SELECT Id FROM EtatsProposition WHERE Nom = '{prop.EtatProposition.Nom}')
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
				EtatProposition_Id = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.Annulee}')
				WHERE EtatProposition_Id = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.EnAttente}')
				AND Id IN (
								SELECT Proposition_Id
								FROM propositionitems
								WHERE Item_Id IN ({idItems})
						   );
				
				";
				MaBd.Commande(requete);
			}
		}

		public void RefuserPropositionsActivesSurAnnonce(int idAnnonce)
		{
			string requete = $@"
				UPDATE Propositions
				SET
				EtatProposition_Id = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.Declassee}')
				WHERE EtatProposition_Id = (SELECT Id FROM EtatsProposition WHERE Nom = '{EtatsProposition.EnAttente}')
						AND AnnonceLiee_Id = {idAnnonce}
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
         INNER JOIN TypesItem as t ON i.Type_Id = t.Id
         INNER JOIN Conditions c ON i.Condition_Id = c.Id
         LEFT JOIN propositionitems ip ON i.Id = ip.Item_Id
         WHERE ip.Proposition_Id = {idProposition}
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
			INSERT INTO propositionitems (Proposition_Id, Item_Id)
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

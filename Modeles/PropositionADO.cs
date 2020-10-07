using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LeCollectionneur.Outils;

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
			//string requete = $@"
			//INSERT INTO Propositions (Id, IdAnnonce, IdUtilisateur, Montant, Date, IdEtatProposition)
			//VALUES
			//(NULL, {prop.AnnonceLiee.Id}, {prop.Proposeur.Id}, {prop.Montant}, '{prop.DateProposition}', SELECT Id FROM EtatsPropositions WHERE Nom = '{prop.EtatProposition}')
			//";
			//MaBd.Commande(requete);
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
				ocPropositions.Add(new Proposition(drProposition));
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

			return new Proposition(TableProposition.Rows[0]);
		}

		#endregion

		#region UPDATE

		private void Modifier(Proposition prop)
		{
			//string requete = $@"
			//UPDATE Propositions (Id, IdAnnonce, IdUtilisateur, Montant, Date, IdEtatProposition)
			//SET
			//IdAnnonce = {prop.AnnonceLiee.Id},
			//IdUtilisateur = {prop.Proposeur.Id},
			//Montant = {prop.Montant},
			//Date = '{prop.DateProposition}',
			//IdEtatProposition = SELECT Id FROM EtatsPropositions WHERE Nom = '{prop.EtatProposition}'
			//WHERE Id = {prop.Id};
			//";

			//MaBd.Commande(requete);
		}

		#endregion

		#region DELETE
	  private void Supprimer(int IdProposition)
	  {
			//string requete = $@"
			//DELETE FROM Propositions
			//WHERE Id = {IdProposition}
			//";
		}
		

		#endregion

	}
}

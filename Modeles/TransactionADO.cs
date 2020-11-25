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
	public class TransactionADO
	{
		private BdBase MaBd;

		public TransactionADO()
		{
			MaBd = new BdBase();
		}

		#region CREATE
		public void Ajouter(Transaction trx)
		{
			string requete = $@"
			INSERT INTO Transactions (PropositionTrx_Id, Date)
			VALUES
			({trx.PropositionTrx.Id}, '{trx.Date:yyyy-MM-dd HH:mm:ss}');
			";

			MaBd.Commande(requete);
		}
		#endregion


		#region READ
		public ObservableCollection<Transaction> RecupererTout()
		{
			ObservableCollection<Transaction> lstTrx = new ObservableCollection<Transaction>();

			string requete = @"
			SELECT id, PropositionTrx_Id, Date
			FROM Transactions
			ORDER BY Date DESC;
			";

			DataSet setTransaction = MaBd.Selection(requete);
			DataTable tableTransaction = setTransaction.Tables[0];

			foreach (DataRow drTrx in tableTransaction.Rows)
			{
				lstTrx.Add(new Transaction(drTrx));
			}

			return lstTrx;
		}

		public Transaction RecupereUn(int idTransaction)
		{
			string requete = $@"
			SELECT Id, PropositionTrx_Id, Date
			FROM Transactions
			WHERE Id = {idTransaction} 
			";

			DataSet setTransaction = MaBd.Selection(requete);
			DataTable tableTransaction = setTransaction.Tables[0];

			return new Transaction(tableTransaction.Rows[0]);
		}

		public ObservableCollection<Transaction> RecupererToutParUtilisateur(int idUtilisateur)
		{
			ObservableCollection<Transaction> lstTrx = new ObservableCollection<Transaction>();

			string requete = $@"
			SELECT id, PropositionTrx_Id, Date
			FROM Transactions
			WHERE PropositionTrx_Id IN (SELECT Id
												FROM Propositions
												WHERE (Proposeur_Id = {idUtilisateur} OR AnnonceLiee_Id IN (SELECT Id
																															FROM Annonces
																															WHERE Annonceur_Id = {idUtilisateur})))

			ORDER BY Date DESC;
			";

			DataSet setTransaction = MaBd.Selection(requete);
			DataTable tableTransaction = setTransaction.Tables[0];

			foreach (DataRow drTrx in tableTransaction.Rows)
			{
				lstTrx.Add(new Transaction(drTrx));
			}

			return lstTrx;
		}
		#endregion


		#region UPDATE
		public void Modifier(Transaction trx)
		{
			string requete = $@"
			UPDATE Transactions
			SET PropositionTrx_Id = {trx.PropositionTrx.Id}, Date = '{trx.Date:yyyy-MM-dd HH:mm:ss}'
			WHERE Id = {trx.Id}
			";

			MaBd.Commande(requete);
		}
		#endregion


		#region DELETE
		public void Supprimer(int idTrx)
		{
			string requete = $@"
			DELETE FROM Transactions
			WHERE Id = {idTrx}
			";

			MaBd.Commande(requete);
		}
		#endregion
	}
}


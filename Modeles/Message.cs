using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
	[Table("Messages")]
	public class Message
	{
		#region Variable
		public int Id { get; set; }
		[Column("Message")]
		public string Contenu { get; set; }
		public DateTime Date { get; set; }
		[NotMapped]
		public int idUtilisateur { get; set; }
		[NotMapped]
		public string user { get; set; }
		[Column("idUtilisateur")]
		public Utilisateur utilisateur { get; set; }
		[NotMapped]
		public bool envoyuseractif { get; set; }
		public bool item { get; set; }
		public bool image { get; set; }
		public bool emoji { get; set; }
		public bool vue { get; set; }
		#endregion

		#region Constructeur
		public Message(string Contenu, Utilisateur UtilisateurActif)
		{
			this.Contenu = Contenu;
			this.idUtilisateur = UtilisateurActif.Id;
			this.Date = DateTime.Now;
			this.user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur).NomUtilisateur;
			utilisateur = UtilisateurActif;
			this.envoyuseractif = idUtilisateur == UtilisateurADO.utilisateur.Id;
			this.item = this.image = this.emoji = false;
		}
		public Message(DataSet Data)
		{

			Date = (DateTime)Data.Tables[0].Rows[0]["Date"];
			Contenu = Data.Tables[0].Rows[0]["Message"].ToString();
			idUtilisateur = (int)Data.Tables[0].Rows[0]["IdUtilisateur"];
			user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur).NomUtilisateur;
			envoyuseractif = idUtilisateur == UtilisateurADO.utilisateur.Id;
			utilisateur = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur);
			if (Data.Tables[0].Rows[0]["item"].ToString() != "")
			{
				item = (bool)Data.Tables[0].Rows[0]["item"];
				image = (bool)Data.Tables[0].Rows[0]["image"];
				emoji = (bool)Data.Tables[0].Rows[0]["emoji"];
			}
			else
			{
				this.item = this.image = this.emoji = false;

			}
		}
		public Message(DataRow Data)
		{
			Date = (DateTime)Data["date"];
			Contenu = Data["Message"].ToString();
			idUtilisateur = (int)Data["utilisateur_id"];
			utilisateur = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur);
			this.user = utilisateur.NomUtilisateur;
			this.envoyuseractif = idUtilisateur == UtilisateurADO.utilisateur.Id;
			if (Data[4].ToString() != "")
			{

				this.item = (bool)Data["item"];
				this.image = (bool)Data["image"];
				this.emoji = (bool)Data["emoji"];
			}
			else
			{
				this.item = this.image = this.emoji = false;

			}
		}
		public Message(DataSet Data, bool Empty)
		{
			if (!Empty)
				Date = (DateTime)Data.Tables[0].Rows[0]["Date"];
			Contenu = Data.Tables[0].Rows[0]["Message"].ToString();
			idUtilisateur = (int)Data.Tables[0].Rows[0]["utilisateur_id"];
			this.user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur).NomUtilisateur;
			this.envoyuseractif = idUtilisateur == UtilisateurADO.utilisateur.Id;
			this.item = this.image = this.emoji = false;
		}
		public Message(DataRow Data, bool Empty)
		{
			if (!Empty)
				Date = (DateTime)Data[1];
			Contenu = Data[2].ToString();
			idUtilisateur = (int)Data[4];
			this.user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur).NomUtilisateur;
			this.envoyuseractif = idUtilisateur == UtilisateurADO.utilisateur.Id;
			this.item = this.image = this.emoji = false;
		}
		public Message() { }
		#endregion
		#region Method

		public int RetourIdUtilisateur()
		{
			return idUtilisateur;
		}
		#endregion
	}
}

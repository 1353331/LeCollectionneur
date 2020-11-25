using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeCollectionneur.Modeles
{
	[Table("Conversations")]
	public class Conversation
	{
		#region  Variable
		public int Id { get; set; }
		public Utilisateur UserActif;
		public Utilisateur UserAutre;

		public List<Message> ListMessage { get; set; }
		public Utilisateur Utilisateur1 { get; set; }
		public Utilisateur Utilisateur2 { get; set; }
		[NotMapped]
		public string NomUserAutre { get; set; }
		[NotMapped]
		public string lastMessage { get; set; }
		[NotMapped]
		public DateTime? date { get; set; }


		#endregion

		#region Constructeur
		//Contructeur Vide
		public Conversation() { }
		//Constructeur Avec 2 Utilisateur
		public Conversation(Utilisateur UtilisateurActif, Utilisateur UtilisateurAutre)
		{
			this.UserActif = UtilisateurActif;
			this.UserAutre = UtilisateurAutre;
			this.NomUserAutre = UserAutre.NomUtilisateur;
			this.lastMessage = getLastMessage().Contenu;
			this.date = getLastMessage().Date;
		}
		//Constructeur avec DataSet
		public Conversation(DataSet data)
		{


			this.Id = (int)data.Tables[0].Rows[0]["id"];
			if ((int)data.Tables[0].Rows[0]["Utilisateur1_Id"] == UtilisateurADO.utilisateur.Id)
			{
				this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["Utilisateur1_Id"]));
				this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["Utilisateur2_Id"]));
			}
			else
			{
				this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["Utilisateur2_Id"]));
				this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["Utilisateur1_Id"]));
			}
			this.NomUserAutre = UserAutre.NomUtilisateur;
			this.lastMessage = getLastMessage().Contenu;
			this.date = getLastMessage().Date;
		}
		//Contructeur avec DataRow
		public Conversation(DataRow data)
		{
			this.Id = (int)data["id"];
			if ((int)data["Utilisateur1_Id"] == UtilisateurADO.utilisateur.Id)
			{
				this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["Utilisateur1_Id"]));
				this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["Utilisateur2_Id"]));
			}
			else
			{
				this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["Utilisateur2_Id"]));
				this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["Utilisateur1_Id"]));
			}
			this.NomUserAutre = UserAutre.NomUtilisateur;
			this.lastMessage = getLastMessage().Contenu;
			this.date = getLastMessage().Date;
		}
		//Constructeur Avec 2 Utilisateur
		public Conversation(Utilisateur UtilisateurActif, Utilisateur UtilisateurAutre, List<Message> messages)
		{
			this.ListMessage = messages;
			this.UserActif = UtilisateurActif;
			this.UserAutre = UtilisateurAutre;
			this.NomUserAutre = UserAutre.NomUtilisateur;
			this.lastMessage = getLastMessage().Contenu;
			this.date = getLastMessage().Date;
		}
		//Constructeur avec DataSet
		public Conversation(DataSet data, List<Message> messages)
		{
			this.ListMessage = messages;
			this.Id = (int)data.Tables[0].Rows[0]["id"];
			this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["Utilisateur1_Id"]));
			this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data.Tables[0].Rows[0]["Utilisateur2_Id"]));
			this.NomUserAutre = UserAutre.NomUtilisateur;
			this.lastMessage = getLastMessage().Contenu;
			this.date = getLastMessage().Date;
		}
		//Contructeur avec DataRow
		public Conversation(DataRow data, List<Message> messages)
		{
			this.ListMessage = messages;
			this.Id = (int)data["id"];
			this.UserActif = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["IdUtilisateur_Id1"]));
			this.UserAutre = new Utilisateur(UtilisateurADO.GetUserDataSet((int)data["IdUtilisateur_Id2"]));
			this.NomUserAutre = UserAutre.NomUtilisateur;
			this.lastMessage = getLastMessage().Contenu;
			this.date = getLastMessage().Date;
		}
		#endregion

		#region Method
		public Message getLastMessage()
		{
			ListMessage = ConversationADO.messages;
			if (ListMessage == null || ListMessage.Count == 0)
				return new Message();
			else
				return ListMessage[ListMessage.Count() - 1];
		}

		private void reset()
		{


		}
		#endregion
	}
}

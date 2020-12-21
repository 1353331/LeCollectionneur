using LeCollectionneur.Outils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

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
		public static int nbrEmoji =25;
		public System.Drawing.Bitmap imgMessage;
		public static ObservableCollection<Emoji.Wpf.EmojiData.Emoji> lstEmoji = getEmojie();
		#endregion

		#region Constructeur
		public Message(string Contenu, Utilisateur UtilisateurActif, bool Send)
		{
			if (Send)
				this.Contenu = transform(Contenu);
			else
				this.Contenu = Contenu;
			this.idUtilisateur = UtilisateurActif.Id;
			this.Date = DateTime.Now;
			this.user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur).NomUtilisateur;
			utilisateur = UtilisateurActif;
			this.envoyuseractif = idUtilisateur == UtilisateurADO.utilisateur.Id;
			this.item = this.image = this.emoji = false;
			if(item)
            {
				imgMessage=	Fichier.RecupererImageServeur(Contenu);
            }
		}
		public Message(DataSet Data,bool Send)
		{

			Date = (DateTime)Data.Tables[0].Rows[0]["Date"];
			if(Send)
				Contenu = transform( Data.Tables[0].Rows[0]["Message"].ToString());
			else
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
			if (item)
			{
				imgMessage = Fichier.RecupererImageServeur(Contenu);
			}
		}
		public Message(DataRow Data,bool Send)
		{
			Date = (DateTime)Data["date"];
			if(Send)
				Contenu = transform(Data["Message"].ToString());
			else
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
			if (item)
			{
				imgMessage = Fichier.RecupererImageServeur(Contenu);
			}
		}
		public Message(DataSet Data, bool Empty, bool Send)
		{
			if (!Empty)
				Date = (DateTime)Data.Tables[0].Rows[0]["Date"];
			if(Send)
				Contenu = transform(Data.Tables[0].Rows[0]["Message"].ToString());
			else
				Contenu = Data.Tables[0].Rows[0]["Message"].ToString();
			idUtilisateur = (int)Data.Tables[0].Rows[0]["utilisateur_id"];
			this.user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur).NomUtilisateur;
			this.envoyuseractif = idUtilisateur == UtilisateurADO.utilisateur.Id;
			this.item = this.image = this.emoji = false;
			if (item)
			{
				imgMessage = Fichier.RecupererImageServeur(Contenu);
			}
		}
		public Message(DataRow Data, bool Empty, bool Send)
		{
			if (!Empty)
				Date = (DateTime)Data[1];
			if(Send)
				Contenu = transform(Data[2].ToString());
			else
				Contenu = Data[2].ToString();
			idUtilisateur = (int)Data[4];
			this.user = new UtilisateurADO().RechercherUtilisateurById(idUtilisateur).NomUtilisateur;
			this.envoyuseractif = idUtilisateur == UtilisateurADO.utilisateur.Id;
			this.item = this.image = this.emoji = false;
			if (item)
			{
				imgMessage = Fichier.RecupererImageServeur(Contenu);
			}
		}
		public Message() { }
		#endregion
		#region Method

		public int RetourIdUtilisateur()
		{
			return idUtilisateur;
		}
		private string transform(string message)
        {
		var temp = "";
		var emo = "";
		try
		{

			for (int e = 0; e < message.Length; e++)
			{
				if (message[e] == '{' && message[e + 1] == '{')
				{
					emo = lstEmoji[int.Parse(message[e + 2].ToString())*10 + int.Parse(message[e + 3].ToString())].Text;
				
					temp += emo;		
					e += 5;
				}
				else
				{
					temp += message[e];
				}
						
			}
		}
			
        catch { }

			
				return temp;
			}
		public static ObservableCollection<Emoji.Wpf.EmojiData.Emoji> getEmojie()
		{
			var temp = new ObservableCollection<Emoji.Wpf.EmojiData.Emoji>();
			var tempEmojie = Emoji.Wpf.EmojiData.AllEmoji.ToList();
			var e = 0;
			foreach (var item in tempEmojie)
			{

				temp.Add(item);
				if (e == nbrEmoji)
					break;
				e++;
			}
			return temp;
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.Outils.Interfaces;

namespace LeCollectionneur.VuesModeles
{
	public class ModalEnvoyerMessage_VM
	{

		public ICommand cmdEnvoyerMessage { get; set; }
		public ICommand cmdAnnuler { get; set; }
		public Utilisateur Destinataire { get; set; }
		private string _message;

		public string Message
		{
			get { return _message; }
			set 
			{
				_message = value; 
			}
		}


		private ConversationADO convADO = new ConversationADO();
		public ModalEnvoyerMessage_VM(Utilisateur destinataire)
		{
			Destinataire = destinataire;
			cmdEnvoyerMessage = new Commande(cmdEnvMessage);
			cmdAnnuler = new Commande(cmdAnnulerMessage);
		}

		private void cmdEnvMessage(object param)
		{
			try
			{
				string message = Message.Replace("'", @"\'");
				convADO.EnvoyerMessage(message, ConversationADO.ChercherIdConversation(UtilisateurADO.utilisateur, Destinataire));
				MessageBox.Show("Message envoyé!", "Confirmation", MessageBoxButton.OK, MessageBoxImage.Information);
			}
			catch (Exception e)
			{
				MessageBox.Show($"Erreur lors de l'envoi du message: {e.Message}", "Erreur", MessageBoxButton.OK, MessageBoxImage.Error);
				return;
			}
			
			IFenetreFermeable fenetre = param as IFenetreFermeable;
			fenetre.Fermer();
		}

		private void cmdAnnulerMessage(object param)
		{
			if (MessageBox.Show("Voulez-vous annuler votre message?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
			{
				IFenetreFermeable fenetre = param as IFenetreFermeable;
				fenetre.Fermer();
			}
		}
	}
}

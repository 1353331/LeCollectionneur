﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using LeCollectionneur.Modeles;
using LeCollectionneur.Outils.Interfaces;
using LeCollectionneur.VuesModeles;

namespace LeCollectionneur.Vues
{
	/// <summary>
	/// Logique d'interaction pour ModalEnvoyerMessage.xaml
	/// </summary>
	public partial class ModalEnvoyerMessage : Window, IFenetreFermeable
	{
		public ModalEnvoyerMessage(Utilisateur destinataire)
		{
			InitializeComponent();
			DataContext = new ModalEnvoyerMessage_VM(destinataire);
		}

		public void Fermer()
		{
			Close();
		}
	}
}

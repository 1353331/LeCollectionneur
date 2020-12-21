using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using LeCollectionneur.Modeles;

namespace LeCollectionneur
{
	/// <summary>
	/// Logique d'interaction pour App.xaml
	/// </summary>
	public partial class App : Application
	{
		
		public void ChangementTheme()
		{
			if (UtilisateurADO.utilisateur.DarkMode)
			{
				// Mettre le thème dark.
				App.Current.Resources.MergedDictionaries.Insert(1, new ResourceDictionary() { Source = new Uri("pack://application:,,,/MaterialDesignThemes.Wpf;component/Themes/MaterialDesignTheme.Dark.xaml") });
			}
			else
			{
				// Enlever le thème dark.
				App.Current.Resources.MergedDictionaries.RemoveAt(1);
			}
		}
	}
}

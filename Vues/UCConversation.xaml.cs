﻿using LeCollectionneur.Modeles;
using LeCollectionneur.Outils;
using LeCollectionneur.VuesModeles;
using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour UCConversation.xaml
    /// </summary>
    public partial class UCConversation : UserControl
    {
        public UCConversation()
        {
            InitializeComponent();
            DataContext = new Conversation_VM();
        }

        private void btnAjouterConversation_Click(object sender, RoutedEventArgs e)
        {
            
            ajouterConversation ajouterConversation = new ajouterConversation();
            ajouterConversation.ShowDialog();
            
        }

        private void btnVoir_Click(object sender, RoutedEventArgs e)
        {
            var myValue = ((Button)sender).Tag;
            ItemADO item = new ItemADO();
            Item temp=item.RecupererUn( Convert.ToInt32(myValue));
            var t = new ModalDetailsItem(temp);
            t.Show();
        }
    }
}

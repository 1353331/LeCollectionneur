﻿using LeCollectionneur.VuesModeles;
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
using System.Windows.Shapes;

namespace LeCollectionneur.Vues
{
    /// <summary>
    /// Logique d'interaction pour ModalAjoutItemAnnonce.xaml
    /// </summary>
    public partial class ModalAjoutItemAnnonce : Window
    {
        public ModalAjoutItemAnnonce()
        {
            InitializeComponent();
            DataContext = new AjoutItemAnnonce_VM();
        }
    }
}
using LeCollectionneur.Outils;
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
    /// Logique d'interaction pour modalEnvoyerImage.xaml
    /// </summary>
    public partial class modalEnvoyerImage : Window
    {
        public modalEnvoyerImage(string LienImage)
        {
            InitializeComponent();
            var bitmapImage = new BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(LienImage, UriKind.Absolute);
            bitmapImage.EndInit();
            Img.Source = bitmapImage;
        }
        private bool OkButton = false;
        public bool OKButtonClicked
    {
        get { return OkButton; }
    }
        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            OkButton = true;
            this.Close();
        }

        private void Refuse_Click(object sender, RoutedEventArgs e)
        {
            OkButton = false ;
            this.Close();
        }
    }
}

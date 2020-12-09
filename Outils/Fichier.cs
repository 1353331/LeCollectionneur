using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace LeCollectionneur.Outils
{
     static class Fichier
    {
        #region Proprietes
        private static string nomUtilisateur = "LeCollectionneur";
        private static string motDePasse = "SE7tr9sKAu";
        private static string urlImages = "ftp://420.cstj.qc.ca/%2fLeCollectionneur/%2fVersion05/%2fimages/";
        private static FtpWebRequest _requete;
        private static string nomFichier;
        private static ManualResetEvent operationComplete;
        private static Exception exceptionAttrappee;
        #endregion

        #region Fichierlocal
        public  static string ImporterFichier()
        {
            string nomFichier = "";
            Stream checkStream = null;
            
            // Fenêtre d'explorateur de fichiers.
            Microsoft.Win32.OpenFileDialog explorateurFichier = new Microsoft.Win32.OpenFileDialog();
            // Options de la fenêtre
            explorateurFichier.Multiselect = false;
            explorateurFichier.Filter = "All Image Files | *.png;*.jpg;*.bmp;*.gif";
            // Ouverture de la fenêtre.
            bool? resultat = explorateurFichier.ShowDialog();
            if (resultat.GetValueOrDefault()) // Default = false, alors si resultat est null, ce sera tout de même false.
            {
                try
                {
                    if ((checkStream=explorateurFichier.OpenFile())!=null)
                    {
                        nomFichier = explorateurFichier.FileName;
                        
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return nomFichier;
        }
        public static string ImporterFichierJPGPNGConversation()
        {
            string nomFichier = "";
            Stream checkStream = null;

            // Fenêtre d'explorateur de fichiers.
            Microsoft.Win32.OpenFileDialog explorateurFichier = new Microsoft.Win32.OpenFileDialog();
            // Options de la fenêtre
            explorateurFichier.Multiselect = false;
            explorateurFichier.Filter = "All Image Files | *.png;*.jpg";
            // Ouverture de la fenêtre.
            bool? resultat = explorateurFichier.ShowDialog();
            if (resultat.GetValueOrDefault()) // Default = false, alors si resultat est null, ce sera tout de même false.
            {
                try
                {
                    if ((checkStream = explorateurFichier.OpenFile()) != null)
                    {
                        FileInfo info = new FileInfo(explorateurFichier.FileName);
                        if (info.Length <= 5242880)
                            nomFichier = explorateurFichier.FileName;
                        else
                            MessageBox.Show("Image Trop Volumineuse");
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            
            return nomFichier;
        }
        #endregion

        #region Ftp
        // Source : https://docs.microsoft.com/en-us/dotnet/api/system.net.ftpwebrequest?view=netcore-3.1
        public static void TeleverserFichierFTP(int idItem, string cheminPhysique)
        {
            // Thread telechargement = new Thread(() => {
            string cheminFtp = urlImages + $"item{idItem}.jpg";
                ManualResetEvent objetAttendu;
                exceptionAttrappee = null;
                operationComplete = new ManualResetEvent(false);
                Object Statut = new object();
                nomFichier = cheminPhysique;
                FtpWebRequest requete = (FtpWebRequest)WebRequest.Create(cheminFtp);
                requete.Method = WebRequestMethods.Ftp.UploadFile;
                requete.Credentials = new NetworkCredential(nomUtilisateur, motDePasse);
                _requete = requete;
                //_requete.EnableSsl = true;
                _requete.BeginGetRequestStream(new AsyncCallback(EndGetRequestCallBack),Statut);
                objetAttendu = operationComplete;
                objetAttendu.WaitOne();
                if (!(exceptionAttrappee is null))
                    MessageBox.Show("Téléchargement de l'image raté.");
                

            //});
            //    telechargement.Start();
            //if (true)
            //    telechargement.Abort();
            
        }
        public static string TeleverserFichierFTPConversation(string cheminPhysique)
        {
            // Thread telechargement = new Thread(() => {
            string cheminFtp = urlImages + $"Conversation{DateTime.Now.Year}{DateTime.Now.Month}{DateTime.Now.Day}{DateTime.Now.Hour}{DateTime.Now.Minute}{DateTime.Now.Second}{DateTime.Now.Millisecond}.jpg";
            ManualResetEvent objetAttendu;
            exceptionAttrappee = null;
            operationComplete = new ManualResetEvent(false);
            Object Statut = new object();
            nomFichier = cheminPhysique;
            FtpWebRequest requete = (FtpWebRequest)WebRequest.Create(cheminFtp);
            requete.Method = WebRequestMethods.Ftp.UploadFile;
            requete.Credentials = new NetworkCredential(nomUtilisateur, motDePasse);
            _requete = requete;
            //_requete.EnableSsl = true;
            _requete.BeginGetRequestStream(new AsyncCallback(EndGetRequestCallBack), Statut);
            objetAttendu = operationComplete;
            objetAttendu.WaitOne();
            if (!(exceptionAttrappee is null))
            {
                MessageBox.Show("Téléchargement de l'image raté.");
                return "";
            }

            return cheminFtp;
            //});
            //    telechargement.Start();
            //if (true)
            //    telechargement.Abort();

        }
        private static void EndGetRequestCallBack(IAsyncResult asyncResult)
        {
            Object Statut = asyncResult.AsyncState;
            Stream fluxRequete = null;
            // End the asynchronous call to get the request stream.
            try
            {
                fluxRequete = _requete.EndGetRequestStream(asyncResult);
                // Copy the file contents to the request stream.
                const int bufferLength = 2048;
                byte[] buffer = new byte[bufferLength];
                int count = 0;
                int readBytes = 0;
                FileStream stream = File.OpenRead(nomFichier);
                do
                {
                    readBytes = stream.Read(buffer, 0, bufferLength);
                    fluxRequete.Write(buffer, 0, readBytes);
                    count += readBytes;
                }
                while (readBytes != 0);
                
                // IMPORTANT: Close the request stream before sending the request.
                fluxRequete.Close();
                // Asynchronously get the response to the upload request.
                _requete.BeginGetResponse( new AsyncCallback(EndGetResponseCallback),Statut);
            }
            // Return exceptions to the main application thread.
            catch (Exception e)
            {

                exceptionAttrappee = e;
                return;
            }
        }
        private static void EndGetResponseCallback(IAsyncResult asyncResult)
        {
            Object Statut = asyncResult.AsyncState;
            string DescriptionStatut;
            FtpWebResponse reponse = null;
            try
            {
                reponse = (FtpWebResponse)_requete.EndGetResponse(asyncResult);
                reponse.Close();
                DescriptionStatut = reponse.StatusDescription;
                // Signal the main application thread that
                // the operation is complete.
                operationComplete.Set();
            }
            // Return exceptions to the main application thread.
            catch (Exception e)
            {
                exceptionAttrappee = e;
                MessageBox.Show(e.Message);
            }
        }

        public static Bitmap RecupererImageServeur(string fichierImage)
        {
            Bitmap imageRetour= null;
            string cheminComplet = urlImages + fichierImage;
            Uri serveurUri =new Uri(cheminComplet);

            if (serveurUri.Scheme != Uri.UriSchemeFtp)
                return imageRetour;
            try
            {
                byte[] octetsImage = GetOctetsImage(cheminComplet);
                // Ici,on a les bytes de l'image
                MemoryStream mStream = new MemoryStream();
                mStream.Write(octetsImage, 0, Convert.ToInt32(octetsImage.Length));
                imageRetour = new Bitmap(mStream,false);
            }
            catch (Exception e)
            {
                // Remettre l'image à null, 
            }

            return imageRetour;
        }

        public static BitmapImage TransformerBitmapEnBitmapImage(Bitmap bitmap)
        {
            if (!(bitmap is null))
            {

            
                using (var memory = new MemoryStream())
                {
                    bitmap.Save(memory, ImageFormat.Jpeg);
                    memory.Position = 0;

                    var bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.StreamSource = memory;
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();

                    return bitmapImage;
                }
            }
            return null;
        }
        private static byte[] GetOctetsImage(string cheminComplet)
        {
            WebClient ClientFtp = new WebClient();
            ClientFtp.Credentials = new NetworkCredential(nomUtilisateur, motDePasse);
            byte[] octetsImage = ClientFtp.DownloadData(cheminComplet);
            return octetsImage;
        }
        #endregion
    }
}

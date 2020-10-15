using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace LeCollectionneur.Outils
{
     static class Fichier
    {
        private static string nomUtilisateur = "LeCollectionneur";
        private static string motDePasse = "SE7tr9sKAu";
        private static string urlImages = "ftp://420.cstj.qc.ca/%2fLeCollectionneur/%2fimages/";
        private static FtpWebRequest _requete;
        private static string nomFichier;
        private static ManualResetEvent operationComplete;
        private static Exception exceptionAttrappee;
       public  static string ImporterFichier()
        {
            string nomFichier = "";
            Stream checkStream = null;
            
            // Fenêtre d'explorateur de fichiers.
            Microsoft.Win32.OpenFileDialog explorateurFichier = new Microsoft.Win32.OpenFileDialog();
            // Options de la fenêtre
            explorateurFichier.Multiselect = false;
            explorateurFichier.Filter = "All Image Files | *.*";
            // Ouverture de la fenêtre.
            bool? resultat = explorateurFichier.ShowDialog();
            if (resultat.GetValueOrDefault()) // Default = false, alors si resultat est null, ce sera tout de même false.
            {
                try
                {
                    if ((checkStream=explorateurFichier.OpenFile())!=null)
                    {
                        nomFichier = explorateurFichier.FileName;
                        MessageBox.Show($"Importation de {nomFichier} réussie");
                    }
                }
                catch(Exception e)
                {
                    MessageBox.Show(e.Message);
                }
            }
            return nomFichier;
        }

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
        #endregion
    }
}

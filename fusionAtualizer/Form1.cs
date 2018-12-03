using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Windows.Forms;
using System.IO.Compression;

namespace fusionAtualizer
{
    public partial class frmPrincipal : Form
    {
        public frmPrincipal()
        {
            InitializeComponent();
        }
        private static Boolean OutDatedFusion()
        {
            try { 
                string address = "https://dmiautomacao.000webhostapp.com/Fusion.zip";
                HttpWebRequest fusionfile = (HttpWebRequest)WebRequest.Create(address);
                HttpWebResponse fusionFileResponse = (HttpWebResponse)fusionfile.GetResponse();
                fusionFileResponse.Close();
                string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Fusion.zip";
                DateTime localFileModifiedTime = File.GetLastWriteTime(path);
                DateTime onlineFileModifiedTime = fusionFileResponse.LastModified;
                return localFileModifiedTime >= onlineFileModifiedTime ? false : true;
            }
            catch (WebException e)
            {
                MessageBox.Show("Erro Ocorrido: " + e.Message + "\nStatus: " + e.Status);
                return false;
            }

        }

        private static Boolean OutDatedPdv()
        {
            try
            {
                string address = "https://dmiautomacao.000webhostapp.com/Pdv.zip";
                System.Net.HttpWebRequest pdvfile = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(address);
                System.Net.HttpWebResponse pdvFileResponse = (System.Net.HttpWebResponse)pdvfile.GetResponse();
                pdvFileResponse.Close();

                string path = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Pdv.zip";
                DateTime localFileModifiedTime = File.GetLastWriteTime(path);
                DateTime onlineFileModifiedTime = pdvFileResponse.LastModified;
                return localFileModifiedTime >= onlineFileModifiedTime ? false : true;
            }
            catch (WebException e)
            {
                MessageBox.Show("Erro Ocorrido:\n " + e.Message + "\nStatus: " + e.Status);
                return false;
            }
        }

        private void FusionDownload()
        {
            string address = "https://dmiautomacao.000webhostapp.com/Fusion.zip";
            System.Net.WebClient client = new System.Net.WebClient();
            Uri uri = new Uri(address);
            client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
            client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadFileFusionZip);
            // Specify a progress notification handler.
            client.DownloadProgressChanged += new System.Net.DownloadProgressChangedEventHandler(DownloadProgressFusionZip);
            try { 
                client.DownloadFileAsync(uri, System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Fusion.zip");
                while (client.IsBusy) { Application.DoEvents(); }
                client.Dispose();
            }
            catch (WebException e)
            {
                MessageBox.Show("Erro Ocorrido: " + e.Message + "\nStatus: " + e.Status);
            }
        }

        private void DownloadProgressFusionZip(object sender, DownloadProgressChangedEventArgs e)
        {
            pb.Value = e.ProgressPercentage;
            lblRetaguarda.Text = "Status: Atualizando Fusion " + e.ProgressPercentage.ToString() + "%";
        }

        private void DownloadFileFusionZip(object sender, AsyncCompletedEventArgs e)
        {
            UnzipFileFusion();
        }

        private void PdvDownload()
        {
            string address = "https://dmiautomacao.000webhostapp.com/Pdv.zip";
            WebClient client = new WebClient();
            Uri uri = new Uri(address);
            client.Headers.Add("User-Agent", "Mozilla/4.0 (compatible; MSIE 8.0)");
            client.DownloadFileCompleted += new System.ComponentModel.AsyncCompletedEventHandler(DownloadFilePdv);
            client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(DownloadProgressPdv);
            try { 
                client.DownloadFileAsync(uri, System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Pdv.zip");
                while (client.IsBusy) { Application.DoEvents(); }
                client.Dispose();
            }
            catch (WebException e)
            {
                MessageBox.Show("Erro Ocorrido:\n " + e.Message + "\nStatus: " + e.Status);
            }
        }

        private void DownloadProgressPdv(object sender, DownloadProgressChangedEventArgs e)
        {
            pb.Value = e.ProgressPercentage;
            lblPdv.Text = "Status: Atualizando PDV " + e.ProgressPercentage.ToString() + "%";
        }

        private void DownloadFilePdv(object sender, AsyncCompletedEventArgs e)
        {
            UnzipFilePdv();
        }

        private void UnzipFileFusion()
        {
            string zipPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Fusion.zip";
            string extractPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();

            // Normalizes the path.
            extractPath = Path.GetFullPath(extractPath);

            if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                extractPath += Path.DirectorySeparatorChar;
            }

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        // are case-insensitive.
                        if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                            entry.ExtractToFile(destinationPath);
                    }
                }
            }
        }

        private void UnzipFilePdv()
        {
            string zipPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString() + "Pdv.zip";
            string extractPath = System.AppDomain.CurrentDomain.BaseDirectory.ToString();

            // Normalizes the path.
            extractPath = Path.GetFullPath(extractPath);

            if (!extractPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                extractPath += Path.DirectorySeparatorChar;
            }

            using (ZipArchive archive = ZipFile.OpenRead(zipPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    if (entry.FullName.EndsWith(".exe", StringComparison.OrdinalIgnoreCase))
                    {
                        // Gets the full path to ensure that relative segments are removed.
                        string destinationPath = Path.GetFullPath(Path.Combine(extractPath, entry.FullName));

                        // Ordinal match is safest, case-sensitive volumes can be mounted within volumes that
                        // are case-insensitive.
                        if (destinationPath.StartsWith(extractPath, StringComparison.Ordinal))
                            entry.ExtractToFile(destinationPath);
                    }
                }
            }
        }

        private void frmPrincipal_Load(object sender, EventArgs e)
        {
            lblRetaguarda.Parent = pictureBox1;
            lblStatus.Parent = pictureBox1;
            lblPdv.Parent = pictureBox1;
            label1.Parent = pictureBox1;

            lblRetaguarda.BackColor = Color.Transparent;
            lblStatus.BackColor = Color.Transparent;
            lblPdv.BackColor = Color.Transparent;
            label1.BackColor = Color.Transparent;

            lblPdv.Text = OutDatedPdv() ? "Pdv: Desatualizado" : "Pdv: Atualizado";

            lblRetaguarda.Text = OutDatedFusion() ? "Retaguarda: Desatualizado" : "Retaguarda: Atualizado";
        }

        private void btnAtualizar_Click(object sender, EventArgs e)
        {
            string statusPdv = lblPdv.Text;
            string statusFusion = lblRetaguarda.Text;
            if (statusPdv.Substring(5).Equals("Desatualizado"))
            {
                PdvDownload();
            }

            if (statusFusion.Substring(12).Equals("Desatualizado"))
            {
                FusionDownload();
            }
        }
    }
}

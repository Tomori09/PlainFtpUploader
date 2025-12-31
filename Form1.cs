using System;
using System.IO;
using System.Net;
using System.Windows.Forms;

namespace FtpUploader
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        // Browse local file
        private void btnBrowse_Click(object sender, EventArgs e)
        {
            if (openFileDialog1.ShowDialog() == DialogResult.OK)
            {
                txtLocalPath.Text = openFileDialog1.FileName;
            }
        }

        // UPLOAD to FileZilla Server
        private void btnUpload_Click(object sender, EventArgs e)
        {
            try
            {
                string ftpUrl = txtFtpUrl.Text.Trim();  // Must include filename
                string localFile = txtLocalPath.Text;

                if (string.IsNullOrEmpty(ftpUrl) || string.IsNullOrEmpty(localFile))
                {
                    MessageBox.Show("Please provide FTP URL and local file path.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(ftpUrl);
                request.Method = WebRequestMethods.Ftp.UploadFile;
                request.Credentials = new NetworkCredential(txtUsername.Text, txtPassword.Text);

                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                byte[] fileBytes = File.ReadAllBytes(localFile);
                request.ContentLength = fileBytes.Length;

                using (Stream requestStream = request.GetRequestStream())
                {
                    requestStream.Write(fileBytes, 0, fileBytes.Length);
                }

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                {
                    MessageBox.Show("Upload complete!\n" + response.StatusDescription, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (WebException ex)
            {
                MessageBox.Show("FTP Upload Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // DOWNLOAD from FileZilla Server
        private void btnDownload_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtFtpUrl.Text))
                {
                    MessageBox.Show("Please provide FTP URL including filename.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                if (saveFileDialog1.ShowDialog() != DialogResult.OK)
                    return;

                FtpWebRequest request = (FtpWebRequest)WebRequest.Create(txtFtpUrl.Text.Trim());
                request.Method = WebRequestMethods.Ftp.DownloadFile;
                request.Credentials = new NetworkCredential(txtUsername.Text, txtPassword.Text);

                request.UsePassive = true;
                request.UseBinary = true;
                request.KeepAlive = false;

                using (FtpWebResponse response = (FtpWebResponse)request.GetResponse())
                using (Stream ftpStream = response.GetResponseStream())
                using (FileStream fileStream = new FileStream(saveFileDialog1.FileName, FileMode.Create))
                {
                    ftpStream.CopyTo(fileStream);
                }

                MessageBox.Show("Download complete!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (WebException ex)
            {
                MessageBox.Show("FTP Download Error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Unexpected error:\n" + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}

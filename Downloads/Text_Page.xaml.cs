using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using Windows.Storage;

namespace Downloads
{
    public partial class Text_Activity : PhoneApplicationPage
    {
        String[] url = new String[5];
        int x;
        BackgroundWorker bw = new BackgroundWorker();
        public Text_Activity()
        {
            InitializeComponent();
            bw.WorkerReportsProgress = true;
            bw.WorkerSupportsCancellation = true;
            bw.DoWork += new DoWorkEventHandler(bw_DoWork);
            bw.ProgressChanged += new ProgressChangedEventHandler(bw_ProgressChanged);
            bw.RunWorkerCompleted += new RunWorkerCompletedEventHandler(bw_RunWorkerCompleted);
        }

        private void go_back(object sender, RoutedEventArgs e)
        {
            if (bw.WorkerSupportsCancellation == true)
            {
                bw.CancelAsync();
            }

            NavigationService.GoBack();
        }

        private void start(object sender, RoutedEventArgs e)
        {
            if (bw.IsBusy != true)
            {
                bw.RunWorkerAsync();
            }
            url[0] = url1.Text;
            url[1] = url2.Text;
            url[2] = url3.Text;
            url[3] = url4.Text;
            url[4] = url5.Text;
        }

        private void bw_DoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            for (int y = 0; y < 5; y++)
            {
                var client = new WebClient();
                client.OpenReadCompleted += client_OpenReadCompleted;
                client.OpenReadAsync(new Uri(url[y]));
            }
            for (int z = 0; z < 5; z++)
            {
                for (int i = 1; i <= 10; i++)
                {
                    if ((worker.CancellationPending == true))
                    {
                        e.Cancel = true;
                        break;
                    }
                    else
                    {
                        // Perform a time consuming operation and report progress.
                        System.Threading.Thread.Sleep(500);
                        worker.ReportProgress(i * 10);
                    }
                }
            }
        }
        async void client_OpenReadCompleted(object sender, OpenReadCompletedEventArgs e)
        {
            byte[] buffer = new byte[e.Result.Length];
            await e.Result.ReadAsync(buffer, 0, buffer.Length);
            x++;
            using (IsolatedStorageFile storageFile = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream stream = storageFile.OpenFile("file" + x + ".doc", FileMode.Create))
                {
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                }
            }
            StorageFolder local = Windows.Storage.ApplicationData.Current.LocalFolder;
            StorageFile txtfile = await local.GetFileAsync("file" + x + ".doc");

            // Launch the pdf file.
            //await Windows.System.Launcher.LaunchFileAsync(txtfile);
        }
        private void bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Cancelled == true)
            {
                this.tbProgress.Text = "Canceled!";
            }

            else if (!(e.Error == null))
            {
                this.tbProgress.Text = ("Error: " + e.Error.Message);
            }

            else
            {
                this.tbProgress.Text = "Done!";
                MessageBox.Show("Done!");
            }

        }
        private void bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            this.tbProgress.Text = (e.ProgressPercentage.ToString() + "%");
        }


    }

}
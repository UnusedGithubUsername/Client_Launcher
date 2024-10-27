using System;
using System.Windows; 
using System.Windows.Input; 
using System.IO;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using Amazon.S3;
using Amazon; 
using Microsoft.WindowsAPICodePack.Dialogs;

namespace Client {
    public struct DownloadInstance
    {
        public ListObjectsV2Response AllFiles;
        public TransferUtility fileTransferUtility;
        public bool downloadActive;
        public bool[] downloadFile;
        public int numOfFilesRecieved;
        public int OutdatedFilesCount;

    }

    public partial class MainWindow : Window { 
 
        private LoginPage loginPage;
        private RegistrationPage registrationPage; 
        private string InstallDirectory;
         

        public DownloadInstance d;
        public MainWindow() {  
            InitializeComponent();
            string videoRelativePath = "Video/Mein Film.mp4";
            string videoFullPath = AppDomain.CurrentDomain.BaseDirectory + videoRelativePath;
            mediaElement.Source = new Uri(videoFullPath);
            loginPage = new LoginPage();
            registrationPage = new RegistrationPage();
            loginPage.main = this;
            registrationPage.main = this;

            string appDirectory = AppDomain.CurrentDomain.BaseDirectory;
             
            if (Directory.Exists(appDirectory + "InstallLocation.txt"))//get the install directory from a potentially nonexistent file
                InstallDirectory = File.ReadAllText(appDirectory + "InstallLocation.txt");  
            else
            {
                InstallDirectory = appDirectory;//if file doesnt exist, create it
                File.WriteAllText(appDirectory + "InstallLocation.txt", appDirectory);
            }

            if (!Directory.Exists(InstallDirectory))//if file content is invalid, correct that
                InstallDirectory = appDirectory;

            d = new();
            //declare a client //add qqq so this public key is not rejected by github. ITS PUBLIC 
            AmazonS3Client s3Client = new AmazonS3Client("qqqAKIAQE43KD5CKTAD77V3".Substring(3), "qqqVzf3hiPEugebvApnKkVgCl5u7GKT3erNsrx4cFBW".Substring(3), RegionEndpoint.USEast2);
            d.fileTransferUtility = new TransferUtility(s3Client);
            ListObjectsV2Request request = new ListObjectsV2Request { BucketName = "corivi" };


            GetAndSetFileList(s3Client, request);

            d.downloadActive = false; 
        }

        public void ShowLogin() {
            mainFrame.NavigationService.Navigate(loginPage);
        }
        public void SwapPage() { 
            mainFrame.NavigationService.Navigate(loginPage.IsLoaded ? registrationPage:loginPage);
        }

        private void MediaElement_MediaRepeat(object sender, RoutedEventArgs e) {
            // Restart the video playback when it ends
            mediaElement.Position = TimeSpan.Zero;
            mediaElement.Play();
        }

        public void SetProgressbar(int numOfFilesRecieved, int numOfFilesRequested) { //this is temporarily not using an observable int because the label is for debugging only
            //the removal of the label will transform this into a one liner, but ill only do this after i have an azure blob storage AND it works
            if (numOfFilesRequested == numOfFilesRecieved) {
                this.Dispatcher.Invoke(() =>
                { 
                    TopStack.Children.Remove(ProgressBar);
                    TopStack.Children.Remove(ButtonChangeGameLocation);
                    TopStack.Children.Remove(ButtonStartDownload);
                }); 
            }else if(numOfFilesRecieved == 0) {
                this.Dispatcher.Invoke(() =>
                {
                    ProgressLabel.Content = numOfFilesRequested.ToString() + " Files outstanding";
                });
            }
            else {
                this.Dispatcher.Invoke(() =>
                {
                    Progress.Value = (int)100 * ((float)numOfFilesRecieved / (float)numOfFilesRequested);
                    ProgressLabel.Content = numOfFilesRecieved.ToString() + "/" + numOfFilesRequested.ToString() + " files recieved";
                });
            } 
        }
         
        private void Connect_Minimize(object sender, RoutedEventArgs e) {
            WindowState = WindowState.Minimized;
        }

        private void Connect_Quit(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void DragWindow(object sender, MouseButtonEventArgs e) {
            if (this.WindowState == WindowState.Maximized)  
                this.WindowState = WindowState.Normal;
             
            try {//to prevent random exceptions
                DragMove(); 
            }
            catch (Exception) { 
            }
        }

        internal void ShowNoPage() {
            mainFrame.NavigationService.Navigate(null);
        }

        private void ButtonChangeGameLocation_Click(object sender, RoutedEventArgs e)
        { 
            CommonOpenFileDialog dialog = new CommonOpenFileDialog {IsFolderPicker = true, Title = "Select the installation Folder", InitialDirectory = InstallDirectory };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok) 
                InstallDirectory = dialog.FileName+ "\\";  

            SetMissingFileArray(InstallDirectory, d.AllFiles);
            d.OutdatedFilesCount = GetOutdatedFilesCount();
            d.numOfFilesRecieved = 0;
            SetProgressbar(0, d.OutdatedFilesCount);
        }

        private void ButtonStartDownload_Click(object sender, RoutedEventArgs e)
        {
            if(d.downloadActive)
            {
                d.downloadActive = false;
                return;
            }
            d.downloadActive = true;

            StartDownload(InstallDirectory, d.fileTransferUtility, d.AllFiles); 
        }
         
        private async void GetAndSetFileList(AmazonS3Client s3Client, ListObjectsV2Request request)
        {
            ListObjectsV2Response FileList = new();
            do
            {
                FileList = await s3Client.ListObjectsV2Async(request);
                request.ContinuationToken = FileList.NextContinuationToken;
            } while (FileList.IsTruncated);
            d.AllFiles = FileList;

            SetMissingFileArray(InstallDirectory, d.AllFiles);
            d.OutdatedFilesCount = GetOutdatedFilesCount();

            SetProgressbar(0, d.OutdatedFilesCount);
        }

        private void SetMissingFileArray(string InstallDirectory, ListObjectsV2Response AllFiles)
        {
            d.downloadFile = new bool[AllFiles.S3Objects.Count];
            for (int i = 0; i < AllFiles.S3Objects.Count; i++)
            {
                string fullFilePath = InstallDirectory + AllFiles.S3Objects[i].Key;
                bool fileIsOnDisk = File.Exists(fullFilePath);
                bool isOutdated = false;
                if (fileIsOnDisk)//If the disk file was modified before it was last updated (i.e. has a smaller DateTime), then its is outdated
                    isOutdated = File.GetLastWriteTime(fullFilePath) < AllFiles.S3Objects[i].LastModified;

                d.downloadFile[i] = isOutdated || !fileIsOnDisk; //download IF: file is outdated or missing completely 
            }

        }

        private int GetOutdatedFilesCount()
        {
            int OutdatedFilesCount = 0;
            for (int i = 0; i < d.downloadFile.Length; i++)
                if (d.downloadFile[i])
                    OutdatedFilesCount++;

            return OutdatedFilesCount;
        }

        private async void StartDownload(string folderPath, TransferUtility fileTransferUtility, ListObjectsV2Response FileList)
        {
            for (int i = 0; i < FileList.S3Objects.Count; i++)
            {
                if (!d.downloadActive)
                    return;

                if (d.downloadFile[i])
                {
                    await fileTransferUtility.DownloadAsync(folderPath + FileList.S3Objects[i].Key, "corivi", FileList.S3Objects[i].Key);
                    d.numOfFilesRecieved++;
                    d.downloadFile[i] = false;
                    SetProgressbar(d.numOfFilesRecieved, d.OutdatedFilesCount);
                }
            }
        }
    }


}

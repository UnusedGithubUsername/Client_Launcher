using System;
using System.Windows; 
using System.Windows.Input;
using System.Windows.Navigation;

namespace Client {
    public partial class MainWindow : Window { 
 
        private LoginPage loginPage;
        private RegistrationPage registrationPage;

        public MainWindow() {  
            InitializeComponent();
            string videoRelativePath = "Video/Mein Film.mp4";
            string videoFullPath = AppDomain.CurrentDomain.BaseDirectory + videoRelativePath;
            mediaElement.Source = new Uri(videoFullPath);
            loginPage = new LoginPage();
            registrationPage = new RegistrationPage();
            loginPage.main = this;
            registrationPage.main = this;
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
    }


}

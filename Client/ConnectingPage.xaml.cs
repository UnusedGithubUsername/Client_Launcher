using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Client {
    /// <summary>
    /// Interaction logic for ConnectingPage.xaml
    /// </summary>
    public partial class ConnectingPage : Page {

        public ConnectingPage() {
            InitializeComponent();
            SetVideoSource();

        }

        private void Connect_Click(object sender, RoutedEventArgs e) {
            string ipAdress = TextfieldIP.Text;
            MainWindow.Instance.Connect_Click(ipAdress);
        }
        private void Connect_Minimize(object sender, RoutedEventArgs e) {
            MainWindow.Instance.WindowState = WindowState.Minimized; 
        }
        private void Connect_Quit(object sender, RoutedEventArgs e) {
            System.Windows.Application.Current.Shutdown();
        }

        private void MediaElement_MediaRepeat(object sender, RoutedEventArgs e) {
            // Restart the video playback when it ends
            mediaElement.Position = TimeSpan.Zero;
            mediaElement.Play();
        }

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            MainWindow.Instance.DragMove();
        }

        private void SetVideoSource() {
            // Construct a pack URI for the video file
            string videoRelativePath = "Video/Mein Film.mp4";
            string videoFullPath = AppDomain.CurrentDomain.BaseDirectory + videoRelativePath;

            mediaElement.Source = new Uri(videoFullPath);
        }

    }
}

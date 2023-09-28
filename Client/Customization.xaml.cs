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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client {
    /// <summary>
    /// Interaction logic for Customization.xaml
    /// </summary>
    public partial class Customization : Page {
        public Customization() {
            InitializeComponent();
        }

        private void Click(bool left, int index) {

        }

        private void Click_L4(object sender, RoutedEventArgs e) {
            Click(true, 4);
        }
        private void Click_L3(object sender, RoutedEventArgs e) {
            Click(true, 3);
        }
        private void Click_L2(object sender, RoutedEventArgs e) {
            Click(true, 2);
        }
        private void Click_L1(object sender, RoutedEventArgs e) {
            Click(true, 1);
        }
        private void Click_R1(object sender, RoutedEventArgs e) {
            Click(false, 1);
        }
        private void Click_R2(object sender, RoutedEventArgs e) {
            Click(false, 2);
        }
        private void Click_R3(object sender, RoutedEventArgs e) {
            Click(false, 3);
        }
        private void Click_R4(object sender, RoutedEventArgs e) {
            Click(false, 4);
        }


        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            MainWindow.Instance.DragMove();
        }

        private void Connect_Minimize(object sender, RoutedEventArgs e) {
            MainWindow.Instance.WindowState = WindowState.Minimized;
        }

        private void Connect_Quit(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }

        private void ReqFileButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
    }


}

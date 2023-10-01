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
    enum Skill {
        s1,
        s2,
        s3
    }

    public partial class Customization : Page {
        public static Customization Instance;
        private int[] initialStats = new int[4];
        private int[] stats = new int[4];
        private int[] skills = new int[10];
        

        public Customization() {
            InitializeComponent();
            Instance = this;
            SetCharacterStats(new byte[14*4]);
        }

        public void SetCharacterStats(byte[] characterData) {
            Buffer.BlockCopy(characterData, 0, stats, 0, initialStats.Length * 4);
            Buffer.BlockCopy(characterData, 0, initialStats, 0, stats.Length * 4);
            Buffer.BlockCopy(characterData, stats.Length * 4, skills, 0, skills.Length * 4);

            this.Dispatcher.Invoke(() =>
            {
                Mid1.Content = " " + stats[0].ToString();
                Mid2.Content = " " + stats[1].ToString();
                Mid3.Content = " " + stats[2].ToString();
                Mid4.Content = " " + stats[3].ToString();
            });

        }

        private void Click_Save(object sender, RoutedEventArgs e) {
            MainWindow.Instance.SaveUserData(stats, skills);
        }

        private void Click(bool left, int index) {

            Label l;
            switch (index) {
                case (0):
                    l = Mid1;
                    break;
                case (1):
                    l = Mid2;
                    break;
                case (2):
                    l = Mid3;
                    break;
                case (3):
                    l = Mid4;
                    break; 
                default:
                    l = Mid1;
                    break;
            }

            stats[index] += left ? -1 : 1;
            stats[index] = Math.Max(0, stats[index]);//cap value at 0

            this.Dispatcher.Invoke(() =>
            {
                l.Content = " " + stats[index];   
            });
        }

        private void Click_L4(object sender, RoutedEventArgs e) {
            Click(true, 3);
        }
        private void Click_L3(object sender, RoutedEventArgs e) {
            Click(true, 2);
        }
        private void Click_L2(object sender, RoutedEventArgs e) {
            Click(true, 1);
        }
        private void Click_L1(object sender, RoutedEventArgs e) {
            Click(true, 0);
        }
        private void Click_R1(object sender, RoutedEventArgs e) {
            Click(false, 0);
        }
        private void Click_R2(object sender, RoutedEventArgs e) {
            Click(false, 1);
        }
        private void Click_R3(object sender, RoutedEventArgs e) {
            Click(false, 2);
        }
        private void Click_R4(object sender, RoutedEventArgs e) {
            Click(false, 3);
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

using System; 
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Input; 

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
        public int guid = 0;
        public int loadedCharacterIndex = 0;
        private int[] initialStats = new int[4];
        private int[] stats = new int[4];
        private int[] skills = new int[10];
         
        public Customization() {
            InitializeComponent();
            Instance = this;
            SetCharacterStats(new byte[14*4], -1);
        }

        public void SetGuid(int Guid) {
            guid = Guid;
            this.Dispatcher.Invoke(() =>
            {
                GuidTextfield.Content = Guid.ToString();
            });
        }

        public void SetCharacterStats(byte[] characterData, int charIndex) {
            loadedCharacterIndex = charIndex;

            Buffer.BlockCopy(characterData, 0, stats, 0, initialStats.Length * 4);
            Buffer.BlockCopy(characterData, 0, initialStats, 0, stats.Length * 4);
            Buffer.BlockCopy(characterData, stats.Length * 4, skills, 0, skills.Length * 4);

            this.Dispatcher.Invoke(() =>
            {
                Mid1.Content = stats[0]> 9 ? " " + stats[0].ToString(): "  " + stats[0].ToString();
                Mid2.Content = stats[1]> 9 ? " " + stats[1].ToString(): "  " + stats[1].ToString();
                Mid3.Content = stats[2]> 9 ? " " + stats[2].ToString(): "  " + stats[2].ToString();
                Mid4.Content = stats[3]> 9 ? " " + stats[3].ToString(): "  " + stats[3].ToString();
            }); 
        }

        private void Click_Save(object sender, RoutedEventArgs e) {
            if (loadedCharacterIndex == -1) {
                return;
            }
            MainWindow.Instance.SaveUserData(guid, loadedCharacterIndex, stats, skills);
        }

        private void Click_Req(object sender, RoutedEventArgs e) {
            MainWindow.Instance.ReqCharData(guid, 1);
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
                l.Content = stats[index] > 9 ? " " + stats[index].ToString() : "  " + stats[index].ToString();
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

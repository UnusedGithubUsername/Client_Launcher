using System; 
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Input;
using System.Windows.Media;
namespace Client {
    /// <summary>
    /// Interaction logic for Customization.xaml
    /// </summary>
    public enum Skill {
        MassSpells = 0,  // Skill that enables AoE on Spells
        ArcaneKnowledge = 1, //learn level 3 spells
        PrimalAttunement = 2, //Choose a second attuned magic school, your first attuned school decreases Spellslotcost by 2;
        NeuralFastpass = 3, //Consecutive uses of Skills and Spells reduces the casting cost for further uses by 20%, One Skill and one spell can have up to 3 Stacks at the same time

    }

    public partial class Customization : Page {
        public static Customization Instance;
        public int guid = 0;
        public int loadedCharacterIndex = 0;
        private int[] statsPerLevel = new int[4];
        private int[] baseStats = new int[4];
        private int[] skills = new int[10];
         
        public Customization() {
            InitializeComponent();
            Instance = this;
            SetCharacterStats(new byte[18*4], -1);
        }

        public void SetGuid(int Guid, byte[] InventoryData) {
            guid = Guid;
            this.Dispatcher.Invoke(() =>
            {
                GuidTextfield.Content = Guid.ToString();

                Style iStyle = (Style)FindResource("InventoryItemButtonStyle");

                int numOfItems = InventoryData.Length / 20;  

                int[] data = new int[numOfItems*5];
                Buffer.BlockCopy(InventoryData, 0, data, 0, data.Length*4);
                Button[] buttons = new Button[numOfItems];

 

                StackPanel SSSP = InventoryTopBar; //useless assignment. This is only done to reduce warningcount by 2
                for (int i = 0; i < numOfItems; i++) {
                    if (i % 10 == 0) {//create a stackpanel for 10 elements, create a new one for the next 10
                        SSSP = new();
                        SSSP.Orientation = Orientation.Horizontal;
                        SSSP.Height = 50;
                        Inventory.Children.Add(SSSP); 
                    }
                    //Add thhe sp to our inventory
                    buttons[i] = new();

                    string imgSrc;
                    int item_id = data[i * 5 + 2];
                    int character_experience_used = data[i * 5 + 3];
                    int item_level = data[i * 5 + 4];

                    if (item_id > 1000) {
                        imgSrc = "/Video/spellforceRuneTransparent.png";
                    } else {
                        imgSrc = "/Video/weapon" + item_id.ToString() + ".png";
                    }
                    CustomButton.SetImageSource(buttons[i], imgSrc);

                    if (data[i*5+3]>1) {
                        buttons[i].Content = data[i * 5 + 3].ToString();
                    }
                    if (!(data[i * 5 + 3] > 1)) {
                        CustomButton.SetImageSource2(buttons[i], "/Video/spellforceRuneTransparentBackground.png");
                        buttons[i].Click += Click_Req; //Buttons pass their tag, so that the correct method can be called

                    } else {
                        CustomButton.SetImageSource2(buttons[i], "/Video/weapon"+ item_id.ToString()  +".png");
                        buttons[i].Click += ShowXP; //Buttons pass their tag, so that the correct method can be called
                        xp = character_experience_used;

                    }

                    buttons[i].Style = iStyle;
                    buttons[i].Height = 40;
                    buttons[i].Width = 40; 
                    buttons[i].Tag = (data[5 * i]);//postgres indices start at 1
                    buttons[i].Margin = new Thickness(5, 5, 5, 5);
                    buttons[i].Background = PickColorBasedOnInt(data[i*5 +2]); 
                    SSSP.Children.Add(buttons[i]);
                }
            });



        }
        int xp = 0;
        public void ShowXP(Object sender, RoutedEventArgs args) {
            Mid1.Content = xp;
        }

        public void SetCharacterStats(byte[] characterData, int charIndex) {
            loadedCharacterIndex = charIndex;

            //all values are multiplied by 4 because we count ints, and ints are 4x larger than bytes
            Buffer.BlockCopy(characterData, 0, baseStats, 0, baseStats.Length * 4);
            Buffer.BlockCopy(characterData, baseStats.Length * 4, statsPerLevel, 0, statsPerLevel.Length * 4);
            Buffer.BlockCopy(characterData, baseStats.Length * 8, skills, 0, skills.Length * 4);

            this.Dispatcher.Invoke(() =>
            {
                Mid1.Content = baseStats[0]> 9 ? " " + baseStats[0].ToString(): "  " + baseStats[0].ToString();
                Mid2.Content = baseStats[1]> 9 ? " " + baseStats[1].ToString(): "  " + baseStats[1].ToString();
                Mid3.Content = baseStats[2]> 9 ? " " + baseStats[2].ToString(): "  " + baseStats[2].ToString();
                Mid4.Content = baseStats[3]> 9 ? " " + baseStats[3].ToString(): "  " + baseStats[3].ToString();
            });
        }

        private void Click_Save(object sender, RoutedEventArgs e) {
            if (loadedCharacterIndex == -1) {
                return;
            }
            MainWindow.Instance.SaveUserData(guid, loadedCharacterIndex, baseStats, statsPerLevel, skills);
        }

        private void Click_Req(object sender, RoutedEventArgs e) { 
            Button b = (Button)sender;
            int characterID = (int)b.Tag;
            MainWindow.Instance.ReqCharData(guid, characterID);
        }

        public static SolidColorBrush PickColorBasedOnInt(int value) {
            switch (value) {
                case 1001:
                    return new SolidColorBrush(Colors.Red);
                case 1002:
                    return new SolidColorBrush(Colors.Blue);
                case 1003:
                    return new SolidColorBrush(Colors.Green);
                // Add more cases for values 4 through 10 as needed
                default:
                    return new SolidColorBrush(Colors.Teal); // Default color if value is out of range
            }
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

            baseStats[index] += left ? -1 : 1;
            baseStats[index] = Math.Max(0, baseStats[index]);//cap value at 0

            this.Dispatcher.Invoke(() =>
            {
                l.Content = baseStats[index] > 9 ? " " + baseStats[index].ToString() : "  " + baseStats[index].ToString();
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

    public static class CustomButton {
        public static readonly DependencyProperty ImageSourceProperty =
            DependencyProperty.RegisterAttached(
                "ImageSource",
                typeof(string),
                typeof(CustomButton),
                new PropertyMetadata(string.Empty));

        public static readonly DependencyProperty ImageSourceProperty2 =
            DependencyProperty.RegisterAttached(
                "ImageSource2",
                typeof(string),
                typeof(CustomButton),
                new PropertyMetadata(string.Empty));

        public static string GetImageSource(DependencyObject obj) {
            return (string)obj.GetValue(ImageSourceProperty);
        }

        public static void SetImageSource(DependencyObject obj, string value) {
            obj.SetValue(ImageSourceProperty, value);
        }
        public static void SetImageSource2(DependencyObject obj, string value) {
            obj.SetValue(ImageSourceProperty2, value);
        }
    }
}

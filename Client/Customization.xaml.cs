using System;
using System.IO;
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

    public struct CharacterButtonData {
        public int item_id;
        public int level;
        public int item_guid;
        public Button button;
    }

    public partial class Customization : Page {
        public static Customization Instance;
        public int UserGuid = 0;

        private CustomizedCharacter CurrentUIStats = new();
        private int[] deltaStatss = new int[4];

        int loadedCharacterIndex = -1;
        int xp = 0;

        int[] data = Array.Empty<int>();
        CharacterButtonData[] buttonData = Array.Empty<CharacterButtonData>();

        CustomizedCharacter[] characterData;

        CustomizedCharacter characterDataServer;
        public Customization() {
            InitializeComponent();
            Instance = this;

            //now load the base values of characters from data files
            int numOfFiles = 0;
            for (int i = 0; i < 100; i++) { // count the number of Character-base-values
                string characterFilePath = MainWindow.FilesPath + "CharacterBaseValues\\" + i.ToString() + ".characterData";
                if (!File.Exists(characterFilePath))
                    break;
                numOfFiles++;
            }
            characterData = new CustomizedCharacter[numOfFiles];
            for (int i = 0; i < numOfFiles; i++) {
                string characterFilePath = MainWindow.FilesPath + "CharacterBaseValues\\" + i.ToString() + ".characterData";
                characterData[i] = new CustomizedCharacter(File.ReadAllBytes(characterFilePath));
            }
        }

        public void SetGuid(int Guid, byte[] InventoryData) {
            UserGuid = Guid;
            this.Dispatcher.Invoke(() =>
            {
                CharacterCustomizationButtons.Visibility = Visibility.Collapsed;

                GuidTextfield.Content = Guid.ToString();

                Style iStyle = (Style)FindResource("InventoryItemButtonStyle");

                int numOfItems = InventoryData.Length / 20;

                data = new int[numOfItems * 5];
                buttonData = new CharacterButtonData[numOfItems];
                Buffer.BlockCopy(InventoryData, 0, data, 0, data.Length * 4);


                StackPanel SSSP = InventoryTopBar; //useless assignment. This is only done to reduce warningcount by 2
                for (int i = 0; i < numOfItems; i++) {
                    if (i % 10 == 0) {//create a stackpanel for 10 elements, create a new one for the next 10
                        SSSP = new();
                        SSSP.Orientation = Orientation.Horizontal;
                        SSSP.Height = 50;
                        Inventory.Children.Add(SSSP);
                    }
                    //Add thhe sp to our inventory
                    buttonData[i].button = new();
                    buttonData[i].item_id = data[i * 5 + 2];
                    string imgSrc;
                    string imgSrc2;

                    if (buttonData[i].item_id != 1) {
                        imgSrc = "/Video/spellforceRuneTransparent.png";
                        imgSrc2 = "/Video/spellforceRuneTransparentBackground.png";
                        buttonData[i].button.Click += Click_Req; //Buttons pass their tag, so that the correct method can be called

                    }
                    else {
                        imgSrc = "/Video/weapon" + buttonData[i].item_id.ToString() + ".png";
                        imgSrc2 = "/Video/weapon" + buttonData[i].item_id.ToString() + ".png";
                        SetXP(data[i * 5 + 3]);
                    }

                    buttonData[i].level = (int)Math.Sqrt((data[i * 5 + 3] / 10));
                    buttonData[i].item_guid = data[5 * i];

                    CustomButton.SetImageSource(buttonData[i].button, imgSrc);
                    CustomButton.SetImageSource2(buttonData[i].button, imgSrc2);

                    buttonData[i].button.Style = iStyle;
                    buttonData[i].button.Height = 40;
                    buttonData[i].button.Width = 40;
                    buttonData[i].button.Tag = i; //we lookup the button data in the buttonData array using the index saved here
                    buttonData[i].button.Margin = new Thickness(5, 5, 5, 5);
                    buttonData[i].button.Background = PickColorBasedOnInt(data[i * 5 + 2]);
                    SSSP.Children.Add(buttonData[i].button);
                }
            });
        }

        public void SetXP(int xpAmount) {
            xp = xpAmount;
            this.Dispatcher.Invoke(() =>
            {
                XP_Field.Content = xp.ToString() + " XP Available";
            });

        }

        public void SetCharacterLevel(int characterGuid, int newLevel) {

            for (int i = 0; i < buttonData.Length; i++) {
                if (buttonData[i].item_guid == characterGuid) {
                    buttonData[i].level = newLevel;

                    this.Dispatcher.Invoke(() =>
                    {
                        LevelField.Content = "Level " + newLevel.ToString();
                    });
                    break;
                }
            }
            SetCharacterPointsLeft(CurrentUIStats.stats);

        }

        public void SetCharacterPointsLeft(byte[] newStats) {
            byte[] baseStats = characterData[buttonData[loadedCharacterIndex].item_id - 1000].stats;
            int totalCustomPoints = buttonData[loadedCharacterIndex].level * 2; //2 for adding, 2 for taking per level
            int pointsPreviouslyAllocated = characterDataServer.statpointsFullyAllocated;
            int pointsAllocatedThisRequest = GetFullyAllocatedPoints(characterDataServer.stats, CurrentUIStats.stats);

            int pointsLeftToAllocate = totalCustomPoints - pointsAllocatedThisRequest - pointsPreviouslyAllocated;

            int pointsUsed = 0;
            int pointsUsedPositive = 0;
            int pointsUsedNegative = 0;

            //ok when all is fine and i level up: 0->1  oben 2, mitte 2, unten nix
            //after saved,                              oben 2, mitte nix, unten 2
            //next level normal:                        oben 4, mitte 2, unten 2
            //after reskill attempt:                    oben 0, mitte 2, unten 2

            //TODO we should work with (make ui) and network the stats we cahgne istread of absolute stats
            int[] deltaServerStats = new int[4];
            int[] deltaStats = new int[4];
            for (int i = 0; i < 4; i++) {
                deltaServerStats[i] = newStats[i] - characterDataServer.stats[i];
                deltaStats[i] = newStats[i] - baseStats[i];
            }

            for (int i = 0; i < 4; i++) {

                pointsUsed -= deltaStats[i];
                pointsUsedPositive += Math.Max(deltaStats[i], 0);
                pointsUsedNegative += Math.Max(-deltaStats[i], 0);

            }

            //int pointsLeftToAllocate = (totalCustomPoints - CurrentUIStats.statpointsFullyAllocated);
            int pointDifference = (pointsUsedNegative - pointsUsedPositive) - pointsLeftToAllocate;

            this.Dispatcher.Invoke(() =>
            {
                StatpointsLeft.Content = pointsUsed.ToString() + "/" + pointsLeftToAllocate.ToString();//now set points left stuff

                if (pointDifference != 0) {
                    Left1.Visibility = Visibility.Visible;
                    Left2.Visibility = Visibility.Visible;
                    Left3.Visibility = Visibility.Visible;
                    Left4.Visibility = Visibility.Visible;
                }
                else {
                    Left1.Visibility = Visibility.Hidden;
                    Left2.Visibility = Visibility.Hidden;
                    Left3.Visibility = Visibility.Hidden;
                    Left4.Visibility = Visibility.Hidden;

                    if (deltaServerStats[0] > 0)
                        Left1.Visibility = Visibility.Visible;
                    if (deltaServerStats[1] > 0)
                        Left2.Visibility = Visibility.Visible;
                    if (deltaServerStats[2] > 0)
                        Left3.Visibility = Visibility.Visible;
                    if (deltaServerStats[3] > 0)
                        Left4.Visibility = Visibility.Visible;

                }
                if (pointsUsed > 0) {
                    Right1.Visibility = Visibility.Visible;
                    Right2.Visibility = Visibility.Visible;
                    Right3.Visibility = Visibility.Visible;
                    Right4.Visibility = Visibility.Visible;
                }
                else {
                    Right1.Visibility = Visibility.Hidden;
                    Right2.Visibility = Visibility.Hidden;
                    Right3.Visibility = Visibility.Hidden;
                    Right4.Visibility = Visibility.Hidden;
                }
            });
        }

        public void SetCharacterStats(byte[] charDataServer, int charGuid) {

            characterDataServer = new CustomizedCharacter(charDataServer);

            for (int i = 0; i < buttonData.Length; i++) {
                if (buttonData[i].item_guid == charGuid) {
                    loadedCharacterIndex = i;
                    break;
                }
            }

            deltaStatss = new int[4];

            Buffer.BlockCopy(charDataServer, 0, CurrentUIStats.stats, 0, CurrentUIStats.stats.Length);
            Buffer.BlockCopy(charDataServer, CurrentUIStats.stats.Length, CurrentUIStats.statsPerLevel, 0, CurrentUIStats.statsPerLevel.Length);
            Buffer.BlockCopy(charDataServer, CurrentUIStats.stats.Length, CurrentUIStats.skills, 0, CurrentUIStats.skills.Length);



            byte[] baseStatss = characterData[buttonData[loadedCharacterIndex].item_id - 1000].stats;

            int pointsUsed = 0;
            for (int i = 0; i < 4; i++) {
                pointsUsed += Math.Abs(CurrentUIStats.stats[i] - baseStatss[i]);
            }
            int levelRequired = pointsUsed / 4;

            SetCharacterPointsLeft(CurrentUIStats.stats);
            this.Dispatcher.Invoke(() =>
            {
                CharacterCustomizationButtons.Visibility = Visibility.Visible;

                Mid1.Content = CurrentUIStats.stats[0] > 9 ? " " + CurrentUIStats.stats[0].ToString() : "  " + CurrentUIStats.stats[0].ToString();
                Mid2.Content = CurrentUIStats.stats[1] > 9 ? " " + CurrentUIStats.stats[1].ToString() : "  " + CurrentUIStats.stats[1].ToString();
                Mid3.Content = CurrentUIStats.stats[2] > 9 ? " " + CurrentUIStats.stats[2].ToString() : "  " + CurrentUIStats.stats[2].ToString();
                Mid4.Content = CurrentUIStats.stats[3] > 9 ? " " + CurrentUIStats.stats[3].ToString() : "  " + CurrentUIStats.stats[3].ToString();
            });
        }



        private void Click_Req(object sender, RoutedEventArgs e) {
            Button b = (Button)sender;
            CharacterButtonData objectData = buttonData[(int)b.Tag];
            int characterID = objectData.item_guid;
            MainWindow.Instance.ReqCharData(UserGuid, characterID);

            this.Dispatcher.Invoke(() =>
            {
                LevelField.Content = "Level " + objectData.level.ToString();
            });
        }

        private void Click_Reskill(object sender, RoutedEventArgs e) {
            MainWindow.Instance.ReskillCharData(UserGuid, buttonData[loadedCharacterIndex].item_guid);
        }

        private void Click_Save(object sender, RoutedEventArgs e) {
            if (CurrentUIStats.stats[0] + CurrentUIStats.stats[1] + CurrentUIStats.stats[2] + CurrentUIStats.stats[3] != 60)
                return;//not all points have been used

            byte[] deltaStats = new byte[4];
            for (int i = 0; i < 4; i++) {
                deltaStats[i] = (byte)(CurrentUIStats.stats[i] - characterDataServer.stats[i]);
            }


            MainWindow.Instance.SaveCharacterStats(UserGuid, buttonData[loadedCharacterIndex].item_guid, deltaStats, CurrentUIStats.statsPerLevel, CurrentUIStats.skills, (byte)0);
        }

        private void Click_Levelup(object sender, RoutedEventArgs e) {
            MainWindow.Instance.LevelupCharacter(UserGuid, buttonData[loadedCharacterIndex].item_guid, buttonData[loadedCharacterIndex].level + 1);
        }

        private int GetFullyAllocatedPoints(byte[] previousBaseStats, byte[] newStats) {

            int[] deltaStats = new int[4];
            int totalDeltaStatsPositive = 0;
            int totalDeltaStatsNegative = 0;
            for (int i = 0; i < 4; i++) {
                deltaStats[i] = (newStats[i] - previousBaseStats[i]);
                totalDeltaStatsPositive += Math.Max(deltaStats[i], (byte)0);
                totalDeltaStatsNegative += Math.Max(-deltaStats[i], (byte)0);
            }

            return Math.Min(totalDeltaStatsNegative, totalDeltaStatsPositive); // 
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
            if (CurrentUIStats.stats[index] == 0 && left) {
                return;//cap stats at 0
            }
            if (left) {
                CurrentUIStats.stats[index]--;
            }
            else {
                CurrentUIStats.stats[index]++;
            }
            deltaStatss[index] = !left ? deltaStatss[index] + 1 : deltaStatss[index] - 1; // if we decreased a stat, increase delta by -1
            SetCharacterPointsLeft(CurrentUIStats.stats);

            this.Dispatcher.Invoke(() =>
            {
                l.Content = CurrentUIStats.stats[index] > 9 ? " " + CurrentUIStats.stats[index].ToString() : "  " + CurrentUIStats.stats[index].ToString();
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

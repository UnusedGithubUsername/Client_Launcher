using Client.Models;
using System; 
using System.Collections.ObjectModel;
using System.ComponentModel; 
using System.Windows;
using System.Windows.Controls; 
using System.Windows.Threading;

namespace Client
{
    /// <summary>
    /// Interaction logic for CustomizationOptions.xaml
    /// </summary>
    public partial class CustomizationOptions : Page, INotifyPropertyChanged
    {
        private int _currentSkill = -1;

        private int currentCharacterID = -1;

        public Customization ParentUI;
        public int currentlyCustomizedSkill
        {
            get
            {
                return _currentSkill;
            }
            set
            {
                _currentSkill = value;
                OnPropertyChanged(nameof(currentlyCustomizedSkill));
                OnPropertyChanged(nameof(CurrentUIStats));
            }
        }

        public CharacterStat_Model CurrentUIStats { get; set; }

        public static ObservableCollection<SkillChoice_Model> skillInfoLibrary { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public CustomizationOptions()
        {
            skillInfoLibrary = new ObservableCollection<SkillChoice_Model>();
            ReadSkillInfo();
            OnPropertyChanged("skillInfoLibrary");
            CurrentUIStats = new CharacterStat_Model();
            InitializeComponent();
        }

        public void SetCharacterStats(byte[] charDataServer, CharacterRune_Model character)
        {
            currentCharacterID = character.ItemGUId;
            CurrentUIStats.SetAllStats(charDataServer, character);
            OnPropertyChanged(nameof(CurrentUIStats));
        }

        public void ConfirmSkillChoice(object sender, RoutedEventArgs e)
        {
            int ID = (int)((Button)sender).Tag;
            CurrentUIStats.SetSkill(currentlyCustomizedSkill, (byte)ID);
            CurrentUIStats.skills[currentlyCustomizedSkill] = (byte)ID;
            currentlyCustomizedSkill = -1;
            CurrentUIStats.UpdateGUI();
            OnPropertyChanged(nameof(CurrentUIStats));
        }

        public void ShowSkillchoice(object sender, RoutedEventArgs e)
        {
            currentlyCustomizedSkill = int.Parse(((Button)sender).Tag.ToString());  
        }

        public void Click(object sender, RoutedEventArgs e)
        {
            bool left = ((Button)sender).Content.ToString() == "left";
            int index = int.Parse(((Button)sender).Tag.ToString());
            if (!(CurrentUIStats.stats[index] == 0 && left))
            {
                if (!left)
                {
                    CurrentUIStats.Increment(index);
                }
                else
                {
                    CurrentUIStats.Decrement(index);
                } 
            }
            OnPropertyChanged(nameof(CurrentUIStats));
        }

        private void Click_Reskill(object sender, RoutedEventArgs e)
        {
            App.Instance.ReskillCharData(App.UserGuid, currentCharacterID);
        }

        private void Click_Save(object sender, RoutedEventArgs e)
        {
            if (CurrentUIStats.stats[0] + CurrentUIStats.stats[1] + CurrentUIStats.stats[2] + CurrentUIStats.stats[3] == 60)
            {
                byte[] deltaStats = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    deltaStats[i] = (byte)(CurrentUIStats.stats[i] - CurrentUIStats.ServersideData.stats[i]);
                }
                App.Instance.SaveCharacterStats(App.UserGuid, currentCharacterID, deltaStats, CurrentUIStats);
            }
        }

        private void Click_Levelup(object sender, RoutedEventArgs e)
        {
            App.Instance.LevelupCharacter(App.UserGuid, currentCharacterID, CurrentUIStats.level + 1);
        }

        public void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            double multiplier = base.ActualHeight / 450.0;
            ((DispatcherObject)this).Dispatcher.Invoke((Action)delegate
            {
                SkillGrid.Height = 350.0 * multiplier;
                CharStatGrid.ColumnDefinitions[0].Width = new GridLength(70.0 * multiplier);
                SkillGrid.Width = 70.0 * multiplier;
                StatGrid.Width = 200.0 * multiplier;
            });
        }

        public void SetCharacterLevel(int level)
        {
            CurrentUIStats.level = level;
            CurrentUIStats.SetStatsToDefault();
        }

        public void ReadSkillInfo()
        {
            SkillID[] ids = (SkillID[])Enum.GetValues(typeof(SkillID));
            for (int i = 0; i < ids.Length; i++)
            {
                ObservableCollection<SkillChoice_Model> observableCollection = skillInfoLibrary;
                string filesPath = App.FilesPath;
                int num = (int)ids[i];
                observableCollection.Add(new SkillChoice_Model(filesPath + "Skills\\" + num + ".skill"));
            }
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public void UpdateUI()
        {
            CurrentUIStats.UpdateGUI();
            OnPropertyChanged(nameof(CurrentUIStats));
            OnPropertyChanged(nameof(CurrentUIStats.AvailableAbilitypoints));

        }

    }
}

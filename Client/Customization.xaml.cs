using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics; 
using System.Windows.Input;
using System.Windows.Controls.Primitives; 
using System.Collections.ObjectModel;
using Client.Models;
using System.ComponentModel;

namespace Client { 
    public partial class Customization : Window, INotifyPropertyChanged {
        public static Customization Instance;
        public CharacterStat_Model CurrentUIStats { get; set; }
        public ObservableCollection<Friend_Model> friendsList { get; set; }
        public ObservableCollection<Friend_Model> friendsRequestList { get; set; }
        public ObservableCollection<CharacterRune_Model> CharacterData { get; set; }
        public ObservableCollection<SkillChoice_Model> skillInfoLibrary { get; set; } 

        public int UserGuid { get { return _userGuid; } set { _userGuid = value; OnPropertyChanged(nameof(UserGuid)); } }
        private int _userGuid = 0;
        public int loadedCharacterIndex { get { return _loadedCharacterIndex; } set { _loadedCharacterIndex = value; OnPropertyChanged(nameof(loadedCharacterIndex)); } }
        private int _loadedCharacterIndex = -1;
        public int loadedFriendIndex { get { return _loadedFriendIndex; } set { _loadedFriendIndex = value; OnPropertyChanged(nameof(loadedFriendIndex)); } }
        private int _loadedFriendIndex = -1;
        public int xp { get { return _xp; } set { _xp = value; OnPropertyChanged(nameof(xp)); } }
        private int _xp = -10; 
        public string Username { get { return _username; } set { _username = value; OnPropertyChanged(nameof(Username)); } }
        private string _username = "";
        public int currentlyCustomizedSkill { get { return _currentSkill; } set { _currentSkill = value; OnPropertyChanged(nameof(currentlyCustomizedSkill)); } }
        private int _currentSkill = -1;  

        CharacterDataServer[] baseStatsOfAllCharacters;//baseStats downlaoded from the server 
        CharacterStat_Model ServersideCharacterData; //this struct is read and replace only and can not be modified

        private ObservableCollection<FriendChat_Model> AllChats;
        public FriendChat_Model CurrentChat { get; set; }
        public Customization() {
            Instance = this; 
            InitializeComponent();

            skillInfoLibrary = new();
            ReadSkillInfo();
            OnPropertyChanged(nameof(skillInfoLibrary));

            AllChats = new();
            CurrentUIStats = new();
            ServersideCharacterData = new();
            CharacterData = new(); 
            friendsList = new();
            friendsRequestList = new();
             //now load the base values of characters from data files
            int numOfFiles = 0;
            for (int i = 0; i < 100; i++) { // count the number of Character-base-values
                string characterFilePath = App.FilesPath + "CharacterBaseValues\\" + i.ToString() + ".characterData";
                if (!File.Exists(characterFilePath))
                    break;

                numOfFiles++;
            }
            baseStatsOfAllCharacters = new CharacterDataServer[numOfFiles];
            for (int i = 0; i < numOfFiles; i++) {
                string characterFilePath = App.FilesPath + "CharacterBaseValues\\" + i.ToString() + ".characterData";
                baseStatsOfAllCharacters[i] = new CharacterDataServer(File.ReadAllBytes(characterFilePath));
                //baseStatsOfAllCharacters[i].SetAllStats();
            }
             
        }

        private void ChatInputBox_KeyDown(object sender, KeyEventArgs e) {
            if (e.Key != Key.Enter)
                return;

            App.Instance.SendMessage(UserGuid, CurrentChat.Guid, ChatInputBox.Text); 
            ChatInputBox.Clear();
        }

        public void MessageRecieved(int guid, string message) {
            int index = -1;
            for (int i = 0; i < AllChats.Count; i++) 
                if(AllChats[i].Guid == guid)
                    index=i;

            if (index == -1) {
                index = AllChats.Count;
                AllChats.Add(new FriendChat_Model(guid));
            }

            this.Dispatcher.Invoke(() =>
            {  
                AllChats[index].MessageRecieved(message);
            }); 
        }

        public void FriendRequestRecieved(int guid, string name) {

            friendsRequestList.Add(new Friend_Model(name, guid, true));
            OnPropertyChanged(nameof(friendsRequestList));
        }

        public void ReadSkillInfo() {//the txt contains 1 line for the imagename and one for the description   
            SkillID[] ids = (SkillID[])Enum.GetValues(typeof(SkillID)); 
            
            for (int i = 0; i < ids.Length; i++)   
                skillInfoLibrary.Add(new(App.FilesPath + "Skills\\" + ((int)ids[i]).ToString() + ".skill")); 
        }
 
        public void Click_AddFriend(object sender, RoutedEventArgs e) {
            string friendGuid = AddFriend.Text;
            if (!IsAllDigits(friendGuid))
                return;//before converting to int, check if its actually a number

            AddFriend.Text = "";
            App.Instance.Friend(UserGuid, int.Parse(friendGuid), FriendrequestAction.RequestFriendship);
        }

        public static bool IsAllDigits(string text) {
            if (text.Length == 0)
                return false;

            for (int i = 0; i < text.Length; i++) 
                if (!char.IsDigit(text[i]))
                    return false;
             
            return true;
        }

        public void SetFriendslist(int[] friendIDs, string[] friendNames) {
            
            this.Dispatcher.Invoke(() => {
                friendsList.Clear();
                for (int i = 0; i < friendNames.Length; i++)
                    friendsList.Add(new(friendNames[i], friendIDs[i], false));

            });
             
            OnPropertyChanged(nameof(friendsList));
        }

        public void AcceptFriend(object sender, RoutedEventArgs e) {
            Button b = (Button)sender;
            int friendGuid = (int)(b.Tag);
            App.Instance.Friend(UserGuid, friendGuid, FriendrequestAction.Accept);


            int index = 0;
            for (int i = 0; i < friendsRequestList.Count; i++) {
                if (friendsRequestList[i].FriendId == friendGuid)
                    index = i;
            }
            friendsList.Add(new Friend_Model(friendsRequestList[index].FriendName, friendsRequestList[index].FriendId, false)); 
            friendsRequestList.RemoveAt(index);

            OnPropertyChanged(nameof(friendsList));
            OnPropertyChanged(nameof(friendsRequestList));

        }
        public void RejectFriend(object sender, RoutedEventArgs e) {
            Button b = (Button)sender;
            int friendGuid = (int)(b.Tag);
            App.Instance.Friend(UserGuid, friendGuid, FriendrequestAction.Deny);

            for (int i = 0; i < friendsRequestList.Count; i++) {
                if(friendsRequestList[i].FriendId == friendGuid)
                    friendsRequestList.RemoveAt(i);
            }
            OnPropertyChanged(nameof(friendsRequestList));

        }
        public void BlockFriend(object sender, RoutedEventArgs e) {
            Button b = (Button)sender;
            int friendGuid = (int)(b.Tag);
            App.Instance.Friend(UserGuid, friendGuid, FriendrequestAction.Block);

            for (int i = 0; i < friendsRequestList.Count; i++) {
                if (friendsRequestList[i].FriendId == friendGuid)
                    friendsRequestList.RemoveAt(i);
            }
            OnPropertyChanged(nameof(friendsRequestList));

        }
        public void RemoveFriend(object sender, RoutedEventArgs e) {
            Button b = (Button)sender;
            int friendGuid = (int)(b.Tag);
            App.Instance.Friend(UserGuid, friendGuid, FriendrequestAction.Remove);
            for (int i = 0; i < friendsRequestList.Count; i++) {
                if (friendsRequestList[i].FriendId == friendGuid)
                    friendsRequestList.RemoveAt(i);
            }
            OnPropertyChanged(nameof(friendsList));

        }
         
        public void ConfirmSkillChoice(object sender, RoutedEventArgs e) {
             

            int ID = (int)((Button)sender).Tag; //the button in the menu has the skillID attatched as a tag

            CurrentUIStats.SetSkill(currentlyCustomizedSkill, (byte)(ID));
            CurrentUIStats.skills[currentlyCustomizedSkill] = (byte)ID;
            currentlyCustomizedSkill = -1;
            CurrentUIStats.UpdateGUI();

            OnPropertyChanged(nameof(CurrentUIStats));
        }
          
        public void ShowSkillchoice(object sender, RoutedEventArgs e) {
            currentlyCustomizedSkill = int.Parse(((Button)sender).Tag.ToString());
        }

        public void SetItems(int Guid, int[] guids, int[] levels, int[] types, string username, int playerXP) {
            
            UserGuid = Guid;
            xp = playerXP;
            Username = username;    
            this.Dispatcher.Invoke(() =>
            {
                CharacterData.Clear();
                for (int i = 0; i < guids.Length; i++)     
                    CharacterData.Add(new(guids[i], levels[i], types[i], i));  
            });

            OnPropertyChanged(nameof(CharacterData));
        }
         
        public void SetCharacterLevel(int characterGuid, int newLevel, int remainingXP) {
            xp = remainingXP;
            for (int i = 0; i < CharacterData.Count; i++) 
                if (CharacterData[i].ItemGUId == characterGuid)  
                    CharacterData[i].level = newLevel; 
                    
            CurrentUIStats.level = newLevel;
            OnPropertyChanged(nameof(CurrentUIStats));

        }
          
        public void SetCharacterStats(byte[] charDataServer, int charGuid) {


            for (int i = 0; i < CharacterData.Count; i++) {
                if (CharacterData[i].ItemGUId == charGuid) {
                    loadedCharacterIndex = i;
                    break;
                }
            }
            ServersideCharacterData.SetAllStats(charDataServer, CharacterData[loadedCharacterIndex]); 
            CurrentUIStats.SetAllStats(charDataServer, CharacterData[loadedCharacterIndex]);

            currentlyCustomizedSkill = -1;

            OnPropertyChanged(nameof(CurrentUIStats));
        }
         
        private void Click_Friend(object sender, RoutedEventArgs e) {
            //unload the characterUI and load the friendUI
            loadedCharacterIndex = -1;
            loadedFriendIndex = 1; 
            OnPropertyChanged(nameof(loadedCharacterIndex));

            Button b = (Button)sender;
            int friendGuid =(int)(b.Tag);
            loadedFriendIndex = friendGuid;
            int chatIndex = -1;
            for (int i = 0; i < AllChats.Count; i++) 
                if (AllChats[i].Guid == friendGuid)
                    chatIndex = i;
             
            //create a new chat if we cant find the correct one
            if(chatIndex == -1) {
                chatIndex = AllChats.Count;
                AllChats.Add(new(friendGuid)); 
            }

            CurrentChat = AllChats[chatIndex];

            OnPropertyChanged(nameof(CurrentChat));
            OnPropertyChanged(nameof(CurrentChat.Chatoutput));
        }

        private void Click_Req(object sender, RoutedEventArgs e) {
            loadedFriendIndex = -1;

            Button b = (Button)sender;
            CharacterRune_Model objectData = CharacterData[(int)b.Tag];
            int characterID = objectData.ItemGUId;
            App.Instance.ReqCharData(UserGuid, characterID);
        }

        private void Click_Reskill(object sender, RoutedEventArgs e) {
            App.Instance.ReskillCharData(UserGuid, CharacterData[loadedCharacterIndex].ItemGUId);
        }

        private void Click_Save(object sender, RoutedEventArgs e) {
            if (CurrentUIStats.stats[0] + CurrentUIStats.stats[1] + CurrentUIStats.stats[2] + CurrentUIStats.stats[3] != 60)
                return;//not all points have been used

            byte[] deltaStats = new byte[4];
            for (int i = 0; i < 4; i++) {
                deltaStats[i] = (byte)(CurrentUIStats.stats[i] - ServersideCharacterData.stats[i]);
            }
            App.Instance.SaveCharacterStats(UserGuid, CharacterData[loadedCharacterIndex].ItemGUId, deltaStats,  CurrentUIStats);
        }

        private void Click_Levelup(object sender, RoutedEventArgs e) {
            App.Instance.LevelupCharacter(UserGuid, CharacterData[loadedCharacterIndex].ItemGUId, CharacterData[loadedCharacterIndex].level + 1);
        }
          
        private void Click(object sender, RoutedEventArgs e) {

            bool left = ((Button)sender).Content.ToString() == "left";
            int index = int.Parse(((Button)sender).Tag.ToString()); 

            if (CurrentUIStats.stats[index] == 0 && left)  
                return;//cap stats at 0
             
            if (!left)  
                CurrentUIStats.Increment(index); 
            else  
                CurrentUIStats.Decrement(index);

            OnPropertyChanged(nameof(CurrentUIStats));
             
        }
        //"{Binding currentlyCustomizedSkill, Converter={StaticResource IntToVisibilityConverter}}"
        private void Click_Play(object sender, RoutedEventArgs e) {
            string unityGamePath = App.FilesPath + "GameBuild2023/My Project.exe"; // Path to your Unity game executable

            //as argument we provide the full package that the user would log in with
            byte[] msg = App.Instance.con.loginPackage;
            string arguments = Convert.ToBase64String(msg);  

            ProcessStartInfo startInfo = new ProcessStartInfo(unityGamePath, arguments);
            Process.Start(startInfo);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Page_SizeChanged(object sender, SizeChangedEventArgs e) {
            double multiplier = this.ActualHeight / 450;
            this.Dispatcher.Invoke(() =>
            {
                SkillGrid.Height = 350 * multiplier;
                 
                CharStatGrid.ColumnDefinitions[0].Width = new GridLength( 70 * multiplier);
                SkillGrid.Width = 70 * multiplier;

                StatGrid.Width = 200 * multiplier;
            });

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
    } 

}

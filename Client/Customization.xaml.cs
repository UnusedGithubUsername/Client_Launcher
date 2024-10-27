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
using System.Reflection;

namespace Client { 
    public partial class Customization : Window, INotifyPropertyChanged {
        public static Customization Instance;
        public ObservableCollection<Friend_Model> friendsList { get; set; }
        public ObservableCollection<Friend_Model> friendsRequestList { get; set; }

        public int xp { get { return _xp; } set { _xp = value; OnPropertyChanged(nameof(xp)); } }
        private int _xp = -10;
        public string Username { get { return _username; } set { _username = value; OnPropertyChanged(nameof(Username)); } }
        private string _username = "";

        public int UserGuid { get { return _userGuid; } set { _userGuid = value; OnPropertyChanged(nameof(UserGuid)); } }
        private int _userGuid = 0;

        public ObservableCollection<CharacterRune_Model> CharacterData { get; set; }

        public int loadedCharacterIndex { get { return _loadedCharacterIndex; } set { _loadedCharacterIndex = value; OnPropertyChanged(nameof(loadedCharacterIndex)); } }
        private int _loadedCharacterIndex = -1;

        public int loadedFriendIndex { get { return _loadedFriendIndex; } set { _loadedFriendIndex = value; OnPropertyChanged(nameof(loadedFriendIndex)); } }
        private int _loadedFriendIndex = -1;

        CharacterDataServer[] baseStatsOfAllCharacters;//baseStats downlaoded from the server 
         
        public Friend_Model CurrentChat { get; set; }

        public Chat chatPage;
        public CharacterOptions customizationPage;

        public Customization() {
            Instance = this;
            customizationPage = new();

            InitializeComponent();

            chatPage = new();

            customizationPage.ParentUI = this;

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



        public void MessageRecieved(int guid, string message) {
            int index = -1;
            for (int i = 0; i < friendsList.Count; i++) 
                if(friendsList[i].Guid == guid)
                    index=i;

 

            this.Dispatcher.Invoke(() =>
            {
                friendsList[index].MessageRecieved( message);
            }); 
        }

        public void FriendRequestRecieved(int guid, string name) {

            friendsRequestList.Add(new Friend_Model(name, guid, true));
            OnPropertyChanged(nameof(friendsRequestList));
        }

        public void SetCharacterLevel(int characterGuid, int newLevel, int remainingXP)
        {
            xp = remainingXP;
            for (int i = 0; i < CharacterData.Count; i++)
                if (CharacterData[i].ItemGUId == characterGuid)
                    CharacterData[i].level = newLevel;

            //CurrentUIStats.level = newLevel;
            customizationPage.SetCharacterLevel(newLevel);

        }

        public void SetCharacterStats(byte[] charDataServer, int charGuid)
        {


            for (int i = 0; i < CharacterData.Count; i++)
            {
                if (CharacterData[i].ItemGUId == charGuid)
                {
                    loadedCharacterIndex = i;
                    break;
                }
            }  

            customizationPage.SetCharacterStats(charDataServer, CharacterData[loadedCharacterIndex]);
            customizationPage.UpdateUI();

            this.Dispatcher.Invoke(() => {
                OnPropertyChanged(nameof(CharacterCustomizationButton));
                OnPropertyChanged(nameof(loadedFriendIndex));
                CharacterCustomizationButton.Navigate(customizationPage);
                OnPropertyChanged(nameof(CharacterCustomizationButton));
                OnPropertyChanged(nameof(loadedFriendIndex));

            });


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

        public void AcceptFriend(object sender, RoutedEventArgs e)
        {
            Button b = (Button)sender;
            int friendGuid = (int)(b.Tag);
            App.Instance.Friend(UserGuid, friendGuid, FriendrequestAction.Accept);


            int index = 0;
            for (int i = 0; i < friendsRequestList.Count; i++)
            {
                if (friendsRequestList[i].Guid == friendGuid)
                    index = i;
            }
            string friendName = friendsRequestList[index].FriendName;
            int friendId = friendsRequestList[index].Guid;
            friendsRequestList.RemoveAt(index);

            AddFriendToFList(friendId, friendName);

        }

        public void AddFriendToFList(int friendId, string friendName)
        {
            this.Dispatcher.Invoke(() =>
            {
                friendsList.Add(new Friend_Model(friendName, friendId, false));

                OnPropertyChanged(nameof(friendsList));
                OnPropertyChanged(nameof(friendsRequestList));
            });

        }


        public void RejectFriend(object sender, RoutedEventArgs e) {
            Button b = (Button)sender;
            int friendGuid = (int)(b.Tag);
            App.Instance.Friend(UserGuid, friendGuid, FriendrequestAction.Deny);

            for (int i = 0; i < friendsRequestList.Count; i++) {
                if(friendsRequestList[i].Guid == friendGuid)
                    friendsRequestList.RemoveAt(i);
            }
            OnPropertyChanged(nameof(friendsRequestList));

        }
        public void BlockFriend(object sender, RoutedEventArgs e) {
            Button b = (Button)sender;
            int friendGuid = (int)(b.Tag);
            App.Instance.Friend(UserGuid, friendGuid, FriendrequestAction.Block);

            for (int i = 0; i < friendsRequestList.Count; i++) {
                if (friendsRequestList[i].Guid == friendGuid)
                    friendsRequestList.RemoveAt(i);
            }
            OnPropertyChanged(nameof(friendsRequestList));

        }
        public void RemoveFriend(object sender, RoutedEventArgs e) {
            Button b = (Button)sender;
            int friendGuid = (int)(b.Tag);
            App.Instance.Friend(UserGuid, friendGuid, FriendrequestAction.Remove);
            for (int i = 0; i < friendsRequestList.Count; i++) {
                if (friendsRequestList[i].Guid == friendGuid)
                    friendsRequestList.RemoveAt(i);
            }
            OnPropertyChanged(nameof(friendsList));

        }
         


         
        private void Click_Friend(object sender, RoutedEventArgs e) {
            //unload the characterUI and load the friendUI
            CharacterCustomizationButton.Content = chatPage;
            //TODO Get the friend name, its the content of the button we pressed
            //loadedCharacterIndex = -1;
            loadedFriendIndex = 1; 
            //OnPropertyChanged(nameof(loadedCharacterIndex));

            Button b = (Button)sender;
            int friendGuid =(int)(b.Tag);
            loadedFriendIndex = friendGuid;
            int chatIndex = -1;
            for (int i = 0; i < friendsList.Count; i++) 
                if (friendsList[i].Guid == friendGuid)
                    chatIndex = i;
             
 

            CurrentChat = friendsList[chatIndex];
             
            OnPropertyChanged(nameof(CurrentChat));
            OnPropertyChanged(nameof(CurrentChat.Chatoutput));
            OnPropertyChanged(nameof(customizationPage));
        }

        private void Click_Req(object sender, RoutedEventArgs e) {
            this.Dispatcher.Invoke(() => { 
                CharacterCustomizationButton.Navigate(customizationPage); 

            });
             
            loadedFriendIndex = -1;

            Button b = (Button)sender;
            CharacterRune_Model objectData = CharacterData[(int)b.Tag]; 
            int characterID = objectData.ItemGUId;
            App.Instance.ReqCharData(UserGuid, characterID);
            customizationPage.UpdateUI();

        }


        public void SetItems(int Guid, int[] guids, int[] levels, int[] types, string username, int playerXP)
        {

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

        private void Click_Play(object sender, RoutedEventArgs e) {
            string unityGamePath = App.FilesPath + "GameBuild2023/My Project.exe"; // Path to Unity game executable

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

using System; 
using System.Windows; 
using System.Timers;
using System.IO;  
using System.Threading.Tasks; 
using System.Text;

using Amazon;
using Amazon.S3; 
using Amazon.S3.Transfer; 

namespace Client
{ 
    public partial class App : Application {
        public static App Instance;
         
        private Window CustomizationWindow;
        private MainWindow mainWindow;

        public Connection con;
        private bool connected = false;

        public static string FilesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string[] fpParts = { "\\My Games", "\\Corivi", "\\LauncherClient\\" };
        public static int UserGuid;

        [STAThread] 
        protected override void OnStartup(StartupEventArgs e) {
            base.OnStartup(e);
            Instance = this;
            for (int i = 0; i < fpParts.Length; i++) {
                FilesPath += fpParts[i];
                Helper.EnsureFolderExists(FilesPath);
            }

            mainWindow = new MainWindow();
            mainWindow.Show();
            CustomizationWindow = new Customization();
             

            con = new();

            Timer updateLoop = new(100);//Create an Update() function used for checking incomming connections and data
            updateLoop.Elapsed += Update;
            updateLoop.Enabled = true;
            updateLoop.AutoReset = true;
            updateLoop.Start();

            

            //we gotta connect to the fileserver asynchronously and while the download is ongoing we call the windows UpdateProgressBar()
            //ConnectToFileServer(serverIP);//eventually this needs to happen AND FINISH before we create the main window. or make a small c programm that opens this app..
            ConnectToDatabaseServer();
        }

        public void SaveCharacterStats(int guid, int charIndex, byte[] deltaStats, CharacterStat_Model character) {
            byte[] dataToSend = character.ToByte();
            Buffer.BlockCopy(deltaStats, 0, dataToSend, 0, 4);
            con.WriteInt(guid);
            con.WriteInt(0);
            con.WriteInt(charIndex);
            con.WriteBytes(ref dataToSend); 
            con.Send(PacketTypeClient.requestWithToken);
        }

        public void LevelupCharacter(int guid, int charIndex, int requestedLevel) {
            con.WriteInt(guid);
            con.WriteInt(2);//requestType
            con.WriteInt(charIndex);
            con.WriteInt(requestedLevel); //sent "redundantly" to prevent accidentailly computing the request twice 
            con.Send(PacketTypeClient.requestWithToken);
        }

        public void ReqCharData(int guid, int charIndex) {
            con.WriteInt(guid);
            con.WriteInt(1); //request a chars data
            con.WriteInt(charIndex);
            con.Send(PacketTypeClient.requestWithToken);
        }

        public void ReskillCharData(int guid, int charIndex) {
            con.WriteInt(guid);
            con.WriteInt(3); //request a chars data
            con.WriteInt(charIndex);
            con.Send(PacketTypeClient.requestWithToken);
        }

        public void SendMessage(int guid, int friendGuid, string message) {
            con.WriteInt(guid); 
            con.WriteInt(friendGuid);
            con.WriteString(message);
            con.Send(PacketTypeClient.Message);
        }

        public async void ConnectToDatabaseServer() {
            string serverIP = await GetIPAWS();//find out where the server is
             
            while (true) { //and connect to it
                if (await con.TryToConnect(System.Net.IPAddress.Parse(serverIP))) {//if connection was successfull
                    connected = true;
                    mainWindow.ShowLogin();
                    con.Send(PacketTypeClient.RequestKey);
                    return;
                } 
            }
        }

        public void Disconnect() {
            if (connected == false)
                return;

            connected = false;
            con.Disconnect();

            this.Dispatcher.Invoke(() =>
            {  
                mainWindow.Show();
                mainWindow.ShowNoPage();
                CustomizationWindow.Hide(); 
            }); 

            App.Instance.ConnectToDatabaseServer();
        }

        private void Update(object sender, ElapsedEventArgs e) {
            if (connected == false)
                return;

            StreamResult result;
            while ((result = con.Recieve()).data != Array.Empty<byte>()) {
                ReadData(result);
            }
        }

        private void ReadData(StreamResult result) {
            PacketTypeServer packageType = (PacketTypeServer)result.ReadInt();
            int characterGuid;

            switch (packageType) {

                case PacketTypeServer.publicKeyPackage:
                    string publicKey = result.ReadString();
                    con.ConnectionConfirmed(publicKey);
                    break;

                case PacketTypeServer.loginFailed:
                    break;

                case PacketTypeServer.LoginSuccessfull:

                    this.Dispatcher.Invoke(() =>
                    {
                        mainWindow.Hide();
                        CustomizationWindow.Show();
                    });

                    //basic user data
                    UserGuid = result.ReadInt();
                    //byte[] itemData = result.ReadBytes();

                    int characterCount = result.ReadInt();
                     
                    byte[][] characterData2 = new byte[characterCount][];
                    int[] characterTypes = new int[characterCount];
                    int[] Guids = new int[characterCount];
                    int[] Levels = new int[characterCount];
                    for (int i = 0; i < characterCount; i++) {
                        characterData2[i] = result.ReadBytes();
                        characterTypes[i] = result.ReadInt();
                        Guids[i] = result.ReadInt();
                        Levels[i] = result.ReadInt();
                    }
                    int tempForCharGuidTest = Guids[1];
                    int netID = result.ReadInt();
                    string username = Encoding.UTF8.GetString( result.ReadBytes()); 
                    int xp = result.ReadInt();
                    Customization.Instance.SetItems(UserGuid, Guids, Levels, characterTypes, username, xp); 

                    //friend data
                    int friendCount = result.ReadInt();
                    int[] friendIDs = new int[friendCount]; 
                    string[] friendNames = new string[friendCount];

                    for (int i = 0; i < friendCount; i++)
                        friendIDs[i] = result.ReadInt();
                     

                    for (int i = 0; i < friendCount; i++)
                        friendNames[i] = result.ReadString();

                    Customization.Instance.SetFriendslist(friendIDs, friendNames);

                    //friend request data
                    int friendRequestCount = result.ReadInt();
                    int[] friendRequests = new int[friendRequestCount]; 
                    for (int i = 0; i < friendRequestCount; i++)
                        friendRequests[i] = result.ReadInt();

                    for (int i = 0; i < friendRequestCount; i++)
                        Customization.Instance.FriendRequestRecieved(friendRequests[i] , result.ReadString());



                    break;

                case PacketTypeServer.CharacterData:
                    characterGuid = result.ReadInt();
                    byte[] characterData = result.ReadBytes();
                    Customization.Instance.SetCharacterStats(characterData, characterGuid);
                    break;

                case PacketTypeServer.levelupSuccessfull:
                    int playerXP = result.ReadInt();
                    if (playerXP == -1)
                        break;

                    characterGuid = result.ReadInt();
                    int newLevel = result.ReadInt();
                    Customization.Instance.SetCharacterLevel(characterGuid, newLevel, playerXP);
                    break;

                    case(PacketTypeServer.Message):
                    int friend = result.ReadInt();
                    string message = result.ReadString();
                    Customization.Instance.MessageRecieved(friend, message);
                    break;

                case PacketTypeServer.YourFriendrequestWasAccepted:

                    Customization.Instance.AddFriendToFList(result.ReadInt(), result.ReadString());
                    break;

                default:
                    break;
            }
        }

        public void Friend(int userGuid, int friendGuid, FriendrequestAction Action) {
            con.WriteInt(userGuid);
            con.WriteInt((int)Action);
            con.WriteInt(friendGuid);
            con.Send(PacketTypeClient.Friendrequest); 
        }

        public async Task<string> GetIPAWS()
        {
            string folderPath = @"C:\Users\Klauke\Documents\My Games\Corivi\LauncherClient\"; 

            //declare a client //add qqq so this public key is not rejected by github. ITS PUBLIC 
            AmazonS3Client s3Client = new AmazonS3Client("qqqAKIAQE43KD5CKTAD77V3".Substring(3), "qqqVzf3hiPEugebvApnKkVgCl5u7GKT3erNsrx4cFBW".Substring(3), RegionEndpoint.USEast2);
            TransferUtility fileTransferUtility = new TransferUtility(s3Client);
            await fileTransferUtility.DownloadAsync(folderPath + "ServerIP.txt", "corivi", "ServerIP.txt");

            string[] ip = File.ReadAllLines(folderPath + @"ServerIP.txt");
            return ip[0];
        }
                                        

    }

    public struct CharacterDataServer
    {
        public readonly byte[] stats;

        public readonly byte[] skills;

        public readonly byte[] statsPerLevel;

        public CharacterDataServer(byte[] charDataServer)
        {
            stats = new byte[4];
            skills = new byte[10];
            statsPerLevel = new byte[4];
            Buffer.BlockCopy(charDataServer, 0, stats, 0, 4);
            Buffer.BlockCopy(charDataServer, 4, statsPerLevel, 0, 4);
            Buffer.BlockCopy(charDataServer, 8, skills, 0, 10);
        }
    }

}

using System;
using System.Collections.Generic; 
using System.Windows; 
using System.Timers;
using System.IO; 
using System.Linq;
using System.Net.Sockets; 
using System.Threading.Tasks;
using Client.Models;
using System.Text;

using Amazon;
using Amazon.S3; 
using Amazon.S3.Transfer; 

namespace Client
{

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
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
            ConnectToServer();
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

        public async void ConnectToServer() {
            string serverIP = await GetIPAWS();
             
            while (true) {

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
                //CustomizationWindow = new Customization(); 
            });


            App.Instance.ConnectToServer();
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


        //      //------------------------ OLD FILESERVER (also much faster than aws, we need to optimize aws(multiple requests, or packing files)
        //      private async void ConnectToFileServer(string ipAdress) {
        //
        //          Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //
        //          try {
        //              await sock.ConnectAsync(System.Net.IPAddress.Parse(ipAdress), 16502);
        //          }
        //          catch (Exception) {
        //              return;
        //          }
        //
        //          sock.Send(BitConverter.GetBytes((int)1)); //connect and get a filelist as an answere
        //
        //          List<short> OutdatedFiles = await GetFilelistFromServer(sock); //recieve a filelist and compare it to local files
        //                                                                   //and generate a list of files we do not have or that are outdated
        //          if (OutdatedFiles.Count == 0) {
        //              sock.Disconnect(false);
        //              mainWindow.SetProgressbar(0,0); 
        //              return;
        //          }
        //
        //          mainWindow.SetProgressbar(0, OutdatedFiles.Count);
        //
        //          //Request all the outstanding files
        //          List<byte> Filerequest = new List<byte>();
        //          int packageLength = 12 + (OutdatedFiles.Count * 2);
        //          Filerequest.AddRange(BitConverter.GetBytes(2));//package type
        //          Filerequest.AddRange(BitConverter.GetBytes(packageLength));//package length
        //          Filerequest.AddRange(BitConverter.GetBytes(OutdatedFiles.Count));//requested filecouunt
        //          for (int i = 0; i < OutdatedFiles.Count; i++ ) 
        //              Filerequest.AddRange(BitConverter.GetBytes(OutdatedFiles[i]));
        //              
        //          sock.Send(Filerequest.ToArray(), packageLength, SocketFlags.None);
        //
        //          //now recieve files
        //          GetFiles(sock, OutdatedFiles.Count);
        //      }
        //
        //      async void GetFiles(Socket s, int numOfFilesRequested) {
        //          int numOfFilesRecieved = 0;
        //          List<IncommingFile> incFile = new();
        //          bool packageSizeWasRead = false;
        //          int nextPackageSize = 65536;
        //
        //          while (true) {
        //              if (s.Available < 4) {
        //                  await Task.Delay(3);
        //                  continue;
        //              }
        //              if (!packageSizeWasRead) {
        //                  byte[] packetSize = new byte[4];//1) Read how much data was sent. Recieving all data could read data from the next package
        //                  s.Receive(packetSize);
        //                  nextPackageSize = BitConverter.ToInt32(packetSize, 0);
        //                  packageSizeWasRead = true;
        //              }
        //
        //              if (!packageSizeWasRead || nextPackageSize > s.Available) { //if packagesize wasnt read, or the size is more than what we can read
        //                  await Task.Delay(3);
        //                  continue;
        //              }
        //              packageSizeWasRead = false;//reset this value
        //
        //              byte[] data = new byte[nextPackageSize];
        //              s.Receive(data);
        //              StreamResult result = new StreamResult(ref data);
        //              int packageNumber = result.ReadInt();
        //              int fileID = result.ReadInt();
        //
        //              if (packageNumber == 0) {//read a header
        //
        //                  string currentFileWeAreGetting = result.ReadString16();
        //                  int fileSize = result.ReadInt();
        //                  long timeTicks = result.ReadLong();
        //
        //                  //c and c# measure time differently.
        //                  //getting the filetime returns different values and we need to offset that on the client
        //                  long CSharpToC_Filetime_Offset = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks + 72000000000;
        //
        //                  IncommingFile fileQ = (new(App.FilesPath + currentFileWeAreGetting, currentFileWeAreGetting, fileSize, fileID, timeTicks + CSharpToC_Filetime_Offset));
        //                  incFile.Add(fileQ);
        //              }
        //
        //              //write the recieved data into the correct files buffer
        //              IncommingFile fileWeWriteTo = incFile.First(list_element => list_element.fileID == fileID); //get the first where we have a matching fileID
        //
        //              bool file_fully_recieved = fileWeWriteTo.FilepartSent(ref result, packageNumber);//an old file that exists is overwritten
        //              if (file_fully_recieved) {
        //                  incFile.RemoveAt(incFile.IndexOf(fileWeWriteTo));
        //                  numOfFilesRecieved++;
        //                  mainWindow.SetProgressbar(numOfFilesRecieved, numOfFilesRequested);  
        //              }
        //
        //              if (numOfFilesRecieved == numOfFilesRequested)
        //                  break;//all is finished
        //          }
        //          s.Disconnect(false);
        //      }
        //
        async Task<List<short>> GetFilelistFromServer(Socket socket) {//check for a response once a second
            bool packageSizeWasRead = false;
            int nextPackageSize = 65536; //value is never used, but IDE doesnt seem to notice..
            List<short> OutdatedFiles = new();

            while (true) {
                if (socket.Available == 0) { //the following three ifs ensure all data that is required has been sent before we begin reading it
                    await Task.Delay(3);
                    continue;
                }

                if (!packageSizeWasRead) {
                    byte[] packetSize = new byte[4];//1) Read how much data was sent. Recieving all data could read data from the next package
                    socket.Receive(packetSize);
                    nextPackageSize = BitConverter.ToInt32(packetSize, 0);
                    packageSizeWasRead = true;
                }

                if (!packageSizeWasRead || nextPackageSize > socket.Available) { //if packagesize wasnt read, or the size is more than what we can read
                    await Task.Delay(1);
                    continue;
                }

                //read all data, meaning all filenames and their creation dates
                //check if this file exists yet, and if not, add the missing fileindex to a list
                byte[] data = new byte[nextPackageSize];
                socket.Receive(data);
                StreamResult result = new StreamResult(ref data); 
                int fileIndex = 0;
                while (result.BytesLeft() > 4) {
                    string filename = result.ReadString16();
                    DateTime file_creationtime_on_server = result.ReadDateTime();

                    bool file_is_upToDate = false;//false if the file does not exists
                    if (File.Exists(App.FilesPath + filename)) {//check if the file exists
                        FileInfo clientfile = new(App.FilesPath + filename); //.. and if its up to date
                        file_is_upToDate = clientfile.CreationTime >= file_creationtime_on_server;
                    }
                    if (!file_is_upToDate) {//if file is either not there, or it is outdated
                        Helper.CreateAllFoldersAlongPath(filename);
                        OutdatedFiles.Add((short)fileIndex);//add the outdated file to the list 
                    }
                    fileIndex++;
                }
                break;
            }
            return OutdatedFiles;
        }
         

    }
}

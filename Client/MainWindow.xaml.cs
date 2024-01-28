using System;
using System.Windows;
using System.Windows.Controls;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;

using System.Threading.Tasks;
namespace Client {
    public partial class MainWindow : Window {

        public static MainWindow Instance; //WPF sucks and can not link things that inherit from window or page by refference. Only static ones work.... so static it is
        private readonly Page connectingPage;// MVVM is stupid
        private readonly Page MainMenu;
        private readonly Page CustomizationPage;

        public static string FilesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        public static string[] fpParts = { "\\My Games", "\\Corivi", "\\LauncherClient\\" };
        public static string[] fpParts2 = { "\\My Games", "\\Heroes3", "\\HoTA HD\\" };

        public Connection con;
        private bool connected = false;
        private long lastConnectionAttempt = 0; //if an attempt was made recently, dont connect. Connectiong while an attempt is ongoing causes crashes 

        private bool allFilesAreUpToDate = false;

        public MainWindow() {
            InitializeComponent();
            Instance = this;

            for (int i = 0; i < fpParts.Length; i++) {
                FilesPath += fpParts[i];
                Helper.EnsureFolderExists(FilesPath);
            }

            connectingPage = new ConnectingPage();
            MainMenu = new MainMenuPage();
            CustomizationPage = new Customization();

            DataContext = this;
            con = new();

            Timer updateLoop = new(100);//Create an Update() function used for checking incomming connections and data
            updateLoop.Elapsed += Update;
            updateLoop.Enabled = true;
            updateLoop.AutoReset = true;
            updateLoop.Start();

            mainFrame.NavigationService.Navigate(connectingPage);
            ConnectToServer("127.0.0.1");
            ConnectToFileServer("87.150.137.27");
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
                    int guid = result.ReadInt();
                    byte[] itemData = result.ReadBytes();
                    int netID = result.ReadInt();
                    Customization.Instance.SetGuid(guid, itemData);

                    byte[] loginFile = Helper.CombineBytes(Encoding.UTF8.GetBytes(con.savedPublicKey), BitConverter.GetBytes(guid), (con.clientToken), BitConverter.GetBytes(con.i32clientToken));
                    File.WriteAllBytes(MainWindow.FilesPath + "\\userlogin.dat", loginFile);
                    this.Dispatcher.Invoke(() =>
                    {
                        mainFrame.NavigationService.Navigate(CustomizationPage);
                        TextboxErrorMsg.Content = "";
                    });
                    break;

                case PacketTypeServer.CharacterData:
                    characterGuid = result.ReadInt();
                    byte[] characterData = result.ReadBytes();
                    Customization.Instance.SetCharacterStats(characterData, characterGuid);
                    break;

                case PacketTypeServer.levelupSuccessfull:
                    int successfullLogin = result.ReadInt();
                    if (successfullLogin != -1) {
                        characterGuid = result.ReadInt();
                        int newLevel = result.ReadInt();
                        Customization.Instance.SetCharacterLevel(characterGuid, newLevel);
                        Customization.Instance.SetXP(successfullLogin);
                    }
                    else {
                        //do something to notify user of failed request. maybe
                    }

                    break;

                default:
                    break;
            }
        }

        private async void ConnectToFileServer(string ipAdress) {

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try {
                sock.Connect(System.Net.IPAddress.Parse(ipAdress), 16502);
            }
            catch (Exception) {

                return;
            }
            sock.Send(BitConverter.GetBytes((int)1)); //connect

            List<string> OutdatedFiles = await WaitForResponse(sock); //recieve a filelist and compare it to local files
                                                                      //and generate a list of files we do not have or that are outdated
            if (OutdatedFiles.Count == 0) {
                sock.Disconnect(false);
                allFilesAreUpToDate = true;
                this.Dispatcher.Invoke(() =>
                {
                    ConnectingPage.Instance.Progress.Value = 100;
                    ConnectingPage.Instance.ProgressLabel.Content = "All files are up to date";
                });
                return;
            }

            this.Dispatcher.Invoke(() =>
            {
                ConnectingPage.Instance.ProgressLabel.Content = OutdatedFiles.Count.ToString() + " Files outstanding";
            });

            byte[] dataPackage = new byte[65536];
            byte[] bytes = BitConverter.GetBytes(2); // 2 is the package type meaning the package contains outdates files list
            Buffer.BlockCopy(bytes, 0, dataPackage, 0, 4); //write package type into first bytes 0-3
                                                           //write nothing into bytes 4-7 (space for package size)
            bytes = BitConverter.GetBytes(OutdatedFiles.Count);
            Buffer.BlockCopy(bytes, 0, dataPackage, 8, 4);//write filecount into bytes 8-11
            int bufferPosition = 12;

            for (int i = 0; i < OutdatedFiles.Count; i++) {
                bytes = BitConverter.GetBytes(OutdatedFiles[i].Length);
                Buffer.BlockCopy(bytes, 0, dataPackage, bufferPosition, 4); //write nameLen
                bufferPosition += 4;

                bytes = Encoding.UTF8.GetBytes(OutdatedFiles[i]);
                Buffer.BlockCopy(bytes, 0, dataPackage, bufferPosition, bytes.Length); //write name
                bufferPosition += bytes.Length;
            }

            bytes = BitConverter.GetBytes(bufferPosition);
            Buffer.BlockCopy(bytes, 0, dataPackage, 4, 4); //write package size into bytes 4-7 
            sock.Send(dataPackage, bufferPosition, SocketFlags.None);

            //now recieve files
            GetFiles(sock, OutdatedFiles.Count);
        }

        async void GetFiles(Socket s, int numOfFilesRequested) {
            int numOfFilesRecieved = 0;
            List<IncommingFile> incFile = new();
            bool packageSizeWasRead = false;
            int nextPackageSize = 65536;

            while (true) {
                if (s.Available < 4) {
                    await Task.Delay(3);
                    continue;
                }
                if (!packageSizeWasRead) {
                    byte[] packetSize = new byte[4];//1) Read how much data was sent. Recieving all data could read data from the next package
                    s.Receive(packetSize);
                    nextPackageSize = BitConverter.ToInt32(packetSize, 0);
                    packageSizeWasRead = true;
                }

                if (!packageSizeWasRead || nextPackageSize > s.Available) { //if packagesize wasnt read, or the size is more than what we can read
                    await Task.Delay(3);
                    continue;
                }
                packageSizeWasRead = false;//reset this value

                byte[] data = new byte[nextPackageSize];
                s.Receive(data);
                StreamResult result = new StreamResult(ref data);
                int packageNumber = result.ReadInt();
                int fileID = result.ReadInt();

                if (packageNumber == 0) {//read a header

                    string currentFileWeAreGetting = result.ReadString();
                    int fileSize = result.ReadInt();
                    long timeTicks = result.ReadLong();

                    //c and c# measure time differently.
                    //getting the filetime returns different values and we need to offset that on the client
                    long CSharpToC_Filetime_Offset = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks + 72000000000;

                    IncommingFile fileQ = (new(FilesPath + currentFileWeAreGetting, currentFileWeAreGetting, fileSize, fileID, timeTicks + CSharpToC_Filetime_Offset));
                    incFile.Add(fileQ);
                }

                //write the recieved data into the correct files buffer
                IncommingFile fileWeWriteTo = incFile.First(list_element => list_element.fileID == fileID); //get the first where we have a matching fileID

                bool file_fully_recieved = fileWeWriteTo.FilepartSent(ref result, packageNumber);//an old file that exists is overwritten
                if (file_fully_recieved) {
                    incFile.RemoveAt(incFile.IndexOf(fileWeWriteTo));
                    numOfFilesRecieved++;
                    this.Dispatcher.Invoke(() =>
                    {
                        ConnectingPage.Instance.Progress.Value = (int)100 * ((float)numOfFilesRecieved / (float)numOfFilesRequested);
                        ConnectingPage.Instance.ProgressLabel.Content = numOfFilesRecieved.ToString() + "/" + numOfFilesRequested.ToString() + " files recieved";
                    });

                }

                if (numOfFilesRecieved == numOfFilesRequested)
                    break;//all is finished
            }
            s.Disconnect(false);
        }

        async Task<List<string>> WaitForResponse(Socket s) {//check for a response once a second
            List<string> OutdatedFiles = new();
            bool packageSizeWasRead = false;
            int nextPackageSize = 65536;
            while (true) {
                if (s.Available < 4) { //the following three ifs check if all data that is required has been sent.
                    await Task.Delay(3);
                    continue;
                }
                if (!packageSizeWasRead) {
                    byte[] packetSize = new byte[4];//1) Read how much data was sent. Recieving all data could read data from the next package
                    s.Receive(packetSize);
                    nextPackageSize = BitConverter.ToInt32(packetSize, 0);
                    packageSizeWasRead = true;
                }

                if (!packageSizeWasRead || nextPackageSize > s.Available) { //if packagesize wasnt read, or the size is more than what we can read
                    await Task.Delay(3);
                    continue;
                }
                //read all data
                byte[] data = new byte[nextPackageSize];
                s.Receive(data);
                StreamResult result = new StreamResult(ref data);

                while (result.BytesLeft() > 4) {
                    string filename = result.ReadString();
                    DateTime file_creationtime_on_server = result.ReadDateTime();

                    bool file_is_upToDate = false;
                    if (File.Exists(FilesPath + filename)) {//check if the file exists
                        FileInfo clientfile = new(FilesPath + filename); //.. and if its up to date
                        file_is_upToDate = clientfile.CreationTime >= file_creationtime_on_server;
                    }
                    if (file_is_upToDate) {
                        continue;
                    }

                    Helper.CreateAllFoldersAlongPath(filename);
                    OutdatedFiles.Add(filename);//add the outdated file to the list  
                }
                break;
            }
            return OutdatedFiles;
        }

        private void Update(object sender, ElapsedEventArgs e) {
            if (connected == false)  
                return; 

            StreamResult result;
            while ((result = con.Recieve()).data != Array.Empty<byte>()) {
                ReadData(result);
            }
        }

        private void ConnectToServer(string ipAdress) {
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() < lastConnectionAttempt + 10) {
                return;
            }

            lastConnectionAttempt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            if (con.TryToConnect(System.Net.IPAddress.Parse(ipAdress))) {//if connection was successfull
                connected = true;
                TextboxErrorMsg.Content = "";
                if (con.i32clientToken == 69420) {
                    mainFrame.Navigate(CustomizationPage);
                }
                else {
                    mainFrame.NavigationService.Navigate(MainMenu);
                }
            }
        }



        public void SaveCharacterStats(int guid, int charIndex, byte[] baseStats, byte[] statIncrease, byte[] skills, byte statspointsFullyAllocatedd) {
            byte[] dataToSend = new byte[baseStats.Length + statIncrease.Length + skills.Length + 1];
            Buffer.BlockCopy(baseStats, 0, dataToSend, 0, baseStats.Length);
            Buffer.BlockCopy(statIncrease, 0, dataToSend, baseStats.Length, statIncrease.Length);
            Buffer.BlockCopy(skills, 0, dataToSend, baseStats.Length + statIncrease.Length, skills.Length);
            dataToSend[18] = statspointsFullyAllocatedd;
            con.WriteInt(guid);
            con.WriteInt(0);
            con.WriteInt(charIndex);
            con.WriteBytes(ref dataToSend);
            con.Send(PacketTypeClient.requestWithToken);
        }

        public void LevelupCharacter(int guid, int charIndex, int requestedLevel) {
            ;
            con.WriteInt(guid);
            con.WriteInt(2);//requestType
            con.WriteInt(charIndex);
            con.WriteInt(requestedLevel); //always has to be one level above the current.
                                          //sent redundantly to prevent accidentailly sending and computing the request twice
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

        public void Disconnect() {
            connected = false;
            this.Dispatcher.Invoke(() =>
            {
                mainFrame.NavigationService.Navigate(connectingPage);
            });
            con.Disconnect();
        }

        public void Connect_Click(string ipAdress) {
            if (connected == true) {
                return;
            }
            ConnectToServer(ipAdress);
        }
    }
}

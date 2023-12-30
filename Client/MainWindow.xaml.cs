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
        private string currentFileWeAreGetting = "";
        private readonly List<IncommingFile> incFile = new();

        public Connection con;   
        private bool connected = false;
        private long lastConnectionAttempt = 0; //if an attempt was made recently, dont connect. Connectiong while an attempt is ongoing causes crashes 

        List<OutdatedFile> current_file_update = new();

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
            ConnectToFileServer("127.0.0.1");
        }

        private async void ConnectToFileServer(string ipAdress) {

            Socket sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            sock.Connect(System.Net.IPAddress.Parse(ipAdress), 16502);
            sock.Send(BitConverter.GetBytes((int)1));
            List<OutdatedFile> OutdatedFiles = await WaitForResponse(sock);

            byte[] bytes;
            int bufferPosition = 0;
            byte[] buffer = new byte[65536];
            bytes = BitConverter.GetBytes(2); // 2 is the package type
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, 4);
            bufferPosition += 4;

            bytes = BitConverter.GetBytes(OutdatedFiles.Count);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, 4);
            bufferPosition += 4;
            for (int i = 0; i < OutdatedFiles.Count; i++) {
                bytes = BitConverter.GetBytes(OutdatedFiles[i].filename.Length);
                Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, 4);
                bufferPosition += 4;

                bytes = Encoding.UTF8.GetBytes(OutdatedFiles[i].filename);
                Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
                bufferPosition += bytes.Length; 
            }
            sock.Send(buffer, bufferPosition, SocketFlags.None);
            await Task.Delay(2000); 
            con.WriteInt(OutdatedFiles.Count);
            for (int i = 0; i < OutdatedFiles.Count; i++) {
                con.WriteString(OutdatedFiles[i].filename);
            }
            con.Send(PacketType.file);

            bool allFilesSent = await GetFiles(sock, OutdatedFiles.Count);
            sock.Disconnect(false);

            return;
        }

        async Task<bool> GetFiles(Socket s, int numOfFilesRequested) {
            int numOfFilesRecieved = 0;
            while (true) {
                if (s.Available == 0) {
                    await Task.Delay(1000);
                    continue;
                }

                StreamResult result = new StreamResult(ref s);

                int packageType = result.ReadInt();

                if(packageType == -1) {
                    break;
                }

                int packageNumber = result.ReadInt();
                int fileID = result.ReadInt();

                if (packageNumber == 0) {
                    currentFileWeAreGetting = result.ReadString();
                    int fileSize = result.ReadInt();
                    long timeTicks = result.ReadLong();

                    //c and c# measure time differently.
                    //getting the filetime returns different values and we need to offset that on both c# server and client
                    long CSharpToC_Filetime_Offset = new DateTime(1601, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks + 72000000000;

                    IncommingFile fileQ = (new(FilesPath + currentFileWeAreGetting, currentFileWeAreGetting, fileSize, fileID, timeTicks + CSharpToC_Filetime_Offset));
                    incFile.Add(fileQ);
                }

                IncommingFile fileWeWriteTo = incFile.First(list_element => list_element.fileID == fileID); //get the first where we have a matching fileID
                bool sending_finished = fileWeWriteTo.FilepartSent(ref result, packageNumber);//an old file that exists is overwritten
                if (sending_finished) {
                    incFile.RemoveAt(incFile.IndexOf(fileWeWriteTo));
                    numOfFilesRecieved++;
                }

                if (numOfFilesRecieved == numOfFilesRequested)
                    break;
            }
            return true;
        }

        async Task<List<OutdatedFile>> WaitForResponse(Socket s) {//check for a response once a second
            List<OutdatedFile> OutdatedFiles = new();
            while (true) {
                if (s.Available == 0) { 
                    await Task.Delay(1000);
                    continue;
                }

                StreamResult result = new StreamResult(ref s);
                int pType = result.ReadInt();

                int fileIndex = 0;
                while (result.BytesLeft() > 4) {
                    string filename = result.ReadString();
                    DateTime file_creationtime_on_server = result.ReadDateTime();

                    if (File.Exists(FilesPath + filename)) {//check if the file exists
                        FileInfo clientfile = new(FilesPath + filename); //.. and if its up to date
                        bool isUpToDate = clientfile.CreationTime >= file_creationtime_on_server;
                        if (isUpToDate) {
                            continue;//if it is, congrats, all is fine, otherwise add it to the Outdated files queue
                        }
                    }

                    //add the outdated file to the list
                    OutdatedFile of = new(filename);

                    //Check if every folder and subfolder along the filepath exists. create every missing one
                    string[] parts = filename.Split('\\');//Split the filename. Check Dir /Base/ then /Base/Part1/, then Base/Part1/Part2/.. etc.
                    string subPath = FilesPath;
                    for (int j = 0; j < parts.Length - 1; j++) {
                        subPath += "\\" + parts[j];
                        Helper.EnsureFolderExists(subPath);
                    }
                    of.file_index = fileIndex;
                    OutdatedFiles.Add(of);
                    fileIndex++;
                }

                break;

            }
            return OutdatedFiles;
        }

        private void Update(object sender, ElapsedEventArgs e) {
            if (connected == false) {
                return;
            }
            
            StreamResult result;
            while ((result = con.Recieve()).data != Array.Empty<byte>() ) { 
                ReadData(result);
            }   
        }

        private void ReadData(StreamResult result) {
            PacketType packageType = (PacketType)result.ReadInt();
            switch (packageType) { 

                case PacketType.Login:
                    int guid = result.ReadInt();
                    byte[] itemData = result.ReadBytes();
                    Customization.Instance.SetGuid(guid, itemData);

                    byte[] loginFile = Helper.CombineBytes(Encoding.UTF8.GetBytes(con.savedPublicKey), BitConverter.GetBytes(guid), (con.clientToken), BitConverter.GetBytes(con.i32clientToken));
                    File.WriteAllBytes(MainWindow.FilesPath + "\\userlogin.dat", loginFile);
                    this.Dispatcher.Invoke(() =>
                    {
                        mainFrame.NavigationService.Navigate(CustomizationPage);
                        TextboxErrorMsg.Content = "";
                    });
                    break;

                case PacketType.requestCharacterData:
                    int characterIndex = result.ReadInt();
                    byte[] characterData = result.ReadBytes();
                    Customization.Instance.SetCharacterStats(characterData, characterIndex);
                    break;

                case PacketType.publicKeyPackage:
                    string publicKey = result.ReadString(); 
                    con.ConnectionConfirmed(publicKey);  
                    con.Send(PacketType.UpdateFilesRequest);  
                    break; 

                case PacketType.loginFailed: 
                    break;

                default:
                    break;
            } 
        }
          
        private void ConnectToServer(string ipAdress) {
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() < lastConnectionAttempt + 10) {
                return;
            }

            lastConnectionAttempt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
              
            if (con.TryToConnect(System.Net.IPAddress.Parse(ipAdress))){//if connection was successfull
                connected = true;
                TextboxErrorMsg.Content = "";
                if(con.i32clientToken == 69420) {
                    mainFrame.Navigate(CustomizationPage);
                } else {
                    mainFrame.NavigationService.Navigate(MainMenu);
                }
            }  
        }

         

        public void SaveUserData(int guid, int charIndex, int[] baseStats, int[] statIncrease,  int[] skills) {
            byte[] dataToSend = new byte[baseStats.Length * 4 + statIncrease.Length * 4 + skills.Length * 4];
            Buffer.BlockCopy(baseStats, 0, dataToSend, 0, baseStats.Length*4);
            Buffer.BlockCopy(statIncrease, 0, dataToSend, baseStats.Length * 4, statIncrease.Length * 4);
            Buffer.BlockCopy(skills, 0, dataToSend, baseStats.Length * 4 + statIncrease.Length * 4, skills.Length * 4);
            con.WriteInt(guid);
            con.WriteInt(0);
            con.WriteInt(charIndex);
            con.WriteBytes(ref dataToSend);
            con.Send(PacketType.requestWithToken);
        }

        public void ReqCharData(int guid, int charIndex) { 
            con.WriteInt(guid);
            con.WriteInt(1); //request a chars data
            con.WriteInt(charIndex);
            con.Send(PacketType.requestWithToken);
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

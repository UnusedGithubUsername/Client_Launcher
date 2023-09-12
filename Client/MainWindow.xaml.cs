using System;
using System.Windows;
using System.Windows.Controls;
using System.Net.Sockets;
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Linq;

namespace Client {
    public partial class MainWindow : Window {
        public static MainWindow Instance; //WPF sucks and can not link things that inherit from window or page by refference. Only static ones work.... so static it is

        private string FilesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string[] fpParts = { "\\My Games", "\\Corivi", "\\LauncherClient\\" }; 

        private const int port = 16501; 

        long lastConnectionAttempt = 0; //if an attempt was made recently, dont connect. Connectiong while an attempt is ongoing causes crashes 
        bool connected = false;
        public ClientConnection con;

        Page connectingPage;// MVVM is stupid
        Page MainMenu;

        string currentFileWeAreGetting = "";
        List<IncommingFile> incFile = new();

        FileStream fs;
        byte[] fileBuffer = new byte[int.MaxValue/10];
        int bufferPos = 0;

        public MainWindow() {
            InitializeComponent();
            Instance = this;

            for (int i = 0; i < fpParts.Length; i++) {
                FilesPath += fpParts[i];
                ClientHelperClass.EnsureFolderExists(FilesPath);
            }

            connectingPage = new ConnectingPage();
            MainMenu = new MainMenuPage();

            DataContext = this;
            con = new();
            con.lastPackageTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            Timer updateLoop = new(100);//Create an Update() function used for checking incomming connections and data
            updateLoop.Elapsed += Update;
            updateLoop.Enabled = true;
            updateLoop.AutoReset = true;
            updateLoop.Start();

            mainFrame.NavigationService.Navigate(connectingPage);
            ConnectToServer("127.0.0.1");
        }

        private void Update(object sender, ElapsedEventArgs e) {
            if (connected == false) {
                return;
            }

            long currentTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

            while (con.client.Available > 0) {
                con.lastPackageTime = currentTime;
                ReadData(con.client);
            } //else if (con.lastPackageTime + 10 < currentTime) {
                //Disconnect();

             
        }

        private void ReadData(Socket _client) {
            StreamResult result = new(ref _client);
            PacketType packageType = (PacketType)result.ReadInt();
            switch (packageType) {
                case PacketType.keepAlive://due to rewrite this isno longer needed. Active connections are too much unneeded work for the server
                    break;
                case PacketType.login:
                    int guid = result.ReadInt();
                    MainMenuPage.Instance.SetGuid(guid);
                    this.Dispatcher.Invoke(() =>
                    {
                        TextboxErrorMsg.Content = "";
                    });
                    break;
                case PacketType.requestCharacterData:
                    break;
                case PacketType.saveCharacterData:
                    break;
                case PacketType.publicKeyPackage:
                    string publicKey = result.ReadString();
                    MainMenuPage.Instance.SetPKey(publicKey);
                    MainMenuPage.Instance.CheckForUpdate();
                    break;
                case PacketType.file:
                    int packageNumber = result.ReadInt();
                    int fileID = result.ReadInt();
                    string filePath = FilesPath; 

                    if (packageNumber == 0) {
                        currentFileWeAreGetting = result.ReadString();
                        int fileSize = result.ReadInt();
                        incFile.Add ( new(FilesPath+ currentFileWeAreGetting, currentFileWeAreGetting, fileSize, fileID));  
                    }
                     
                    IncommingFile fileWeWriteTo = incFile.First(s => s.fileID == fileID);
                    bool str = fileWeWriteTo.FilepartSent(ref result, packageNumber, fileID);
                    if (str) {
                        incFile.RemoveAt(incFile.IndexOf(fileWeWriteTo));
                    }
                     
                    
                    break;
                case PacketType.UpdateFilesRequest:
                     
                    //we recieve a list of files and their creation dates.
                    //compare them to what we aleady have. 
                    //fileNames we still need or need to update are added to the list
                    //We later call a loop where we request all those files one by one 
                    List<OutdatedFile> OutdatedFiles = new();

                    while (result.BytesLeft() > 4) {
                        string filename = result.ReadString();
                        DateTime dt = result.ReadDateTime();

                        bool fileIsUpToDate = false;
                        if (File.Exists(FilesPath + filename)) {
                            FileInfo fi = new(FilesPath + filename);
                            //fi.CreationTime = dt;
                            if (fi.CreationTime == dt) {
                                fileIsUpToDate = true;
                            }
                        }
                        if (!fileIsUpToDate) {
                            OutdatedFile of = new(filename, dt);
                            string[] parts = filename.Split( '\\');
                            string subPath = FilesPath;
                            for (int j = 0; j < parts.Length - 1; j++) {

                                subPath +=  "\\"+parts[j];


                                bool exists = Directory.Exists(subPath);

                                if (!exists)
                                    Directory.CreateDirectory(subPath);
                            }


                            OutdatedFiles.Add(of);
                        } 
                    }
                    MainMenuPage.Instance.ReqFiles(ref OutdatedFiles);

                    break;
                default:
                    break;
            }

        }



        private bool ConnectToServer(string ipAdress) {
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() < lastConnectionAttempt + 10) {
                return false;
            }

            lastConnectionAttempt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            System.Net.IPAddress ip = System.Net.IPAddress.Parse(ipAdress);

            try {
                con.client.Connect(ip, port);
                connected = true;
                TextboxErrorMsg.Content = "";
                mainFrame.NavigationService.Navigate(MainMenu);
                con.lastPackageTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
                return true;
            }
            catch (SocketException) {
                TextboxErrorMsg.Content = "Socket Exception: \n Could not find the Server\n Please enter IP and click connect";
            }
            catch (Exception e) {
                TextboxErrorMsg.Content = e.ToString();
            }
            return false;
        }

         

        public void SaveUserData() {

        }

        public void Disconnect() {
            connected = false;
            this.Dispatcher.Invoke(() =>
            {
                mainFrame.NavigationService.Navigate(connectingPage);
            });
            con.client.Close();
            con.client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect_Click(string ipAdress) {
            if (connected == true) {
                return;
            }
            ConnectToServer(ipAdress);
        }
    }
}

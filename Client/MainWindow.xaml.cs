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
        Page connectingPage;// MVVM is stupid
        Page MainMenu;

        private string FilesPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        private string[] fpParts = { "\\My Games", "\\Corivi", "\\LauncherClient\\" };
        string currentFileWeAreGetting = "";
        List<IncommingFile> incFile = new();

        public Connection conn; 
        public Socket client;  
        private const int port = 16501;
        bool connected = false;
        long lastConnectionAttempt = 0; //if an attempt was made recently, dont connect. Connectiong while an attempt is ongoing causes crashes 

        public MainWindow() {
            InitializeComponent();
            Instance = this;

            for (int i = 0; i < fpParts.Length; i++) {
                FilesPath += fpParts[i];
                Connection.EnsureFolderExists(FilesPath);
            }

            connectingPage = new ConnectingPage();
            MainMenu = new MainMenuPage();

            DataContext = this;

            Connection.CreateSocket();
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp); 

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

            while (client.Available > 0) { 
                ReadData(client);
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
                    conn = new(publicKey);  
                    conn.Send(ref client, PacketType.UpdateFilesRequest); 

                    break;
                case PacketType.file:
                    int packageNumber = result.ReadInt();
                    int fileID = result.ReadInt();
                    string filePath = FilesPath; 

                    if (packageNumber == 0) {
                        currentFileWeAreGetting = result.ReadString();
                        int fileSize = result.ReadInt();
                        long timeTicks = result.ReadLong();
                        incFile.Add ( new(FilesPath+ currentFileWeAreGetting, currentFileWeAreGetting, fileSize, fileID, timeTicks));  
                    }
                     
                    IncommingFile fileWeWriteTo = incFile.First(s => s.fileID == fileID);
                    bool str = fileWeWriteTo.FilepartSent(ref result, packageNumber);
                    if (str) {
                        incFile.RemoveAt(incFile.IndexOf(fileWeWriteTo));
                    }
                     
                    
                    break;
                case PacketType.UpdateFilesRequest:
                     
                    //we recieve a list of files and their creation dates.
                    //compare them to what we aleady have. 
                    //fileNames we still need or need to update are added to the list 
                    List<OutdatedFile> OutdatedFiles = new();

                    while (result.BytesLeft() > 4) {
                        string filename = result.ReadString();
                        DateTime dt = result.ReadDateTime();

                        bool fileIsUpToDate = false;
                        if (File.Exists(FilesPath + filename)) {//check if the file exists
                            FileInfo fi = new(FilesPath + filename); //.. and if its up to date
                            if (fi.CreationTime == dt) {
                                fileIsUpToDate = true;//if it is, congrats, all is fine, otherwise add it to the Outdated files queue
                            }
                        }
                        if (!fileIsUpToDate) {
                            OutdatedFile of = new(filename, dt);

                            //Check if every folder and subfolder along the filepath exists. create every missing one
                            string[] parts = filename.Split( '\\');//Split the filename. Check Dir /Base/ then /Base/Part1/, then Base/Part1/Part2/.. etc.
                            string subPath = FilesPath;
                            for (int j = 0; j < parts.Length - 1; j++) { 
                                subPath +=  "\\"+parts[j];
                                  
                                if (!Directory.Exists(subPath))
                                    Directory.CreateDirectory(subPath);
                            } 
                            OutdatedFiles.Add(of);
                        } 
                    } 

                    // Write every file thats missing to the stream and then send the packet
                    Connection.WriteInt(OutdatedFiles.Count);
                    for (int i = 0; i < OutdatedFiles.Count; i++) { 
                        Connection.WriteString(OutdatedFiles[i].filename);
                    }
                    conn.Send(ref client, PacketType.file);

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
                client.Connect(ip, port);
                connected = true;
                TextboxErrorMsg.Content = "";
                mainFrame.NavigationService.Navigate(MainMenu); 
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
            client.Close();
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void Connect_Click(string ipAdress) {
            if (connected == true) {
                return;
            }
            ConnectToServer(ipAdress);
        }
    }
}

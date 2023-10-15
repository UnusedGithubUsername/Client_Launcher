using System;
using System.Windows;
using System.Windows.Controls; 
using System.Timers;
using System.IO;
using System.Collections.Generic;
using System.Linq; 
using System.Text;  

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
                case PacketType.keepAlive://due to rewrite this isno longer needed. Active connections are too much unneeded work for the server
                    break;
                case PacketType.requestWithPassword:
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
                case PacketType.saveCharacterData:
                    break;
                case PacketType.publicKeyPackage:
                    string publicKey = result.ReadString(); 
                    con.ConnectionConfirmed(publicKey);  
                    con.Send(PacketType.UpdateFilesRequest); 

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
                                Helper.EnsureFolderExists(subPath);
                            } 
                            OutdatedFiles.Add(of);
                        } 
                    }

                    // Write every file thats missing to the stream and then send the packet
                    con.WriteInt(OutdatedFiles.Count);
                    for (int i = 0; i < OutdatedFiles.Count; i++) { 
                        con.WriteString(OutdatedFiles[i].filename);
                    }
                    con.Send(PacketType.file);

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

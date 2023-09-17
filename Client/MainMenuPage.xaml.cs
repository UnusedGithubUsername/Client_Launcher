using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

namespace Client {



    public partial class MainMenuPage : Page {
        //The client does not need a password to login after a connection was interrupted. Just his GUId and a token ( a randomly generated number from the client)
        //A token acts as a cookie. saves db calls and improves security/performance

        public int clientToken = 0;//the client chooses a token, so another client cant fake his login

 
        public static MainMenuPage Instance;
        public MainMenuPage() {

            InitializeComponent();
            Instance = this; 
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
             


            Socket con = MainWindow.Instance.con.client;
            MainWindow.Instance.conn.SendLoginPacket(ref con, clientToken, EmailTextfield.Text, PasswordTextfield.Text); 
        }

        public void ReqFileButton_Click(object sender, RoutedEventArgs e) { 
        }

        public void ReqFiles(ref List<OutdatedFile> files) {
            Socket netStream = MainWindow.Instance.con.client;
            int unencryptedFiledataLen = 4;//4 beause num of filenames in an int = 4 bytes
            for (int i = 0; i < files.Count; i++) {
                unencryptedFiledataLen += 4;//each string starts with stringlen
                unencryptedFiledataLen += files[i].filename.Length; //UTF8Encoding means 1 byte per char
            }

            byte[] filesDataUnencrypted = new byte[unencryptedFiledataLen];
            int filesDataPointer = 0;

            try {
               // Connection.WriteInt((int)PacketType.file);
                //Connection.WriteInt(13);

                byte[] bytes;
                bytes = BitConverter.GetBytes(files.Count); 
                Buffer.BlockCopy(bytes, 0, filesDataUnencrypted, filesDataPointer, bytes.Length);
                filesDataPointer += bytes.Length;
                 
                for (int i = 0; i < files.Count; i++) {
                    string filename = files[i].filename;

                    bytes = BitConverter.GetBytes(files[i].filename.Length);
                    Buffer.BlockCopy(bytes, 0, filesDataUnencrypted, filesDataPointer, bytes.Length);
                    filesDataPointer += bytes.Length;

                    bytes = Encoding.UTF8.GetBytes(files[i].filename);
                    Buffer.BlockCopy(bytes, 0, filesDataUnencrypted, filesDataPointer, bytes.Length);
                    filesDataPointer += bytes.Length;


                }
                Connection.WriteBytes(ref filesDataUnencrypted);
                MainWindow.Instance.conn.Send(ref netStream, PacketType.file);
            }
            catch (IOException) {
                MainWindow.Instance.Disconnect();
            }
        }

        public void CheckForUpdate() {

        }

        public void SetGuid(int Guid) {
            this.Dispatcher.Invoke(() =>
            {
                GuidTextfield.Content = Guid.ToString();
            });
        }
 

        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            MainWindow.Instance.DragMove();
        }

        private void Connect_Minimize(object sender, RoutedEventArgs e) {
            MainWindow.Instance.WindowState = WindowState.Minimized;
        }

        private void Connect_Quit(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }



        public void SetPKey2(string pkey) {

            this.Dispatcher.Invoke(() =>
            {
                TextfieldPublicKey.Content = pkey;

            });
        }
    }
}

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

        private int clientToken = 0;//the client chooses a token, so another client cant fake his login

        private RSACryptoServiceProvider rsa;//the server sends back a login token to verify that we knew email and password. (w/o sending it every time)

        public static MainMenuPage Instance;
        public MainMenuPage() {

            InitializeComponent();
            Instance = this;
            rsa = new RSACryptoServiceProvider();//encrypt the password with the public key we got from the server
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {

            string Email = EmailTextfield.Text;
            byte[] emailBytes = Encoding.UTF8.GetBytes(EmailTextfield.Text);

            Random rnd = new();
            clientToken = rnd.Next(1, Int32.MaxValue);

            //merge (salt) the password with the email so a leaked pw database can not be used with precomputed keyword tables
            byte[] pwBytes = Encoding.UTF8.GetBytes(PasswordTextfield.Text);
            byte[] saltedPwBytes = new byte[pwBytes.Length + emailBytes.Length];
            Buffer.BlockCopy(pwBytes, 0, saltedPwBytes, 0, pwBytes.Length);
            Buffer.BlockCopy(emailBytes, 0, saltedPwBytes, pwBytes.Length, emailBytes.Length);
            // then encrypt that using an irreversible hash function. scrypt was recommended in 2016 but GPUs can now work on it, so its about as good as sha256
            SHA256 shaObj = SHA256.Create();
            byte[] encryptedSaltedPW = shaObj.ComputeHash(saltedPwBytes);

            Socket netStream = MainWindow.Instance.con.client;
            try {
                ClientHelperClass.WriteInt((int)PacketType.login);
                ClientHelperClass.WriteInt(clientToken);
                ClientHelperClass.WriteString(ref Email);
                ClientHelperClass.WriteBytes(ref encryptedSaltedPW);
                ClientHelperClass.Send(ref netStream, ref rsa);
                // MainWindow.Instance.con.client.Client.Send(ref netStream);

            }
            catch (IOException) {
                MainWindow.Instance.Disconnect();
            }
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
                ClientHelperClass.WriteInt((int)PacketType.file);
                ClientHelperClass.WriteInt(clientToken);

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
                ClientHelperClass.Send(ref netStream, ref rsa, filesDataUnencrypted);
            }
            catch (IOException) {
                MainWindow.Instance.Disconnect();
            }
        }

        public void CheckForUpdate() {
            Socket netStream = MainWindow.Instance.con.client;
            try {
                ClientHelperClass.WriteInt((int)PacketType.UpdateFilesRequest);
                ClientHelperClass.WriteInt(clientToken);
                ClientHelperClass.Send(ref netStream, ref rsa);
            }
            catch (IOException) {
                MainWindow.Instance.Disconnect();
            }
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
            System.Windows.Application.Current.Shutdown();
        }

        public void SetPKey(string pkey) {
            rsa.FromXmlString(pkey);
        }
    }
}

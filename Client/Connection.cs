using System;
using System.Text; //for UTF8
using System.Net.Sockets;//for TCPClient
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace Client {

    public class Connection {

        private const int port = 16501;
        private readonly byte[] buffer = new byte[65404]; // close to max tcp package size of 65536 minus 132

        private Socket sock;
        private int bufferPosition;
        private readonly RSACryptoServiceProvider rsa; 
         
        public int savedGuid = 0;
        public string savedPublicKey = "";
        public byte[] clientToken = Array.Empty<byte>();
        public int i32clientToken = -1;

        public Connection() {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            rsa = new();
            bufferPosition = 0;

            string path = MainWindow.FilesPath + "\\userlogin.dat";
            //in case the login is still valid recover all the info from the file: 1) GUId, 2) Token. So the file has just pKey, guid and token
            if (File.Exists(path)) {
                byte[] content = File.ReadAllBytes(path);
                savedPublicKey = Encoding.UTF8.GetString(content, 0, content.Length - 136);
                rsa.FromXmlString(savedPublicKey); 
                savedGuid = BitConverter.ToInt32(content, content.Length - 136);
                clientToken = new byte[128];//  
                Buffer.BlockCopy(content, content.Length - 132, clientToken, 0, 128); 
                i32clientToken = BitConverter.ToInt32(content, content.Length - 4);

            }
        }

        public bool TryToConnect(System.Net.IPAddress ip) {
            try {
                sock.Connect(ip, port);
                return true;
            }
            catch (Exception) {
                sock.Connect(ip, 8080);

                return false;
            }
        }

        /// <summary> returns true if we already have a valid login  </summary>
        /// This is called before we send anything deliberately. That means the client token is always generated before we send anything
        public bool ConnectionConfirmed(string publicKey) { 

            if (publicKey == savedPublicKey)
                return true;

            //if the server restarted all logins are reset and the pKey aswell. so if pKey is different we need to relog. So just generate a token and wait.  
            savedPublicKey = publicKey;
            rsa.FromXmlString(publicKey);
            Random rnd = new();
            i32clientToken = rnd.Next(1, Int32.MaxValue);//this token is meaningless until the client makes requests.
                                                             //when a request is made, the server checks if the GUId has an active login session. the session has a token. The clients token needs to be the sessions token
            clientToken = rsa.Encrypt(BitConverter.GetBytes(i32clientToken), false);
            return false;
               
        }

        public void Send(PacketType pType) {
             
            byte[] dataPacket = new byte[bufferPosition];
            Buffer.BlockCopy(buffer, 0, dataPacket, 0, bufferPosition);
            dataPacket = Helper.CombineBytes(BitConverter.GetBytes((int)pType), clientToken, dataPacket);
            bufferPosition = 0;

            try {
                sock.Send(dataPacket);//wríte how many characters are in the string we send
            }
            catch (Exception) {

                MainWindow.Instance.Disconnect();
            }
        } 

        public StreamResult Recieve() {

            if (sock.Available == 0) {
                return new StreamResult();
            }

            return new StreamResult(ref sock);
        } 

        public void Disconnect() {
            sock.Close();
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public void SendLoginPacket(string eMail, string password) { 

            byte[] bEMail = Encoding.UTF8.GetBytes(eMail);
            //merge (salt) the password with the email so a leaked pw database can not be used with precomputed keyword tables
            byte[] pwBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPwBytes = new byte[pwBytes.Length + bEMail.Length];
            Buffer.BlockCopy(pwBytes, 0, saltedPwBytes, 0, pwBytes.Length);
            Buffer.BlockCopy(bEMail, 0, saltedPwBytes, pwBytes.Length, bEMail.Length);

            // then encrypt that using an irreversible hash function. scrypt was recommended in 2016 but GPUs can now work on it, so its about as good as sha256
            SHA256 shaObj = SHA256.Create(); 
            byte[] bPType = BitConverter.GetBytes((int)PacketType.requestWithPassword);
            byte[] bToken = BitConverter.GetBytes(i32clientToken);//this token will be the thing that verifies requests from this client
            byte[] bEMail_L = BitConverter.GetBytes(eMail.Length); 
            byte[] bHashedPW = shaObj.ComputeHash(saltedPwBytes);
            byte[] bHashedPW_L = BitConverter.GetBytes(bHashedPW.Length); 

            byte[] data = Helper.CombineBytes(bToken, bEMail_L, bEMail, bHashedPW_L, bHashedPW); 
            data = rsa.Encrypt(data, false);
            data = Helper.CombineBytes(bPType, data);//write the package type to the front of the dataPacket
             
            try {
                sock.Send(data);//wríte how many characters are in the string we send
            }
            catch (Exception) { 
                MainWindow.Instance.Disconnect();
            }
        }

        public void WriteString(string message) {
            WriteInt(message.Length);
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
            bufferPosition += bytes.Length; 
        }

        public void WriteBytes(ref byte[] bytes) {
            WriteInt(bytes.Length);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
            bufferPosition += bytes.Length; 
        }

        public void WriteInt(int message) {
            byte[] bytes = BitConverter.GetBytes(message);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
            bufferPosition += bytes.Length; 
        }

        public void WriteFloat(float message) {
            byte[] bytes = BitConverter.GetBytes(message);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
            bufferPosition += bytes.Length; 
        } 
    }
}

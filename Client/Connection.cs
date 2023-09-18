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
        private static int bufferPosition;
        private RSACryptoServiceProvider rsa; 
        private byte[] clientToken = Array.Empty<byte>();

        public Connection() {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            rsa = new();
            bufferPosition = 0;
        }

        public void CreateToken(string publicKey) {
            rsa.FromXmlString(publicKey);
             
            Random rnd = new();
            int i32clientToken = rnd.Next(1, Int32.MaxValue);
            clientToken = rsa.Encrypt( BitConverter.GetBytes(i32clientToken), false);
        }

        public static void EnsureFolderExists(string folderName) {
            if (!Directory.Exists(folderName)) {
                Directory.CreateDirectory(folderName);
            }
        } 

        public bool ConnectToServer(System.Net.IPAddress ip) {
            try {
                sock.Connect(ip, port);
                return true;
            }
            catch (Exception) {

                return false;
            }
        }

        public void Disconnect() {
            sock.Close();
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        public StreamResult Recieve() {

            if (sock.Available == 0) {
                return new StreamResult();
            }

            return new StreamResult(ref sock);
        }

        public void SendLoginPacket(int token, string eMail, string password) {

            byte[] bEMail = Encoding.UTF8.GetBytes(eMail);
            //merge (salt) the password with the email so a leaked pw database can not be used with precomputed keyword tables
            byte[] pwBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPwBytes = new byte[pwBytes.Length + bEMail.Length];
            Buffer.BlockCopy(pwBytes, 0, saltedPwBytes, 0, pwBytes.Length);
            Buffer.BlockCopy(bEMail, 0, saltedPwBytes, pwBytes.Length, bEMail.Length);
            // then encrypt that using an irreversible hash function. scrypt was recommended in 2016 but GPUs can now work on it, so its about as good as sha256
            SHA256 shaObj = SHA256.Create();
             
            byte[] bPType = BitConverter.GetBytes((int)PacketType.login);
            byte[] bToken = BitConverter.GetBytes(token);
            byte[] bEMail_L = BitConverter.GetBytes(eMail.Length);
            //byte[] bEMail
            byte[] bHashedPW = shaObj.ComputeHash(saltedPwBytes);
            byte[] bHashedPW_L = BitConverter.GetBytes(bHashedPW.Length); 
            byte[] data = CombineBytes(bToken, bEMail_L, bEMail, bHashedPW_L, bHashedPW); 
            data = rsa.Encrypt(data, false);
            data = CombineBytes(bPType, data);//write the package type to the front of the dataPacket
             
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

        public void Send(PacketType pType ) {
            int dataToSendLength = bufferPosition;
             
            byte[] dataToEncrypt = new byte[dataToSendLength];
            Buffer.BlockCopy(buffer, 0, dataToEncrypt, 0, bufferPosition); 
            dataToEncrypt = CombineBytes(BitConverter.GetBytes((int)pType), clientToken, dataToEncrypt);  
            bufferPosition = 0;

            try {
                sock.Send(dataToEncrypt);//wríte how many characters are in the string we send
            }
            catch (Exception) {

                MainWindow.Instance.Disconnect();
            } 
        }

        public static byte[] CombineBytes(byte[] first, byte[] second) {
            byte[] bytes = new byte[first.Length + second.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            return bytes;
        }

        public static byte[] CombineBytes(byte[] first, byte[] second, byte[] third) {
            byte[] bytes = new byte[first.Length + second.Length + third.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, bytes, first.Length + second.Length, third.Length);
            return bytes;
        }

        public static byte[] CombineBytes(byte[] first, byte[] second, byte[] third, byte[] fourth) {// I could reduce linecount, but c# would copy the arrays AGAIN. I could use ref though
            byte[] bytes = new byte[first.Length + second.Length + third.Length + fourth.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, bytes, first.Length + second.Length, third.Length);
            Buffer.BlockCopy(fourth, 0, bytes, first.Length + second.Length + third.Length, fourth.Length);
            return bytes;
        }

        public static byte[] CombineBytes(byte[] first, byte[] second, byte[] third, byte[] fourth, byte[] fifth) {
            byte[] bytes = new byte[first.Length + second.Length + third.Length + fourth.Length + fifth.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, bytes, first.Length + second.Length, third.Length);
            Buffer.BlockCopy(fourth, 0, bytes, first.Length + second.Length + third.Length, fourth.Length);
            Buffer.BlockCopy(fifth, 0, bytes, first.Length + second.Length + third.Length + fourth.Length, fifth.Length);
            return bytes;
        }

        public static byte[] CombineBytes(byte[] first, byte[] second, byte[] third, byte[] fourth, byte[] fifth, byte[] sixth) {
            byte[] bytes = new byte[first.Length + second.Length + third.Length + fourth.Length + fifth.Length + sixth.Length];
            Buffer.BlockCopy(first, 0, bytes, 0, first.Length);
            Buffer.BlockCopy(second, 0, bytes, first.Length, second.Length);
            Buffer.BlockCopy(third, 0, bytes, first.Length + second.Length, third.Length);
            Buffer.BlockCopy(fourth, 0, bytes, first.Length + second.Length + third.Length, fourth.Length);
            Buffer.BlockCopy(fifth, 0, bytes, first.Length + second.Length + third.Length + fourth.Length, fifth.Length);
            Buffer.BlockCopy(sixth, 0, bytes, first.Length + second.Length + third.Length + fourth.Length + fifth.Length, sixth.Length);
            return bytes;
        }

    }
}

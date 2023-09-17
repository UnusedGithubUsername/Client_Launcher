using System;
using System.Text; //for UTF8
using System.Net.Sockets;//for TCPClient
using System.Security.Cryptography;
using System.IO;
using System.Linq;

namespace Client {

    public class Connection {
        
        private static readonly byte[] buffer = new byte[65408]; // close to max tcp package size minus 128
        private static int bufferPosition = 0;
        private RSACryptoServiceProvider rsaa;

        private byte[] clientToken;

        public Connection(string publicKey) {
            rsaa = new();
            rsaa.FromXmlString(publicKey);



            Random rnd = new();
            int i32clientToken = rnd.Next(1, Int32.MaxValue);
            clientToken = rsaa.Encrypt( BitConverter.GetBytes(i32clientToken), false);
        }

        public static void EnsureFolderExists(string folderName) {
            if (!Directory.Exists(folderName)) {
                Directory.CreateDirectory(folderName);
            }
        } 

        public void SendLoginPacket(ref Socket con, int token, string eMail, string password) {

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
            data = rsaa.Encrypt(data, false);
            data = CombineBytes(bPType, data);


            try {
                con.Send(data);//wríte how many characters are in the string we send
            }
            catch (Exception) {

                MainWindow.Instance.Disconnect();
            }
        }

        public static void WriteString(ref string message) {
            WriteInt(message.Length);
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
            bufferPosition += bytes.Length; 
        }

        public static void WriteBytes(ref byte[] bytes) {
           // WriteInt(bytes.Length);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
            bufferPosition += bytes.Length; 
        }

        public static void WriteInt(int message) {
            byte[] bytes = BitConverter.GetBytes(message);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
            bufferPosition += bytes.Length; 
        }

        public static void WriteFloat(float message) {
            byte[] bytes = BitConverter.GetBytes(message);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
            bufferPosition += bytes.Length; 
        }

        public void Send(ref Socket netStream, PacketType pType ) {
            int dataToSendLength = bufferPosition;
             
            byte[] dataToEncrypt = new byte[dataToSendLength];
            Buffer.BlockCopy(buffer, 0, dataToEncrypt, 0, bufferPosition);
           // dataToEncrypt = rsaa.Encrypt(dataToEncrypt, false);
            dataToEncrypt = CombineBytes(BitConverter.GetBytes((int)pType), clientToken, dataToEncrypt);
          // if (unencryptedData != null) {
          //     dataToSendLength = 132;
          //     dataToSendLength += unencryptedData == null ? 0 : unencryptedData.Length; //if we have additional data, we send additional data
          //     byte[] dataToSend = new byte[dataToSendLength];
          //
          //     Buffer.BlockCopy(dataToEncrypt, 0, dataToSend, 0, 132);
          //     Buffer.BlockCopy(unencryptedData, 0, dataToSend, 132, unencryptedData.Length);
          //
          //     dataToEncrypt = dataToSend; 
          // }

            bufferPosition = 0;

            try {
                netStream.Send(dataToEncrypt);//wríte how many characters are in the string we send
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

        public static byte[] CombineBytes(byte[] first, byte[] second, byte[] third, byte[] fourth) {
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

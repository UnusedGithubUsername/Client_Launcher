using System;
using System.Text; //for UTF8
using System.Net.Sockets;//for TCPClient
using System.Security.Cryptography;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Markup;
namespace Client {

    public class Connection {

        private const int port = 16501;
        private readonly byte[] buffer = new byte[65404]; // close to max tcp package size of 65536 minus 132

        private Socket sock;
        private int bufferPosition;
        private readonly RSACryptoServiceProvider rsa; 
         
        public int savedGuid = 0; 
        public byte[] clientToken = Array.Empty<byte>();
        public int i32clientToken = -1;
        public byte[] loginPackage;

        public Connection() {
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            rsa = new();
            bufferPosition = 0;
        }

        

        public async Task<bool> TryToConnect(System.Net.IPAddress ip) { 
            try { 
                await sock.ConnectAsync(ip, port);
                return true;
            }
            catch (Exception) {  
                return false;
            }
        }

        /// <summary> returns true if we already have a valid login  </summary>
        /// This is called before we send anything deliberately. That means the client token is always generated before we send anything
        public bool ConnectionConfirmed(string publicKey) {  
            //if the server restarted all logins are reset and the pKey aswell. so if pKey is different we need to relog. So just generate a token and wait.  
             
            rsa.FromXmlString(publicKey);
            Random rnd = new();
            i32clientToken = rnd.Next(1, Int32.MaxValue);//this token is meaningless until the client makes requests.
                                                             //when a request is made, the server checks if the GUId has an active login session. the session has a token. The clients token needs to be the sessions token
            clientToken = rsa.Encrypt(BitConverter.GetBytes(i32clientToken), false);
            return false; 
        }

        public void Send(PacketTypeClient pType) {
             
            byte[] dataPacket = new byte[bufferPosition];
            Buffer.BlockCopy(buffer, 0, dataPacket, 0, bufferPosition);
            dataPacket = Helper.CombineBytes(BitConverter.GetBytes((int)pType), clientToken, dataPacket);
            bufferPosition = 0;

            try {
                sock.Send(dataPacket);//wríte how many characters are in the string we send
            }
            catch (Exception) { 
                App.Instance.Disconnect();
            }
        } 

        public StreamResult Recieve() { 
            if (sock.Available == 0) 
                return new StreamResult();//if no data is there, return an empty result



            byte[] packetSize = new byte[4];//1) Read how much data was sent. Recieving all data could read data from the next package 
            sock.Receive(packetSize, SocketFlags.Peek);
            int dataSize = BitConverter.ToInt32(packetSize, 0); 
            if (sock.Available < dataSize)
                return new StreamResult(); //if data has not fully arrived, wait until it has fully arrived

            if (dataSize < 4 || dataSize > 65536)
            { //if a transmission error occured and useless data was sent

                byte[] dataToDelete = new byte[sock.Available];//read the useless data and throw it away
                sock.Receive(dataToDelete);
                return new StreamResult();
            }

            return new StreamResult(ref sock, dataSize + 4); //since we peeked we need to reread the first 4 bytes
        } 

        public void Disconnect() {
            sock.Close();
            sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
 

        public void SendLoginPacket(string eMail, string password, string username, bool registration = false) {
            PacketTypeClient ptype = registration ? PacketTypeClient.RegisterUser : PacketTypeClient.Login;
            byte[] bEMail = Encoding.UTF8.GetBytes(eMail);
            byte[] bUName = Encoding.UTF8.GetBytes(username);
            //merge (salt) the password with the email so a leaked pw database can not be used with precomputed keyword tables
            byte[] pwBytes = Encoding.UTF8.GetBytes(password);
            byte[] saltedPwBytes = new byte[pwBytes.Length + bEMail.Length];
            Buffer.BlockCopy(pwBytes, 0, saltedPwBytes, 0, pwBytes.Length);
            Buffer.BlockCopy(bEMail, 0, saltedPwBytes, pwBytes.Length, bEMail.Length);
            // then encrypt that using an irreversible hash function. scrypt was recommended in 2016 but GPUs can now work on it, so its about as good as sha256
            SHA256 shaObj = SHA256.Create(); 
            byte[] bPType = BitConverter.GetBytes((int)ptype);//int
            byte[] bToken = BitConverter.GetBytes(i32clientToken);//int this token will be the thing that verifies requests from this client
            byte[] bEMail_L = BitConverter.GetBytes(bEMail.Length);
            byte[] bUName_L = BitConverter.GetBytes(bUName.Length);
            byte[] bHashedPW = shaObj.ComputeHash(saltedPwBytes);
            string DebugPW = Encoding.UTF8.GetString(bHashedPW);

            byte[] bHashedPW_L = BitConverter.GetBytes(bHashedPW.Length); 
            byte[] bNetID = BitConverter.GetBytes(0); // a netid that is sent back on the gameserver

            loginPackage = Helper.CombineBytes(bToken, bEMail_L, bEMail, bUName_L, bUName);
            loginPackage = Helper.CombineBytes(loginPackage, bHashedPW_L, bHashedPW, bNetID);

            loginPackage = rsa.Encrypt(loginPackage, false);
            loginPackage = Helper.CombineBytes(bPType, loginPackage);//write the package type to the front of the dataPacket
             
            try {
                sock.Send(loginPackage);//wríte how many characters are in the string we send
            }
            catch (Exception) { 
                App.Instance.Disconnect();
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

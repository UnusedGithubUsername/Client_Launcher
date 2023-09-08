using System;
using System.Text; //for UTF8
using System.Net.Sockets;//for TCPClient
using System.Security.Cryptography;
using System.IO;

namespace Client {


    public struct ClientConnection {
        public Socket client;
        public bool connected = false;
        public long lastPackageTime = 0;

        public ClientConnection() {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }
    }

    public struct StreamResult {
        public byte[] data;
        public int dataIndex;

        public StreamResult(ref Socket client) { 

            byte[] ds = new byte[4];
            client.Receive(ds);
            int dataSize = BitConverter.ToInt32(ds, 0);

            data = new byte[dataSize];
            client.Receive(data);
            dataIndex = 0;
        }

        public int ReadInt() {
            int intRead = BitConverter.ToInt32(data, dataIndex);
            dataIndex += 4;
            return intRead;
        }

        public int BytesLeft() {
            return data.Length - dataIndex;
        }

        public string ReadString() {
            int strLen = ReadInt();
            byte[] stringstr = new byte[strLen];
            Buffer.BlockCopy(data, dataIndex, stringstr, 0, stringstr.Length);
            string str = Encoding.UTF8.GetString(stringstr);
            dataIndex += strLen;
            return str;
        }

        public DateTime ReadDateTime() {
            long ticks = BitConverter.ToInt64(data, dataIndex);
            dataIndex += 8;
            DateTime dt = new(ticks);
            return dt;
        }
    }

    public enum PacketType {
        keepAlive,
        login,
        requestCharacterData,
        saveCharacterData,
        publicKeyPackage,
        file,
        UpdateFilesRequest
    }

    public struct OutdatedFile {
        public string filename;
        public DateTime creationTime;
        public OutdatedFile(string _filename, DateTime _creationTime) {
            filename = _filename;
            creationTime = _creationTime;
        } 
    }

    public static class ClientHelperClass {

        public static void EnsureFolderExists(string folderName) {
            if (!Directory.Exists(folderName)) {
                Directory.CreateDirectory(folderName);
            }
        }

        private static readonly byte[] buffer = new byte[65408]; // close to max tcp package size minus 128
        private static int bufferPosition = 0;

        public static void WriteString(ref string message) {
            WriteInt(message.Length);
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            Buffer.BlockCopy(bytes, 0, buffer, bufferPosition, bytes.Length);
            bufferPosition += bytes.Length; 
        }

        public static void WriteBytes(ref byte[] bytes) {
            WriteInt(bytes.Length);
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

        public static void Send(ref Socket netStream, ref RSACryptoServiceProvider rsa, byte[]? unencryptedData = null) {
            int dataToSendLength = bufferPosition;

            byte[] dataToEncrypt = new byte[dataToSendLength];
            Buffer.BlockCopy(buffer, 0, dataToEncrypt, 0, bufferPosition);
            dataToEncrypt = rsa.Encrypt(dataToEncrypt, false);

            if (unencryptedData != null) {
                dataToSendLength = 128;
                dataToSendLength += unencryptedData == null ? 0 : unencryptedData.Length; //if we have additional data, we send additional data
                byte[] dataToSend2 = new byte[dataToSendLength];

                Buffer.BlockCopy(dataToEncrypt, 0, dataToSend2, 0, 128);
                Buffer.BlockCopy(unencryptedData, 0, dataToSend2, 128, unencryptedData.Length);

                dataToEncrypt = dataToSend2; 
            }

            bufferPosition = 0;

            try {
                netStream.Send(dataToEncrypt);//wríte how many characters are in the string we send
            }
            catch (Exception) {

                MainWindow.Instance.Disconnect();
            } 
        }
    }
}

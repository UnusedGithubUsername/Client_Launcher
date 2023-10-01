using System;
using System.Text; //for UTF8
using System.Net.Sockets;//for TCPClient
using System.Security.Cryptography;
using System.IO;

namespace Client {

    public static class Helper{
         
        public static void EnsureFolderExists(string folderName) {
            if (!Directory.Exists(folderName)) {
                Directory.CreateDirectory(folderName);
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
         
    }

    public struct IncommingFile {
        public const int MAX_FILE_BYTES = 65524;
          
        public string name;
        public int headerOffset;
        public byte[] data;
        public bool[] packetRecieved;
        public int fileID;
        public long CreationTime;

        public IncommingFile(string filePath, string filename, int dataSize, int fileIdentifier, long creationT) {

            CreationTime = creationT;
            headerOffset = 4 + 4 + filename.Length + 8; //int for nameLen, int for filesize, name in utf8 is 1 byte per char 
            data = new byte[dataSize];
            name = filePath;

            fileID = fileIdentifier;
            if (File.Exists(filePath)) {
                File.Delete(filePath);// Delete the outdated file if it exists
            }

            //calc how many packets we need 
            int normalPackageSize = headerOffset + dataSize; // All Packets have the same size (except the last). But the data size varies. 
            int numOfPacketsRequired = (int)(normalPackageSize / MAX_FILE_BYTES);
            if (normalPackageSize % MAX_FILE_BYTES != 0) {
                numOfPacketsRequired++; // casting to an int cuts off the last package. If we have a partial package we still need to allocate memory for it
            }
            packetRecieved = new bool[numOfPacketsRequired];
        }

        public bool FilepartSent(ref StreamResult result, int packageNumber) {
            //every file we send has max bytes, first package has the name first, so subtract that because that wasnt data for the file
            int bufferPosition = packageNumber * MAX_FILE_BYTES - headerOffset;
            if (packageNumber == 0) {
                bufferPosition = 0;
            }

            //Save the data that was sent
            Buffer.BlockCopy(result.data, result.dataIndex, data, bufferPosition, result.BytesLeft());
            result.dataIndex += result.BytesLeft();//not needed. all was read 
            packetRecieved[packageNumber] = true;

            //Check if the file is complete
            bool AllWasSent = true;
            for (int i = 0; i < packetRecieved.Length; i++) {
                if (packetRecieved[i] == false) {
                    AllWasSent = false;
                }
            }

            if (AllWasSent) {
                File.WriteAllBytes(name, data);  //Create the new file and synchronize the creation time
                FileInfo fi = new(name);
                fi.CreationTime = new DateTime(CreationTime);
            }
            return AllWasSent;
        }
    }

    public struct StreamResult {
        public byte[] data;
        public int dataIndex;

        public StreamResult() {
            data = Array.Empty<byte>();
            dataIndex = 0;
        }

        public StreamResult(ref Socket client) {
              
            byte[] packetSize = new byte[4];//1) Read how much data was sent. Recieving all data could read data from the next package
            client.Receive(packetSize);
            int dataSize = BitConverter.ToInt32(packetSize, 0);

            data = new byte[dataSize];//recieve all the data that is expected from the package
            client.Receive(data);
            dataIndex = 0;
        }

        public int ReadInt() {
            int intRead = BitConverter.ToInt32(data, dataIndex);
            dataIndex += 4;
            return intRead;
        }

        public long ReadLong() {
            long intRead = BitConverter.ToInt64(data, dataIndex);
            dataIndex += 8;
            return intRead;
        }

        public byte[] ReadBytes() {
            int arrayLen = this.ReadInt();
            byte[] byteArray = new byte[arrayLen];
            Buffer.BlockCopy(data, dataIndex, byteArray, 0, arrayLen);
            dataIndex += arrayLen;
            return byteArray;
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
        requestCharacterData,
        saveCharacterData,
        publicKeyPackage,
        file,
        UpdateFilesRequest,
        requestWithToken,
        requestWithPassword


    }

    public struct OutdatedFile {
        public string filename;
        public DateTime creationTime;
        public OutdatedFile(string _filename, DateTime _creationTime) {
            filename = _filename;
            creationTime = _creationTime;
        }
    }

}

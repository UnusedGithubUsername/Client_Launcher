using System;
using System.Text; //for UTF8
using System.Net.Sockets;//for TCPClient
using System.Security.Cryptography;
using System.IO;

namespace Client { 
     

    public struct IncommingFile {
        public const int MAX_FILE_BYTES = 65524;

        public static int fCount = 0;

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
                FileInfo fi = new FileInfo(name);
                fi.CreationTime = new DateTime(CreationTime);
            }
            return AllWasSent;
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

        public long ReadLong() {
            long intRead = BitConverter.ToInt64(data, dataIndex);
            dataIndex += 8;
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

}

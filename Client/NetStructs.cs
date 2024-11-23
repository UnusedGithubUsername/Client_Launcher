using System;
using System.Text; //for UTF8
using System.Net.Sockets;//for TCPClient 
using System.IO;

namespace Client {

    public static class Helper {

        public static void EnsureFolderExists(string folderName) {
            if (!Directory.Exists(folderName)) {
                Directory.CreateDirectory(folderName);
            }
        }

        public static void CreateAllFoldersAlongPath(string filename) {
            //Check if every folder and subfolder along the filepath exists. create every missing one
            string[] parts = filename.Split('\\');//Split the filename. Check Dir /Base/ then /Base/Part1/, then Base/Part1/Part2/.. etc.
            string subPath = App.FilesPath;
            for (int j = 0; j < parts.Length - 1; j++) {
                subPath += "\\" + parts[j];
                Helper.EnsureFolderExists(subPath);
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
            headerOffset = 4 + 4 + filename.Length*2 + 8; //int for nameLen, int for filesize, name in utf8 is 1 byte per char 
            data = new byte[dataSize];
            name = filePath;

            fileID = fileIdentifier;
            if (File.Exists(filePath)) {
                File.Delete(filePath);// Delete the outdated file if it exists
            }

            //calc how many packets we need 
            int totalNetworkedBytes = headerOffset + dataSize; // All Packets have the same size (except the last). But the data size varies. 
            int numOfPacketsRequired = (int)(totalNetworkedBytes / MAX_FILE_BYTES);
            if (totalNetworkedBytes % MAX_FILE_BYTES != 0) {
                numOfPacketsRequired++; // casting to an int cuts off the last package. If we have a partial package we still need to allocate memory for it
            }
            packetRecieved = new bool[numOfPacketsRequired];
        }

 
    }

    public struct StreamResult {
        public byte[] data;
        public int dataIndex;

        public StreamResult() {
            data = Array.Empty<byte>();
            dataIndex = 0;
        } 

        public StreamResult(ref Socket client, int dataSize) { 
            //recive data normally 
            data = new byte[dataSize];//recieve all the data that is expected from the package. Client.Available is NOT NECESSARILY equal.
            client.Receive(data);//if the server sends packets rapidly, available will be higher.
             
            dataIndex = 4; //the first 4 bytes are just the total size minus 4. we can always just skip over that because we read that with socketflag peek
        } 

        public int ReadInt() {
            int intRead = BitConverter.ToInt32(data, dataIndex);
            dataIndex += 4;
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
         
    }

    public enum PacketTypeClient { 
        KeepAlive = 0,
        RequestKey = 1,
        requestWithToken,
        Login = 3,
        RegisterUser = 4,
        ForewardLoginPacket,
        Friendrequest,
        Message=9
    }

    public enum FriendrequestAction {
        RequestFriendship,
        Accept,
        Deny,
        Block,
        Remove
    }

    public enum PacketTypeServer {

        publicKeyPackage,
        LoginSuccessfull,
        loginFailed,
        CharacterData,
        levelupSuccessfull,
        Message=9,
        YourFriendrequestWasAccepted = 10
    }

    public enum SkillID {
        None = 0,

        Strength = 1,
        ExplosiveArrows = 10,


        Vitality = 2,
        PhysResist = 20,
        PhysImmun = 200,

        Intelligence = 3,
        ExplosiveSpells = 30,

        Wisdom = 4,
        SecondaryAttunement = 40,
        //OneWithEverything = 41,


        //testskill7 = 6,
        //testskill8 = 7,
        //testskill9 = 8,
        //Nothing = 9, //abilities with 90 to 99 have no prequisites
        NeuralFastpass = 90,

    }

    public struct SkillFile {
        public SkillID ID = SkillID.None;
        public string Name = "skill1";
        public string Description = "descriptionxxx";

        public SkillFile(SkillID id, string name, string skillDescription) {
            ID = id;
            Name = name;
            Description = skillDescription;
        }
         

        public SkillFile(string path) {
            byte[] bytes = File.ReadAllBytes(path);
            ID = (SkillID)BitConverter.ToInt16(bytes, 0);

            byte nameLen = bytes[2];
            char[] nameArray = new char[nameLen];
            Buffer.BlockCopy(bytes, 3, nameArray, 0, nameArray.Length * 2);
            Name = new string(nameArray); 

            byte descLen = bytes[nameLen * 2 + 3];
            char[] descArray = new char[descLen];
            Buffer.BlockCopy(bytes, (nameArray.Length * 2) + 4, descArray, 0, descArray.Length * 2);
            Description = new string(descArray);
        }
    }



}

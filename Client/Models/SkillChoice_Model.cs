using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.ComponentModel;
using System.IO;

namespace Client.Models { 

    public class SkillChoice_Model : INotifyPropertyChanged {
        public SkillID ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } 
        public string img { get ;  set ;  }
              
        
        public SkillChoice_Model(string path) {

            byte[] bytes = File.ReadAllBytes(path);
            ID = (SkillID)BitConverter.ToInt16(bytes, 0);

            byte nameLen = bytes[2];
            char[] nameArray = new char[nameLen];
            Buffer.BlockCopy(bytes, 3, nameArray, 0, nameArray.Length * 2);
            Name = new string(nameArray);

            byte descLen = bytes[nameLen * 2 + 3];
            char[] descArray = new char[descLen];
            Buffer.BlockCopy(bytes, (nameArray.Length * 2) + 4, descArray, 0, descArray.Length * 2);
            Description = BreakUpStringWithLinebreaks(new string(descArray));

            string skillName = Enum.GetName(typeof(SkillID), ID);
            img = "/Video/" + skillName + ".png";
        }

         
        public static string BreakUpStringWithLinebreaks(string text) {
            int chunkSize = 60; // Adjust chunk size as needed
            char[] charArray = text.ToCharArray();

            if (text.Length < chunkSize * 1.4f)
                return text;

            StringBuilder result = new StringBuilder();

            for (int i = 0; i < text.Length; i = i) { //add linebreaks every x characters. but dont just break up words in the middle
                int chunkLength = Math.Min(chunkSize, text.Length - i);//chunks cant be longer than textLength

                for (int j = Math.Min(12, chunkLength); j > 0; j--) {

                    if (text.Length - i < chunkSize * 1.3f)
                        break;//if the last next chunk would be tiny, just stop chunking up the string

                    int indexToCheckForBlank = i + chunkLength - j;
                    if (charArray[indexToCheckForBlank] == ' ') {
                        chunkLength = indexToCheckForBlank - i + 1;
                        break;
                    }
                }

                result.Append(text.Substring(i, chunkLength));

                i += chunkLength;

                // Add separator except for the last chunk
                if (i < text.Length)
                    result.Append('\n');
                else
                    break;
            }
            return result.ToString();
        }
         
        public event PropertyChangedEventHandler PropertyChanged; 
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        } 
    }
}

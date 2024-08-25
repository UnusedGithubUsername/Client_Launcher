using System; 
using System.ComponentModel; 
using System.Text; 

namespace Client.Models {
    public class CharacterStat_Model : INotifyPropertyChanged { 
        public string Name { get; set; } 
        public byte[] stats { get; set; } 
        public byte[] skills { get; set; }

        public byte[] statsPerLevel = new byte[4];


        public byte statpointsFullyAllocated = 0;
        public byte skillpointsFullyAllocated = 0;

        private int _level = -1;
        public int level { get { return _level; } 
            set {
                _level = value;

                RevertChanges();
            }
        }

        private void RevertChanges() {
            //Revert changes made if a levelup is done;
            AvailableSkillpoints = _level - skillpointsFullyAllocated;
            IncreasableStatpoints = 0;
            DecreasableStatpoints = level * 2 - statpointsFullyAllocated;

            for (int i = 0; i < 4; i++)
                stats[i] = ServersideData.stats[i];

            for (int i = 0; i < 10; i++)
                skills[i] = ServersideData.skills[i];

            for (int i = 0; i < 4; i++)
                statsPerLevel[i] = ServersideData.statsPerLevel[i];

            for (int i = 0; i < 4; i++)
                currentStatDelta[i] = 0;

            UpdateGUI();
        }

        public int AvailableSkillpoints { get {  return _AvailableSkillpoints;  } set { _AvailableSkillpoints = value; OnPropertyChanged(nameof(AvailableSkillpoints)); } }
        private int _AvailableSkillpoints = -1; 
        public int IncreasableStatpoints { get { return _AvailableStatpoints; } set { _AvailableStatpoints = value; OnPropertyChanged(nameof(_AvailableStatpoints)); } }
        private int _AvailableStatpoints = -1;

        public int DecreasableStatpoints { get; set; }

        private int[] currentStatDelta = new int[4];
        public int[] CurrentStatDelta { get { return currentStatDelta; } set { currentStatDelta = value; } }


        internal void SetSkill(int currentlyCustomizedSkill, byte iD) {
            skills[currentlyCustomizedSkill] = iD;

            int changes = 0;
            for (int i = 0; i < 10; i++)  
                if (ServersideData.skills[i] != skills[i])
                    changes++;
             

            AvailableSkillpoints = level - changes - skillpointsFullyAllocated; 
        }
        public CharacterDataServer ServersideData;
        public void SetAllStats(byte[] charDataServer, CharacterRune_Model BaseData) {
            statpointsFullyAllocated = charDataServer[18];
            skillpointsFullyAllocated = charDataServer[19];
            Name = Encoding.UTF8.GetString(charDataServer, 21, charDataServer[20]);
            ServersideData = new(charDataServer); 
            level = BaseData.level; 
        }

        public CharacterStat_Model() {
            skills = new byte[10];
            stats = new byte[4];
            Name = "Empty"; 
        }

        public byte[] ToByte() {
            byte[] nameArray = Encoding.UTF8.GetBytes(Name);

            byte[] dataAsByte = new byte[21 + nameArray.Length];

            Buffer.BlockCopy(stats,     0,      dataAsByte, 0, 4);
            Buffer.BlockCopy(statsPerLevel, 0,  dataAsByte, 4, 4);
            Buffer.BlockCopy(skills,        0,  dataAsByte, 8, 10);
            dataAsByte[18] = statpointsFullyAllocated;
            dataAsByte[19] = skillpointsFullyAllocated;
            dataAsByte[20] = (byte)nameArray.Length;

            Buffer.BlockCopy(nameArray, 0, dataAsByte, 21, nameArray.Length);
            return dataAsByte; 
        }
         
        public string[] skillsImg {
            get {
                string[] imgPath = new string[skills.Length];
                for (int i = 0; i < imgPath.Length; i++) {
                    int libraryIndex = -1;
                    for (int j = 0; j < Customization.Instance.skillInfoLibrary.Count; j++) {
                        if (Customization.Instance.skillInfoLibrary[j].ID == (SkillID)skills[i])
                            libraryIndex = j;
                    }
                    imgPath[i] = Customization.Instance.skillInfoLibrary[libraryIndex].img;
                }

                return imgPath;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        internal void Increment(int index) {
            if(currentStatDelta[index] < 0) //we only gain decreasor points if the stat was decreased first, so the delta stats is negative
                DecreasableStatpoints++;

            IncreasableStatpoints--;
            currentStatDelta[index]++;
            stats[index]++;
            UpdateGUI();
        }

        internal void Decrement(int index) {
            if (currentStatDelta[index] <= 0) //we only use decreasor points if we acturally decrease the stat below its normal. that means the delta stat is 0 or lower
                DecreasableStatpoints--;

            IncreasableStatpoints++;
            currentStatDelta[index]--;
            stats[index]--;
            UpdateGUI();
        }

        public void UpdateGUI() {
            OnPropertyChanged(nameof(level)); 
            OnPropertyChanged(nameof(Name));  
            OnPropertyChanged(nameof(stats));
            OnPropertyChanged(nameof(skillsImg));
            OnPropertyChanged(nameof(IncreasableStatpoints));
            OnPropertyChanged(nameof(DecreasableStatpoints));
            OnPropertyChanged(nameof(DecreasableStatpoints));

        }


    }

    public struct CharacterDataServer {
        public readonly byte[] stats;
        public readonly byte[] skills;
        public readonly byte[] statsPerLevel;

        public CharacterDataServer(byte[] charDataServer) {
            stats = new byte[4];
            skills = new byte[10];
            statsPerLevel = new byte[4];
            Buffer.BlockCopy(charDataServer, 0, stats, 0, 4);
            Buffer.BlockCopy(charDataServer, 4, statsPerLevel, 0, 4);
            Buffer.BlockCopy(charDataServer, 8, skills, 0, 10);
        }
    }
}

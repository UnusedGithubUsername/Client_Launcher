using System;
using System.ComponentModel;
using System.Text;
using Client;
using Client.Models;

public class CharacterStat_Model : INotifyPropertyChanged
{
    private byte[] statsPerLevel = new byte[4];

    private byte statpointsFullyAllocated = 0;

    private byte skillpointsFullyAllocated = 0;

    private int _level = -1;

    private int _AvailableAbilitypoints = 1;

    private int _AvailableStatpoints = 1;

    private int[] currentStatDelta = new int[4];

    private int itemID; 

    public CharacterDataServer ServersideData;

    public string Name { get; set; }

    public byte[] stats { get; set; }

    public byte[] skills { get; set; }

    public int level 
    { 
        get { return _level; } 
        set {  _level = value; } 
    }

    public int AvailableAbilitypoints
    {
        get { return _AvailableAbilitypoints; }
        set { _AvailableAbilitypoints = value;  OnPropertyChanged(nameof(AvailableAbilitypoints));  }
    }

    public int IncreasableStatpoints  
    {
        get { return _AvailableStatpoints; }
        set { _AvailableStatpoints = value;  }
    }

    private int _DecreasableStatpoints = 1; //somehow this determines visibility no matter how many times it is changed
    public int DecreasableStatpoints
    {
        get { return _DecreasableStatpoints; }
        set { _DecreasableStatpoints = value; }
    }
     

    public int[] CurrentStatDelta
    {
        get
        {
            return currentStatDelta;
        }
        set
        {
            currentStatDelta = value;
        }
    }

    public string[] skillsImg
    {
        get
        {
            string[] imgPath = new string[skills.Length];
            for (int i = 0; i < imgPath.Length; i++)
            {
                int libraryIndex = -1;
                for (int j = 0; j < CustomizationOptions.skillInfoLibrary.Count; j++) 
                    if (CustomizationOptions.skillInfoLibrary[j].ID == (SkillID)skills[i]) 
                        libraryIndex = j; 
                 
                imgPath[i] = CustomizationOptions.skillInfoLibrary[libraryIndex].img;
            }
            return imgPath;
        }
    }


    public void SetStatsToDefault()
    {
        AvailableAbilitypoints = _level - skillpointsFullyAllocated;
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

    internal void SetSkill(int currentlyCustomizedSkill, byte iD)
    {
        skills[currentlyCustomizedSkill] = iD;
        int changes = 0;
        for (int i = 0; i < 10; i++) 
            if (ServersideData.skills[i] != skills[i]) 
                changes++; 

        AvailableAbilitypoints = level - changes - skillpointsFullyAllocated;
    }

    public void SetAllStats(byte[] charDataServer, CharacterRune_Model BaseData)
    {
        ServersideData = new CharacterDataServer(charDataServer);

        statpointsFullyAllocated = charDataServer[18];
        skillpointsFullyAllocated = charDataServer[19];
        Name = Encoding.UTF8.GetString(charDataServer, 21, charDataServer[20]);
        level = BaseData.level;
        itemID = BaseData.ItemID; 

        SetStatsToDefault();
    }

    public CharacterStat_Model()
    {
        skills = new byte[10];
        stats = new byte[4];
        Name = "Empty";
    }

    public byte[] ToByte()
    {
        byte[] nameArray = Encoding.UTF8.GetBytes(Name);
        byte[] dataAsByte = new byte[21 + nameArray.Length];
        Buffer.BlockCopy(stats, 0, dataAsByte, 0, 4);
        Buffer.BlockCopy(statsPerLevel, 0, dataAsByte, 4, 4);
        Buffer.BlockCopy(skills, 0, dataAsByte, 8, 10);
        dataAsByte[18] = statpointsFullyAllocated;
        dataAsByte[19] = skillpointsFullyAllocated;
        dataAsByte[20] = (byte)nameArray.Length;
        Buffer.BlockCopy(nameArray, 0, dataAsByte, 21, nameArray.Length);
        return dataAsByte;
    }


    internal void Increment(int index)
    {
        if (currentStatDelta[index] < 0) 
            DecreasableStatpoints++;
        
        IncreasableStatpoints--;
        currentStatDelta[index]++;
        stats[index]++; 
    }

    internal void Decrement(int index)
    { 
        if (currentStatDelta[index] <= 0) 
            DecreasableStatpoints--;
         
        IncreasableStatpoints++;
        currentStatDelta[index]--;
        stats[index]--; 
    }

    public void UpdateGUI()
    {
        OnPropertyChanged(nameof(level));
        OnPropertyChanged(nameof(Name));
        OnPropertyChanged(nameof(stats));
        OnPropertyChanged(nameof(skillsImg));
        OnPropertyChanged(nameof(IncreasableStatpoints));
        OnPropertyChanged(nameof(DecreasableStatpoints));
    }

    public event PropertyChangedEventHandler PropertyChanged; 
    protected virtual void OnPropertyChanged(string propertyName)
    {
        this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

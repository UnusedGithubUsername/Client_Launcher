using System; 
using System.Windows.Media; 
using System.ComponentModel;

namespace Client.Models {
    public class CharacterRune_Model: INotifyPropertyChanged  {
         
         

        private int _level = 0;
        private float _opacity = 0.0f;
        public int ItemID;
        public SolidColorBrush Background { get; set; }
        public int ItemGUId;
        public int Index { get; set; }

        public int level {
            get { return _level; }
            set {
                _level = value;
                _opacity = 0.5f + ((float)_level / 20f);
                 
                OnPropertyChanged(nameof(Opacityy)); 
            }
        }
     

        public float Opacityy {
            get { return _opacity;   }
        }



        public CharacterRune_Model(int guid, int levels, int id,  int i) {

            ItemID = id;
            Background = PickColorBasedOnInt(ItemID);
            level = levels;
            ItemGUId = guid; 
            Index = i;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }


        public static SolidColorBrush PickColorBasedOnInt(int value) {
            switch (value) {
                case 1001:
                    return new SolidColorBrush(Colors.Red);
                case 1002:
                    return new SolidColorBrush(Colors.Blue);
                case 1003:
                    return new SolidColorBrush(Colors.Green);
                // Add more cases for values 4 through 10 as needed
                default:
                    return new SolidColorBrush(Colors.Teal); // Default color if value is out of range
            }
        }
    }


}
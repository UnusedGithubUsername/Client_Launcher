using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Client.Models   {
    public class Friend_Model : INotifyPropertyChanged {

        public string FriendName { get; set; }
        public int FriendId { get; set; }

        public bool NotYetAccepted { get; set; }

        public Friend_Model(string name, int guid, bool onlyRequest) { 
            FriendName = name;
            FriendId = guid;
            NotYetAccepted = onlyRequest; 
        } 

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

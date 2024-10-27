using System.Collections.ObjectModel;
using System.ComponentModel; 


namespace Client.Models   {
    public class Friend_Model : INotifyPropertyChanged {
        public Friend_Model(string name, int guid, bool onlyRequest)
        {
            FriendName = name;
            Guid = guid;
            NotYetAccepted = onlyRequest;
            Chatoutput = new();
            lastMessage = "";
        }

        public string FriendName { get; set; }
        public int Guid { get; set; }
        public bool NotYetAccepted { get; set; }  
        public string lastMessage { get; set; }
        public ObservableCollection<string> Chatoutput { get; set; }

        public void MessageRecieved(string message)
        {
            Chatoutput.Add(message);
            lastMessage = message;

            OnPropertyChanged(nameof(Chatoutput));
            OnPropertyChanged(nameof(lastMessage));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

using System.Collections.ObjectModel;
using System.ComponentModel; 


namespace Client.Models   {

    public class ChatMessage
    {
        public string Text { get; set; }
        public bool IsRightBound { get; set; }
    }
    public class Friend_Model : INotifyPropertyChanged {

        public string FriendName { get; set; }
        public int Guid { get; set; }
        public bool NotYetAccepted { get; set; }  
        public string lastMessage { get; set; }
        public ObservableCollection<ChatMessage> Chatoutput { get; set; }

        public Friend_Model(string name, int guid, bool onlyRequest)
        {
            FriendName = name;
            Guid = guid;
            NotYetAccepted = onlyRequest;
            Chatoutput = new();
            lastMessage = "";
        }

        public void MessageRecieved(string message)
        {
            ChatMessage msg = new();
            msg.Text = FriendName + ": "+ message;
            msg.IsRightBound = false;
            Chatoutput.Add(msg);
            lastMessage = message;
             
        }

        public void MessageSent(string message)
        {
            ChatMessage msg = new();
            msg.Text = message;
            msg.IsRightBound = true;
            Chatoutput.Add(msg);

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName) {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}

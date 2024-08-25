using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Collections.ObjectModel;
using Client.Models;
using System.ComponentModel;
using System.Collections.ObjectModel;
using Client.Models;
using System.ComponentModel;

using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Client.Models {
    public class FriendChat_Model {
         
        public FriendChat_Model(int guid) {
            Chatoutput = new();
             
            lastMessage = "";
            Guid = guid;
        }

        public ObservableCollection<string> Chatoutput { get; set; }
        public int Guid { get; set; }
        public string lastMessage { get; set; }

        

        public void MessageRecieved(string message) {
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

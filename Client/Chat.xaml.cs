using Client.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Client
{

    /// <summary>
    /// Interaction logic for Chat.xaml
    /// </summary>
    public partial class Chat : Page, INotifyPropertyChanged
    {

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public Friend_Model CurrentChat { get; set; }  

        public Chat()
        { 
            InitializeComponent();
        }

        private void ChatInputBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;

            e.Handled = true;
            CurrentChat.MessageSent(ChatInputBox.Text);
            App.Instance.SendMessage(5, CurrentChat.Guid, (ChatInputBox.Text));
            ChatInputBox.Text = null;   
        }

        public void LoadChat( Friend_Model friendChat)
        { 
            CurrentChat = friendChat; 
            OnPropertyChanged(nameof(CurrentChat));
        }

        private void RemoveFriend(object sender, EventArgs e)
        {
            MessageBox.Show("Removing friend");
        }
    }
}

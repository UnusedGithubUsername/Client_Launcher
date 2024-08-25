using System;
using System.Windows;
using System.Windows.Controls; 

namespace Client {
    public partial class LoginPage : Page {
        public MainWindow main;

        private void Page_Loaded(object sender, RoutedEventArgs e) {
            EmailTextfield.Focus(); // Set focus to the TextBox

        }
        public LoginPage() {
            InitializeComponent(); 
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) {
            App.Instance.con.SendLoginPacket(EmailTextfield.Text, PasswordTextfield.Password, string.Empty) ; 
        }

        private void SwapToRegistration(object sender, RoutedEventArgs e) {
            main.SwapPage();
        }

    }
}

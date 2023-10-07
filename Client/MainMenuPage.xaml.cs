using System;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Security.Cryptography;
using System.Net.Sockets;
using System.IO;
using System.Collections.Generic;

namespace Client {



    public partial class MainMenuPage : Page {
        //The client does not need a password to login after a connection was interrupted. Just his GUId and a token ( a randomly generated number from the client)
        //A token acts as a cookie. saves db calls and improves security/performance
          
        public static MainMenuPage Instance;
        public MainMenuPage() {

            InitializeComponent();
            Instance = this; 
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e) { 
            MainWindow.Instance.con.SendLoginPacket(EmailTextfield.Text, PasswordTextfield.Text); 
        }
   
        private void Grid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            MainWindow.Instance.DragMove();
        }

        private void Connect_Minimize(object sender, RoutedEventArgs e) {
            MainWindow.Instance.WindowState = WindowState.Minimized;
        }

        private void Connect_Quit(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }        
        
        private void ReqFileButton_Click(object sender, RoutedEventArgs e) {
            Application.Current.Shutdown();
        }
         
        
    }
}

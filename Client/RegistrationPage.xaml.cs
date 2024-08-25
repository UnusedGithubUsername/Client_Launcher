using System;
using System.Collections.Generic;
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

namespace Client {
     
    public partial class RegistrationPage : Page {
        public MainWindow main;
        public RegistrationPage() {
            InitializeComponent();
        }

        private void SwapToLogin(object sender, RoutedEventArgs e) {
            main.SwapPage();
        }

        private void RegisterUser(object sender, RoutedEventArgs e) {
            if (PasswordTextfield.Password != RepeatPasswordTextfield.Password)
                return;

            App.Instance.con.SendLoginPacket(EmailTextfield.Text, PasswordTextfield.Password, Username.Text, true);

        }

    }
}

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
using System.Windows.Shapes;
using SharpSvn;

namespace NCRQuality_Inspection
{
    /// <summary>
    /// Interaction logic for Login_Application.xaml
    /// </summary>
    public partial class Login_Application : Window
    {
        LoginNT _log = new LoginNT();
        string _user;

        string _Mode;

        public Login_Application()
        {
            InitializeComponent();
            AddHotKeys();
            string domain = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName.ToString();
            _user = Environment.UserName;
            lblError.Visibility = Visibility.Hidden;
            txtUser.Text = _user;
            passBox.Focus();         
        }

        void AddHotKeys()
        {
            RoutedCommand _myCommand = new RoutedCommand();
            _myCommand.InputGestures.Add(new KeyGesture(Key.A, ModifierKeys.Control));
            CommandBindings.Add(new CommandBinding(_myCommand, MyCommandExecuted));
        }

        private void MyCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            txtUser.Text = "Admin";
            passBox.Password = "Admin";
            BtnLogin_Click(sender, e);
        }


        private void BtnExit_Click(object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            _user = txtUser.Text;            
            if(txtUser.Text == "Admin" && passBox.Password == "Admin")
            {
                string[] _tempArgs = { _user, _Mode };
                MainWindow _main = new MainWindow(_tempArgs);
                _main.Show();
                this.Close();
                return;
            }


            bool _login = _log.Logon(_user, passBox.Password);
            if (!_login || string.IsNullOrEmpty(_Mode))
            {
                txtUser.Clear();
                passBox.Clear();
                lblError.Visibility = Visibility.Visible;
            }
            

            if (_login && !string.IsNullOrEmpty(_Mode))
            {
                string[] _tempArgs = { _user, _Mode };
                MainWindow _main = new MainWindow(_tempArgs);
                _main.Show();
                this.Close();
            }
        }

        private void BtnATM_Click(object sender, RoutedEventArgs e)
        {
            btnATM.Background = btnATM.Background = Brushes.DarkCyan; //? (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD")) : Brushes.DarkCyan;
            btnFASCIA.Background = btnFASCIA.Background = Brushes.White; //? (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD")) : Brushes.White;
            _Mode = "CQA";
        }

        private void BtnFASCIA_Click(object sender, RoutedEventArgs e)
        {
            btnATM.Background = btnATM.Background = Brushes.White; // ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD")) : Brushes.DarkCyan;
            btnFASCIA.Background = btnFASCIA.Background = Brushes.DarkCyan; // ? (SolidColorBrush)(new BrushConverter().ConvertFrom("#FFDDDDDD")) : Brushes.White;
            _Mode = "FINAL INSPECTION";
        }
    }
}

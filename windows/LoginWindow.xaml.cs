using OpSy_Cryptor.database;
using OpSy_Cryptor.model;
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

namespace OpSy_Cryptor
{
    /// <summary>
    /// Interaction logic for LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : Window
    {
        private RegisterWindow registerWindow;
        private User UserObject { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            registerWindow = new();
            registerWindow.ShowDialog();
            User userObject = (User)registerWindow.Tag;
            if (userObject is not null)
            {
                UserObject = userObject;
                usernameTextbox.Text = UserObject.Username;
                LoginButton_Click(this, null);
            };
        }

        private async void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string username = usernameTextbox.Text;
            string password = passwordTextbox.Password;

            if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
            {
                Mouse.OverrideCursor = Cursors.Wait;
                passwordTextbox.IsEnabled = false;
                usernameTextbox.IsEnabled = false;

                MongoDBConnect mongoDBConnect = new();

                try
                {
                    UserObject = await mongoDBConnect.LoginUserAsync(username, password);

                    if (registerWindow is not null)
                    {
                        registerWindow.Close();
                    }

                    Mouse.OverrideCursor = Cursors.Arrow;

                    MainWindow mainWindow = new(UserObject);
                    mainWindow.Show();

                    Close();
                }
                catch (Exception ex)
                {
                    Mouse.OverrideCursor = Cursors.Arrow;
                    passwordTextbox.IsEnabled = true;
                    usernameTextbox.IsEnabled = true;
                    MessageBox.Show(ex.Message);
                }

            }
        }
    }
}

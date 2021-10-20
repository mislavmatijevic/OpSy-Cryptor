using OpSy_Cryptor.database;
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
        private string UserObjectID { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            registerWindow = new();
            registerWindow.ShowDialog();
            string userObjectId = (string)registerWindow.Tag;
            if (!string.IsNullOrWhiteSpace(userObjectId))
            {
                UserObjectID = userObjectId;
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
                    await mongoDBConnect.LoginUserAsync(username, password);

                    if (registerWindow is not null)
                    {
                        registerWindow.Close();
                    }

                    Mouse.OverrideCursor = Cursors.Arrow;

                    MainWindow mainWindow = new(UserObjectID);
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
            else
            {
                MessageBox.Show("Unesite sve potrebne podatke!", "Neuspjeh", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

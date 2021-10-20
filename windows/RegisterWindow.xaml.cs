using MongoDB.Driver;
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
using System.Windows.Shapes;

namespace OpSy_Cryptor
{
    /// <summary>
    /// Interaction logic for RegisterWindow.xaml
    /// </summary>
    public partial class RegisterWindow : Window
    {
        public RegisterWindow()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(usernameTextBox.Text) || string.IsNullOrWhiteSpace(password1TextBox.Password) || string.IsNullOrWhiteSpace(password2TextBox.Password))
            {
                MessageBox.Show("Unesite sve potrebne podatke!", "Neuspjeh", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (password1TextBox.Password != password2TextBox.Password)
            {
                MessageBox.Show("Lozinke se ne poklapaju!", "Neuspjeh", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {

                Mouse.OverrideCursor = Cursors.Wait;
                usernameTextBox.IsEnabled = false;
                password1TextBox.IsEnabled = false;
                password2TextBox.IsEnabled = false;
                try
                {
                    MongoDBConnect dBConnect = new();
                    User user = await dBConnect.RegisterUserAsync(usernameTextBox.Text, password1TextBox.Password);
                    MessageBox.Show($"Registrirani ste pod identifikacijskim brojem:\n{user.Id}\nVaš privatni, javni i tajni ključ čekaju Vas u datoteci!");
                    Mouse.OverrideCursor = Cursors.Arrow;
                    Close();
                }
                catch (MongoWriteConcernException mwcex)
                {
                    if (mwcex.Code == 11000)
                    {
                        MessageBox.Show("Odaberite drugo korisničko ime", "Korisničko ime zauzeto", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                    else
                    {
                        MessageBox.Show(mwcex.Message, "Korisničko ime zauzeto", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Korisničko ime zauzeto", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                Mouse.OverrideCursor = Cursors.Arrow;
                usernameTextBox.IsEnabled = true;
                password1TextBox.IsEnabled = true;
                password2TextBox.IsEnabled = true;
            }
        }
    }
}

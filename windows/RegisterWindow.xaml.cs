using MongoDB.Driver;
using OpSy_Cryptor.common;
using OpSy_Cryptor.database;
using OpSy_Cryptor.model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        private readonly string pathToDirectory = AppDomain.CurrentDomain.BaseDirectory + "OpSy_Generirano";

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
                    Tag = user;
                    CompleteRegistration(user);
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

        private void CreateKeyFile(string fileName, string contents)
        {
            string fullPath = $"{pathToDirectory}/{fileName}";

            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            using (FileStream fs = File.Create(fullPath))
            {
                byte[] binaryContent = new UTF8Encoding(true).GetBytes(contents);
                fs.Write(binaryContent, 0, binaryContent.Length);
            }
        }

        private void CompleteRegistration(User user)
        {
            Mouse.OverrideCursor = Cursors.Arrow;

            Directory.CreateDirectory(pathToDirectory);
            CreateKeyFile("javni_kljuc.txt", Encryption.Object.PublicKeyString);
            CreateKeyFile("privatni_kljuc.txt", Encryption.Object.PrivateKeyString);
            CreateKeyFile("tajni_kljuc.txt", Encryption.Object.SecretKeyString);

            MessageBoxResult result = MessageBox.Show($"Korisnički podaci:\n" +
                $"ID: {user.Id}\n" +
                $"Korisničko ime: {user.Username}\n" +
                $"Javni ključ: {user.PubKey}\n\n" +
                $"Vaš privatni, javni i tajni ključ čekaju Vas u direktoriju \"tajno\"!\n" +
                $"Želite li pogledati nove datoteke?", "Registracija obavljena", MessageBoxButton.YesNo, MessageBoxImage.Information);
            if (result == MessageBoxResult.Yes)
            {
                Process ExplorerWindowProcess = new();
                ExplorerWindowProcess.StartInfo.FileName = "explorer.exe";
                ExplorerWindowProcess.StartInfo.Arguments = $"/select,\"{pathToDirectory}\\javni_kljuc.txt\"";
                ExplorerWindowProcess.Start();
            }
            Close();
        }
    }
}

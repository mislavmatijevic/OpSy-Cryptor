using Microsoft.Win32;
using OpSy_Cryptor.database;
using OpSy_Cryptor.model;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OpSy_Cryptor.windows
{
    public partial class ChooseContactDialog : Window
    {
        private List<User> foundUsers;
        private string chosenPublicKey;

        public ChooseContactDialog()
        {
            InitializeComponent();
            infoLabel.Content = "";
        }

        private async void UsersTextBox_KeyUpAsync(object sender, KeyEventArgs e)
        {
            if (usersTextBox.Text.Length >= 3)
            {
                Mouse.OverrideCursor = Cursors.Wait;

                infoLabel.Content = "";
                if (string.IsNullOrWhiteSpace(usersTextBox.Text))
                {
                    return;
                }
                usersTextBox.IsReadOnly = true;
                foundUsers = await MongoDBConnect.GetInstance().GetUsersFromDBAsync(usersTextBox.Text);
                usersTextBox.IsReadOnly = false;
                usersTextBox.Focus();


                if (foundUsers.Count > 1)
                {
                    infoLabel.Content = "Ponuđeni korisnici: ";
                    foreach (User user in foundUsers)
                    {
                        infoLabel.Content += $"{user.Username} ";
                    }
                }
                else if (foundUsers.Count == 1)
                {
                    Tag = foundUsers[0].PubKey;
                    usersTextBox.Text = foundUsers[0].Username;
                    infoLabel.Content = "Korisnik pronađen!";
                    infoLabel.Foreground = Brushes.LightGreen;
                    chosenPublicKey = foundUsers[0].PubKey;
                    e.Handled = true;
                    acceptButton.Focus();
                }
                else
                {
                    infoLabel.Content = "Nema pronađenih korisnika!";
                    infoLabel.Foreground = Brushes.Red;
                }

                Mouse.OverrideCursor = Cursors.Arrow;

            }
            else
            {
                chosenPublicKey = "";
                infoLabel.Content = "";
            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(chosenPublicKey))
            {
                MessageBox.Show("Unesite javni ključ ili odaberite primatelja!", "Problem pri dekripciji!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                Tag = chosenPublicKey;
                Close();
            }
        }

        private void LoadPublicKeyFileButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Odabir datoteke s javnim ključem druge strane";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(openFileDialog.FileName) && openFileDialog.CheckPathExists)
            {
                string path = openFileDialog.FileName;
                chosenPublicKey = File.ReadAllText(path);
            }
        }
    }
}

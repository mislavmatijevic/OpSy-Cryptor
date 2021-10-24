using OpSy_Cryptor.database;
using OpSy_Cryptor.model;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace OpSy_Cryptor.windows
{
    public partial class ChooseContactDialog : Window
    {
        public List<User> FoundUsers { get; set; }

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

                FoundUsers = await MongoDBConnect.GetInstance().GetUsersFromDBAsync(usersTextBox.Text);

                if (FoundUsers.Count > 1)
                {
                    infoLabel.Content = "Ponuđeni korisnici: ";
                    foreach (User user in FoundUsers)
                    {
                        infoLabel.Content += $"{user.Username} ";
                    }
                }
                else if (FoundUsers.Count == 1)
                {
                    Tag = FoundUsers[0].PubKey;
                    usersTextBox.Text = FoundUsers[0].Username;
                    infoLabel.Content = "Korisnik pronađen!";
                    infoLabel.Foreground = Brushes.LightGreen;
                }
                else
                {
                    infoLabel.Content = "Nema pronađenih korisnika!";
                    infoLabel.Foreground = Brushes.Red;
                }

                Mouse.OverrideCursor = Cursors.Arrow;

            }
        }

        private void AcceptButton_Click(object sender, RoutedEventArgs e)
        {
            if (Tag is null && string.IsNullOrWhiteSpace(publicKeyTextBox.Text))
            {
                MessageBox.Show("Unesite javni ključ ili odaberite primatelja!", "Problem pri dekripciji!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else if (Tag is null)
            {
                Tag = publicKeyTextBox.Text;
            }
            Close();
        }
    }
}

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

namespace OpSy_Cryptor.windows
{
    /// <summary>
    /// Interaction logic for chooseContactDialog.xaml
    /// </summary>
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

                MongoDBConnect mongoDBConnect = new();
                FoundUsers = await mongoDBConnect.GetUsersFromDBAsync(usersTextBox.Text);

                if (FoundUsers.Count == 1)
                {
                    Tag = FoundUsers[0].PubKey;
                    usersTextBox.Text = FoundUsers[0].Username;
                    infoLabel.Content = "Korisnik pronađen!";
                    infoLabel.Foreground = Brushes.LightGreen;
                }
                else if (FoundUsers.Count == 0)
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

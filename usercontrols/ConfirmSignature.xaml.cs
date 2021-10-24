using Microsoft.Win32;
using OpSy_Cryptor.common;
using OpSy_Cryptor.database;
using OpSy_Cryptor.model;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpSy_Cryptor.usercontrols
{
    public partial class ConfirmSignature : UserControl
    {

        private readonly SelectedFile selectedFile;

        public ConfirmSignature(SelectedFile _selectedFile)
        {
            selectedFile = _selectedFile;
            InitializeComponent();
            messageLabel.Content = "";
        }

        private enum MessageState
        {
            Error,
            Success,
            Info
        }

        private void DisplayMessage(string message, MessageState messageState)
        {
            switch (messageState)
            {
                case MessageState.Error:
                    messageLabel.Foreground = Brushes.Red;
                    break;
                case MessageState.Success:
                    messageLabel.Foreground = Brushes.LightGreen;
                    break;
                case MessageState.Info:
                    messageLabel.Foreground = Brushes.White;
                    break;
                default:
                    break;
            }
            messageLabel.Content = message;
        }

        private async void LoadSignatureButton_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(openFileDialog.FileName) && openFileDialog.CheckPathExists)
            {
                string path = openFileDialog.FileName;
                string contents = File.ReadAllText(path);
                try
                {
                    string userId = contents.Split('.')[0];
                    string signature = contents.Split('.')[1];
                    long timestamp = long.Parse(contents.Split('.')[2]);

                    DisplayMessage($"Provjera potpisa...", MessageState.Info);
                    User presumedSigner = await MongoDBConnect.GetInstance().GetUserByIdAsync(userId);

                    bool isVerified = EncryptionClass.VerifySignature(selectedFile.SHA256Hash, signature, presumedSigner.PubKey);

                    if (isVerified)
                    {
                        DisplayMessage($"Korisnik {presumedSigner.Username} digitalno je potpisao ovu datoteku {DateTime.FromFileTimeUtc(timestamp).Day}. {DateTime.FromFileTimeUtc(timestamp).Month}. {DateTime.FromFileTimeUtc(timestamp).Year}. u {DateTime.FromFileTimeUtc(timestamp).Hour}:{DateTime.FromFileTimeUtc(timestamp).Minute}!", MessageState.Success);
                    }
                    else
                    {
                        DisplayMessage($"Potpis je krivotvoren!", MessageState.Error);
                    }
                }
                catch
                {
                    DisplayMessage($"Potpis nije valjan.", MessageState.Error);
                }
            }
        }
    }
}

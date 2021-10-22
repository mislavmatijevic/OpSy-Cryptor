using OpSy_Cryptor.common;
using OpSy_Cryptor.windows;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OpSy_Cryptor.usercontrols
{
    public partial class CryptAsymmetric : UserControl
    {
        private readonly SelectedFile selectedFile;

        public CryptAsymmetric(SelectedFile _selectedFile)
        {
            selectedFile = _selectedFile;
            InitializeComponent();
            cryptButton.Content = $"Kriptiraj {selectedFile.Name}";
            decryptButton.Content = $"Dekriptiraj {selectedFile.Name}";
        }

        private async void CryptButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseContactDialog chooseContactDialog = new();
            chooseContactDialog.ShowDialog();
            string recepientPublicKey = (string)chooseContactDialog.Tag;
            if (!string.IsNullOrWhiteSpace(recepientPublicKey))
            {
                try
                {
                    string encryptedFile = EncryptionClass.GetInstance().EncryptAsymmetricECDH(selectedFile.Contents, recepientPublicKey);

                    string path = selectedFile.Path + "__asimetrično-kriptirano.txt";

                    await File.WriteAllTextAsync(path, encryptedFile);
                    ExplorerNavigator.NavigateWindowsExplorerTo(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Problem pri enkripciji!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            ChooseContactDialog chooseContactDialog = new();
            chooseContactDialog.ShowDialog();
            string senderPublicKey = (string)chooseContactDialog.Tag;
            if (!string.IsNullOrWhiteSpace(senderPublicKey))
            {
                try
                {
                    byte[] decryptedFile = EncryptionClass.GetInstance().DecryptAsymmetricECDH(selectedFile.Contents, senderPublicKey);

                    string path = selectedFile.Path + "__asimetrično-dekriptirano";

                    if (selectedFile.Path.Contains("__asimetrično-kriptirano.txt"))
                    {
                        string purePath = selectedFile.Path.Split("__asimetrično-kriptirano.txt")[0];

                        int startIndex = purePath.LastIndexOf(".") + 1;
                        string extension = purePath[startIndex..];
                        purePath = purePath.Substring(0, purePath.LastIndexOf('.'));

                        path = $"{purePath}__asimetrično-dekriptirano.{extension}";
                    }

                    await File.WriteAllBytesAsync(path, decryptedFile);
                    ExplorerNavigator.NavigateWindowsExplorerTo(path);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Problem pri dekripciji!", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}

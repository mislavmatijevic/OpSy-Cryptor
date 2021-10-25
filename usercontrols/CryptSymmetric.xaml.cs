using Microsoft.Win32;
using OpSy_Cryptor.common;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace OpSy_Cryptor.usercontrols
{
    public partial class CryptSymmetric : UserControl
    {
        private readonly SelectedFile selectedFile;

        public CryptSymmetric(SelectedFile _selectedFile)
        {
            selectedFile = _selectedFile;
            InitializeComponent();
            cryptButton.Content = $"Kriptiraj {selectedFile.Name}";
            decryptButton.Content = $"Dekriptiraj {selectedFile.Name}";
        }

        private static string ChooseSecretKey()
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Odabir datoteke s tajnim ključem";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(openFileDialog.FileName) && openFileDialog.CheckPathExists)
            {
                string path = openFileDialog.FileName;
                return File.ReadAllText(path);
            }
            return null;
        }

        private async void CryptButton_Click(object sender, RoutedEventArgs e)
        {
            string secretKey = ChooseSecretKey();
            if (secretKey is null)
            {
                return;
            }

            try
            {
                string encryptedFile = EncryptionClass.GetInstance().EncryptSymmetricAES(selectedFile.Contents, secretKey);
                string path = selectedFile.Path + "__simetrično-kriptirano.txt";
                await File.WriteAllTextAsync(path, encryptedFile);
                ExplorerNavigator.NavigateWindowsExplorerTo(path);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem pri enkripciji!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            string secretKey = ChooseSecretKey();
            try
            {
                byte[] decryptedFile = EncryptionClass.GetInstance().DecryptSymmetricAES(selectedFile.Contents, secretKey);

                string path = selectedFile.Path + "__simetrično-dekriptirano";

                if (selectedFile.Path.Contains("__simetrično-kriptirano.txt"))
                {
                    string purePath = selectedFile.Path.Split("__simetrično-kriptirano.txt")[0];

                    int startIndex = purePath.LastIndexOf(".") + 1;
                    string extension = purePath[startIndex..];
                    purePath = purePath.Substring(0, purePath.LastIndexOf('.'));

                    path = $"{purePath}__simetrično-dekriptirano.{extension}";
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

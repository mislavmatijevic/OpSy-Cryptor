using OpSy_Cryptor.common;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpSy_Cryptor.usercontrols
{
    /// <summary>
    /// Interaction logic for CryptSymmetric.xaml
    /// </summary>
    public partial class CryptSymmetric : UserControl
    {

        private SelectedFile selectedFile;

        public CryptSymmetric(SelectedFile _selectedFile)
        {
            selectedFile = _selectedFile;
            InitializeComponent();
            cryptButton.Content = $"Kriptiraj {selectedFile.Name}";
            decryptButton.Content = $"Dekriptiraj {selectedFile.Name}";
        }

        private async void CryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string encryptedFile = Encryption.Object.EncryptSymmetricAES(selectedFile.Contents, Encryption.Object.GetSecretKey);
                await File.WriteAllTextAsync(selectedFile.Path + "_encrypted.txt", encryptedFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem pri enkripciji!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private async void DecryptButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                byte[] decryptedFile = Encryption.Object.DecryptSymmetricAES(selectedFile.Contents, Encryption.Object.GetSecretKey);
                await File.WriteAllBytesAsync(selectedFile.Path + "_decrypted", decryptedFile);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem pri dekripciji!", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

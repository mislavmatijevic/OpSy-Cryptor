using OpSy_Cryptor.common;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpSy_Cryptor.usercontrols
{
    public partial class CreateSignature : UserControl
    {
        private readonly SelectedFile selectedFile;
        private readonly string userID;
        public CreateSignature(SelectedFile _selectedFile, string _userID)
        {
            selectedFile = _selectedFile;
            userID = _userID;
            InitializeComponent();
        }

        private void SignFileButton_Click(object sender, RoutedEventArgs e)
        {
            if (confirmSignatureCheckBox.IsChecked == true)
            {
                string hashedContentBase64 = Convert.ToBase64String(selectedFile.SHA256Hash);
                string signature = EncryptionClass.GetInstance().GetSignature(Convert.FromBase64String(hashedContentBase64), userID);

                string newPath = selectedFile.Path + "__potpis.txt";
                File.WriteAllText(newPath, signature);
                ExplorerNavigator.NavigateWindowsExplorerTo(newPath);
                signFileButton.Background = Brushes.LightGreen;
                signFileButton.Foreground = Brushes.Black;
                signFileButton.Content = $"Dokument potpisan ({selectedFile.Name}__potpis.txt)!";
            }
        }
    }
}

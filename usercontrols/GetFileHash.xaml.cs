using OpSy_Cryptor.common;
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpSy_Cryptor.usercontrols
{
    /// <summary>
    /// Interaction logic for GetFileHash.xaml
    /// </summary>
    public partial class GetFileHash : UserControl
    {
        private readonly string hashedContentBase64;
        public string HashedContentBase64
        {
            get => hashedContentBase64;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            set => value = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }

        private readonly string hashedContentHex;
        public string HashedContentHex
        {
            get => hashedContentHex;
#pragma warning disable IDE0059 // Unnecessary assignment of a value
            set => value = null;
#pragma warning restore IDE0059 // Unnecessary assignment of a value
        }
        private readonly SelectedFile selectedFile;

        public GetFileHash(SelectedFile _selectedFile)
        {
            hashedContentBase64 = EncryptionClass.GetHashSHA256(_selectedFile.Contents);
            hashedContentHex = BitConverter.ToString(Convert.FromBase64String(hashedContentBase64)).Replace("-", "");
            DataContext = this;
            InitializeComponent();
            selectedFile = _selectedFile;
        }

        private void SaveFileButton_Click(object sender, RoutedEventArgs e)
        {
            string newPath = selectedFile.Path + "_SHA256.txt";

            string fileContent = $"SHA256 sažetak datoteke {selectedFile.Name}:\n" +
                $"\n------------------------------------------------------------------\n" +
                $"\nBase64String: {hashedContentBase64}\n" +
                $"\n------------------------------------------------------------------\n" +
                $"\nHeksadecimal: {hashedContentHex}\n" +
                $"\n------------------------------------------------------------------";

            File.WriteAllText(newPath, fileContent);
            ExplorerNavigator.NavigateWindowsExplorerTo(newPath);
            saveFileButton.Background = Brushes.LightGreen;
            saveFileButton.Foreground = Brushes.Black;
            saveFileButton.Content = "Pohranjeno!";
        }
    }
}

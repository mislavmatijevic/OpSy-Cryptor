using Microsoft.Win32;
using OpSy_Cryptor.common;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace OpSy_Cryptor.usercontrols
{
    public partial class LoadFiles : UserControl
    {
        private SelectedFile selectedFile;

        public LoadFiles()
        {
            DataContext = this;
            InitializeComponent();
        }

        public delegate void OnFileSelected(SelectedFile selectedFile);
        public event OnFileSelected OnFileSelectedEvent;

        private void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new();
            openFileDialog.Title = "Odabir datoteke nad kojom će se izvršavati operacije";
            openFileDialog.ShowDialog();
            if (!string.IsNullOrWhiteSpace(openFileDialog.FileName) && openFileDialog.CheckPathExists)
            {
                string path = openFileDialog.FileName;
                byte[] contents = File.ReadAllBytes(path);

                selectedFile = new SelectedFile(path, contents);
                OnFileSelectedEvent(selectedFile);
                loadFileButton.Content = $"Odabrana datoteka {openFileDialog.SafeFileName}!";

                loadFileButton.Background = Brushes.LightGreen;
                loadFileButton.Foreground = Brushes.Black;
                contentTextBlock.Visibility = Visibility.Visible;
                contentTextBox.Visibility = Visibility.Visible;
                contentTextBox.Text = selectedFile.Name[(selectedFile.Name.Length-3)..] == "txt" ? System.Text.Encoding.UTF8.GetString(selectedFile.Contents) : "Ova datoteka nije tekstualna!";
            }
        }
    }
}

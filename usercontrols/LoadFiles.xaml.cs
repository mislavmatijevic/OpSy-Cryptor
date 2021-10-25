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
        public LoadFiles()
        {
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

                OnFileSelectedEvent(new SelectedFile(path, contents));
                loadFileButton.Content = $"Odabrana datoteka {openFileDialog.SafeFileName}!";
                loadFileButton.Background = Brushes.LightGreen;
                loadFileButton.Foreground = Brushes.Black;
            }
        }
    }
}

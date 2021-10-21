using Microsoft.Win32;
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
    /// Interaction logic for LoadFiles.xaml
    /// </summary>
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
            openFileDialog.ShowDialog();
            if (openFileDialog.CheckPathExists)
            {
                string path = openFileDialog.FileName;
                byte[] contents = File.ReadAllBytes(path);

                OnFileSelectedEvent(new SelectedFile(path, contents));
            }
        }
    }
}

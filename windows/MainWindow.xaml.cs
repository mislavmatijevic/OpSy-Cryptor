using OpSy_Cryptor.common;
using OpSy_Cryptor.usercontrols;
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

namespace OpSy_Cryptor
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private string UserObjectID { get; set; }
        private SelectedFile selectedFile;

        public MainWindow(string userObjectID)
        {
            InitializeComponent();
            UserObjectID = userObjectID;
            loadedFileTextBox.Content = ""; // Remove default tekst.

            navMenu.ChangeStateEvent += NavMenu_ChangeStateEvent;
        }

        private void NavMenu_ChangeStateEvent(Option option)
        {
            switch (option)
            {
                case Option.HelpShown:
                    contentControl.Content = new HelpScreen();
                    break;
                case Option.LoadFile:
                    LoadFiles loadFiles = new();
                    loadFiles.OnFileSelectedEvent += LoadFiles_OnFileSelectedEvent;
                    contentControl.Content = loadFiles;
                    break;
                case Option.CryptSymetric:
                    contentControl.Content = new HelpScreen();
                    break;
                case Option.CryptASymetric:
                    contentControl.Content = new HelpScreen();
                    break;
                case Option.CalculateHash:
                    contentControl.Content = new HelpScreen();
                    break;
                case Option.DigitalSignature:
                    contentControl.Content = new HelpScreen();
                    break;
                case Option.CheckSignature:
                    contentControl.Content = new HelpScreen();
                    break;
                default:
                    break;
            }
        }

        private void LoadFiles_OnFileSelectedEvent(SelectedFile _selectedFile)
        {
            selectedFile = _selectedFile;
            loadedFileTextBox.Content = $"Učitana datoteka: {_selectedFile.Path}";
        }
    }
}

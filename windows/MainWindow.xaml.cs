using OpSy_Cryptor.common;
using OpSy_Cryptor.model;
using OpSy_Cryptor.usercontrols;
using System;
using System.IO;
using System.Windows;

namespace OpSy_Cryptor
{
    public partial class MainWindow : Window
    {

        private User UserObject { get; set; }
        private SelectedFile selectedFile = null;
        private readonly string pathToDirectory;

        public MainWindow(User userObject)
        {
            UserObject = userObject;
            pathToDirectory = $"{AppDomain.CurrentDomain.BaseDirectory}OpSy Generirano {UserObject.Username}";
            LoadKeys();

            InitializeComponent();

            loadedFileTextBlock.Text = ""; // Remove default tekst.
            usernameTextBlock.Text = "Korisnik: " + UserObject.Username;
            navMenu.ChangeStateEvent += NavMenu_ChangeStateEvent;
        }

        private string ReadKeyFromFile(string fileName)
        {
            string fullPath = $"{pathToDirectory}/{fileName}";

            if (!File.Exists(fullPath))
            {
                throw new Exception($"Datoteka {fileName} ne postoji! Datoteke s ključevima moraju biti u direktoriju {pathToDirectory}!");
            }

            return File.ReadAllText(fullPath);
        }

        private void LoadKeys()
        {
            try
            {
                EncryptionClass.GetInstance().LoadPrivateKeyECDH(ReadKeyFromFile("privatni_kljuc.txt"));
                EncryptionClass.GetInstance().LoadSecretKeyAES(ReadKeyFromFile("tajni_kljuc.txt"));
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Problem pri čitanju datoteke!", MessageBoxButton.OK, MessageBoxImage.Error);
                Environment.Exit(0);
            }

        }

        private void NavMenu_ChangeStateEvent(Option option)
        {
            if (option == Option.LoadFile)
            {
                LoadFiles loadFiles = new();
                loadFiles.OnFileSelectedEvent += LoadFiles_OnFileSelectedEvent;
                contentControl.Content = loadFiles;
            }
            else if (selectedFile is null)
            {
                MessageBox.Show("Morate odabrati datoteku nad kojom će se radnje izvesti!", "Neuspjeh", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            else
            {
                switch (option)
                {
                    case Option.HelpShown:
                        contentControl.Content = new HelpScreen();
                        break;
                    case Option.CryptSymmetric:
                        contentControl.Content = new CryptSymmetric(selectedFile);
                        break;
                    case Option.CryptASymmetric:
                        contentControl.Content = new CryptAsymmetric(selectedFile);
                        break;
                    case Option.CalculateHash:
                        contentControl.Content = new GetFileHash(selectedFile);
                        break;
                    case Option.DigitalSignature:
                        contentControl.Content = new CreateSignature(selectedFile, UserObject.Id.ToString());
                        break;
                    case Option.CheckSignature:
                        contentControl.Content = new ConfirmSignature(selectedFile);
                        break;
                    default:
                        break;
                }
            }
        }

        private void LoadFiles_OnFileSelectedEvent(SelectedFile _selectedFile)
        {
            selectedFile = _selectedFile;
            loadedFileTextBlock.Text = $"Učitana datoteka: {selectedFile.Path}";
            navMenu.IsFileLoaded = true;
        }
    }
}

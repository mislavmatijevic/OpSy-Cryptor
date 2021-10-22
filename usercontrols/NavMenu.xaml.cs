using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;

namespace OpSy_Cryptor
{

    public enum Option
    {
        HelpShown,
        LoadFile,
        CryptSymmetric,
        CryptASymmetric,
        CalculateHash,
        DigitalSignature,
        CheckSignature
    }

    public partial class NavMenu : INotifyPropertyChanged
    {
        public NavMenu()
        {
            DataContext = this;
            IsFileLoaded = false;
            InitializeComponent();
        }

        private bool isFileLoaded;
        public bool IsFileLoaded
        {
            get => isFileLoaded;
            set
            {
                if (isFileLoaded != value)
                {
                    isFileLoaded = value;
                    OnPropertyChanged();
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public delegate void ChangeState(Option option);
        public event ChangeState ChangeStateEvent;

        public void CryptSymmetric_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
            {
                ChangeStateEvent(Option.CryptSymmetric);
            }
        }

        public void CryptAsymmetric_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
            {
                ChangeStateEvent(Option.CryptASymmetric);
            }
        }

        public void CalculateHash_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
            {
                ChangeStateEvent(Option.CalculateHash);
            }
        }

        public void DigitallySign_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
            {
                ChangeStateEvent(Option.DigitalSignature);
            }
        }

        public void CheckSignature_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
            {
                ChangeStateEvent(Option.CheckSignature);
            }
        }

        public void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
            {
                ChangeStateEvent(Option.LoadFile);
            }
        }


    }
}

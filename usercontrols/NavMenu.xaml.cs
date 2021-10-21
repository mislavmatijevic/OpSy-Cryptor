using System.Windows;
using System.Windows.Controls;

namespace OpSy_Cryptor
{
    /// <summary>
    /// Interaction logic for NavMenu.xaml
    /// </summary>

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

    public partial class NavMenu : UserControl
    {
        public delegate void ChangeState(Option option);
        public event ChangeState ChangeStateEvent;

        public NavMenu()
        {
            InitializeComponent();
        }

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

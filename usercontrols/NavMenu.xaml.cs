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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace OpSy_Cryptor
{
    /// <summary>
    /// Interaction logic for NavMenu.xaml
    /// </summary>

    public enum Option
    {
        HelpShown,
        LoadFile,
        CryptSymetric,
        CryptASymetric,
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

        public void CryptSymetric_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
                ChangeStateEvent(Option.CryptSymetric);
        }

        public void CryptAsymetric_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
                ChangeStateEvent(Option.CryptASymetric);
        }

        public void CalculateHash_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
                ChangeStateEvent(Option.CalculateHash);
        }

        public void DigitallySign_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
                ChangeStateEvent(Option.DigitalSignature);
        }

        public void CheckSignature_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
                ChangeStateEvent(Option.CheckSignature);
        }

        public void LoadFile_Click(object sender, RoutedEventArgs e)
        {
            if (ChangeStateEvent is not null)
                ChangeStateEvent(Option.LoadFile);
        }


    }
}

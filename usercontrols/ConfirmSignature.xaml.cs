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

namespace OpSy_Cryptor.usercontrols
{
    /// <summary>
    /// Interaction logic for ConfirmSignature.xaml
    /// </summary>
    public partial class ConfirmSignature : UserControl
    {
        public ConfirmSignature()
        {
            InitializeComponent();
        }
        /*
         *  https://stackoverflow.com/questions/47363321/how-to-sign-a-message-using-ecdsa-with-an-ecdiffiehellman-instance-in-net-frame
            ECDiffieHellmanPublicKey ecdhPk; //Compute this with public key bytes
            var ecPkParams = ecdhPk.ExportParameters();
            var verifier = ECDsa.Create(ecPkParams);
            var verifyResult = verifier.VerifyData(bytesToBeVerified, signatureBytesToCheck, HashAlgorithmName.SHA256);
         */
    }
}

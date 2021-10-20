using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace OpSy_Cryptor.common
{
    public class Encryption
    {
        public static string GetPasswordHash(string password)
        {
            using SHA256 mySHA256 = SHA256.Create();
            byte[] hashedPassword = mySHA256.ComputeHash(Encoding.ASCII.GetBytes(password));
            return Convert.ToBase64String(hashedPassword);
        }

        public static string GetSalt()
        {
            var salt = new byte[32];
            using (var random = new RNGCryptoServiceProvider())
            {
                random.GetNonZeroBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }
    }
}

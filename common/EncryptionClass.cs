using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace OpSy_Cryptor.common
{
    internal struct KeyPairs
    {
        public byte[] privateKey;
        public byte[] publicKey;
        public byte[] secretKey;
    }
    public class EncryptionClass : IDisposable
    {
        private static readonly string CONTENT_BEGIN = "-----ENKRIPTIRANI SADRŽAJ-----";
        private static readonly string CONTENT_END = "-----KRAJ ENKRIPTIRANOG SADRŽAJA-----";
        private static readonly string IV_BEGIN = "-----INICIJALIZACIJSKI VEKTOR-----";
        private static readonly string IV_END = "-----KRAJ INICIJALIZACIJSKOG VEKTORA-----";

        private ECDiffieHellmanCng diffieHellmanObject;
        private readonly Aes aesObject;
        private readonly byte[] initializationVector;
        private KeyPairs keyPairs;

        private EncryptionClass()
        {
            aesObject = new AesCryptoServiceProvider // 256bit
            {
                // Padding error without this. (probably)
                Padding = PaddingMode.Zeros
            };
            initializationVector = aesObject.IV;
        }

        private static EncryptionClass instance;

        public static EncryptionClass GetInstance()
        {
            if (instance == null)
            {
                instance = new();
            }
            return instance;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (aesObject is not null)
                {
                    aesObject.Dispose();
                }
            }
        }

        public void LoadSecretKeyAES(string secretKeyBase64)
        {
            aesObject.Key = Convert.FromBase64String(secretKeyBase64);
            keyPairs.secretKey = aesObject.Key;
        }

        public void LoadPrivateKeyECDH(string privateKeyBase64)
        {
            keyPairs.privateKey = Convert.FromBase64String(privateKeyBase64);
            byte[] publicKey;

            diffieHellmanObject = new ECDiffieHellmanCng(CngKey.Import(keyPairs.privateKey, CngKeyBlobFormat.EccPrivateBlob))
            {
                KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash,
                HashAlgorithm = CngAlgorithm.Sha256
            };
            publicKey = diffieHellmanObject.PublicKey.ToByteArray();

            keyPairs.privateKey = Convert.FromBase64String(privateKeyBase64);
            keyPairs.publicKey = publicKey.ToArray();
        }

        // Defines and loads a private key for ECDH and allows its export out of DH object. Loading it derives public key. Secret AES key is created seperately.
        public void GenerateKeys()
        {
            byte[] privateKey;
            using (ECDiffieHellmanCng diffieHellmanKey = new(
                CngKey.Create(
                    CngAlgorithm.ECDiffieHellmanP256, // Necessarry for ExportPolicy below.
                    null,
                    new CngKeyCreationParameters { ExportPolicy = CngExportPolicies.AllowPlaintextExport })))
            {
                diffieHellmanKey.KeyDerivationFunction = ECDiffieHellmanKeyDerivationFunction.Hash;
                diffieHellmanKey.HashAlgorithm = CngAlgorithm.Sha256;

                privateKey = diffieHellmanKey.Key.Export(CngKeyBlobFormat.EccPrivateBlob);
            }

            string privateKeyBase64 = Convert.ToBase64String(privateKey.ToArray());

            aesObject.GenerateKey();
            keyPairs.secretKey = aesObject.Key;

            LoadPrivateKeyECDH(privateKeyBase64);
        }
        public string GetPrivateKey => Convert.ToBase64String(keyPairs.privateKey);
        public string GetPublicKey => Convert.ToBase64String(keyPairs.publicKey);
        public string GetIV => Convert.ToBase64String(initializationVector);
        public string GetSecretKey => Convert.ToBase64String(keyPairs.secretKey);

        public string EncryptSymmetricAES(byte[] readableContent, string secretKeyBase64)
        {
            try
            {
                aesObject.Key = Convert.FromBase64String(secretKeyBase64);
            }
            catch (Exception e)
            {
                throw new Exception($"Ključ nije ispravan za AES!\nProvjerite još jednom javni ključ primatelja:\n\n{e}");
            }
            aesObject.IV = initializationVector;

            byte[] cryptedContentBytes;
            try
            {
                using MemoryStream cryptedContentStream = new();
                using (ICryptoTransform encryptor = aesObject.CreateEncryptor())
                {
                    using CryptoStream cryptoStream = new(cryptedContentStream, encryptor, CryptoStreamMode.Write);
                    cryptoStream.Write(readableContent, 0, readableContent.Length);
                    cryptoStream.FlushFinalBlock();
                }
                cryptedContentBytes = cryptedContentStream.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception($"Problem pri kriptiranju poruke:\n\n{e}");
            }

            string encryptedContents = Convert.ToBase64String(cryptedContentBytes).Trim();

            return $"{CONTENT_BEGIN}\n{encryptedContents}\n{CONTENT_END}\n{IV_BEGIN}\n{GetIV}\n{IV_END}";
        }

        public string EncryptAsymmetricECDH(byte[] readableContent, string receiverPublicKeyBase64)
        {
            if (receiverPublicKeyBase64 == Convert.ToBase64String(keyPairs.publicKey))
            {
                throw new Exception("Potreban je primateljev javni ključ, a ne vaš!");
            }

            byte[] receiverPublicKeyBytes;
            CngKey receiverPublicKeyImported;
            try
            {
                receiverPublicKeyBytes = Convert.FromBase64String(receiverPublicKeyBase64);
                receiverPublicKeyImported = CngKey.Import(receiverPublicKeyBytes, CngKeyBlobFormat.EccPublicBlob);
            }
            catch (Exception)
            {
                throw new Exception($"Neispravan ključ!");
            }

            // Secret AES key is derived from public key of sender and private key of the user preforming the action.
            string symmetricKey = Convert.ToBase64String(diffieHellmanObject.DeriveKeyMaterial(receiverPublicKeyImported));

            return EncryptSymmetricAES(readableContent, symmetricKey);
        }

        public byte[] DecryptSymmetricAES(byte[] cryptedContent, string secretKeyBase64)
        {
            return DecryptSymmetricAES(Encoding.UTF8.GetString(cryptedContent), secretKeyBase64);
        }

        public byte[] DecryptSymmetricAES(string cryptedContent, string secretKeyBase64)
        {
            if (!Regex.IsMatch(cryptedContent, "-----ENKRIPTIRANI SADRŽAJ-----\n.+\n-----KRAJ ENKRIPTIRANOG SADRŽAJA-----\n-----INICIJALIZACIJSKI VEKTOR-----\n.+\n-----KRAJ INICIJALIZACIJSKOG VEKTORA-----"))
            {
                throw new Exception("Nije uočen ispravan obrazac poruke!\nObrazac ispravno kriptirane poruke mora biti:\n\n" +
                    $"{CONTENT_BEGIN}\n" +
                    "{kriptirani sadržaj}\n" +
                    $"{CONTENT_END}\n" +
                    $"{IV_BEGIN}\n" +
                    "{IV}\n" +
                    $"{IV_END}\n\n");
            }

            string cryptedContentBase64 = cryptedContent.Split('\n')[1];
            string initializationVectorBase64 = cryptedContent.Split('\n')[4];

            byte[] cryptedContentBytes;
            try
            {
                cryptedContentBytes = Convert.FromBase64String(cryptedContentBase64);
            }
            catch
            {
                throw new Exception("Poruka nije ispravno kriptirana ili kriptirana uopće!");
            }

            try
            {
                aesObject.Key = Convert.FromBase64String(secretKeyBase64);
            }
            catch (Exception e)
            {
                throw new Exception($"Ključ nije ispravan za AES!\n{e}");
            }

            try
            {
                aesObject.IV = Convert.FromBase64String(initializationVectorBase64);
            }
            catch (Exception e)
            {
                throw new Exception($"Inicijalizacijski vektor nije ispravan!\nProvjerite još jednom IV od pošiljatelja!\n\n{e}");
            }

            byte[] readableContent;
            try
            {
                using MemoryStream decryptedContent = new();
                using (ICryptoTransform decryptor = aesObject.CreateDecryptor())
                {
                    using CryptoStream cryptoStream = new(decryptedContent, decryptor, CryptoStreamMode.Write);
                    cryptoStream.Write(cryptedContentBytes, 0, cryptedContentBytes.Length);
                    cryptoStream.FlushFinalBlock();
                }

                readableContent = decryptedContent.ToArray();
            }
            catch (Exception e)
            {
                throw new Exception($"Problem pri dekriptiranju poruke:\n\n{e}");
            }

            return readableContent;
        }

        public byte[] DecryptAsymmetricECDH(byte[] cryptedContent, string senderPublicKeyBase64)
        {
            return DecryptAsymmetricECDH(Encoding.UTF8.GetString(cryptedContent), senderPublicKeyBase64);
        }

        public byte[] DecryptAsymmetricECDH(string cryptedContent, string senderPublicKeyBase64)
        {
            if (senderPublicKeyBase64 == Convert.ToBase64String(keyPairs.publicKey))
            {
                throw new Exception($"Potreban je pošiljateljev javni ključ, a ne vaš!");
            }

            byte[] senderPublicKeyBytes;
            CngKey senderPublicKeyImport;
            try
            {
                senderPublicKeyBytes = Convert.FromBase64String(senderPublicKeyBase64);
                senderPublicKeyImport = CngKey.Import(senderPublicKeyBytes, CngKeyBlobFormat.EccPublicBlob);
            }
            catch (Exception e)
            {
                throw new Exception($"Primateljev javni ključ nije ispravan:\n\n{e}");
            }

            // Ključ za simetrični AES dobiva se derivacijom javnog ključa pošiljatelja i privatnog ključa korisnika.
            byte[] symmetricKey = diffieHellmanObject.DeriveKeyMaterial(senderPublicKeyImport);
            string symmetricKeyBase64 = Convert.ToBase64String(symmetricKey);

            return DecryptSymmetricAES(cryptedContent, symmetricKeyBase64);
        }

        public string GetSignature(byte[] binaryContent, string signature)
        {
            using ECDsaCng eclipticCurveSignatureAlgorithm = new(CngKey.Import(keyPairs.privateKey, CngKeyBlobFormat.EccPrivateBlob));
            string signedContent = Convert.ToBase64String(eclipticCurveSignatureAlgorithm.SignHash(binaryContent));
            return $"{signature}.{signedContent}";
        }

        public static bool VerifySignature(byte[] hashBytes, string signature, string signerPublicKeyBase64)
        {
            byte[] signatureBytes = Convert.FromBase64String(signature);
            using ECDsaCng eclipticCurveSignatureAlgorithm = new(CngKey.Import(Convert.FromBase64String(signerPublicKeyBase64), CngKeyBlobFormat.EccPublicBlob));
            return eclipticCurveSignatureAlgorithm.VerifyHash(hashBytes, signatureBytes);
        }

        public static byte[] GetHashSHA256(byte[] binaryContent)
        {
            using SHA256 mySHA256 = SHA256.Create();
            return mySHA256.ComputeHash(binaryContent);
        }

        public static string GetSalt()
        {
            byte[] salt = new byte[32];
            using (RNGCryptoServiceProvider random = new())
            {
                random.GetNonZeroBytes(salt);
            }

            return Convert.ToBase64String(salt);
        }
    }

}

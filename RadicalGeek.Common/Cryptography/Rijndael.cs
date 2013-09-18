using System;
using System.Security.Cryptography;
using System.Text;
using RadicalGeek.Common.Collections;

namespace RadicalGeek.Common.Cryptography
{
    public static class Rijndael
    {
        private static readonly RijndaelManaged rijndaelManaged = new RijndaelManaged { Mode = CipherMode.CBC };
        private const string HashAlgorithm = "SHA1";

        public static string Decrypt(string cipherText, EncryptionKey encryptionKey, int passwordIterations = 2)
        {
            return Decrypt(cipherText, encryptionKey.PassPhrase, encryptionKey.SaltValue, encryptionKey.InitVector, passwordIterations);
        }

        public static string Decrypt(string cipherText, string passPhrase, string saltValue, string initVector, int passwordIterations = 2)
        {
            if (cipherText == null)
                throw new ArgumentNullException("cipherText");

            ValidateParameters(ref passPhrase, ref saltValue, ref initVector);

            if (cipherText == string.Empty)
                return string.Empty;

            CryptoBytes cryptoBytes = cryptoBytesCache[new Tuple<string,string,string,int>(passPhrase, saltValue, initVector, passwordIterations)];

            byte[] inputBytes = Convert.FromBase64String(cipherText);

            ICryptoTransform decrypter = rijndaelManaged.CreateDecryptor(cryptoBytes.Password, cryptoBytes.Vector);

            byte[] bytes = decrypter.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Encoding.UTF8.GetString(bytes);
        }

        private static void ValidateParameters(ref string passPhrase, ref string saltValue, ref string initVector)
        {
            passPhrase = passPhrase.Trim();
            saltValue = saltValue.Trim();
            initVector = initVector.Trim();
            if (passPhrase.Length != 20)
                throw new ArgumentException("The pass phrase must be 20 characters long.", "passPhrase");
            if (saltValue.Length != 20)
                throw new ArgumentException("The salt value must be 20 characters long.", "saltValue");
            if (initVector.Length != 18)
                throw new ArgumentException("The vector must be 18 characters long.", "initVector");
        }

        public static string Encrypt(string plainText, EncryptionKey encryptionKey, int passwordIterations = 2)
        {
            return Encrypt(plainText, encryptionKey.PassPhrase, encryptionKey.SaltValue, encryptionKey.InitVector,
                           passwordIterations);
        }

        public static string Encrypt(string plainText, string passPhrase, string saltValue, string initVector, int passwordIterations = 2)
        {
            if (plainText == string.Empty)
                return string.Empty;

            ValidateParameters(ref passPhrase, ref saltValue, ref initVector);

            if (plainText == null)
                throw new ArgumentNullException("plainText");

            CryptoBytes cryptoBytes = cryptoBytesCache[new Tuple<string, string, string, int>(passPhrase, saltValue, initVector, passwordIterations)];

            byte[] inputBytes = Encoding.UTF8.GetBytes(plainText);

            ICryptoTransform encrypter = rijndaelManaged.CreateEncryptor(cryptoBytes.Password, cryptoBytes.Vector);

            byte[] bytes = encrypter.TransformFinalBlock(inputBytes, 0, inputBytes.Length);

            return Convert.ToBase64String(bytes);
        }

        private static readonly CacheList<Tuple<string,string,string,int>,CryptoBytes> cryptoBytesCache = new CacheList<Tuple<string, string, string, int>, CryptoBytes>(GetCryptoBytes);

        private static CryptoBytes GetCryptoBytes(Tuple<string,string,string,int> tuple)
        {
            return GetCryptoBytes(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4);
        }

        private static CryptoBytes GetCryptoBytes(string passPhrase, string saltValue, string initVector, int passwordIterations)
        {
            byte[] vectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltBytes = Encoding.ASCII.GetBytes(saltValue);
            byte[] passwordBytes;

            using (PasswordDeriveBytes passwordDeriveBytes = new PasswordDeriveBytes(passPhrase, saltBytes, HashAlgorithm, passwordIterations))
                passwordBytes = passwordDeriveBytes.GetBytes(32);
            return new CryptoBytes(vectorBytes, passwordBytes);
        }

    }
}

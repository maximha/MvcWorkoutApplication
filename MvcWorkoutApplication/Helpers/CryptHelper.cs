using MvcWorkoutApplication.Api_Models;
using System;
using System.Security.Cryptography;
using System.Text;

namespace MvcWorkoutApplication.Helpers
{
    public static class CryptHelper
    {

        /*private static RijndaelManaged crypter()
        {
            return new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };
        }

        public static RijndaelManaged crypter(byte[] keyBytes)
        {
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }*/

        public static RijndaelManaged GetRijndaelManaged(String secretKey)
        {
            var keyBytes = new byte[16];
            var secretKeyBytes = Encoding.UTF8.GetBytes(secretKey);
            Array.Copy(secretKeyBytes, keyBytes, Math.Min(keyBytes.Length, secretKeyBytes.Length));
            return new RijndaelManaged
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128,
                Key = keyBytes,
                IV = keyBytes
            };
        }

        public static string generateAESKey()
        {
            RijndaelManaged crypt = new RijndaelManaged()
            {
                Mode = CipherMode.CBC,
                Padding = PaddingMode.PKCS7,
                KeySize = 128,
                BlockSize = 128
            };
            crypt.GenerateKey();
            string key = Convert.ToBase64String(crypt.Key);
            return key;
        }

        /*public static string generateAESKey()
        {
            RijndaelManaged crypt = crypter();
            crypt.GenerateKey();
            string key = Convert.ToBase64String(crypt.Key);
            return key;
        }*/

        /*public static string Encrypt(string plainText)
        {
            string aesKey = Globals.global_AES_Key;
            byte[] keyBytes = Convert.FromBase64String(aesKey);
            byte[] dataBytes = Convert.FromBase64String(plainText);
            byte[] encryptedData = crypter(keyBytes).CreateEncryptor()
                .TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return Convert.ToBase64String(encryptedData);
        }*/

        public static byte[] Encrypt(byte[] plainBytes, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateEncryptor()
                .TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        }
        public static String Encrypt(String plainText)
        {
            string aesKey = Globals.global_AES_Key;
            var plainBytes = Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(Encrypt(plainBytes, GetRijndaelManaged(aesKey)));
        }

        /*public static string Decrypt(string encryptedText)
        {
            string aesKey = Globals.global_AES_Key;
            byte[] keyBytes = Encoding.UTF8.GetBytes(aesKey); //Convert.FromBase64String(aesKey);
            string str = encryptedText.Substring(0, encryptedText.Length - 1);
            byte[] dataBytes = Convert.FromBase64String(encryptedText);
            byte[] decryptedData = crypter(keyBytes).CreateDecryptor()
                .TransformFinalBlock(dataBytes, 0, dataBytes.Length);
            return Convert.ToBase64String(decryptedData);
        }*/
        public static byte[] Decrypt(byte[] encryptedData, RijndaelManaged rijndaelManaged)
        {
            return rijndaelManaged.CreateDecryptor()
                .TransformFinalBlock(encryptedData, 0, encryptedData.Length);
        }

        public static String Decrypt(String encryptedText)
        {
            string aesKey = Globals.global_AES_Key;
            var encryptedBytes = Convert.FromBase64String(encryptedText);
            return Encoding.UTF8.GetString(Decrypt(encryptedBytes, GetRijndaelManaged(aesKey)));
        }

        public static string getHash(string input)
        {
            Byte[] inputBytes = Encoding.UTF8.GetBytes(input);

            Byte[] hashedBytes = new SHA256CryptoServiceProvider().ComputeHash(inputBytes);

            return BitConverter.ToString(hashedBytes);
        }


    }
}
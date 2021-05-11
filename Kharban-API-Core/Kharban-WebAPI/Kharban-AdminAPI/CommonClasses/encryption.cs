using System;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Kharban_AdminAPI.Common
{
    public static class AESEncryption
    {
        private static string iCompleteEncodedKey = "OTFpM0YzNUt6SGQwVjFpOUloSy9rdz09LEwvUjhYNUtoR3UveXVlTDk3WnoweTA1cFVvMzkzZE10ZzZXN1d2YytMcFk9";
      
        public static string GenerateKey(int iKeySize)
        {
            RijndaelManaged aesEncryption = new RijndaelManaged();
            aesEncryption.KeySize = iKeySize;
            aesEncryption.BlockSize = 128;
            aesEncryption.Mode = CipherMode.CBC;
            aesEncryption.Padding = PaddingMode.PKCS7;
            aesEncryption.GenerateIV();
            string ivStr = Convert.ToBase64String(aesEncryption.IV);
            aesEncryption.GenerateKey();
            string keyStr = Convert.ToBase64String(aesEncryption.Key);
            string completeKey = ivStr + "," + keyStr;

            return Convert.ToBase64String(ASCIIEncoding.UTF8.GetBytes(completeKey));
        }

        public static string Encrypt(this string iPlainStr, int iKeySize = 256)
        {
            if (!string.IsNullOrEmpty(iPlainStr))
            {
                RijndaelManaged aesEncryption = new RijndaelManaged();
                aesEncryption.KeySize = iKeySize;
                aesEncryption.BlockSize = 128;
                aesEncryption.Mode = CipherMode.CBC;
                aesEncryption.Padding = PaddingMode.PKCS7;
                aesEncryption.IV = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[0]);
                aesEncryption.Key = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[1]);
                byte[] plainText = ASCIIEncoding.UTF8.GetBytes(iPlainStr);
                ICryptoTransform crypto = aesEncryption.CreateEncryptor();
                byte[] cipherText = crypto.TransformFinalBlock(plainText, 0, plainText.Length);
                return WebUtility.UrlEncode(Convert.ToBase64String(cipherText));
            }
            return "";
        }

        public static string Decrypt(this string iEncryptedText, int iKeySize = 256)
        {
            if (!string.IsNullOrEmpty(iEncryptedText))
            {
                iEncryptedText = WebUtility.UrlDecode(iEncryptedText);
                RijndaelManaged aesEncryption = new RijndaelManaged();
                aesEncryption.KeySize = iKeySize;
                aesEncryption.BlockSize = 128;
                aesEncryption.Mode = CipherMode.CBC;
                aesEncryption.Padding = PaddingMode.PKCS7;
                aesEncryption.IV = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[0]);
                aesEncryption.Key = Convert.FromBase64String(ASCIIEncoding.UTF8.GetString(Convert.FromBase64String(iCompleteEncodedKey)).Split(',')[1]);
                ICryptoTransform decrypto = aesEncryption.CreateDecryptor();
                byte[] encryptedBytes = Convert.FromBase64CharArray(iEncryptedText.ToCharArray(), 0, iEncryptedText.Length);
                return ASCIIEncoding.UTF8.GetString(decrypto.TransformFinalBlock(encryptedBytes, 0, encryptedBytes.Length));
            }
            return "";
        }
    }
}
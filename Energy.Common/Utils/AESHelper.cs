using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Energy.Common.Utils
{
    /// <summary>
    /// AES加密解密工具类
    /// </summary>
    public class AESHelper
    {
        /// <summary>
        /// AES128加密
        /// </summary>
        /// <param name="text">需要加密的字符串</param>
        /// <param name="password">加密密码</param>
        /// <param name="iv">加密密钥</param>
        /// <returns>加密后的字节数组</returns>
        public static byte[] AESEncrypt(string text,string password,string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = Encoding.UTF8.GetBytes(password);
            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len=keyBytes.Length;

            Array.Copy(pwdBytes,keyBytes,len);

            rijndaelCipher.Key = keyBytes;

            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transformer = rijndaelCipher.CreateEncryptor();

            byte[] plainText = Encoding.UTF8.GetBytes(text);
            byte[] cipherBytes = transformer.TransformFinalBlock(plainText, 0, plainText.Length);

            return cipherBytes;
        }

        public static string AESDecrypt(byte[] source,string password,string iv)
        {
            RijndaelManaged rijndaelCipher = new RijndaelManaged();
            rijndaelCipher.Mode = CipherMode.CBC;
            rijndaelCipher.Padding = PaddingMode.PKCS7;
            rijndaelCipher.KeySize = 128;
            rijndaelCipher.BlockSize = 128;

            byte[] pwdBytes = Encoding.UTF8.GetBytes(password);
            byte[] keyBytes = new byte[16];

            int len = pwdBytes.Length;
            if (len > keyBytes.Length)
                len = keyBytes.Length;

            Array.Copy(pwdBytes, keyBytes, len);

            rijndaelCipher.Key = keyBytes;

            byte[] ivBytes = Encoding.UTF8.GetBytes(iv);
            rijndaelCipher.IV = ivBytes;

            ICryptoTransform transformer = rijndaelCipher.CreateDecryptor();
            byte[] plainText;

            try
            {
                plainText = transformer.TransformFinalBlock(source, 0, source.Length);
            }
            catch
            {
                return Encoding.UTF8.GetString(source);
            }
            

            return Encoding.UTF8.GetString(plainText);
        }

        static string GetAESString(byte[] cipherBytes)
        {
            StringBuilder textBuilder = new StringBuilder();
            for (int i = 0; i < cipherBytes.Length; i++)
            {
                textBuilder.Append(cipherBytes[i].ToString("x2"));
            }

            return textBuilder.ToString();
        }

    }
}

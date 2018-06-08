using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace TCPServer.Utils
{
    /// <summary>
    /// MD5加密辅助类
    /// </summary>
    public class MD5Helper
    {
        /// <summary>
        /// 静态方法：根据传入的字符串，将字符串加密成MD5
        /// </summary>
        /// <param name="rawCode">原始字符串</param>
        /// <returns>加密后的MD5字符串</returns>
        public static string GetMD5(string rawCode)
        {
            MD5 md5 = MD5.Create();

            byte[] rawData = Encoding.UTF8.GetBytes(rawCode);
            byte[] encrytedData = md5.ComputeHash(rawData);

            return GetMD5String(encrytedData);
        }

        /// <summary>
        /// 将MD5字节数组，转换成MD5字符串
        /// </summary>
        /// <param name="data">字节数组</param>
        /// <returns>字符串</returns>
        static string GetMD5String(byte[] data)
        {
            StringBuilder textBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                textBuilder.Append(data[i].ToString("x2"));
            }

            return textBuilder.ToString();
        }
    }
}

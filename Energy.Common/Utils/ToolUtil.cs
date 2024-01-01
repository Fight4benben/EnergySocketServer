using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Utils
{
    public class ToolUtil
    {
        /// <summary>
        /// 将yyyyMMddHHmmss类型字符串时间转换成DateTime
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static DateTime GetDateTimeFromString(string time)
        {
            DateTime dt = DateTime.ParseExact(time,"yyyyMMddHHmmss",System.Globalization.CultureInfo.CurrentCulture);
             dt = dt.AddSeconds(-dt.Second);
            return dt;
        }

        public static DateTime GetDateTimeFromString(string time, string format)
        {
            DateTime dt = DateTime.ParseExact(time, format, System.Globalization.CultureInfo.CurrentCulture);
            return dt;
        }

        public static byte[] AppendPacketHeader(int type,byte[] data)
        {
            int length = data.Length;
            byte[] lengthArr = BitConverter.GetBytes(length);

            //Array.Reverse(lengthArr);

            byte[] Header = new byte[3];
            Header[0] = 0x1F;
            Header[1] = 0x1F;
            Header[2] = (byte)type;

            byte[] FixedHeader = new byte[7];

            Buffer.BlockCopy(Header, 0, FixedHeader, 0, Header.Length);
            Buffer.BlockCopy(lengthArr,0,FixedHeader,Header.Length,lengthArr.Length);

            byte[] SendData = new byte[7+data.Length];

            Buffer.BlockCopy(FixedHeader, 0, SendData, 0, FixedHeader.Length);
            Buffer.BlockCopy(data, 0, SendData, FixedHeader.Length, data.Length);


            return SendData;
        }

        /// <summary>
        /// 判断可空浮点型数据是否为空或null
        /// </summary>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public static string JudgeParamValue(float? paramValue)
        {
            if (paramValue == null)
                return "NULL";

            if (string.IsNullOrEmpty(paramValue.ToString()))
                return "NULL";

            return paramValue.ToString();
        }
    }
}

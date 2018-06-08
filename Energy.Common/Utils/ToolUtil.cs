using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Utils
{
    public class ToolUtil
    {
        public static DateTime GetDateTimeFromString(string time)
        {
            DateTime dt = DateTime.ParseExact(time,"yyyyMMddHHmmss",System.Globalization.CultureInfo.CurrentCulture);
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
    }
}

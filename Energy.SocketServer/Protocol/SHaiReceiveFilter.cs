using SuperSocket.Facility.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.Common.Utils;

namespace Energy.SocketServer.Protocol
{
    public class SHaiReceiveFilter:FixedHeaderReceiveFilter<SHaiRequestInfo>
    {
        public SHaiReceiveFilter()
            : base(7)
        { }

        protected override int GetBodyLengthFromHeader(byte[] header, int offset, int length)
        {
            int result = (int)(
                          header[offset + 3] * 256 * 256 * 256 +
                          header[offset + 4] * 256 * 256 +
                          header[offset + 5] * 256 +
                          header[offset + 6]
                         );
            return result;
        }

        protected override SHaiRequestInfo ResolveRequestInfo(ArraySegment<byte> header, byte[] bodyBuffer, int offset, int length)
        {
            SHaiRequestInfo request = new SHaiRequestInfo();
            
            request.FixedContent = header.Array.Take(2).ToArray();
            request.MessageType = header.Array.ToArray()[2];
            byte[] data = bodyBuffer.Skip(offset).Take(length).ToArray();
            if (request.MessageType == 3)
            {
                request.Key = "ENERGYDATA";
                request.Data = AESHelper.AESDecrypt(data, Runtime.m_AESValue, Runtime.m_AESValue);
            }
            else
            {
                if (request.MessageType == 1)
                    request.Key = "VALIDATE";
                else
                    request.Key = "HEARTBEAT";

                request.Data = Encoding.UTF8.GetString(data);
            }


            return request;
        }
    }
}

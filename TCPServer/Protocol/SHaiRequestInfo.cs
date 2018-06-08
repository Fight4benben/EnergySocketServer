using SuperSocket.SocketBase.Protocol;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Protocol
{
    public class SHaiRequestInfo : IRequestInfo
    {
        /// <summary>
        /// [No Use]
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// [Fixed Size Content 2 bytes:0x1F1F]
        /// </summary>
        public byte[] FixedContent { get; set; }

        /// <summary>
        /// [MessageType with 1 byte]
        /// 0x1: Identity message,Following Data is plaintext
        /// 0x2: HeartBeat message, Following Data is plaintext
        /// 0x03: EnergyConsumeData, Following Data is AES Encrypted Data
        /// </summary>
        public byte MessageType { get; set; }

        /// <summary>
        /// [Variable length data : Energy Data with AES Encryption]
        /// </summary>
        public string Data { get; set; }
    }
}

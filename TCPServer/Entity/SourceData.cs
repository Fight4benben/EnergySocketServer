using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Entity
{
    public class SourceData
    {
        public string BuildID { get; set; }
        public string GatewayID { get; set; }
        public string CollectTime { get; set; }
        public string DatagramType { get; set; }
        public int Status { get; set; }
        public string JsonData { get; set; }
    }
}

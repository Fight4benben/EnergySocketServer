using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Entity
{
    public class MeterList
    {
        public MeterList()
        { }

        public MeterList(string buildId,string gatewayId,DateTime collectTime,bool processStatus,List<Meter> meters)
        {
            this.BuildId = buildId;
            this.GatewayId = gatewayId;
            this.CollectTime = collectTime;
            this.ProcessStatus = processStatus;
            this.Meters = meters;
        }

        public string BuildId { get; set; }
        public string GatewayId { get; set; }
        public DateTime CollectTime { get; set; }
        public bool ProcessStatus{ get; set; }
        public List<Meter> Meters { get; set; }
    }
}

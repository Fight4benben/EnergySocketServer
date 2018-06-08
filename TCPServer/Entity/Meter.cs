using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Entity
{
    public class Meter 
    {
        public Meter()
        { }

        public Meter(string meterId,List<MeterParam> meterParams)
        {
            this.MeterId = meterId;
            this.MeterParams = meterParams;
        }
        public string MeterId { get; set; }
        public List<MeterParam> MeterParams { get; set; }
    }
}

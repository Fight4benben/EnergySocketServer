using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Entity
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
        public Meter(bool status,string meterId, List<MeterParam> meterParams)
        {
            this.Status = status;
            this.MeterId = meterId;
            this.MeterParams = meterParams;
        }
        public bool Status { get; set; }
        public string MeterId { get; set; }
        public List<MeterParam> MeterParams { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Entity
{
    public class YangZhuChangMeter
    {
        public string BuildId { get; set; }
        public string GatewayName { get; set; }
        public string MeterCode { get; set; }
        public DateTime Time { get; set; }
        public decimal Value { get; set; }
        public int Status { get; set; }
    }
}

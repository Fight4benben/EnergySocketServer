using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Entity
{
    public class BaseElecData
    {
        public string BuildID { get; set; }
        public string MeterCode { get; set; }
        public DateTime Time { get; set; }
        public float? PF { get; set; }
        public float? Fr { get; set; }
    }
}

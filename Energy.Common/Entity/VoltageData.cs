using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Entity
{
    public class VoltageData
    {
        public string BuildID { get; set; }
        public string MeterCode { get; set; }
        public DateTime Time { get; set; }
        public float? U { get; set; }
        public float? Ua { get; set; }
        public float? Ub { get; set; }
        public float? Uc { get; set; }
        public float? Uab { get; set; }
        public float? Ubc { get; set; }
        public float? Uca { get; set; }
    }
}

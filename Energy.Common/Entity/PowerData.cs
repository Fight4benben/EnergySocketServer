using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Entity
{
    public class PowerData
    {
        public string BuildID { get; set; }
        public string MeterCode { get; set; }
        public DateTime Time { get; set; }
        public float? P { get; set; }
        public float? Pa { get; set; }
        public float? Pb { get; set; }
        public float? Pc { get; set; }
        public float? Q { get; set; }
        public float? Qa { get; set; }
        public float? Qb { get; set; }
        public float? Qc { get; set; }
        public float? S { get; set; }
        public float? Sa { get; set; }
        public float? Sb { get; set; }
        public float? Sc { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Entity
{
    public class CurrentData
    {
        public string BuildID { get; set; }
        public string MeterCode { get; set; }
        public DateTime Time { get; set; }
        public float? I { get; set; }
        public float? Ia { get; set; }
        public float? Ib { get; set; }
        public float? Ic { get; set; }
    }
}

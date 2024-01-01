using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Entity
{
    /// <summary>
    /// 能耗值实体类：EPI，LJLL，LLHeat等
    /// </summary>
    public class OriginEnergyData
    {
        public string BuildID { get; set; }
        public string MeterCode { get; set; }
        public DateTime Time { get; set; }
        public float? Value { get; set; }
        public bool Calced { get; set; }
    }

    public class RentEnergyData
    {
        public string BuildID { get; set; }
        public string MeterCode { get; set; }
        public DateTime Day { get; set; }
        public decimal? StartValue { get; set; }
        public decimal? EndValue { get; set; }
        public int Calced { get; set; }
    }
}

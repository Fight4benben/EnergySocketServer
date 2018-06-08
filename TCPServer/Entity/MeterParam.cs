using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer.Entity
{
    public class MeterParam
    {
        public MeterParam()
        { }

        public MeterParam(string paramName,string paramError,float? paramValue)
        {
            this.ParamName = paramName;
            this.ParamError = paramError;
            this.ParamValue = paramValue;
        }
        public string ParamName { get; set; }
        public string ParamError { get; set; }
        public float? ParamValue { get; set; }
    }
}

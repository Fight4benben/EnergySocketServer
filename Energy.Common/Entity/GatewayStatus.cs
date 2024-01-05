﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Energy.Common.Entity
{
    public class GatewayStatus
    {
        public string BuildId { get; set; }
        public string GatewayId { get; set; }
        public DateTime LastUploadTime { get; set; }
        public int Status { get; set; }
        public DateTime XmlTime { get; set; }
    }
}
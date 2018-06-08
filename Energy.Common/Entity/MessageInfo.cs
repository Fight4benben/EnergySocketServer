using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Energy.Common.Entity
{
    public class MessageInfo
    {
        public MessageInfo()
        { }

        public MessageInfo(string buildid,string gatewayid,string messagetype,string messageattr,string content)
        {
            this.BuildID = buildid;
            this.GatewayID = gatewayid;
            this.MessageType = messagetype;
            this.MessageContent = content;
            this.MessageAttribute = messageattr;
        }
        public string BuildID { get; set; }
        public string GatewayID { get; set; }
        public string MessageType { get; set; }
        public string MessageContent { get; set; }
        public string MessageAttribute { get; set; }
    }
}

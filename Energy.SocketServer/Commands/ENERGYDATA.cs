using SuperSocket.SocketBase.Command;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Energy.Common.DAL;
using Energy.Common.Entity;
using Energy.SocketServer.Protocol;
using Energy.Common.Utils;

namespace Energy.SocketServer.Commands
{
    public class ENERGYDATA : CommandBase<SHaiSession, SHaiRequestInfo>
    {
        public override void ExecuteCommand(SHaiSession session, SHaiRequestInfo requestInfo)
        {
            MessageInfo message = XMLHelper.GetMessageInfo(requestInfo.Data);
            ProcessEnergyData(message,session,requestInfo.Data);
        }

        void ProcessEnergyData(MessageInfo message,SHaiSession session,string xmlSource)
        {
            if (message.MessageAttribute == "report")
            {
                Console.WriteLine("{0} -> 接收到：" + message.GatewayID + "网关发来的建筑" + message.BuildID + "的定时上报的能耗数据，数据时间为：" + message.MessageContent,DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                Runtime.m_Logger.Info("{0} -> 接收到：" + message.GatewayID + "网关发来的建筑" + message.BuildID + "的定时上报的能耗数据，数据时间为：" + message.MessageContent, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //保存数据到文件中
                string source = XMLHelper.GetMeterList(xmlSource,Runtime.MySqlConnectString,"report");

                //bool receiveSuccess = MySQLHelper.InsertGatewayDataToDB(message,source,Runtime.MySqlConnectString);
                bool receiveSuccess = FileHelper.SaveJsonData2File($"{message.BuildID}_{message.GatewayID}_{message.MessageContent}_{message.MessageAttribute}_{DateTime.Now.ToString("HHmmssfff")}.json", source, out string msg);
                if (receiveSuccess)
                    Runtime.m_Logger.Info(msg);
                else
                    Runtime.m_Logger.Error(msg);

                try
                {
                    List<GatewayStatus> gatewayList = new List<GatewayStatus>();
                    gatewayList.Add(new GatewayStatus() { BuildId= message.BuildID, GatewayId=message.GatewayID, XmlTime=ToolUtil.GetDateTimeFromString(message.MessageContent),Status=1,LastUploadTime = DateTime.Now });
                    MySQLHelper.InsertGatewayStatusByList(Runtime.MySqlConnectString,gatewayList);
                }
                catch
                {
                }

                //保存到养猪场数据库
                //bool receiveSuccess = true;

                //List<YangZhuChangMeter> meterList = XMLHelper.GetYangZhuChangMeters(xmlSource);

                //try
                //{
                //    int cnt = MySQLHelper.InsertRealTimeValues(Runtime.MySqlConnectString, meterList);
                //    if (cnt > 0)
                //        receiveSuccess = true;
                //}
                //catch (Exception e)
                //{
                //    receiveSuccess = false;
                //    Runtime.m_Logger.Error("数据存储出错，" + e.Message);
                //}


                if (!receiveSuccess)
                    return;

                //回送接收到数据
                byte[] data = XMLHelper.GetXmlBytes(message, "energy_data", message.MessageAttribute, message.MessageContent, "OK");
                session.Send(ToolUtil.AppendPacketHeader(3,data), 0, data.Length+7);
            }
            else if (message.MessageAttribute == "continuous")
            {
                Console.WriteLine("{0} -> 接收到：" + message.GatewayID + "网关发来的建筑" + message.BuildID + "的断点续传的能耗数据,数据时间为：" + message.MessageContent, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                string source = XMLHelper.GetMeterList(xmlSource,Runtime.MySqlConnectString,"continuous");
                //bool receiveSuccess = SQLiteHelper.InsertGatewayDataToDB(message, source);
                bool receiveSuccess = MySQLHelper.InsertGatewayDataToDB(message, source, Runtime.MySqlConnectString);

                //bool receiveSuccess = true;

                //List<YangZhuChangMeter> meterList = XMLHelper.GetYangZhuChangMeters(xmlSource);

                //try
                //{
                //    int cnt = MySQLHelper.InsertRealTimeValues(Runtime.MySqlConnectString, meterList);
                //    if (cnt > 0)
                //        receiveSuccess = true;
                //}
                //catch (Exception e)
                //{
                //    receiveSuccess = false;
                //    Runtime.m_Logger.Error("数据存储出错，" + e.Message);
                //}

                if (!receiveSuccess)
                    return;

                //回送接收到数据
                byte[] data = XMLHelper.GetXmlBytes(message, "energy_data", message.MessageAttribute, message.MessageContent, "OK");
                session.Send(ToolUtil.AppendPacketHeader(3, data), 0, data.Length+7);
            }
            else if (message.MessageAttribute == "finish")
            {
                Console.WriteLine("{0} -> 接收到："+message.GatewayID + "网关发来的建筑" + message.BuildID + "断点续传完成标记。", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                //回送接收到数据
                byte[] data = XMLHelper.GetXmlBytes(message, "energy_data", message.MessageAttribute, message.MessageContent, "OK");
                session.Send(ToolUtil.AppendPacketHeader(3, data), 0, data.Length+7);
            }
        }
    }
}

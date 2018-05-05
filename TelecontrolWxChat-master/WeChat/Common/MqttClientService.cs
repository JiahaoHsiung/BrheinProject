using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace WeChat.Common
{
    public class MqttClientService
    {

        private static volatile MqttClientService _instance = null;
        private static Config.InfoCon _config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~").GetSection("InfoCon") as WeChat.Config.InfoCon;
        private static readonly object LockHelper = new object();

        /// <summary>
        /// 创建单例模式
        /// </summary>
        /// <param name="Topic">传入主题</param>
        /// <returns></returns>
        public static MqttClientService CreateInstance(string Topic, string ClientId)
        {
            if (_instance == null)
            {
                lock (LockHelper)
                {
                    if (_instance == null)
                        _instance = new MqttClientService(Topic, ClientId);
                }
            }
            return _instance;
        }

        /// <summary>
        /// 实例化订阅客户端
        /// </summary>
        public MqttClient SubscribeClient { get; set; }


        public Action<Object, MqttMsgPublishEventArgs> ClientPublishReceivedAction { get; set; }

        public MqttClientService(string Topic,string ClientId)
        {
            try
            {
                // 创建客户端实例 
                //SubscribeClient = new MqttClient(IPAddress.Parse(ipAddress));--弃用
                SubscribeClient = new MqttClient(_config.BrokerHostName);

                // 注册到接收到的消息 
                SubscribeClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
                //随机值
                string clientId = ClientId;//Guid.NewGuid().ToString();
                //连接
                SubscribeClient.Connect(clientId.Replace("\"", ""), _config.UserName, _config.PassWord);

                // 订阅QoS 2的主题"/home/temperature"
                ushort u1 = SubscribeClient.Subscribe(new string[] { string.Format("GateWay/V1/{0}/PUB_RealTimeMsg", Topic.Replace("\"", "")) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                System.Diagnostics.Debug.WriteLine(string.Format("订阅 GateWay/V1/{0}/PUB_RealTimeMsg 成功", Topic.Replace("\"", "")));
                ushort u2 = SubscribeClient.Subscribe(new string[] { string.Format("GateWay/V1/{0}/PUB_NetStatus", Topic.Replace("\"", "")) }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
                System.Diagnostics.Debug.WriteLine(string.Format("订阅 GateWay/V1/{0}/PUB_NetStatus 成功", Topic.Replace("\"", "")));
            }
            catch (Exception e)
            {
                new BrheinLog.Log.LogFactory().GetLog("MqttService").Error(true, e.Message);
                throw;
            }
           /* finally{

            }*/
            
        }
        /// <summary>
        /// 处理收到的消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Client_MqttMsgPublishReceived(object sender, MqttMsgPublishEventArgs e)
        {
            // 处理收到的消息
            System.Diagnostics.Debug.WriteLine(e.Message);
            new BrheinLog.Log.LogFactory().GetLog("MqttService").Info(true, e.Message);
            //_config.ResultValue = e.Message.ToString();
            //ClientPublishReceivedAction.Invoke(sender, e);

        }
        /// <summary>
        /// 发布
        /// </summary>
        /// <param name="Topic">主题</param>
        /// <param name="publishString">发布内容</param>
        public void Client_MqttMsgPublish(string Topic,string publishString)
        {
            SubscribeClient.Publish(string.Format("GateWay/V1/{0}/RealTimeMsg",Topic.Replace("\"", "")), Encoding.UTF8.GetBytes(publishString), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            //SubscribeClient.MqttMsgPublishReceived += Client_MqttMsgPublishReceived;
        }
    }
}

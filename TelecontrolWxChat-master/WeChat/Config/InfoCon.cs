using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace WeChat.Config
{
    /**
     * 在web.config里配置并注册InfoCon.config
     * 在InfoCon.config里写入所需字段
     * 在本文件中配置读写
     * */
    public class InfoCon:ConfigurationSection
    {
        private static ConfigurationProperty _property = new ConfigurationProperty(string.Empty, typeof(KeyValueElementCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);
        /// <summary>
        /// 配置列表
        /// </summary>
        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        private KeyValueElementCollection KeyValues
        {
            get { return (KeyValueElementCollection)base[_property]; }
            set { base[_property] = value; }
        }
        /// <summary>
        /// 主机地址
        /// </summary>
        public string BrokerHostName
        {
            get
            {
                //int _value = 0;
                if (KeyValues["brokerHostName"] == null) return string.Empty;
                else return KeyValues["brokerHostName"].Value;
            }
            set
            {
                if (KeyValues["brokerHostName"] == null) KeyValues["brokerHostName"] = new KeyValueElement() { Key = "brokerHostName", Value = value.ToString() };
                else KeyValues["brokerHostName"].Value = value.ToString();
            }
        }
        /// <summary>
        /// EMQ连接用户
        /// </summary>
        public string UserName
        {
            get
            {
                if (KeyValues["username"] == null) return string.Empty;
                else return KeyValues["username"].Value;
            }
            set
            {
                if (KeyValues["username"] == null) KeyValues["username"] = new KeyValueElement() { Key = "username", Value = value.ToString() };
                else KeyValues["username"].Value = value.ToString();
                
            }
        }
        /// <summary>
        /// EMQ连接密码
        /// </summary>
        public string PassWord
        {
            get
            {
                if (KeyValues["password"] == null) return string.Empty;
                else return KeyValues["password"].Value;
            }
            set
            {
                if (KeyValues["password"] == null) KeyValues["password"] = new KeyValueElement() { Key = "password", Value = value.ToString() };
                else KeyValues["password"].Value = value.ToString();

            }
        }
        /// <summary>
        /// 数据库连接字符
        /// </summary>
        public string ConnectionString
        {
            get
            {
                if (KeyValues["ConnectionString"] == null) return string.Empty;
                else return KeyValues["ConnectionString"].Value;
            }
            set
            {
                if (KeyValues["ConnectionString"] == null) KeyValues["ConnectionString"] = new KeyValueElement() { Key = "ConnectionString", Value = value.ToString() };
                else KeyValues["ConnectionString"].Value = value.ToString();

            }
        }
        /// <summary>
        /// 小程序APPID
        /// </summary>
        public string AppId
        {
            get
            {
                if (KeyValues["APP_ID"] == null) return string.Empty;
                else return KeyValues["APP_ID"].Value;
            }
            set
            {
                if (KeyValues["APP_ID"] == null) KeyValues["APP_ID"] = new KeyValueElement() { Key = "APP_ID", Value = value.ToString() };
                else KeyValues["APP_ID"].Value = value.ToString();
            }
        }
        /// <summary>
        /// 小程序APPSECRET
        /// </summary>
        public string AppSecret
        {
            get
            {
                if (KeyValues["APP_SECRET"] == null) return string.Empty;
                else return KeyValues["APP_SECRET"].Value;
            }
            set
            {
                if (KeyValues["APP_SECRET"] == null) KeyValues["APP_SECRET"] = new KeyValueElement() { Key = "APP_SECRET", Value = value.ToString() };
                else KeyValues["APP_SECRET"].Value = value.ToString();
            }
        }
        /// <summary>
        /// 弃用
        /// </summary>
        public string ResultValue
        {
            get
            {
                if (KeyValues["ResultValue"] == null) return string.Empty;
                else return KeyValues["ResultValue"].Value;
            }
            set
            {
                if (KeyValues["ResultValue"] == null) KeyValues["ResultValue"] = new KeyValueElement() { Key = "ResultValue", Value = value.ToString() };
                else KeyValues["ResultValue"].Value = value.ToString();
            }
        }

        public string GetUserAllEle
        {
            get
            {
                if (KeyValues["GetUserAllEle"] == null) return string.Empty;
                else return KeyValues["GetUserAllEle"].Value;
            }
            set
            {
                if (KeyValues["GetUserAllEle"] == null) KeyValues["GetUserAllEle"] = new KeyValueElement() { Key = "GetUserAllEle", Value = value.ToString() };
                else KeyValues["GetUserAllEle"].Value = value.ToString();
            }
        }

        public string PTP
        {
            get
            {
                if (KeyValues["PTP"] == null) return string.Empty;
                else return KeyValues["PTP"].Value;
            }
            set
            {
                if (KeyValues["PTP"] == null) KeyValues["PTP"] = new KeyValueElement() { Key = "PTP", Value = value.ToString() };
                else KeyValues["PTP"].Value = value.ToString();
            }
        }

        public string GetSceneMAC
        {
            get
            {
                if (KeyValues["GetSceneMAC"] == null) return string.Empty;
                else return KeyValues["GetSceneMAC"].Value;
            }
            set
            {
                if (KeyValues["GetSceneMAC"] == null) KeyValues["GetSceneMAC"] = new KeyValueElement() { Key = "GetSceneMAC", Value = value.ToString() };
                else KeyValues["GetSceneMAC"].Value = value.ToString();
            }
        }

        public string GetEleBoxMAC
        {
            get
            {
                if (KeyValues["GetEleBoxMAC"] == null) return string.Empty;
                else return KeyValues["GetEleBoxMAC"].Value;
            }
            set
            {
                if (KeyValues["GetEleBoxMAC"] == null) KeyValues["GetEleBoxMAC"] = new KeyValueElement() { Key = "GetEleBoxMAC", Value = value.ToString() };
                else KeyValues["GetEleBoxMAC"].Value = value.ToString();
            }
        }
        //GetControlPanelMAC
        public string GetControlPanelMAC
        {
            get
            {
                if (KeyValues["GetControlPanelMAC"] == null) return string.Empty;
                else return KeyValues["GetControlPanelMAC"].Value;
            }
            set
            {
                if (KeyValues["GetControlPanelMAC"] == null) KeyValues["GetControlPanelMAC"] = new KeyValueElement() { Key = "GetControlPanelMAC", Value = value.ToString() };
                else KeyValues["GetControlPanelMAC"].Value = value.ToString();
            }
        }
        public string GetGateWayMAC
        {
            get
            {
                if (KeyValues["GetGateWayMAC"] == null) return string.Empty;
                else return KeyValues["GetGateWayMAC"].Value;
            }
            set
            {
                if (KeyValues["GetGateWayMAC"] == null) KeyValues["GetGateWayMAC"] = new KeyValueElement() { Key = "GetGateWayMAC", Value = value.ToString() };
                else KeyValues["GetGateWayMAC"].Value = value.ToString();
            }
        }
        public string GetCreateTime
        {
            get
            {
                if (KeyValues["GetCreateTime"] == null) return string.Empty;
                else return KeyValues["GetCreateTime"].Value;
            }
            set
            {
                if (KeyValues["GetCreateTime"] == null) KeyValues["GetCreateTime"] = new KeyValueElement() { Key = "GetCreateTime", Value = value.ToString() };
                else KeyValues["GetCreateTime"].Value = value.ToString();
            }
        }
        public string GetHardWareMAC
        {
            get
            {
                if (KeyValues["GetHardWareMAC"] == null) return string.Empty;
                else return KeyValues["GetHardWareMAC"].Value;
            }
            set
            {
                if (KeyValues["GetHardWareMAC"] == null) KeyValues["GetHardWareMAC"] = new KeyValueElement() { Key = "GetHardWareMAC", Value = value.ToString() };
                else KeyValues["GetHardWareMAC"].Value = value.ToString();
            }
        }
        public string AP
        {
            get
            {
                if (KeyValues["aP"] == null) return string.Empty;
                else return KeyValues["aP"].Value;
            }
            set
            {
                if (KeyValues["aP"] == null) KeyValues["aP"] = new KeyValueElement() { Key = "aP", Value = value.ToString() };
                else KeyValues["aP"].Value = value.ToString();
            }
        }
    }
}
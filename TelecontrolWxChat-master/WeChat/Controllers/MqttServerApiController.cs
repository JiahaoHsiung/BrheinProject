using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;
using WeChat.Common;
using WeChat.Controllers.Base;
using WeChat.MysqlDBHelper;
using static WeChat.Common.ConverHelper;

namespace WeChat.Controllers
{
    public class MqttServerApiController : BaseController
    {
        MySqlDBHelper dbhelper = new MySqlDBHelper();
        // GET: MqttServerApi
        public ActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// mqtt订阅；
        /// 一般不会使用
        /// </summary>
        /// <param name="Topic"></param>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        public JsonResult Client(string Topic = "", string ClientId = "")
        {
            if (string.IsNullOrEmpty(Topic)||string.IsNullOrEmpty(ClientId))
                return Json(new { status = StatusCode.FAIL, message = "订阅失败" }, JsonRequestBehavior.AllowGet);
            var res = new MqttClientService(Topic,ClientId);
            bool session = res.SubscribeClient.CleanSession;
            bool Isconnected = res.SubscribeClient.IsConnected;
            return Json(new { status = Isconnected ? StatusCode.SUCCESS : StatusCode.FAIL, message = Isconnected ? "订阅成功" : "订阅失败"}, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// mqtt发布消息；
        /// 连接-订阅-发布
        /// </summary>
        /// <param name="Topic"></param>
        /// <param name="message"></param>
        /// <param name="ClientId"></param>
        /// <returns></returns>
        public JsonResult Publish(string Topic = "", string message = "", string ClientId = "")
        {
            if (string.IsNullOrEmpty(Topic) || string.IsNullOrEmpty(message)||string.IsNullOrEmpty(ClientId))
                return Json(new { status = StatusCode.FAIL, message = "发布失败" }, JsonRequestBehavior.AllowGet);
            else
            {
                try
                {
                    var res = new MqttClientService(Topic,ClientId);
                    res.Client_MqttMsgPublish(Topic, message);
                    return Json(new { status = StatusCode.SUCCESS, message = "发布成功" }, JsonRequestBehavior.AllowGet);
                }
                catch (Exception)
                {
                    return Json(new { status = StatusCode.FAIL, message = "发布失败" }, JsonRequestBehavior.AllowGet);
                    throw;
                }
            }
        }

        /// <summary>
        /// 根据情景ID获取MAC
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetSceneMAC(int? Id)
        {
            if (Id <= 0)
            {
                return Json(new { data = "", message = "数据异常" }, JsonRequestBehavior.AllowGet);
            }
            string sql = _config.GetSceneMAC + Id + _config.aP;
            var a = dbhelper.ExecuteScalar(sql);
            return Json(new { data = a, message = "数据正常" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据电箱ID获取MAC
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetEleBoxMAC(int? Id)
        {
            if (Id <= 0)
            {
                return Json(new { data = "", message = "数据异常" }, JsonRequestBehavior.AllowGet);
            }
            string sql = _config.GetEleBoxMAC + Id + _config.aP;
            var a = dbhelper.ExecuteScalar(sql);
            return Json(new { data = a, message = "数据正常" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据控制面板ID获取MAC
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>

        public JsonResult GetControlPanelMAC(int? Id)
        {
            if (Id <= 0)
            {
                return Json(new { data = "", message = "数据异常" }, JsonRequestBehavior.AllowGet);
            }
            string sql = _config.GetControlPanelMAC + Id + _config.aP;
            var a = dbhelper.ExecuteScalar(sql);
            return Json(new { data = a, message = "数据正常" }, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 根据网关ID获取MAC
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetGateWayMAC(int? Id)
        {
            if (Id <= 0)
            {
                return Json(new { data = "", message = "数据异常" }, JsonRequestBehavior.AllowGet);
            }
            string sql = _config.GetGateWayMAC + Id;
            var a = dbhelper.ExecuteScalar(sql);
            return Json(new { data = a, message = "数据正常" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetHardWareMAC(int? Id)
        {
            string sql = string.Format(_config.GetHardWareMAC, Id);
            var a = dbhelper.ExecuteScalar(sql);
            return Json(new { data = a, message = "数据正常" }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetCreateTime(string NickName = "")
        {
            string sql = _config.GetCreateTime + NickName;
            var result = dbhelper.ExecuteScalar(sql);
            var data = result.ToString();
            return Json(new { data, message = "数据正常" }, JsonRequestBehavior.AllowGet);
        }
    }
}




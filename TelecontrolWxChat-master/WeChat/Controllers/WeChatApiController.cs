using Customer_Manage.fitres;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using WeChat.Common;
using WeChat.Controllers.Base;
using WeChat.Models;
using WeChat.MysqlDBHelper;
using static WeChat.Common.ConverHelper;


namespace WeChat.Controllers
{
    public class WeChatApiController : BaseController
    {

        public ConverHelper util = new ConverHelper();
        MySqlDBHelper dbhelper = new MySqlDBHelper();
        //DataSetToList _dataSetToList = new DataSetToList();
        // GET: WeChatApi
        public ActionResult Index()
        {
            return View();
        }

        public string GetOpenId(string Code="")
        {
            string url = string.Format("https://api.weixin.qq.com/sns/jscode2session?appid=wxecb0d92db344561e&secret=d7d95cc0a72aa15895c5b083cc9e177d&grant_type=authorization_code&js_code={0}",Code);
            WebRequest WRequest = WebRequest.Create(url);
            WRequest.Method = "GET";
            WRequest.ContentType = "text/html;charset=UTF-8";
            WebResponse wResponse = WRequest.GetResponse();
            Stream stream = wResponse.GetResponseStream();
            StreamReader reader = new StreamReader(stream, System.Text.Encoding.Default);
            string str = reader.ReadToEnd();   //url返回的值               
            reader.Close();
            wResponse.Close();

            return str;
        }

        /// <summary>
        /// 获取该用户下所有网关
        /// </summary>
        /// <param name="openId">微信用户的唯一标识</param>
        /// <returns></returns>
        public JsonResult GetAllGateWay(string openId)
        {
           db.Configuration.ProxyCreationEnabled = false;
           var data = from u in db.iot_user
                       where (u.OpenId == openId)
                       select new
                       {
                           u.iot_gateway
                       };
            return Json(data, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 获取该用户信息
        /// </summary>
        /// <param name="openId">用户微信唯一标识号</param>
        /// <returns>用户信息的json数据形式返回</returns>
        public JsonResult GetUserInfo(string openId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.iot_user.Single(u => u.OpenId == openId);
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改网关名
        /// </summary>
        /// <param name="id">网关对应的ID</param>
        /// <param name="name">修改之后的名字</param>
        /// <returns></returns>
        public JsonResult EditGateWay(int id, string name)
        {
            //通过id获取到网关数据
            var data = db.iot_gateway.Where(g => g.ID == id).FirstOrDefault();
            data.Name = name;
            int uprows = db.SaveChanges();
            var taskinfo = new { status = StatusCode.SUCCESS, message = "修改成功" };

            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "修改失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除网关
        /// </summary>
        /// <param name="id">网关id号</param>
        /// <returns></returns>
        public JsonResult DelGateWay(int id)
        {
            var data = db.iot_gateway.Where(g => g.ID == id).FirstOrDefault();
            var taskinfo = new { status = 200, message = "删除成功" };

            if (data != null)
            {
                db.iot_gateway.Remove(data);
                int uprows = db.SaveChanges();

                if (uprows < 1)
                {
                    taskinfo = new { status = StatusCode.SUCCESS, message = "删除失败" };
                }
            }
            else
                taskinfo = new { status = StatusCode.NotFound, message = "所查数据不存在" };

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加网关
        /// </summary>
        /// <param name="model">传入的网关模型</param>
        /// <returns></returns>
        [Cors]
        [HttpPost]
        public JsonResult AddGateWay([System.Web.Http.FromBody]iot_gateway model)
        {
            if (util.IsExist(0, model.MAC))
            {
                return Json(new { status = StatusCode.FAIL, meassage = "网关MAC地址已被使用" });
            }
            //网关的基本信息
            iot_gateway gatway = new iot_gateway
            {
                MAC = model.MAC,
                Name = model.Name,
                CreateTime = new DateTime(),
                EditTIme = null
            };
            db.iot_gateway.Add(gatway);

            var taskinfo = new { status = StatusCode.SUCCESS, message = "添加成功" };
            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "添加失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }
        public JsonResult Test(string openId)
        {
            var user = db.iot_user.Where(u => u.OpenId == openId);
            
            RedirectToAction("AddUser", "WeChatApi", user.FirstOrDefault());
            return Json(new { },JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 添加一个用户
        /// </summary>
        /// <param name="model">传入的用户模型</param>
        /// <returns></returns>
        [Cors]
        [HttpPost]
        public JsonResult AddUser([System.Web.Http.FromBody]iot_user model)
        {
            int count = db.iot_user.Where(u => u.OpenId.Equals(model.OpenId)).Count();
            if (count != 0)
            {
                return Json(new { status = StatusCode.FAIL, meassage = "该用户已经注册" }); 
            }

            iot_user user = new iot_user
            {
                NickName = model.NickName,
                OpenId = model.OpenId,
                Gender = model.Gender,
                Province = model.Province,
                Country = model.Country,
                City = model.City,
                AvatarUrl = model.AvatarUrl,
                CreateTime = new DateTime().Date,
                EditTime = null
            };
            db.iot_user.Add(user);

            var taskinfo = new { status = StatusCode.SUCCESS, message = "添加用户成功" };
            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "添加用户失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 当用户登录时，获取到信息，进行一次数据更新
        /// </summary>
        /// <param name="openId">用户的开放id</param>
        /// <param name="model">从前端页面得到的模型数据</param>
        /// <returns></returns>
        [Cors]
        [HttpPost]
        public JsonResult EditUser(string openId, [System.Web.Http.FromBody] iot_user model)
        {
            var user = db.iot_user.Where(u => u.OpenId == openId).FirstOrDefault();
            var taskinfo = new { status = StatusCode.SUCCESS, message = "用户信息更新成功" };

            user.Province = model.Province;
            user.NickName = model.NickName;
            user.Gender = model.Gender;
            user.Country = model.Country;
            user.City = model.City;
            user.AvatarUrl = model.AvatarUrl;
            user.EditTime = new DateTime().Date;
            
            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "用户信息更新失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 用户和网关表进行绑定
        /// </summary>
        /// <param name="openId">微信的openId</param>
        /// <param name="MAC">传入网关的MAC地址</param>
        /// <returns></returns>
        public JsonResult UserBindGate(string openId, string MAC)
        {
            //给用户添加一个网关的绑定
            //获取到用户，
            var user = db.iot_user.Where(u => u.OpenId == openId).FirstOrDefault();
            //获取到网关，
            var GateWay = db.iot_gateway.Single(g => g.MAC.Equals(MAC));
            //插入一个网关信息进去
            user.iot_gateway.Add(GateWay);
            // 插入一个用户信息进去
            GateWay.iot_user.Add(user);

            var taskinfo = new { status = StatusCode.SUCCESS, message = "绑定成功" };
            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "绑定失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 添加一个软情景
        /// </summary>
        /// <param name="model">传入情景面板模型</param>
        /// <returns></returns>
        [Cors]
        [HttpPost]
        public JsonResult AddScene([System.Web.Http.FromBody] iot_scene model)
        {
            iot_scene scene = new iot_scene
            {
                Name = model.Name,

                GateWayId = model.GateWayId,
                ActionType = util.SetActionType(model.GateWayId),

                CreateTime = new DateTime(),
                EditTime = null
            };

            db.iot_scene.Add(scene);

            var taskinfo = new { status = StatusCode.SUCCESS, message = "添加成功" };
            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "添加失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 删除一个软情景
        /// </summary>
        /// <param name="id">该软情景的唯一标识</param>
        /// <returns></returns>
        public JsonResult DelScene(int id)
        {
            iot_scene scene = db.iot_scene.Find(id);
            db.iot_scene.Remove(scene);

            var taskinfo = new { status = StatusCode.SUCCESS, message = "删除成功" };
            int uprows = db.SaveChanges();

            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "删除失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 修改软情景的名字
        /// </summary>
        /// <param name="sceneId"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public JsonResult EditScene(int sceneId,string name)
        {
            iot_scene scene = db.iot_scene.Find(sceneId);
            scene.Name = name;

            var taskinfo = new { status = StatusCode.SUCCESS, message = "修改成功" };
            int uprows = db.SaveChanges();

            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "修改失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 软情景与设备进行绑定
        /// </summary>
        /// <param name="conId"></param>
        /// <param name="sceId"></param>
        /// <returns></returns>
        public JsonResult Scene_Contr(string conId, int sceId)
        {
            var data = util.SplitString(conId);
            var taskinfo = new { status = StatusCode.SUCCESS, message = "绑定成功" };
            //获取到相应的软情景和设备
            for (int i = 0; i < data.Length; i++)
            {
                var scene = db.iot_scene.Find(sceId);
                var con = db.iot_control_panel.Find(data[i]);

                if (con == null || scene == null)
                {
                    taskinfo = new { status = StatusCode.FAIL, message = "设备不存在，请检查" };
                    return Json(taskinfo, JsonRequestBehavior.AllowGet);
                }

                //将其互相绑定
                scene.iot_control_panel.Add(con);
                con.iot_scene.Add(scene);
            }


            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "绑定失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 获取该用户所有软情景
        /// </summary>
        /// <param name="openId">用户微信的唯一值</param>
        /// <returns></returns>
        public JsonResult GetAllUserScene(string openId)
        {
            //通过用户选择的网关Id进行该网关下情景的查询
            var sql = "SELECT * FROM iot_scene WHERE GateWayID IN ( " +
                                                        "SELECT GateWayID FROM iot_user_gateway WHERE UserId = (" +
                                                        $"SELECT Id FROM iot_user WHERE OpenId = '{openId}'))";
            var temp = db.Database.SqlQuery<iot_scene>(sql).ToList();


            if (temp == null)
            {
                return Json(new { status = StatusCode.NotFound, message = "未找到软情景信息" });
            }

            List<iot_scene> list = new List<iot_scene>();
            //将得到的数据使用result保存起来
            foreach (var item in temp)
            {
                var result = new iot_scene
                {
                    ID = item.ID,
                    GateWayId = item.GateWayId,
                    Name = item.Name,
                    CreateTime = item.CreateTime,
                    ActionType = item.ActionType,
                    EditTime = item.EditTime
                };
                list.Add(result);
            }

            return Json(list, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 软情景的详情
        /// </summary>
        /// <param name="sceneId">情景id值</param>
        /// <returns>该情景下的设备的所有信息的JSON数据</returns>
        public JsonResult SceneDetail(int sceneId)
        {
            db.Configuration.ProxyCreationEnabled = false;
            var data = from s in db.iot_scene
                       where s.ID == sceneId
                       select new
                       {
                           s.iot_control_panel
                       };

            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 清空指定软硬情景下绑定的所有设备
        /// </summary>
        /// <param name="sceneId">软情景id值</param>
        /// <param name="type">0 : 软情景   1： 情景面板</param>
        /// <returns></returns>
        public JsonResult UnbindAllScene(int sceneId,int type)
        {
            string typeName = "iot_scene_controlpanel";
            string typeId = "SceneId";
            if(type == 1)
            {
                typeName = "iot_hardwarescene_controlpanel";
                typeId = "HardwareSceneId";
            }
            string sql = $"delete from {typeName} where {typeId} = {sceneId}";
            var d = db.Database.ExecuteSqlCommand(sql);
            if(d == 0)
            {
                return Json(new {status = StatusCode.FAIL, message = "该情景下并不存在设备" }, JsonRequestBehavior.AllowGet);
            }
            return Json(new {status = StatusCode.SUCCESS,message = "清空成功" }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加一个强电箱信息进数据库   
        /// </summary>
        /// <param name="model">强电箱数据模型</param>
        /// <returns></returns>
        [Cors]
        [HttpPost]
        public JsonResult AddEle([System.Web.Http.FromBody] iot_elebox model)
        {
           
            if (util.IsExist(2,model.Uid))
            {
                return Json(new { status = StatusCode.FAIL, meassage = "该强电箱已被添加，请勿重复添加" });
            }
            iot_elebox ele = new iot_elebox();
            var taskinfo = new { status = StatusCode.SUCCESS, message = "添加成功" };
            //判断是否在空值表中存在该类型的position  如果有就将该position值赋给该模型 并且在空值表中删除该条目
            int position = util.GetGateWayPositon(2,model.GateWayId);

            if (position > 7)
                return Json(new {status = StatusCode.FAIL, message = "强电箱负载达到最大值"});
            ele.Position = position;
            ele.Name = model.Name;
            ele.Uid = model.Uid;
            ele.GateWayId = model.GateWayId;
            ele.CreateTime = new DateTime().Date;

            db.iot_elebox.Add(ele);


            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "添加失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 删除指定id 的强电箱 每个网关只能挂上八个强电箱
        /// </summary>
        /// <param name="id">强电箱的id值</param>
        /// <returns></returns>
        public JsonResult DelEle(int id)
        {
            var data = db.iot_elebox.Find(id);
            //判断是否删除的position是否是最后一个
            try
            {
                util.IsMaxPosition(2, data.Position,data.GateWayId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            //删除该强电箱
            db.iot_elebox.Remove(data);

            var taskinfo = new { status = StatusCode.SUCCESS, message = "删除成功" };
            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "删除失败，请检查是否存在该强电箱" };
            }
            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 更改电箱名称
        /// </summary>
        /// <param name="id">需要更改电箱的id</param>
        /// <param name="name">更改后电箱的名称</param>
        /// <returns></returns>
        [Cors]
        [HttpPost]
        public JsonResult UpEle(int id, string name)
        {
            var ele = db.iot_elebox.Find(id);
            //允许用户更改电箱命名
            ele.Name = name;
            ele.EditTime = new DateTime().Date;
            int uprows = db.SaveChanges();
            var taskinfo = new { status = StatusCode.SUCCESS, message = "电箱信息更新成功" };
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "电箱信息更新失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 获取该用户家中强电箱信息
        /// </summary>
        /// <param name="gateId">用户选择的网关</param>
        /// <returns></returns>
        public JsonResult GetEle(int gateId)
        {
            var data = from e in db.iot_elebox
                       where e.GateWayId == gateId
                       select new
                       {
                           Id = e.ID,
                           e.Uid,
                           e.Position,
                           e.GateWayId,
                           e.Name,
                           e.CreateTime,
                           e.EditTime
                       };

            if (data == null)
            {
                return Json(new { statu = StatusCode.NotFound, message = "获取电箱信息为空，请检查是否存在电箱信息" });
            }

            return Json(data.ToList(), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 添加设备
        /// </summary>
        /// <param name="model">用户扫码后进行数据录入后的数据</param>
        /// <returns></returns>
        [Cors]
        [HttpPost]
        public ActionResult AddControlPanel([System.Web.Http.FromBody]ControleDevice model)
        {
            var taskinfo = new { status = StatusCode.SUCCESS, message = "添加成功" };
            int uprows1 = 0;

            var count = db.iot_control_panel.Where(s => s.Uid.Equals(model.Uid));
            if (count.Count() > 0)
            {
                return Json(new { status = StatusCode.FAIL, meassage = "该设备已被添加，请勿重复添加" });
            }


            iot_control_panel device = new iot_control_panel
            {
                GateWayID = model.GateWayId,
                Name = model.Name,
                Uid = model.Uid,
                Position = util.GetGateWayPositon(1, model.GateWayId)
            };
            if (device.Position > 15)
            {
                return Json(new { status = StatusCode.FAIL, message = "设备负载达到最大值" });
            }
                device.KeyNumber = model.KeyNumber;
                device.ParentID = 0;
                //判断该网关该类设备数量是否达到最大值
               
                db.iot_control_panel.Add(device);
                int uprows = db.SaveChanges();
                int MaxID = db.iot_control_panel.Max(c => c.ID);
               if (model.KeyNumber > 0)
                {
                    for (int i=0;i<model.KeyNumber;i++)
                    {
                    iot_control_panel cpanel = new iot_control_panel
                    {
                        GateWayID = model.GateWayId,
                        Name = model.Name + "-按键" + (i + 1).ToString(),
                        Position = 0,
                        Uid = model.Uid,
                        KeyNumber = 1,
                        ParentID = MaxID
                    };
                    db.iot_control_panel.Add(cpanel);
                        uprows1 = db.SaveChanges();
                     }
                }

                if (uprows < 1)
                {
                    if (uprows1 < 1)
                    {
                      taskinfo = new { status = StatusCode.FAIL, message = "子设备绑定失败" };
                    }
                    taskinfo = new { status = StatusCode.FAIL, message = "添加失败" };
                }
            return Json(taskinfo, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 修改设备名
        /// </summary>
        /// <param name="id">设备id</param>
        /// <param name="name">设备名称</param>
        /// <returns></returns>
        public ActionResult EditControlPanel(int id, string name)
        {
            //获取该分控开关
            var data = db.iot_control_panel.Where(c => c.ID == id).FirstOrDefault();
            //获取分控开关下的分控按键
            var data_temp = db.iot_control_panel.Where(c => c.ParentID == id);

            List<iot_control_panel> temp = data_temp.ToList();

            string temp_name = name + "-按键";

            if (data != null)
            {
                data.Name = name;
            }

            for (int i = 0; i < temp.Count; i++)
            {
                temp[i].Name = temp_name + (i + 1);
            }


            int uprows = db.SaveChanges();

            var taskinfo = new { status = StatusCode.SUCCESS, message = "修改成功" };
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "修改失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 删除设备
        /// </summary>
        /// <param name="id">设备id</param>
        /// <returns></returns>
        public ActionResult DelControlPanel(int id)
        {
            var data = db.iot_control_panel.Where(c => c.ID == id).FirstOrDefault();

           

            if (data != null)
            {
                var chidrendata = db.iot_control_panel.Where(c => c.ParentID == id);

                foreach (var item in chidrendata)
                {
                    try
                    {
                        db.iot_control_panel.Remove(item);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex.Message);
                    }
                }
                db.iot_control_panel.Remove(data);
                util.IsMaxPosition(1, data.Position,data.GateWayID);
            }
            int uprows = db.SaveChanges();

            var taskinfo = new { status = StatusCode.SUCCESS, message = "删除成功" };
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "删除失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 查询网关下所有设备
        /// </summary>
        /// <param name="gatewayid">该用户所选网关的id值</param>
        /// <returns></returns>
        public ActionResult SelControlPanel(int gatewayid)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;
            var data = db.iot_control_panel.Where(c => c.GateWayID == gatewayid).GroupBy(c => c.Uid);
            List<Device> list = new List<Device>();

            if (data != null)
            {
                foreach (var item in data.ToList())
                {
                    foreach (var row in item)
                    {
                        if (row.ParentID == 0)
                        {
                            Device model = new Device();
                            var cpanel = db.iot_control_panel.Where(c => c.ParentID == row.ID);
                            model.ID = row.ID;
                            model.GateWayID = row.GateWayID;
                            model.KeyNumber = row.KeyNumber;
                            model.Name = row.Name;
                            model.Position = row.Position;
                            model.Uid = row.Uid;

                            List<DeviceDetail> dlist = new List<DeviceDetail>();
                            foreach (var rowname in cpanel)
                            {
                                DeviceDetail detail = new DeviceDetail
                                {
                                    ID = rowname.ID,
                                    Name = rowname.Name
                                };
                                dlist.Add(detail);
                            }
                            model.list = dlist;
                            list.Add(model);
                        }
                        if (row.ParentID == -1)
                        {
                            Device model = new Device();
                            var cpanel = db.iot_control_panel.Where(c => c.ParentID == row.ID);
                            model.ID = row.ID;
                            model.GateWayID = row.GateWayID;
                            model.KeyNumber = row.KeyNumber;
                            model.Name = row.Name;
                            model.Position = row.Position;
                            model.Uid = row.Uid;
                            list.Add(model);
                        }

                    }

                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }

            /// <summary>
            /// 得到该用户所有网关下的所有开关插座设备
            /// </summary>
            /// <param name="openId">openId</param>
            public JsonResult GetUserAllCon(string openId)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            //使用SQl语句嵌套查询
            string sql = "SELECT * FROM iot_control_panel WHERE GateWayID IN ( " +
                                                        "SELECT GateWayID FROM iot_user_gateway WHERE UserId = (" +
                                                        $"SELECT UserId FROM iot_user WHERE OpenId = '{openId}' GROUP BY UserId))";
            var temp = db.Database.SqlQuery<iot_control_panel>(sql).ToList();

            if(temp == null)
            {
                return Json(new {statu = StatusCode.NotFound, Message = "未找到任何设备" },JsonRequestBehavior.AllowGet);
            }
            List<Device> list = new List<Device>();

            if (temp != null)
            {
                foreach (var item in temp)
                {
                      
                        if (item.ParentID == 0)
                        {
                            Device model = new Device();
                            var cpanel = db.iot_control_panel.Where(c => c.ParentID == item.ID);
                            model.ID = item.ID;
                            model.GateWayID = item.GateWayID;
                            model.KeyNumber = item.KeyNumber;
                            model.Name = item.Name;
                            model.Position = item.Position;
                            model.Uid = item.Uid;

                            List<DeviceDetail> dlist = new List<DeviceDetail>();
                            foreach (var rowname in cpanel)
                            {
                            DeviceDetail detail = new DeviceDetail
                            {
                                ID = rowname.ID,
                                Name = rowname.Name
                            };
                            dlist.Add(detail);
                            }
                            model.list = dlist;
                            list.Add(model);
                        }
                        if (item.ParentID == -1)
                        {
                            Device model = new Device();
                            var cpanel = db.iot_control_panel.Where(c => c.ParentID == item.ID);
                            model.ID = item.ID;
                            model.GateWayID = item.GateWayID;
                            model.KeyNumber = item.KeyNumber;
                            model.Name = item.Name;
                            model.Position = item.Position;
                            model.Uid = item.Uid;
                            list.Add(model);
                        }
                   
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);

        }

        /// <summary>
        /// 查询该用户所有网关下所有情景面板
        /// </summary>
        /// <param name="openId">用户微信的唯一值</param>
        /// <returns></returns>
        public JsonResult GetUserAllScene(string openId)
        {
            //使用SQl语句嵌套查询
            string sql = "SELECT * FROM iot_scene_panel WHERE GateWayID IN ( " +
                                                        "SELECT GateWayID FROM iot_user_gateway WHERE UserId = (" +
                                                        $"SELECT UserId FROM iot_user WHERE OpenId = '{openId}'))";
            var temp = db.Database.SqlQuery<iot_scene_panel>(sql).ToList();

            if (temp == null)
            {
                return Json(new { statu = StatusCode.NotFound, Message = "未找到任何设备" }, JsonRequestBehavior.AllowGet);
            }
            List<iot_scene_panel> list = new List<iot_scene_panel>();
            //将得到的数据使用result保存起来
            foreach (var item in temp)
            {
                var result = new iot_scene_panel
                {
                    ID = item.ID,
                    GateWayId = item.GateWayId,
                    Name = item.Name,
                    Uid = item.Uid,
                    Position = item.Position,
                    CreateTime = item.CreateTime,
                    EditTime = item.EditTime
                };
                list.Add(result);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// 获取用户所有网关下的所有的强电箱
        /// </summary>
        /// <param name="openId">用户微信唯一Id值</param>
        /// <returns></returns>
        public JsonResult GetUserAllEle(string openId)
        {
            //使用SQl语句嵌套查询
            string sql = _config.GetUserAllEle + openId + _config.PTP;
            /*"SELECT  iot_elebox.*,MAC FROM iot_elebox LEFT JOIN iot_gateway ON iot_elebox.GateWayId =iot_gateway.ID WHERE GateWayID IN ( " +
                                                        "SELECT GateWayID FROM iot_user_gateway WHERE UserId = (" +
                                                        $"SELECT Id FROM iot_user WHERE OpenId = '{openId}'))";*/
            var temp = db.Database.SqlQuery<iot_elebox>(sql).ToList();

            if (temp == null)
            {
                return Json(new { statu = StatusCode.NotFound, Message = "未找到任何设备" }, JsonRequestBehavior.AllowGet);
            }
            List<iot_elebox> list = new List<iot_elebox>();
            //将得到的数据使用result保存起来
            foreach (var item in temp)
            {
                var result = new iot_elebox
                {
                    ID = item.ID,
                    GateWayId = item.GateWayId,
                    Name = item.Name,
                    Uid = item.Uid,
                    Position = item.Position,
                    CreateTime = item.CreateTime,
                    MAC=item.MAC,
                    EditTime = item.EditTime
                };
                list.Add(result);
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 用户网关下硬情景
        /// </summary>
        /// <param name="gatewayid">用户的网关id值</param>
        /// <returns></returns>
        public ActionResult UserToScenePanel(int gatewayid)
        {
            db.Configuration.ProxyCreationEnabled = false;
            db.Configuration.LazyLoadingEnabled = false;
            var data = db.iot_scene_panel.Where(s => s.GateWayId == gatewayid).ToList();
            return Json(data,JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 硬情景详情
        /// </summary>
        /// <param name="penelid"></param>
        /// <returns></returns>
        public ActionResult ScenePanelDatail(int penelid)
        {
            var data = db.iot_hardware_scene.Where(h => h.ScenePanelId == penelid);

            List<ScenePanelInfo> list = new List<ScenePanelInfo>();

            if (data != null)
            {
                foreach (var item in data)
                {
                    ScenePanelInfo model = new ScenePanelInfo
                    {
                        ID = item.ID,
                        Name = item.Name
                    };
                    string NodeName ="按键"+(item.NodeNum+1).ToString();
                    model.NodeNum = NodeName;
                    model.Type=item.Type==0?"只关":"只开";
                    list.Add(model);
                }
            }
            return Json(list, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 添加硬情景面板（一种硬件设备）
        /// </summary>
        /// <param name="model">硬情景面板</param>
        /// <returns></returns>
        [HttpPost]
        [Cors]
        public JsonResult AddScenePanel([System.Web.Http.FromBody] iot_scene_panel model)
        {
            var count = db.iot_scene_panel.Where(s => s.Uid.Equals(model.Uid));
            if (count.Count() > 0)
            {
                return Json(new { status = StatusCode.FAIL, meassage = "该设备已被添加，请勿重复添加" });
            }
            iot_scene_panel data =
                new iot_scene_panel
                {

                    //网关信息为用户选择绑定在那个网关
                    GateWayId = model.GateWayId,
                    Name = model.Name,
                    //Uid为用户使用二维码扫描后自动获取
                    Uid = model.Uid,
                    CreateTime = new DateTime().Date,
                    EditTime = null
                };
            //判断是否需要从空值表中得到position数据
            int position = util.GetGateWayPositon(3, model.GateWayId);
            data.Position = position;

            //进行判断该网关下该设备是否达到承载最大值
            if (position > 15)
            {
                return Json(new { status = StatusCode.FAIL, message = "情景面板负载达到最大值" });
            }
            db.iot_scene_panel.Add(data);
            int uprows = db.SaveChanges();

            var taskinfo = new { status = StatusCode.SUCCESS, message = "添加成功" };
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "添加失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 允许用户对情景面板的名称
        /// </summary>
        /// <param name="id">用户选择的需要更改的情景面板的Id值</param>
        /// <param name="name">用户更改的名称</param>
        /// <returns></returns>
        public JsonResult EditScenepanel(int id, string name)
        {
            //通过id获得情景面板实例，将其属性进行修改
            var data = db.iot_scene_panel.Find(id);
            data.Name = name;

            int uprows = db.SaveChanges();

            var taskinfo = new { status = StatusCode.SUCCESS, message = "修改成功" };
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "修改失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 根据id值进行情景面板信息的删除
        /// </summary>
        /// <param name="id">情景面板的id值</param>
        /// <returns></returns>
        public JsonResult DelScenePanel(int id)
        {
            var data = db.iot_scene_panel.Find(id);
            if(data == null)
            {
                return Json(new { status = StatusCode.NotFound, message = "未找到该情景面板信息" }, JsonRequestBehavior.AllowGet);
            }
            //判断是否需要在blank表中插入数据
            try
            {
                util.IsMaxPosition(3, data.Position,data.GateWayId);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            db.iot_scene_panel.Remove(data);

            int uprows = db.SaveChanges();

            var taskinfo = new { status = StatusCode.SUCCESS, message = "删除成功" };
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "删除失败" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 使软情景和设备进行解绑
        /// </summary>
        /// <param name="sceneId">软情景的id</param>
        /// <param name="cpId">设备id</param>
        /// <returns></returns>
        public JsonResult DisBindScene(int sceneId, int cpId)
        {
            var sce = db.iot_scene.Find(sceneId);
            var cp = db.iot_control_panel.Find(cpId);

            sce.iot_control_panel.Remove(cp);

            int uprows = db.SaveChanges();

            var taskinfo = new { status = StatusCode.SUCCESS, message = "解绑成功" };
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "解绑失败" };
            }
            else
            {
                taskinfo = new { status = StatusCode.FAIL, message = "已绑定此设备" };
            }

            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }



        /// <summary>
        /// 硬情景设备解绑
        /// </summary>
        /// <param name="deviceid"></param>
        /// <param name="sceneid"></param>
        /// <returns></returns>
        public ActionResult SPNodeUnbundLing(int deviceid,int sceneid)
        {
            var data = db.iot_hardware_scene.Single(s => s.ID == sceneid).iot_control_panel.Where(c => c.ID == deviceid);
            var taskinfo = new { status = StatusCode.SUCCESS, message = "解绑成功" };
            if (data != null)
            {
                var cpanel = db.iot_control_panel.Where(c => c.ID == deviceid).First();

                db.iot_hardware_scene.Single(h => h.ID == sceneid).iot_control_panel.Remove(cpanel);

                int uprows = db.SaveChanges();

                if (uprows < 1)
                {
                    taskinfo = new { status = StatusCode.FAIL, message = "解绑失败" };
                }
            }
            else
            {
                taskinfo = new { status = StatusCode.FAIL, message = "已绑定此设备" };
            }
            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }


        /// <summary>
        /// 添加一个情景面板按钮信息
        /// </summary>
        /// <param name="model">按钮模型</param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult AddHardScene([System.Web.Http.FromBody]iot_hardware_scene model)
        {
            iot_hardware_scene hc = new iot_hardware_scene();
            int scenePanelId = model.ScenePanelId; //该按钮属于的情景面板的id值
            int nodeNum = model.NodeNum; //情景面板有两个按钮版本和四个按钮版本，该属性用于标识该按钮为情景面板上的第几个按钮
            //判断是否已经添加过该按键，杜绝重复添加
            var count = db.iot_hardware_scene.Where(h => h.NodeNum == nodeNum && h.ScenePanelId == scenePanelId);
            if(count.Count() > 0)
            {
                return Json(new {status = StatusCode.FAIL, message = "已添加该按键" }, JsonRequestBehavior.DenyGet);
            }

            hc.Type = 0; //该按钮操作的是让所管辖的设备全关还是全开，  0全开  1全关    添加时给默认值0
            hc.Name = "默认情景"; //按钮对应情景的名称、添加时给默认值
            hc.NodeNum = nodeNum;
            hc.ScenePanelId = scenePanelId;
            hc.CreateTime = new DateTime().Date;
            hc.EditTime = null;
            db.iot_hardware_scene.Add(hc);

            int uprows = db.SaveChanges();
            var taskinfo = new { status = StatusCode.SUCCESS, message = "添加成功" };
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "添加失败" };
            }
            return Json(taskinfo, JsonRequestBehavior.DenyGet);
        }

        /// <summary>
        /// 使一个设备（灯）与情景面板上的一个按钮（情景）进行绑定
        /// </summary>
        /// <param name="conId">设备的id值</param>
        /// <param name="hardId">按钮的id值</param>
        /// <returns></returns>
        public JsonResult ConBindHardScene(int hardId, string conId)
        {
            var data = util.SplitString(conId);
            var taskinfo = new { status = StatusCode.SUCCESS, message = "绑定成功" };
            //使用提供的id值获取到两者的entity
            for (int i = 0; i < data.Length;i++)
            { 
            var con = db.iot_control_panel.Find(data[i]);
            var hard = db.iot_hardware_scene.Find(hardId); 
                if (con == null || hard == null)
                {
                    taskinfo = new { status = StatusCode.FAIL, message = "设备不存在，请检查" };
                    return Json(taskinfo, JsonRequestBehavior.AllowGet);
                }
            //将两者进行绑定
            con.iot_hardware_scene.Add(hard);
            hard.iot_control_panel.Add(con);
            }
            
            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "绑定失败" };
            }
            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 允许用户修改按键对应情景的名称和类型值
        /// </summary>
        /// <param name="id">按键的id值</param>
        /// <param name="name">修改的名称</param>
        /// <param name="type">修改后的类型值</param>
        /// <returns></returns>
        public JsonResult EditHardScene(int id, string name,int type)
        {
            var data = db.iot_hardware_scene.Find(id);
            data.Name = name;
            data.Type = type;
            data.EditTime = new DateTime().Date;

            var taskinfo = new { status = StatusCode.SUCCESS, message = "修改成功" };
            int uprows = db.SaveChanges();
            if (uprows < 1)
            {
                taskinfo = new { status = StatusCode.FAIL, message = "修改失败" };
            }
            return Json(taskinfo, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 情景面板上的一个按钮的详细信息
        /// </summary>
        /// <param name="scenePanelId">该按键所在情景面板的id值</param>
        /// <param name="nodeNum">该按键为情景面板的第几个按键</param>
        /// <returns></returns>
        public JsonResult GetNodeDetail(int scenePanelId, int nodeNum)
        {
            db.Configuration.LazyLoadingEnabled = false;
            db.Configuration.ProxyCreationEnabled = false;

            var data = from hs in db.iot_hardware_scene
                       where hs.ScenePanelId == scenePanelId && hs.NodeNum == nodeNum
                       select new
                       {
                           hs.ID,
                           hs.ScenePanelId,
                           hs.Name,
                           hs.Type,
                           hs.iot_control_panel
                       };
            if(data == null)
            {
                return Json(new { status = StatusCode.NotFound, message = "该按钮信息尚未添加" });
            }

            return Json(data.ToList(), JsonRequestBehavior.AllowGet);

        }

    }
}
using System.Collections.Generic;
using System.Linq;
using WeChat.Controllers.Base;

namespace WeChat.Common
{
    public class ConverHelper : BaseController
    {
        /// <summary>
        /// 规定的返回代码
        /// </summary>
        public static class StatusCode
        {
            public static int SUCCESS = 200;//请求(或处理)成功

            public static int FAIL = 401;//内部请求出错

            public static int NotFound = 404;//请求信息未找到

            public static int ParameterError = 400;//请求参数不完整或不正确

            public static int HttpMehtodError = 405;//HTTP请求类型不合法

            public static int URLExpireError = 407;//HTTP请求不合法
        }


        /// <summary>
        /// 获取可用设备位置
        /// </summary>
        /// <param name="type">1设备  2强电箱  3情景面板</param>
        /// <param name="gateId">该设备所处在的网关id</param>
        /// <returns></returns>
        public int GetGateWayPositon(int type, int gateId)
        {
            int position = 0;

            switch (type)
            {
                case 1:
                    var isNullPosition = db.iot_blank_position.Where(b => b.Type == 1 && b.GateWayID == gateId);
                    if (isNullPosition.Count() > 0) 
                    {
                        position = isNullPosition.Min(p => p.Position);
                        var NullPosition = isNullPosition.Where(p => p.Position == position && p.GateWayID == gateId).First();
                        db.iot_blank_position.Remove(NullPosition);
                    }
                    else
                    {
                        var data = db.iot_control_panel;
                        if (data.Count() > 0)
                        {
                            var cpannel = data.Where(c => c.GateWayID == gateId);
                            if (cpannel.Count() > 0)
                            {
                                position = cpannel.Max(c => c.Position) + 1;
                            }
                        }
                    }
                    break;
                case 2:
                    var isNullPosition1 = db.iot_blank_position.Where(b => b.Type == 2 && b.GateWayID == gateId);
                    if (isNullPosition1.Count() > 0)
                    {
                        position = isNullPosition1.Min(p => p.Position);
                        var NullPosition = isNullPosition1.Where(p => p.Position == position && p.GateWayID == gateId).First();
                        db.iot_blank_position.Remove(NullPosition);
                    }
                    else
                    {
                        var data = db.iot_elebox;
                        if (data.Count() > 0)
                        {
                            var ebox = data.Where(e => e.GateWayId == gateId);
                            if (ebox.Count() > 0)
                            {
                                position = ebox.Max(e => e.Position) + 1;
                            }
                        }
                    }
                    break;
                case 3:
                    var isNullPosition2 = db.iot_blank_position.Where(b => b.Type == 3 && b.GateWayID == gateId);
                    if (isNullPosition2.Count() > 0)
                    {
                        position = isNullPosition2.Min(p => p.Position);
                        var NullPosition = isNullPosition2.Where(p => p.Position == position && p.GateWayID == gateId).First();
                        db.iot_blank_position.Remove(NullPosition);
                    }
                    else
                    {
                        var data = db.iot_scene_panel;
                        if (data.Count() > 0)
                        {
                            var cpanel = data.Where(s => s.GateWayId == gateId);
                            if (cpanel.Count() > 0)
                            {
                                position = cpanel.Max(c => c.Position) + 1;
                            }
                        }
                    }
                    break;
            }
            db.SaveChanges();
            return position;
        }




        /// <summary>
        /// 设备在删除时判断是否是删除最大的posion哪一个
        /// </summary>
        /// <param name="type">设备的类型, 1.设备  2.强电箱  3.情景面板</param>
        /// <param name="position">所要删除的posion</param>
        /// <param name="gateId">该设备所处在的网关id</param>
        /// <returns></returns>
        public void IsMaxPosition(int type, int position, int gateId)
        {
            int Max;

            //通过类型进行判断操作的表为那张,那个网关下的那个数据

            switch (type)
            {
                case 1:
                    Max = db.iot_control_panel.Where(c => c.GateWayID == gateId).Max(c => c.Position);
                    break;
                case 2:
                    Max = db.iot_elebox.Where(e => e.GateWayId == gateId).Max(e => e.Position);
                    break;
                case 3:
                    Max = db.iot_scene_panel.Where(sc => sc.GateWayId == gateId).Max(s => s.Position);
                    break;
                default:
                    Max = 99;
                    break;
            }

            if (Max == position)
            {
                return;
            }
            iot_blank_position blank = new iot_blank_position();
            blank.Position = position;
            blank.Type = type;
            blank.GateWayID = gateId;
            db.iot_blank_position.Add(blank);
            db.SaveChanges();
        }

        public void UpUser(iot_user model)
        {
            var data = db.iot_user.Where(u => u.OpenId == model.OpenId).FirstOrDefault();

            if (data != null)
            {
                if (data.AvatarUrl != model.AvatarUrl)
                {
                    data.AvatarUrl = model.AvatarUrl;
                }
                if (data.NickName != model.NickName)
                {
                    data.NickName = model.NickName;
                }
            }
            int uprows = db.SaveChanges();
        }


        /// <summary>
        /// 判断网关的MAC地址，情景面板、开关、强电箱的Uid 地址是否已经存在
        /// </summary>
        /// <param name="type">类型判定，0：网关 1：开关 2：强电箱 3：情景面板</param>
        /// <param name="date">传入的需要判断的MAC地址或Uid值</param>
        public bool IsExist(int type, string date)
        {
            bool result = false;
            string typeName = "Uid";
            string table = "";
            switch (type)
            {
                case 0: table = "iot_gateway"; typeName = "MAC"; break;
                case 1: table = "iot_control_panel"; break;
                case 2: table = "iot_elebox"; break;
                case 3: table = "iot_scene_panel"; break;
                default:
                    break;
            }
            string sql = $"SELECT COUNT(*) FROM {table} WHERE { typeName} = '{ date}'";
            var temp = db.Database.SqlQuery<int>(sql);
            int count = temp.First();

            if (count > 0)
            {
                result = true;
            }

            return result;
        }
        /// <summary>
        /// 将传入的string数组转换成int数组
        /// </summary>
        /// <param name="data">传入的string</param>
        /// <returns>被切割之后的数组</returns>
        public int[] SplitString(string data)
        {
            var temp = data.Split(',');
            int[] result = new int[temp.Length];
            for (int i = 0; i < temp.Length; i++)
            {
                result[i] = int.Parse(temp[i]);
            }

            return result;
        }

        /// <summary>
        /// 所谓的ActionType是软情景的位置，使用的原来废弃的没有用的字段顶替 
        /// </summary>
        /// <param name="gateWayId">网关ID</param>
        /// <returns></returns>
        public int SetActionType(int? gateWayId)
        {
            //得到该网关下的所有软情景的所有的ActionType值
            string sql = $"select ActionType from iot_scene where GateWayId = {gateWayId};";
            //将其List化，并且进行升序排序
            List<int> data = db.Database.SqlQuery<int>(sql).ToList();
            if(data.Count() == 0)
            {
                return 0;
            }

            data.Sort();
            //进行循环，当两者匹配时，  即 data[0] = 0 继续下一轮循环，一旦出现不匹配，就代表这个i值是空的
            //将其返回回去就可以
            for(int i = 0; i < data.Count(); i++)
            {
                if(data[i] == i)
                {
                    continue;
                }
                else
                {
                    return i;
                }
            }
            //如果所有都匹配后，跳出了循环，则表示没有空位，直接给下一个值就可以了
            int max  = data.Max();
            max += 1;
            return max;
        }
    }
}
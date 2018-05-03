using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.Models
{
    public class ControleDevice
    {
        /// <summary>
        /// 网关id
        /// </summary>
        public int GateWayId { get; set; }

        /// <summary>
        /// 唯一id
        /// </summary>
        public string Uid { get; set; }


        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 是否多控开关
        /// </summary>
        public int IsMultiControl { get; set; }

        /// <summary>
        /// 按键数
        /// </summary>
        public int KeyNumber { get; set; }  

    }
}
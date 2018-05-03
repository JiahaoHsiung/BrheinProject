using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WeChat.Models
{
    public class Device
    {
       public int ID { get; set; }

       public int GateWayID { get; set; }

       public string Uid { get; set; }

       public int? KeyNumber { get; set; }

       public int Position { get; set; }

       public string Name { get; set; }

       public List<DeviceDetail> list { get; set; }
    }

    public class DeviceDetail
    {
        public int ID { get; set; }

        public string Name { get; set; }
    }
}
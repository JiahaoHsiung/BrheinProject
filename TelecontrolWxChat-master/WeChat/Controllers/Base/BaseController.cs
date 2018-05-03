using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WeChat.Controllers.Base
{
    public class BaseController:Controller
    {
        protected static Config.InfoCon _config = System.Web.Configuration.WebConfigurationManager.OpenWebConfiguration("~").GetSection("InfoCon") as WeChat.Config.InfoCon;
        public brhein_iotEntities db = new brhein_iotEntities();
    }
}
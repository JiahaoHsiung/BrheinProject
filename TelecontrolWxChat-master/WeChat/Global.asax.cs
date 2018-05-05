using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;

namespace WeChat
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);

            //注册 log4net
            //log4net.Config.XmlConfigurator.Configure(
            //    new System.IO.FileInfo(AppDomain.CurrentDomain.BaseDirectory+ "/log4net.config")
             //   );
           // GlobalConfiguration.Configuration.Filters.Add(new Log.ApiTrackerFilter());
        }
    }
}

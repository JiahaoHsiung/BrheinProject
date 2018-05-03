using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Customer_Manage.fitres
{
    public class LoginAttribute : AuthorizeAttribute
    {


    }




    public class CorsAttribute : AuthorizeAttribute
    {
        public override void OnAuthorization(AuthorizationContext context)
        {
            if (context.HttpContext.Request.HttpMethod == "OPTIONS")
            {
                var cust_result = new JsonResult()
                {
                    Data = new { Status = 202, Msg = "过滤OPTIONS请求" },
                };
                context.Result = cust_result;
            }
        }
    }
}
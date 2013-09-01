using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AjaxTest.App_Code
{
   
    /// <summary>
    /// Handler 的摘要说明
    /// </summary>
    public class Handler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            string name=context.Request.Params[""].ToString();
            string pwd = context.Request.Params[""].ToString();
            if (name != "lms"&&pwd!="lms")
                context.Response.Write("用户名或密码错误。");
            else
            {
                context.Response.Write("正确，进入系统。");
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
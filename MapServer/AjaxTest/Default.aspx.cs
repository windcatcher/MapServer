using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace AjaxTest
{
    public partial class _Default : System.Web.UI.Page, ICallbackEventHandler
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            sCallBackFunctionInvocation = Page.ClientScript.GetCallbackEventReference(this, "message",
               "ShowServerTime", "context");
        }
        public string sCallBackFunctionInvocation;
        public string GetCallbackResult()
        {
            return DateTime.Now.ToString();
        }

        public void RaiseCallbackEvent(string eventArgument)
        {
           
        }
    }
}
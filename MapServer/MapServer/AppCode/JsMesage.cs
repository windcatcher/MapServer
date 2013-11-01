using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MapServer.AppCode
{
    public class JsMesage
    {
        public JsMesage()
        {
            //
            // TODO: 在此处添加构造函数逻辑
            //
        }
        public static void ShowMessage(string strMsg)
        {
            System.Web.HttpContext.Current.Response.Write("<Script Language='JavaScript'>window.alert('" + strMsg + "');</script>");
        }
        public static void ShowMessage(System.Web.UI.Page page, string strMsg)
        {
            page.Response.Write("<Script Language='JavaScript'>window.alert('" + strMsg + "');</script>");
        }
        public static void ShowMessage(string strMsg, string Url)
        {
            System.Web.HttpContext.Current.Response.Write("<Script Language='JavaScript'>window.alert('" + strMsg + "');window.location.href ='" + Url + "'</script>");
        }
        public static void ShowMessage(System.Web.UI.Page page, string strMsg, string Url)
        {
            page.Response.Write("<Script Language='JavaScript'>window.alert('" + strMsg + "');window.location.href ='" + Url + "'</script>");
        }
        public static void ShowConfirm(string strMsg, string strUrl_Yes, string strUrl_No)
        {
            System.Web.HttpContext.Current.Response.Write("<Script Language='JavaScript'>if ( window.confirm('" + strMsg + "')) {  window.location.href='" + strUrl_Yes +
                              "' } else {window.location.href='" + strUrl_No + "' };</script>");
        }
    }

}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESRI.ArcGIS.ADF.Web.UI.WebControls;

namespace MapServer.Common
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

        /// <summary>
        /// 弹出消息框
        /// </summary>
        /// <param name="map">ADFMap控件</param>
        /// <param name="msg">消息</param>
        /// <returns>回调字符</returns>
        public static string ShowMessage(ESRI.ArcGIS.ADF.Web.UI.WebControls.Map map, string msg)
        {
            object[] objs = new object[1];
            string sa = string.Format("alert('{0}')", msg) ;
            objs[0] = sa;
            CallbackResult callbackRes = new CallbackResult(null, null, "JavaScript", objs);
            map.CallbackResults.Add(callbackRes);
            return map.CallbackResults.ToString();
        }
    }

}
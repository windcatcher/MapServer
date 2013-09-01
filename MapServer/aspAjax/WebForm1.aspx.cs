using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aspAjax
{
    public partial class WebForm1 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            int a = 0;
            int b = 0;
            if (Request.QueryString["A"] != null)
            {
                a = Convert.ToInt16(Request.QueryString["A"].ToString());
            }
            if (Request.QueryString["B"] != null)
            {
                b = Convert.ToInt16(Request.QueryString["B"].ToString());
            }
            Response.Write(a + b);
        }
    }
}
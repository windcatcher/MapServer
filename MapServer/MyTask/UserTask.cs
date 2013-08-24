using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Collections.Specialized;
using ESRI.ArcGIS.ADF.Web.UI.WebControls;
namespace MyTask
{
    public class UserTask:ESRI.ArcGIS.ADF.Web.UI.WebControls.FloatingPanelTask
    {
        //panel上面的控件
        private TextBox txtbox1 = null;
        private HtmlInputButton button = null;

        /// <summary>
        /// 添加加载页面上的子控件
        /// </summary>
        protected override void CreateChildControls()
        {
            //清除floatPanel上面的所有控件，然后重新加载
            Controls.Clear();

            //调用基类的创建控件的方法
            base.CreateChildControls();

            //创建TextBox控件
            txtbox1 = new TextBox();
            txtbox1.ID = "textbox1";

            //创建button
            button = new HtmlInputButton();
            button.ID = "OkBut";
            button.Value = "确定";

            //把控件添加到floatpanel中去
            Controls.Add(txtbox1);
            Controls.Add(button);

            //申明各种JavaScript的代码
            string getArgumentJS = string.Format("'txtboxvalue=' + document.getElementById('{0}').value", txtbox1.ClientID);
            string onClick = string.Format("executeTask({0},\"{1}\");", getArgumentJS, this.CallbackFunctionString);
            string onKeyDown = string.Format("if(event.keyCode==13){{{0}return false;}}", onClick);
            button.Attributes.Add("onclick", onClick);
            txtbox1.Attributes.Add("onkeydown", onKeyDown);
        }

        /// <summary>
        /// 得到输入的值
        /// </summary>
        /// <returns></returns>
        public override string GetCallbackResult()
        {
            NameValueCollection kevalcoll = CallbackUtility.ParseStringIntoNameValueCollection(this._callbackArg);
            this.input = kevalcoll["txtboxvalue"];
            return base.GetCallbackResult();
        }

        /// <summary>
        /// 执行业务逻辑区
        /// </summary>
        public override void ExecuteTask()
        {
            this.Results = null;
            if (this.input == null) return;

            string txtvalue = this.input as string;
            //声明显示的结果
            string heading = string.Format("The time on the server is {0}", DateTime.Now.ToShortDateString());
            string detail = string.Format("The value in the Textbox  is:{0}", txtvalue);
            //把结果保存成一个SimpleTaskResult对象
            SimpleTaskResult str = new SimpleTaskResult(heading, detail);
            this.Results = str;
            // throw new NotImplementedException();
        }

        public override List<GISResourceItemDependency> GetGISResourceItemDependencies()
        {
            System.Collections.Generic.List<GISResourceItemDependency> list = new System.Collections.Generic.List<GISResourceItemDependency>();
            return list;
        }
     
    }
}

<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="testAjaxClass.aspx.cs" Inherits="AjaxTest.testAjaxClass" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head>
    <meta http-equiv="Content-Type" content="text/html;charset=iso-8895-1" />
    <title>测试Ajax类</title>
    <script type="text/javascript" src="Scripts/LmaAjax.js"></script>
    <script type="text/javascript">
        function go() {
            var post = "name=" + document.getElementById("txtName").value + "&pwd=" +
        document.getElementById("txtPwd").value;
            var ajax = new Ajax();
            ajax.method = "get";
            ajax.sync = true;
            ajax.URL = "App_Code/Handler.ashx?"+post;
            ajax.PostData = null;
            ajax.OnResponse = parseMethod;
            ajax.send();
        }

        //解析方法
        function parseMethod(responText) {
            var t = document.GetElementById('response');
            if (window.XMLHttpRequest) {
                t.innerText = responText;
            }
            else if (window.ActiveXObject) {
                t.innerText = responText;
            }
        }
    </script>
    >
</head>
<body>
<div id ="'response'"/>
<div>用户名：<input type="text" id="txtName" /></div>
<div>密码：<input type="text" id="txtPwd" /></div>
<div><input type="button" id="btnOK" onclick="go()" value="提交" /><span id="result"></span></div>
<div id="loadingflag" style="display:none">数据提交中，请等待...</div>
</body>
</html>

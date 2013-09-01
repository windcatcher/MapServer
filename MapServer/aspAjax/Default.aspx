<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="aspAjax._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title>Default</title>
    <script language="javascript" type="text/javascript">
        var xmlHttp;

        function createXMLHttpRequest() {
            //判断浏览器类型并创建对象
            //IE
            if (window.ActiveXObject) {
                xmlHttp = new ActiveXObject("Microsoft.XMLHTTP");
            }
            //FF
            else if (window.XMLHttpRequest) {
                xmlHttp = new XMLHttpRequest();
            }
        }

        //光标处于输入框时引发的动作
        function updateTotal() {
            url = "WebForm1.aspx?A=" + form1.elements["A"].value + "&B=" + form1.elements["B"].value;
            xmlHttp.open("GET", url, true);
            xmlHttp.onreadystatechange = doUpdate;
            xmlHttp.send();
            return false;

        function doUpdate() {
            if (xmlHttp.readyState == 4) {
                document.forms[0].elements["TOT"].value = xmlHttp.responseText;
            }
        }
    </script>
</head>
<body onload = "createXMLHttpRequest();">
    <form id="form1" action = "">
        <div>
            <p>
            <input type = "text" id="A" onkeyup = "updateTotal()" value = "0"/>
            <input type = "text"  id="B" onkeyup = "updateTotal()"/ value = "0" />
            </p>
            <p>
            <input type = "text"  id="TOT" />         
            </p>
            </div>
    </form>
</body>
</html>

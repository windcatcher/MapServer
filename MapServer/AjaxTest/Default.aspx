<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AjaxTest._Default" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>Server Time</title>
    <script type="text/javascript">
        function GetServerTime() {
            var message = '';
            var context = '';
            <%=sCallBackFunctionInvocation %>
        }
        function ShowServerTime(timeMessage,context) {
    alert('现在服务器上的时间是：\n'+timeMessage);
}


    </script>
</head>
<body>
    <form id="form1" runat="server">
    <div>
        <input id="Button1" type="button" value="button" onclick="GetServerTime();" /></div>
    </form>
</body>
</html>

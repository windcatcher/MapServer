<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="pageVar.aspx.cs" Inherits="aspAjax.pageVar" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>代码块语法</title>
</head>
<body>
    <form id="form1" runat="server">
    <%if(DateTime.Now.Hour<12) %>
    上午好
    <%else%>
    下午好
    <%for (int i = 0; i < 7; i++) %>
    <%{ %>
    <font size=<%=i+1 %>>hello world</font><br />
    <%} %>
    </form>
</body>
</html>

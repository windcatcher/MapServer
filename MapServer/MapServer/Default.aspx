<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="MapServer._Default" %>

<%@ Register Assembly="ESRI.ArcGIS.ADF.Web.UI.WebControls, Version=10.0.0.0, Culture=neutral, PublicKeyToken=8fc3cc631e44ad86"
    Namespace="ESRI.ArcGIS.ADF.Web.UI.WebControls" TagPrefix="esri" %>
<%@ Register Assembly="ESRI.ArcGIS.ADF.Tasks, Version=10.0.0.0, Culture=neutral, PublicKeyToken=8fc3cc631e44ad86"
    Namespace="ESRI.ArcGIS.ADF.Tasks" TagPrefix="esriTasks" %>
<%@ Register Assembly="MyTask" Namespace="MyTask" TagPrefix="cc1" %>
<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title>根据坐标定位</title>
    <style type="text/css">
        #head
        {
            height: 45px;
        }
        #menu
        {
            height: 20px;
        }
        #searchPoisition
        {
            height: 25px;
        }
        #MapContainer
        {
            height: 500px;
            width: 100%;
            border-style: solid;
            border-width: 1px;
        }
        #mapDiv
        {
            height: 500px;
            width: 80%;
            float: left;
        }
        #tocDiv
        {
            height: 500px;
            width: 20%;
            background-color: Gray;
            float: left;
        }
        </style>
    <script type="text/javascript">
        //**获取坐标 
        function GetCoord() {        
            //x
            var x = document.getElementById("TxtX").value;
            if (x == '') {
                alert('请输入X坐标');
                return;
            }
            if (isNaN(x)) {
                alert('请输入数字！');
                document.getElementById("TxtX").focus();
                return;
            }
            //y
            var y = document.getElementById("TxtY").value;
            if (y == '') {
                alert('请输入Y坐标！');
                return;
            }
            if (isNaN(y)) {
                alert('请输入数字！');
                document.getElementById("TxtY").focus();
                return;
            }
            var message = 'X=' + x + '&Y=' + y; //传递消息
            var context = 'Map1';
            <%=sCallBackFuncStr%>////调用异步处理
        }

        //**获取属性值
        function GetAttr() {
        var attr=document.getElementById("TxtAttr").value;
        if (attr == '') {
                alert('请输入城市名称');
                return;
            }
            var message='attr='+attr;//传递消息
            var context = 'Map1';
            <%=sCallBackFuncStr%>////调用异步处理
        }

        //**创建柱状图
        function createBarTheme() {
            var message='Bar';//传递消息
            var context = 'Map1';
            <%=sCallBackFuncStr%>////调用异步处理
        }

        //**创建饼状图
        function createPieTheme() {
        var message='Pie';
        var context='Map1';
        <%= sCallBackFuncStr%>            
        }
        
        function testAsg() {
        var message='Test';
        var context="Map1";
        <%=sCallBackFuncStr %>
        }


    </script>
</head>
<body>
    <form id="form1" runat="server">
    <asp:Panel ID="Panel1" runat="server">
        <table style="width: 100%;">
            <tr>
                <td id="TaskMenuCell" style="padding-left: 5px;" valign="middle">
                    <asp:Menu ID="TaskMenu" runat="server" Orientation="Horizontal" BackColor="Transparent"
                        DynamicHorizontalOffset="2" Font-Names="Verdana" Font-Size="8pt" ForeColor="White"
                        StaticSubMenuIndent="10px">
                        <Items>
                            <asp:MenuItem Text="柱状图" Value="柱状图"></asp:MenuItem>
                            <asp:MenuItem Text="饼状图" Value="饼状图"></asp:MenuItem>
                        </Items>
                    </asp:Menu>
                </td>
                <td id="ToolbarCell" style="height: 30px;">
                    <esri:Toolbar ID="Toolbar1" runat="server" BuddyControlType="Map" Group="Toolbar1_Group"
                        ToolbarItemDefaultStyle-BackColor="White" ToolbarItemDefaultStyle-Font-Names="微软雅黑"
                        ToolbarItemDefaultStyle-Font-Size="Smaller" ToolbarItemDisabledStyle-BackColor="White"
                        ToolbarItemDisabledStyle-Font-Names="微软雅黑" ToolbarItemDisabledStyle-Font-Size="Smaller"
                        ToolbarItemDisabledStyle-ForeColor="Gray" ToolbarItemHoverStyle-BackColor="White"
                        ToolbarItemHoverStyle-Font-Bold="True" ToolbarItemHoverStyle-Font-Italic="True"
                        ToolbarItemHoverStyle-Font-Names="微软雅黑" ToolbarItemHoverStyle-Font-Size="Smaller"
                        ToolbarItemSelectedStyle-BackColor="White" ToolbarItemSelectedStyle-Font-Bold="True"
                        ToolbarItemSelectedStyle-Font-Names="微软雅黑" ToolbarItemSelectedStyle-Font-Size="Smaller"
                        WebResourceLocation="/aspnet_client/ESRI/WebADF/" Height="34px" Width="286px"
                        ToolbarStyle="ImageOnly">
                        <BuddyControls>
                            <esri:BuddyControl Name="Map1" />
                        </BuddyControls>
                        <ToolbarItems>
                            <esri:Tool ClientAction="DragRectangle" DefaultImage="esriZoomIn.gif" HoverImage="esriZoomInU.gif"
                                JavaScriptFile="" Name="MapZoomIn" SelectedImage="esriZoomInD.gif" ServerActionAssembly="ESRI.ArcGIS.ADF.Web.UI.WebControls"
                                ServerActionClass="ESRI.ArcGIS.ADF.Web.UI.WebControls.Tools.MapZoomIn" Text="Zoom In"
                                ToolTip="Zoom In" />
                            <esri:Tool ClientAction="DragRectangle" DefaultImage="esriZoomOut.gif" HoverImage="esriZoomOutU.gif"
                                JavaScriptFile="" Name="MapZoomOut" SelectedImage="esriZoomOutD.gif" ServerActionAssembly="ESRI.ArcGIS.ADF.Web.UI.WebControls"
                                ServerActionClass="ESRI.ArcGIS.ADF.Web.UI.WebControls.Tools.MapZoomOut" Text="Zoom Out"
                                ToolTip="Zoom Out" />
                            <esri:Tool ClientAction="DragImage" DefaultImage="esriPan.gif" HoverImage="esriPanU.gif"
                                JavaScriptFile="" Name="MapPan" SelectedImage="esriPanD.gif" ServerActionAssembly="ESRI.ArcGIS.ADF.Web.UI.WebControls"
                                ServerActionClass="ESRI.ArcGIS.ADF.Web.UI.WebControls.Tools.MapPan" Text="Pan"
                                ToolTip="Pan" />
                            <esri:Command ClientAction="" DefaultImage="esriFullExt.gif" HoverImage="esriFullExtU.gif"
                                JavaScriptFile="" Name="MapFullExtent" SelectedImage="esriFullExtD.gif" ServerActionAssembly="ESRI.ArcGIS.ADF.Web.UI.WebControls"
                                ServerActionClass="ESRI.ArcGIS.ADF.Web.UI.WebControls.Tools.MapFullExtent" Text="Full Extent"
                                ToolTip="Full Extent" />
                            <esri:Command BuddyItem="MapForward" ClientAction="ToolbarMapBack" DefaultImage="esriBack.gif"
                                Disabled="True" DisabledImage="esriBack.gif" HoverImage="esriBackU.gif" JavaScriptFile=""
                                Name="MapBack" SelectedImage="esriBackD.gif" Text="Back" ToolTip="Map Back Extent" />
                            <esri:Command BuddyItem="MapBack" ClientAction="ToolbarMapForward" DefaultImage="esriForward.gif"
                                Disabled="True" DisabledImage="esriForward.gif" HoverImage="esriForwardU.gif"
                                JavaScriptFile="" Name="MapForward" SelectedImage="esriForwardD.gif" Text="Forward"
                                ToolTip="Map Forward Extent" />
                            <esri:Tool ClientAction="Point" JavaScriptFile="" Name="Select" ServerActionAssembly="MapServer"
                                ServerActionClass="SelectFeature" DefaultImage="~/select.png" DisabledImage="~/select.png"
                                HoverImage="~/select.png" SelectedImage="~/select.png" />
                            <esri:Command ClientAction="" DefaultImage="~/inf.png" DisabledImage="~/inf.png"
                                HoverImage="~/inf.png" JavaScriptFile="" Name="Command" SelectedImage="~/inf.png"
                                ServerActionAssembly="MapServer" ServerActionClass="SelectFeature" />
                        </ToolbarItems>
                    </esri:Toolbar>
                </td>
            </tr>
        </table>
    </asp:Panel>
    <div id="head">
        <div id="menu">
            <input id="btn_themeZhu" type="button" value="柱状图" onclick="return createBarTheme()" />
            <input id="btn_themeBing" type="button" value="饼状图" onclick="return createPieTheme()" />
            <input id="btn_test" type="button" value="XX图" onclick="return testAsg()" />
            <input id="Button6" type="button" value="button" />
            <input id="Button7" type="button" value="button" />
        </div>
        <div id="searchPoisition">
            X坐标<input id="TxtX" type="text" />
            Y坐标<input id="TxtY" type="text" />
            <input id="Button1" type="button" value="坐标定位" onclick="GetCoord()" />
            城市名称<input id="TxtAttr" type="text" /><input id="Button2" type="button" value="属性定位"
                onclick="GetAttr()" />
        </div>
    </div>
    <div id="MapContainer">
        <div id="tocDiv">
            <esri:Toc ID="Toc1" runat="server" Width="100%" Height="100%" BuddyControl="Map1" />
        </div>
        <div id="mapDiv">
            <esri:Map ID="Map1" runat="server" Height="100%" Width="100%" MapResourceManager="MapResourceManager1">
            </esri:Map>
            <esri:MapResourceManager ID="MapResourceManager1" runat="server">
                <ResourceItems>
                    <esri:MapResourceItem Definition="&lt;Definition DataSourceDefinition=&quot;In Memory&quot; DataSourceType=&quot;GraphicsLayer&quot; Identity=&quot;&quot; ResourceDefinition=&quot;&quot; /&gt;"
                        DisplaySettings="visible=True:transparency=0:mime=True:imgFormat=PNG8:height=100:width=100:dpi=96:color=:transbg=False:displayInToc=True:dynamicTiling="
                        LayerDefinitions="" Name="graph" />
                    <esri:MapResourceItem Definition="&lt;Definition DataSourceDefinition=&quot;localhost&quot; DataSourceType=&quot;ArcGIS Server Local&quot; Identity=&quot;To set, right-click project and 'Add ArcGIS Identity'&quot; ResourceDefinition=&quot;Layers@china&quot; /&gt;"
                        DisplaySettings="visible=False:transparency=0:mime=True:imgFormat=PNG8:height=100:width=100:dpi=96:color=:transbg=False:displayInToc=True:dynamicTiling="
                        LayerDefinitions="" Name="china" />
                    <esri:MapResourceItem Definition="&lt;Definition DataSourceDefinition=&quot;http://lms-pc/ArcGIS/services/china/mapserver/WMSServer&quot; DataSourceType=&quot;OGC (WMS) Service&quot; Identity=&quot;dmqfRiLpDtDoSxOJwbSM/Q==&quot; ResourceDefinition=&quot;&quot; /&gt;" 
                        DisplaySettings="visible=True:transparency=0:mime=True:imgFormat=PNG8:height=100:width=100:dpi=96:color=:transbg=False:displayInToc=True:dynamicTiling=" 
                        LayerDefinitions="" Name="WmsChina" />
                </ResourceItems>
            </esri:MapResourceManager>
        </div>
    </div>
    <esri:TaskManager ID="TaskManager1" runat="server" BuddyControl="Menu1" Font-Names="Verdana"
        Font-Size="8pt" ForeColor="Black" Height="16px" Width="200px">
        <asp:Menu ID="Menu1" runat="server">
        </asp:Menu>
        <esri:TaskResults ID="TaskResults1" runat="server" BackColor="#ffffff" Font-Names="Verdana"
            Font-Size="8pt" ForeColor="#000000" Height="50px" Width="200px" Map="Map1" />
        <esriTasks:SearchAttributesTask ID="SearchAttributesTask1" runat="server" ActivityIndicatorImage=""
            BackColor="White" BorderColor="LightSteelBlue" BorderStyle="Outset" BorderWidth="1px"
            DockingContainerElementID="" Font-Names="Verdana" Font-Size="8pt" ForeColor="Black"
            HelpUrl="" InputWidth="" LabelText="请输入省份名称：" NavigationPath="" SearchFields="MapResourceManager1:::china$$$-389840880:::7:::NAME"
            Title="SearchAttributesTask1" TitleBarBackgroundImage="" TitleBarColor="WhiteSmoke"
            TitleBarCssClass="" TitleBarForeColor="Black" TitleBarHeight="20px" TitleBarSeparatorLine="False"
            Transparency="35" WebResourceLocation="/aspnet_client/ESRI/WebADF/" Width="200px">
            <TaskResultsContainers>
                <esri:BuddyControl Name="TaskResults1" />
            </TaskResultsContainers>
        </esriTasks:SearchAttributesTask>
    </esri:TaskManager>
    </form>
</body>
</html>

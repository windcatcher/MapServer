using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESRI.ArcGIS.Server;
using System.Web.UI;
using System.Web.SessionState;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;

namespace MapServer
{

    public class ServerUtility
    {
        //得到服务器上下文            
        public static IServerContext GetServerContext(System.Web.UI.Page page)
        {
             
            if (page == null)
                return null;
            string serverName = "localhost";   //服务器机器名称
            string mapServiceName = "china";   //空间数据服名称
            ESRI.ArcGIS.Server.IServerObjectManager serverObjectManager;

            // 获得SOM,并放入Session变量中        
            if (page.Session["SOM"] == null)
            {
                // 用ADF connection 库
                ESRI.ArcGIS.ADF.Connection.AGS.AGSServerConnection agsServerConnection =
                    new ESRI.ArcGIS.ADF.Connection.AGS.AGSServerConnection();
                agsServerConnection.Host = serverName;
                agsServerConnection.Connect();

                serverObjectManager = agsServerConnection.ServerObjectManager;
                page.Session["SOM"] = serverObjectManager;
            }
            else
            {
                serverObjectManager = page.Session["SOM"] as ESRI.ArcGIS.Server.IServerObjectManager;
            }
            //根据服务名来创建上下文
            ESRI.ArcGIS.Server.IServerContext serverContext =
                serverObjectManager.CreateServerContext(mapServiceName, "MapServer");

            return serverContext;
        }


      
        //创建饼状专题图
        private void CreatePieTheme(System.Web.UI.Page page, int nLayerID, string[] fields)
        {
            if (page == null)
                return;
            // 得到地图服务下的ArcObjects map对象
            ESRI.ArcGIS.Server.IServerContext pServerContext = GetServerContext(page);

            ESRI.ArcGIS.Carto.IMapServer mapServer = (ESRI.ArcGIS.Carto.IMapServer)pServerContext.ServerObject;
            ESRI.ArcGIS.Carto.IMapServerObjects2 mapServerObjects = (ESRI.ArcGIS.Carto.IMapServerObjects2)mapServer;
            string mapName = mapServer.DefaultMapName;
            ESRI.ArcGIS.Carto.IMap aoMap = mapServerObjects.get_Map(mapName);

            ESRI.ArcGIS.Carto.ILayer pLayer = aoMap.get_Layer(nLayerID);//得到图层
            ESRI.ArcGIS.Carto.IGeoFeatureLayer pGeoLayer = pLayer as IGeoFeatureLayer;

            //设置专题图的属性列表
            ESRI.ArcGIS.Carto.IChartRenderer pCharRenderer = pServerContext.CreateObject("esriCarto.ChartRenderer") as IChartRenderer;
            ESRI.ArcGIS.Carto.IRendererFields pRenderFields = pCharRenderer as IRendererFields;
            foreach (string var in fields)
            {
                pRenderFields.AddField(var, var);
            }

            //实例化图表对象并取得元素指定属性的最大值
            ESRI.ArcGIS.Display.IPieChartSymbol pPieSym = pServerContext.CreateObject("esriDisplay.PieChartSymbol") as ESRI.ArcGIS.Display.IPieChartSymbol;
            ESRI.ArcGIS.Display.IChartSymbol pCharSym = pPieSym as ESRI.ArcGIS.Display.IChartSymbol;
            pPieSym.Clockwise = true;
            pPieSym.UseOutline = true;

            pCharSym.MaxValue = GetStaMaxMin(fields, pGeoLayer)[0];

            //设置饼图外围线
            ISimpleLineSymbol pOutLine = pServerContext.CreateObject("esriDisplay.SimpleLineSymbol") as ISimpleLineSymbol;
            pOutLine.Color = GetRGB(255, 0, 128, pServerContext);
            pOutLine.Style = ESRI.ArcGIS.Display.esriSimpleLineStyle.esriSLSSolid;
            pOutLine.Width = 1;

            pPieSym.Outline = pOutLine;
            IMarkerSymbol pMarkSym = pPieSym as IMarkerSymbol;
            pMarkSym.Size = 5;


            //设置饼状图填充效果
            ESRI.ArcGIS.Display.ISymbolArray pSymArr = pPieSym as ISymbolArray;
            ISimpleFillSymbol pSimFillSym = pServerContext.CreateObject("esriDisplay.SimpleFillSymbol") as ESRI.ArcGIS.Display.SimpleFillSymbol;
            pSimFillSym.Color = GetRGB(128, 128, 128, pServerContext);
            pSimFillSym.Outline = pOutLine;

            Random randColor = new Random();
            for (int i = 0; i < fields.Length; i++)
            {
                IFillSymbol pFillSym = pServerContext.CreateObject("esriDisplay.SimpleFillSymbol") as IFillSymbol;
                pFillSym.Color = GetRGB(randColor.Next(255), randColor.Next(255), randColor.Next(255), pServerContext);
                pSymArr.AddSymbol((ISymbol)pFillSym);
            }

            //设置地图图层背景
            pSimFillSym = pServerContext.CreateObject("esriDisplay.SimpleFillSymbol") as ESRI.ArcGIS.Display.SimpleFillSymbol;
            pSimFillSym.Color = GetRGB(255, 128, 255, pServerContext);
            pCharRenderer.BaseSymbol = pSimFillSym as ISymbol;


            //设置饼状图表属性
            IPieChartRenderer pPieChartRenderer = pCharRenderer as IPieChartRenderer;
            pPieChartRenderer.MinValue = 0.1;
            pPieChartRenderer.MinSize = 1;
            pPieChartRenderer.FlanneryCompensation = false;
            pPieChartRenderer.ProportionalBySum = true;
            pPieChartRenderer.ProportionalField = fields[0];
            pCharRenderer.ChartSymbol = pPieSym as IChartSymbol;
            pCharRenderer.Label = "面积";

            //应用饼状专题到指定图层
            pCharRenderer.UseOverposter = false;
            pCharRenderer.CreateLegend();
            pGeoLayer.Renderer = pCharRenderer as IFeatureRenderer;

            //刷新地图显示图表及图例
            mapServerObjects.RefreshServerObjects();
            // Map1.RefreshResource("MapResourceItem0");           
            pServerContext.ReleaseContext();
        }

        /// <summary>
        /// 计算元素指定属性的最大值
        /// </summary>
        /// <param name="fields">元素的属性名称列表</param>
        /// <returns></returns>
        public static double[] GetStaMaxMin(string[] fields, IFeatureLayer pGeoFeatureLyer)
        {
            double pMaxValue = 0;
            double pMinValue = 0;
            double pStaMax;
            double pStaMin;
            double[] PMaxMin = new double[2];

            for (int i = 0; i < fields.Length; i++)
            {

                ICursor pCursor = pGeoFeatureLyer.Search(null, true) as ICursor;
                IDataStatistics pDataSta = new DataStatisticsClass();
                pDataSta.Cursor = pCursor;
                pDataSta.Field = fields[i];
                pStaMax = pDataSta.Statistics.Maximum;
                pStaMin = pDataSta.Statistics.Minimum;
                if (pMaxValue < pStaMax)
                    pMaxValue = pStaMax;
                if (pMinValue > pStaMin)
                    pMinValue = pStaMin;
            }
            PMaxMin[0] = pMaxValue;
            PMaxMin[1] = pMinValue;
            return PMaxMin;
        }

        /// 获取GRB颜色
        /// </summary>
        /// <param name="red"></param>
        /// <param name="green"></param>
        /// <param name="blue"></param>
        /// <returns></returns>
        public static ESRI.ArcGIS.Display.IColor GetRGB(int red, int green, int blue, IServerContext pServerContext)
        {
            IRgbColor rgbColor = pServerContext.CreateObject("esriDisplay.RGBColor") as IRgbColor;
            IColor color = rgbColor as IColor;
            rgbColor.Red = red;
            rgbColor.Green = green;
            rgbColor.Blue = blue;
            return color;
        }
    
    }
}
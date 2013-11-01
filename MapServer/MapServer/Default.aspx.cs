using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ESRI.ArcGIS.ADF.Web;
using ESRI.ArcGIS.ADF.Web.Geometry;
using System.Collections;
using System.Data;
using ESRI.ArcGIS.ADF.Web.DataSources;
using ESRI.ArcGIS.ADF.Web.UI.WebControls;
using ESRI.ArcGIS.ADF.Connection.AGS;
using ESRI.ArcGIS.Server;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.ADF.Web.Display.Graphics;
using ESRI.ArcGIS.ADF.Web.DataSources.Graphics;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using MapServer.AppCode;



namespace MapServer
{
    public partial class _Default : System.Web.UI.Page, ICallbackEventHandler
    {
        public string sCallBackFuncStr;
        protected void Page_Load(object sender, EventArgs e)
        {
            sCallBackFuncStr = Page.ClientScript.GetCallbackEventReference(this, "message", "processCallbackResult",
                 "context", "postBackError", true);
        }

        #region 处理回调函数

        public void RaiseCallbackEvent(string eventArgs)
        {
            switch (eventArgs)
            {
                case "attr":
                    {
                        LocalByAttribute(eventArgs);
                    }
                    break;
                case "Bar":
                    {
                        CreateBarRenderer(7, new string[] { "AREA" });
                    }
                    break;
                case "Pie":
                    {
                        CreatePieTheme(7, new string[] { "AREA" });
                    }
                    break;
                case "X":
                    {
                        LocalByXY(eventArgs);
                    }
                    break;
                case "Test":
                    {
                        TestAsg();
                    }
                    break;
                default:
                    { }
                    break;

            }
        }
        /// <summary>
        /// 根据属性定位
        /// </summary>
        private void LocalByAttribute(string eventArgs)
        {
            if (string.IsNullOrEmpty(eventArgs))
                return;

            try
            {
                IEnumerable func_enum = null;
                DataTable table = null;
                //获取当前map的所有functionlity  图层名BOUNT_poly
                func_enum = Map1.GetFunctionalities();
                //d对所有的functionlity遍历                
                foreach (IGISFunctionality gisFunclity in func_enum)
                {
                    IGISResource gisResr = null;
                    IQueryFunctionality qFunc;
                    gisResr = gisFunclity.Resource;
                    if (gisResr == null)
                        continue;
                    //判断是否支持IQueryFunctionality
                    bool isSurport = false;
                    isSurport = gisResr.SupportsFunctionality(typeof(IQueryFunctionality));
                    if (!isSurport)
                        continue;
                    qFunc = gisResr.CreateFunctionality(typeof(IQueryFunctionality), null) as IQueryFunctionality;
                    if (qFunc == null)
                        continue;
                    //获取图层layerid和layerName
                    string[] layerIds, layerNames;
                    qFunc.GetQueryableLayers(null, out layerIds, out layerNames);
                    if (layerIds == null || layerIds.Length == 0)
                        continue;
                    int layerId = -1;
                    for (int i = 0; i < layerIds.Length; i++)
                    {
                        if (layerNames[i] == "res2_4m")
                        {
                            layerId = i;
                            break;
                        }
                    }
                    if (layerId == -1)
                        continue;
                    //设置过滤器的过滤条件
                    ESRI.ArcGIS.ADF.Web.SpatialFilter sf = new ESRI.ArcGIS.ADF.Web.SpatialFilter();
                    sf.ReturnADFGeometries = true;
                    sf.MaxRecords = 100;

                    string name = CallbackUtility.ParseStringIntoNameValueCollection(eventArgs)["attr"];
                    sf.WhereClause = string.Format("NAME='{0}'", name);
                    //对指定图层进行查询，查询结果保存为
                    table = qFunc.Query(null, layerIds[layerId], sf);
                    if (table == null || table.Rows.Count == 0)
                    {
                        object[] objs = new object[1];
                        string sa = "alert('没有找到该城市')";
                        objs[0] = sa;
                        CallbackResult callbackRes = new CallbackResult(null, null, "JavaScript", objs);
                        Map1.CallbackResults.Add(callbackRes);
                        sCallBackFuncStr = Map1.CallbackResults.ToString();
                        continue;
                    }
                    for (int i = 0; i < table.Columns.Count; i++)
                    {
                        DataColumn column = table.Columns[i];
                        if (column.DataType == typeof(Geometry))
                        {
                            ESRI.ArcGIS.ADF.Web.Geometry.Point adfPt = table.Rows[0][i] as ESRI.ArcGIS.ADF.Web.Geometry.Point;
                            try
                            {
                                Map1.CenterAt(adfPt);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            try
                            {
                                DrawBufferByPoint(adfPt);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                            }
                            sCallBackFuncStr = Map1.CallbackResults.ToString();
                            return;
                        }
                    }
                }
            }
            catch (Exception)
            {


            }
        }

        /// <summary>
        /// 绘制点缓冲
        /// </summary>
        private void DrawBufferByPoint(ESRI.ArcGIS.ADF.Web.Geometry.Point adfPt)
        {
            //1.连接服务器
            AGSServerConnection connection = new AGSServerConnection();
            connection.Host = "localhost";
            connection.Connect();

            //2.获得服务器
            IServerObjectManager pSom = connection.ServerObjectManager;
            IServerContext pSc = pSom.CreateServerContext("china", "MapServer");//服务名和类型
            IMapServer mapServer = pSc.ServerObject as IMapServer;

            //3.使用服务器对象 几何对象转换
            IMapServerObjects pMso = mapServer as IMapServerObjects;
            //ESRI.ArcGIS.Geometry.IGeometry comPt = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ValueObjectToComObject(adfPt, pSc)
            //    as ESRI.ArcGIS.Geometry.IGeometry;////ValueObjectToComObject(pnt, pSC);
            IPoint pt = new ESRI.ArcGIS.Geometry.PointClass();
            pt.X = adfPt.X;
            pt.Y = adfPt.Y;
            ESRI.ArcGIS.Geometry.IGeometry comPt = pt;

            ESRI.ArcGIS.Geometry.SpatialReferenceEnvironment sre = new SpatialReferenceEnvironment();
            ISpatialReference pSR = sre.CreateGeographicCoordinateSystem(4326);
            comPt.SpatialReference = pSR;

            //绘制buffer
            ITopologicalOperator pTOPO = comPt as ITopologicalOperator;
            pTOPO.Simplify();//??
            double bufDis = Map1.Extent.Width / 2;
            IPolygon bufPoly = pTOPO.Buffer(10) as IPolygon;
            bufPoly.Densify(0, 0);
            ESRI.ArcGIS.ADF.ArcGISServer.PolygonN valuePolyN = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ComObjectToValueObject(bufPoly, pSc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.PolygonN)) as ESRI.ArcGIS.ADF.ArcGISServer.PolygonN;
            ESRI.ArcGIS.ADF.Web.Geometry.Polygon adfPoly = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ToAdfPolygon(valuePolyN) as ESRI.ArcGIS.ADF.Web.Geometry.Polygon;


            #region Densify
            ////***Densify
            // ESRI.ArcGIS.Geometry.IPointCollection com_pointcollection = (ESRI.ArcGIS.Geometry.IPointCollection)bufPoly;
            // ESRI.ArcGIS.ADF.Web.Geometry.PointCollection new_adf_pointcollection = ESRI.ArcGIS.ADF.Web.DataSources.ArcGISServer.Converter.ComObjectToValueObject(com_pointcollection, pSc, typeof(ESRI.ArcGIS.ADF.ArcGISServer.poly));
            //ESRI.ArcGIS.ADF.Web.Geometry.PointCollection new_adf_pointcollection = new ESRI.ArcGIS.ADF.Web.Geometry.PointCollection();
            //for (int i = 0; i < com_pointcollection.PointCount - 1; i++)
            //{
            //    ESRI.ArcGIS.ADF.Web.Geometry.Point new_adf_pt = new ESRI.ArcGIS.ADF.Web.Geometry.Point();
            //    new_adf_pt.X = com_pointcollection.get_Point(i).X;
            //    new_adf_pt.Y = com_pointcollection.get_Point(i).Y;
            //    new_adf_pointcollection.Add(new_adf_pt);
            //}
            //ESRI.ArcGIS.ADF.Web.Geometry.Ring new_adf_ring = new ESRI.ArcGIS.ADF.Web.Geometry.Ring();
            //new_adf_ring.Points = new_adf_pointcollection;
            //ESRI.ArcGIS.ADF.Web.Geometry.RingCollection new_adf_ringcollection = new ESRI.ArcGIS.ADF.Web.Geometry.RingCollection();
            //new_adf_ringcollection.Add(new_adf_ring);
            //ESRI.ArcGIS.ADF.Web.Geometry.Polygon new_adf_polygon = new ESRI.ArcGIS.ADF.Web.Geometry.Polygon();
            //new_adf_polygon.Rings = new_adf_ringcollection;
            //ESRI.ArcGIS.ADF.Web.Geometry.Geometry geom = new_adf_polygon as ESRI.ArcGIS.ADF.Web.Geometry.Geometry;
            ////*******Densify
            #endregion

            ESRI.ArcGIS.ADF.Web.Geometry.Geometry geom = adfPoly as ESRI.ArcGIS.ADF.Web.Geometry.Geometry;
            GraphicElement geoEle = new GraphicElement(geom, System.Drawing.Color.Red);
            try
            {
                Map1.Zoom(adfPoly);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            geoEle.Symbol.Transparency = 50;

            IEnumerable gfc = Map1.GetFunctionalities();

            MapResource gMap = null;
            foreach (IGISFunctionality gfunc in gfc)
            {
                if (gfunc.Resource.Name == "graph")
                {
                    gMap = (MapResource)gfunc.Resource;
                    break;
                }
            }
            if (gMap == null)
                return;
            ElementGraphicsLayer glayer = null;
            foreach (DataTable dt in gMap.Graphics.Tables)
            {
                if (dt is ElementGraphicsLayer)
                {
                    glayer = dt as ElementGraphicsLayer;
                    break;
                }
            }
            if (glayer == null)
            {
                glayer = new ElementGraphicsLayer();
                gMap.Graphics.Tables.Add(glayer);
            }

            glayer.Clear();//清除数据
            glayer.Add(geoEle);

            //4.释放服务器对象
            pSc.ReleaseContext();

            if (Map1.ImageBlendingMode == ImageBlendingMode.WebTier)
                Map1.Refresh();
            else if (Map1.ImageBlendingMode == ImageBlendingMode.Browser)
                Map1.RefreshResource(gMap.Name);
            return;

        }

        /// <summary>
        /// 定位到坐标点
        /// </summary>
        /// <param name="eventArgs"></param>
        private void LocalByXY(string eventArgs)
        {
            System.Collections.Specialized.NameValueCollection kvs = ESRI.ArcGIS.ADF.Web.UI.WebControls.CallbackUtility.
                ParseStringIntoNameValueCollection(eventArgs);
            double x, y = 0;
            if (!double.TryParse(kvs["X"], out x))
                return;
            if (!double.TryParse(kvs["Y"], out y))
                return;
            ESRI.ArcGIS.ADF.Web.Geometry.Point centerPt = new ESRI.ArcGIS.ADF.Web.Geometry.Point(x, y);
            try
            {
                Map1.CenterAt(centerPt);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

            // Map1.Zoom(200);
            sCallBackFuncStr = Map1.CallbackResults.ToString();
        }

        public string GetCallbackResult()
        {
            return sCallBackFuncStr;
        }
        #endregion

        #region 专题图
        //创建饼状专题图
        public void CreatePieTheme(System.Web.UI.Page page, int nLayerID, string[] fields)
        {
            if (page == null)
                return;
            // 得到地图服务下的ArcObjects map对象
            ESRI.ArcGIS.Server.IServerContext pServerContext = GetServerContext();

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
        public double[] GetStaMaxMin(string[] fields, IFeatureLayer pGeoFeatureLyer)
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
        public ESRI.ArcGIS.Display.IColor GetRGB(int red, int green, int blue, IServerContext pServerContext)
        {
            IRgbColor rgbColor = pServerContext.CreateObject("esriDisplay.RGBColor") as IRgbColor;
            IColor color = rgbColor as IColor;
            rgbColor.Red = red;
            rgbColor.Green = green;
            rgbColor.Blue = blue;
            return color;
        }


        //创建柱状专题图
        public void CreateBarRenderer(int layerID, string[] fields)
        {
            // 得到地图服务下的ArcObjects map对象
            ESRI.ArcGIS.Server.IServerContext pServerContext = GetServerContext();

            ESRI.ArcGIS.Carto.IMapServer mapServer = (ESRI.ArcGIS.Carto.IMapServer)pServerContext.ServerObject;
            ESRI.ArcGIS.Carto.IMapServerObjects2 mapServerObjects = (ESRI.ArcGIS.Carto.IMapServerObjects2)mapServer;
            string mapName = mapServer.DefaultMapName;
            ESRI.ArcGIS.Carto.IMap aoMap = mapServerObjects.get_Map(mapName);

            ESRI.ArcGIS.Carto.ILayer pLayer = aoMap.get_Layer(layerID);//得到图层
            ESRI.ArcGIS.Carto.IGeoFeatureLayer pGeoLayer = pLayer as IGeoFeatureLayer;
            //设置专题图元素的属性名称列表           
            IChartRenderer pChartRender = pServerContext.CreateObject("esriCarto.ChartRenderer") as IChartRenderer;
            IRendererFields pRenderFields = pChartRender as IRendererFields;
            foreach (string var in fields)
            {
                pRenderFields.AddField(var, var);
            }

            //实例化图表对象并取得元素指定属性的最大值
            IBarChartSymbol pBarChartSymbol = pServerContext.CreateObject("esriDisplay.BarChartSymbol") as IBarChartSymbol;
            IChartSymbol pChartSymbol = pBarChartSymbol as IChartSymbol;

            pChartSymbol.MaxValue = GetStaMaxMin(fields, pGeoLayer)[0];
            pBarChartSymbol.Width = 8;
            IMarkerSymbol pMarkerSymbol = pBarChartSymbol as IMarkerSymbol;
            pMarkerSymbol.Size = 50;

            //设置柱状图每列填充效果
            ISymbolArray pSymbolArray = pBarChartSymbol as ISymbolArray;
            Random ranColor = new Random();
            for (int i = 0; i < fields.Length; i++)
            {
                IFillSymbol pFillSymbol = pServerContext.CreateObject("esriDisplay.SimpleFillSymbol") as IFillSymbol;
                pFillSymbol.Color = GetRGB(ranColor.Next(255), ranColor.Next(255), ranColor.Next(255), pServerContext);
                pSymbolArray.AddSymbol((ISymbol)pFillSymbol);
            }

            //设置地图图层背景
            ESRI.ArcGIS.Display.ISimpleFillSymbol pFSymbol = pServerContext.CreateObject("esriDisplay.SimpleFillSymbol") as ESRI.ArcGIS.Display.SimpleFillSymbol;
            pFSymbol.Color = GetRGB(239, 228, 249, pServerContext);
            pChartRender.BaseSymbol = pFSymbol as ISymbol;

            //应用柱状专题到指定图层
            pChartRender.ChartSymbol = pBarChartSymbol as IChartSymbol;
            pChartRender.Label = "Test";
            pChartRender.UseOverposter = false;
            pChartRender.CreateLegend();
            pGeoLayer.Renderer = pChartRender as IFeatureRenderer;

            //刷新地图显示图表及图例
            mapServerObjects.RefreshServerObjects();
            Map1.RefreshResource("MapResourceItem0");
            Toc1.BuddyControl = "Map1";
            //Toc1.Refresh();
            Map1.Refresh();
            pServerContext.ReleaseContext();

        }

        /// <summary>
        /// 创建饼状专题图
        /// </summary>
        /// <param name="nLayerId">图层Id</param>
        /// <param name="strFileds">字段名称集合</param>
        private void CreatePieTheme(int nLayerId, string[] strFileds)
        {
            CreatePieTheme(this.Page, nLayerId, strFileds);
            ////获取服务器对象
            //IServerContext svrContext =ServerUtility.GetServerContext(this.Page);
            //if (svrContext == null)
            //    return;
            //IMapServer mapSvr = svrContext.ServerObject as IMapServer;
            //if (mapSvr == null)
            //    return;
            ////使用服务器对象
            //IMapServerObjects2 mapSvrObj = mapSvr as IMapServerObjects2;
            //if (mapSvrObj == null)
            //    return;
            ////远程调用AO对象
            //string strMapName = mapSvr.DefaultMapName;
            //IMap aoMap = mapSvrObj.get_Map(strMapName);
            ////得到图层
            //ILayer aoLayer = aoMap.get_Layer(nLayerId);
            //IGeoFeatureLayer aoGeoLyr = aoLayer as IGeoFeatureLayer;

            ////设置专题图的属性列表
            //IChartRenderer aoCharRndr = svrContext.CreateObject("esriCarto.ChartRenderer") as IChartRenderer;
            //IRendererFields aoRndrFlds = aoCharRndr as IRendererFields;
            //foreach (string field in strFileds)
            //{
            //    aoRndrFlds.AddField(field, field);
            //}

        }

        /// <summary>
        /// 得到服务器上下文：AGSServer远程调用AO  
        /// </summary>
        /// <returns></returns>
        private IServerContext GetServerContext()
        {
            string serverName = "localhost";    //服务器机器名称
            string mapSvrName = "china";        //空间数据服名称
            IServerObjectManager svrObjMgr = null;
            //获取SOM，并放入session变量中
            if (Session["SOM"] == null)
            {
                //用ADF connection库
                AGSServerConnection agsServerConnection = new AGSServerConnection();
                agsServerConnection.Host = serverName;
                //建立与服务器的连接
                agsServerConnection.Connect();
                svrObjMgr = agsServerConnection.ServerObjectManager;
            }
            else
            {
                svrObjMgr = Session["SOM"] as ServerObjectManager;
            }

            //根据服务名来创建上下文
            IServerContext svrContext = svrObjMgr.CreateServerContext(mapSvrName, "MapServer");
            return svrContext;
        }
        #endregion
        #region Test

        public void TestAsg()
        {
            IServerContext svrContxt = null;
            try
            {
                //1.连接服务器
                AGSServerConnection agsCon = new AGSServerConnection();
                agsCon.Host = "localhost";
                agsCon.Connect();
                //2.创建serverContext
                IServerObjectManager som = agsCon.ServerObjectManager;
                string mapSvrName = "china";
                string svrType = "MapServer";
                svrContxt = som.CreateServerContext(mapSvrName, svrType);
                //3.创建serverObjects
                IPoint pt = svrContxt.CreateObject("esriGeometry.Point") as IPoint;
                pt.X = 100;
                pt.Y = 200;               
                System.Diagnostics.Debug.WriteLine("ssssssssss");              
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                if (svrContxt != null) { svrContxt.ReleaseContext(); svrContxt = null; }
            }


        }
        #endregion


    }
}
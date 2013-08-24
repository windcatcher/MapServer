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
            if (eventArgs.Contains("attr"))
            {
                LocalByAttribute(eventArgs);
            }
            else
            {
                LocalByXY(eventArgs);
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
                    SpatialFilter sf = new SpatialFilter();
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

                            }
                            try
                            {
                                DrawBufferByPoint(adfPt);
                            }
                            catch (Exception ex)
                            {
                                                                
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
            IPoint pt=new ESRI.ArcGIS.Geometry.PointClass();
            pt.X=adfPt.X;
            pt.Y=adfPt.Y;
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

            }

            // Map1.Zoom(200);
            sCallBackFuncStr = Map1.CallbackResults.ToString();
        }

        public string GetCallbackResult()
        {
            return sCallBackFuncStr;
        }
        #endregion
    }
}
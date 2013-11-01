using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using System.Drawing;
using ESRI.ArcGIS.ADF.Web.DataSources.Graphics;

using ESRI.ArcGIS.ADF.Web;
using ESRI.ArcGIS.ADF.Web.DataSources;
using ESRI.ArcGIS.ADF.Web.UI.WebControls;
using ESRI.ArcGIS.ADF.Web.UI.WebControls.Tools;

public class SelectFeature : IMapServerToolAction, IMapServerCommandAction
{
    public SelectFeature()
    {

    }
    public void ServerAction(ToolEventArgs args)
    {
        Map mapCtrl = null;
        mapCtrl = (Map)args.Control;
        string strName = "";
        PointEventArgs ptArgs = null;
        ptArgs = (PointEventArgs)args;
        System.Drawing.Point pt = ptArgs.ScreenPoint;
        //转换为地图上的点
        ESRI.ArcGIS.ADF.Web.Geometry.Point adfPt = ESRI.ArcGIS.ADF.Web.Geometry.Point.ToMapPoint(pt.X, pt.Y, mapCtrl.Extent
          , (int)mapCtrl.Width.Value, (int)mapCtrl.Height.Value);

        //查找图层
        System.Collections.IEnumerable func_enum = null;
        func_enum = mapCtrl.GetFunctionalities();
        ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapResource grahpResource = null;

        System.Data.DataTable dt;
        foreach (IGISFunctionality gisfunction in func_enum)
        {
            IGISResource gisRes = null;
            gisRes = gisfunction.Resource;
            if (gisfunction.Resource.Name == "graph")
            {
                grahpResource = (MapResource)gisfunction.Resource;//找到内存图像资源
            }
            bool bIsSupported = false;
            //地理资源是否支持查询
            bIsSupported = gisRes.SupportsFunctionality((typeof(IQueryFunctionality)));
            if (!bIsSupported)
                continue;
            IQueryFunctionality qFunc = null;
            qFunc = (IQueryFunctionality)gisRes.CreateFunctionality((typeof(IQueryFunctionality)), null);
            string[] strIds;
            string[] strNames;
            qFunc.GetQueryableLayers(null, out strIds, out strNames);
            if (strIds == null) continue;//如果是遥感影像不支持查询
            int layerIndex = -1;
            for (int i = 0; i < strNames.Length; i++)
            {
                if (strNames[i] == "bou2_4p")
                {
                    layerIndex = i;
                    break;
                }
            }
            if (layerIndex < 0)
                continue;
            //找到该图层
            SpatialFilter sFilter = new SpatialFilter();
            sFilter.ReturnADFGeometries = true;
            sFilter.Geometry = adfPt;
            sFilter.MaxRecords = 100;


            dt = qFunc.Query(null, strIds[layerIndex], sFilter);
            if (dt == null || dt.Rows.Count == 0)
                continue;
            for (int jj = 0; jj < dt.Columns.Count; jj++)
            {
                strName = dt.Rows[0]["name"].ToString();
                if (dt.Columns[jj].DataType == typeof(ESRI.ArcGIS.ADF.Web.Geometry.Geometry))
                {
                    //找到该集合对象
                    ESRI.ArcGIS.ADF.Web.Geometry.Geometry geom = (ESRI.ArcGIS.ADF.Web.Geometry.Geometry)dt.Rows[0][jj];
                    ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer glayer = null;
                    foreach (System.Data.DataTable dt1 in grahpResource.Graphics.Tables)
                    {
                        if (dt1 is ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer)
                        {
                            glayer = (ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer)dt1;
                            break;
                        }
                    }
                    if (glayer == null)
                    {
                        glayer = new ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer();
                        grahpResource.Graphics.Tables.Add(glayer);
                    }
                    glayer.Clear();
                    ESRI.ArcGIS.ADF.Web.Display.Graphics.GraphicElement ge = null;

                    ge = new ESRI.ArcGIS.ADF.Web.Display.Graphics.GraphicElement(geom, System.Drawing.Color.Red);
                    ge.Symbol.Transparency = 50;
                    glayer.Add(ge);
                }
            }



        }



        //
        if (mapCtrl.ImageBlendingMode == ImageBlendingMode.WebTier)
        {
            mapCtrl.Refresh();
        }
        else if (mapCtrl.ImageBlendingMode == ImageBlendingMode.Browser)
        {
            mapCtrl.RefreshResource(grahpResource.Name);
        }
        object[] oa = new object[1];
        string sa = "alert('" + strName + "');";
        oa[0] = sa;
        CallbackResult crl = new CallbackResult(null, null, "javascript", oa);
        mapCtrl.CallbackResults.Add(crl);


    }

    public void ServerAction(ToolbarItemInfo info)
    {

        Map mapctrl = null;
        mapctrl = (Map)info.BuddyControls[0];//得到地图控件

        ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapFunctionality gmf = null;
        foreach (ESRI.ArcGIS.ADF.Web.DataSources.IMapFunctionality imf in mapctrl.GetFunctionalities())
        {
            if (imf.Resource.Name == "graph")//找到内存图像资源
            {
                gmf = (ESRI.ArcGIS.ADF.Web.DataSources.Graphics.MapFunctionality)imf;
                break;
            }
        }

        if (gmf == null)
        {
            return;
        }

        ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer egl = null;
        foreach (System.Data.DataTable dt in gmf.GraphicsDataSet.Tables)
        {
            if (dt is ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer)
            {
                egl = (ESRI.ArcGIS.ADF.Web.Display.Graphics.ElementGraphicsLayer)dt;
                break;
            }
        }

        if (egl != null)
        {
            egl.Clear();//清除选中的图形
        }

        //刷新地图
        if (mapctrl.ImageBlendingMode == ImageBlendingMode.WebTier)
        {
            mapctrl.Refresh();
        }
        else if (mapctrl.ImageBlendingMode == ImageBlendingMode.Browser)
        {
            mapctrl.RefreshResource(gmf.Resource.Name);
        }
    }
}

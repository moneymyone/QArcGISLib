/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib
{
    /// <summary>
    /// 面
    /// </summary>
    public class QPolygon
    {
        #region ============= 11.  IPolygon  =============
        /// <summary>
        /// 平面缓冲区
        /// </summary>
        /// <param name="_pLine"></param>
        /// <param name="_pBufferDis"></param>
        /// <returns></returns>
        public static IPolygon FlatBuffer(IPolyline _pLine, double _pBufferDis)
        {
            object o = System.Type.Missing;

            //分别对输入的线平移两次（正方向和负方向）

            IConstructCurve pConCurve = new PolylineClass();

            pConCurve.ConstructOffset(_pLine, _pBufferDis, ref o, ref o);

            IPointCollection pCol = pConCurve as IPointCollection;

            IConstructCurve pCurve = new PolylineClass();

            pCurve.ConstructOffset(_pLine, -1 * _pBufferDis, ref o, ref o);
            //把第二次平移的线的所有节点翻转
            IPolyline addline = pCurve as IPolyline;

            addline.ReverseOrientation();
            //把第二条的所有节点放到第一条线的IPointCollection里面
            IPointCollection pCol2 = addline as IPointCollection;

            pCol.AddPointCollection(pCol2);
            //用面去初始化一个IPointCollection
            IPointCollection myPCol = new PolygonClass();

            myPCol.AddPointCollection(pCol);
            //把IPointCollection转换为面
            IPolygon pPolygon = myPCol as IPolygon;
            //简化节点次序
            pPolygon.SimplifyPreserveFromTo();

            return pPolygon;
        }

        /// <summary>
        /// 通过线创建面
        /// </summary>
        /// <param name="pPolyline">线</param>
        /// <returns>面</returns>
        public static IPolygon ConstructPolygonFromPolyline(IPolyline pPolyline)
        {
            IGeometryCollection pPolygonGeoCol = new PolygonClass();

            if ((pPolyline != null) && (!pPolyline.IsEmpty))
            {
                IGeometryCollection pPolylineGeoCol = pPolyline as IGeometryCollection;
                ISegmentCollection pSegCol = new RingClass();
                ISegment pSegment = null;
                object missing = Type.Missing;

                for (int i = 0; i < pPolylineGeoCol.GeometryCount; i++)
                {
                    ISegmentCollection pPolylineSegCol = pPolylineGeoCol.get_Geometry(i) as ISegmentCollection;
                    for (int j = 0; j < pPolylineSegCol.SegmentCount; j++)
                    {
                        pSegment = pPolylineSegCol.get_Segment(j);

                        pSegCol.AddSegment(pSegment, ref missing, ref missing);
                    }
                    pPolygonGeoCol.AddGeometry(pSegCol as IGeometry, ref missing, ref missing);
                }
            }
            return pPolygonGeoCol as IPolygon;
        }

        /// <summary>
        /// pointcollection 转 geometry
        /// </summary>
        /// <param name="Points"></param>
        /// <returns></returns>
        public static IGeometry GetPolygonGeometry(IPointCollection Points)
        {
            try
            {
                Ring ring = new RingClass();
                object missing = Type.Missing;

                ring.AddPointCollection(Points);

                IGeometryCollection pointPolygon = new PolygonClass();
                pointPolygon.AddGeometry(ring as IGeometry, ref missing, ref missing);
                IPolygon polyGonGeo = pointPolygon as IPolygon;
                //polyGonGeo.Close();
                polyGonGeo.SimplifyPreserveFromTo();
                return polyGonGeo as IGeometry;
            }
            catch (Exception ex)
            {
                string strErrInfo = "点集转Polygon图形失败！\n" + ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取Geometry的内、外Ring
        /// </summary>
        /// <param name="pGeoPolygon"></param>
        /// <returns></returns>
        public static Dictionary<IRing, List<IRing>> GetPolygonGeometry(IGeometry pGeoPolygon)
        {
            Dictionary<IRing, List<IRing>> dicRings = new Dictionary<IRing, List<IRing>>();
            esriGeometryType pGeoType = pGeoPolygon.GeometryType;

            try
            {
                IPolygon4 polygon4 = pGeoPolygon as IPolygon4;
                //外环个数
                int exRingCount = polygon4.ExteriorRingCount;

                IGeometryBag exteriorRingBag = polygon4.ExteriorRingBag;

                #region method1 获取外环
                //For each exterior rings find the interior rings associated with it 
                IEnumGeometry exteriorRingsEnum = exteriorRingBag as IEnumGeometry;
                exteriorRingsEnum.Reset();
                IRing currentExteriorRing = exteriorRingsEnum.Next() as IRing;

                while (currentExteriorRing != null)
                {
                    if (!dicRings.ContainsKey(currentExteriorRing))
                    {
                        dicRings[currentExteriorRing] = QPolygon.GetInteriorRings(ref polygon4, ref currentExteriorRing);
                    }
                    currentExteriorRing = exteriorRingsEnum.Next() as IRing;
                }
                #endregion

                #region method2 获取外环
                //IGeometryCollection exteriorRings = exteriorRingBag as IGeometryCollection;
                //遍历外部环
                //                 for (int i = 0; i < exteriorRings.GeometryCount; i++)
                //                 {
                //                     IGeometry exteriorRing = exteriorRings.get_Geometry(i);
                //                     List<IRing> rings = new List<IRing>();
                //                     //获取当前外环的内部环
                //                     IGeometryBag interiorRingBag = polygon4.get_InteriorRingBag(exteriorRing as IRing);
                //                     IGeometryCollection interiorRings = interiorRingBag as IGeometryCollection;
                //                     for (int j = 0; j < interiorRings.GeometryCount; j++)
                //                     {
                //                         IGeometry interiorRing = interiorRings.get_Geometry(j);
                //                         rings.Add(interiorRing as IRing);
                //                     }
                //                     dicRings[exteriorRing as IRing] = rings;
                //                 }
                #endregion
                return dicRings;
            }
            catch (Exception ex)
            {
                string strErrInfo = "点集转Polygon图形失败！\n" + ex.Message;
                return null;
            }
        }


        /// <summary>
        /// 获取外部Ring内的内部Ring
        /// </summary>
        /// <param name="polygon"></param>
        /// <param name="exteriorRing"></param>
        /// <returns></returns>
        public static List<IRing> GetInteriorRings(ref IPolygon4 polygon, ref IRing exteriorRing)
        {
            List<IRing> listInterRings = new List<IRing>();
            IGeometryBag interiorRings = polygon.get_InteriorRingBag(exteriorRing);
            IEnumGeometry interiorRingsEnum = interiorRings as IEnumGeometry;
            interiorRingsEnum.Reset();
            IRing currentInteriorRing = interiorRingsEnum.Next() as IRing;
            while (currentInteriorRing != null)
            {
                listInterRings.Add(currentInteriorRing);
                currentInteriorRing = interiorRingsEnum.Next() as IRing;
            }
            return listInterRings;
        }
        #endregion
    }
}

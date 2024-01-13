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
    /// 线
    /// </summary>
    public class QPolyline
    {
        #region =============  IPolyline  =============

        /// <summary>
        /// 返回指定距离打断线之后的打断点
        /// </summary>
        /// <param name="iPolyline"></param>
        /// <param name="intervDis">距离</param>
        /// <returns></returns>
        public static IPoint BreakPoint(IPolyline iPolyline, double intervDis)
        {
            if (iPolyline.Length <= intervDis)
            {
                return null;
            }
            IPoint p = new PointClass();
            bool isSplit;
            int splitIndex, segIndex;
            object o = Type.Missing;
            ESRI.ArcGIS.esriSystem.IClone pl = (iPolyline as ESRI.ArcGIS.esriSystem.IClone).Clone();
            (pl as IPolyline).SplitAtDistance(intervDis, false, false, out isSplit, out splitIndex, out segIndex);

            if (isSplit)
            {
                IPolyline frontLine = new PolylineClass();
                ISegmentCollection lineSegCol = (ISegmentCollection)pl;
                lineSegCol.RemoveSegments(segIndex, lineSegCol.SegmentCount - segIndex, true);
                lineSegCol.SegmentsChanged();
                frontLine = lineSegCol as IPolyline;
                p = frontLine.ToPoint;
            }
            return p;
        }

        /// <summary>
        /// 将一条线以间隔距离interv分割为N等分
        /// </summary>
        /// <param name="iPolyline"></param>
        /// <param name="interv"></param>
        /// <returns></returns>
        public static List<IPolyline> DivideLine2NParts(IPolyline iPolyline, double interv)
        {
            if (iPolyline.Length < interv)
            {
                return null;
            }

            ISegmentCollection tmpLineSegCol = (ISegmentCollection)iPolyline;
            int num = (int)Math.Ceiling(iPolyline.Length / interv);
            List<IPolyline> list = new List<IPolyline>();

            for (int i = 0; i < num - 1; i++)
            {
                bool isSplit;
                int splitIndex, segIndex;
                object o = Type.Missing;
                iPolyline.SplitAtDistance(interv, false, false, out isSplit, out splitIndex, out segIndex);

                if (isSplit)
                {
                    IPolyline frontLine = new PolylineClass();
                    IPolyline backLine = new PolylineClass();
                    ISegmentCollection lineSegCol = (ISegmentCollection)iPolyline;
                    ISegmentCollection backSegCol = (ISegmentCollection)backLine;
                    for (int j = segIndex; j < lineSegCol.SegmentCount; j++)
                    {
                        backSegCol.AddSegment(lineSegCol.get_Segment(j), ref o, ref o);
                    }

                    backSegCol.SegmentsChanged();
                    lineSegCol.RemoveSegments(segIndex, lineSegCol.SegmentCount - segIndex, true);
                    lineSegCol.SegmentsChanged();
                    frontLine = lineSegCol as IPolyline;
                    backLine = backSegCol as IPolyline;
                    list.Add(frontLine);
                    iPolyline = backLine;
                }
            }
            list.Add(iPolyline);
            iPolyline = tmpLineSegCol as IPolyline;
            return list;
        }

        /// <summary>
        /// 计算指定点到线之间的最小距离
        /// </summary>
        /// <param name="iPolyline"></param>
        /// <param name="iPoint"></param>
        /// <returns></returns>
        public static double MinDistance(IPolyline iPolyline, IPoint iPoint)
        {
            double minDist = 0;
            if (iPoint != null && !iPoint.IsEmpty)
            {
                IPoint outPoint = new PointClass();
                double distAlongCurveFrom = 0;
                double distFromCurve = 0;
                bool isRightSide = false;
                iPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, iPoint,
                    false, outPoint, ref distAlongCurveFrom, ref distFromCurve, ref isRightSide);
                minDist = distFromCurve;
            }
            return minDist;
        }

        /// <summary>
        /// 获取两条线要素交点
        /// </summary>
        /// <param name="iPolyline1"></param>
        /// <param name="iPolyline2"></param>
        /// <returns></returns>
        public static IPointCollection GetIntersectPoint(IPolyline iPolyline1, IPolyline iPolyline2)
        {
            IPointCollection pPC = null;
            ITopologicalOperator topoOperator = iPolyline1 as ITopologicalOperator;
            IGeometry geo = topoOperator.Intersect(iPolyline2, esriGeometryDimension.esriGeometry0Dimension);
            if (!geo.IsEmpty)
            {
                IPointCollection pc = geo as IPointCollection;
                pPC = pc;
            }
            return pPC;
        }

        /// <summary>
        /// 查询多段线的中间点
        /// </summary>
        /// <param name="pPolyline">传入的多段线</param>
        /// <returns>中间点</returns>
        public static IPoint QueryMiddlePoint(IPolyline pPolyline)
        {
            if (null == pPolyline)
            {
                return null;
            }
            double dLength = pPolyline.Length;
            IPoint pPoint = new PointClass();
            pPolyline.QueryPoint(esriSegmentExtension.esriNoExtension, dLength * 0.5, false, pPoint);
            return pPoint;
        }


        /// <summary>
        /// 由点集生成多段线的Geometry
        /// </summary>
        /// <param name="Points"></param>
        /// <returns></returns>
        public static IGeometry GetPolylineGeometry(IPointCollection Points)
        {
            try
            {
                IPolyline pPolyline = Points as IPolyline;
                IGeometry pGeoPolyline = pPolyline as IGeometry;

                return pGeoPolyline;

                //                 ITopologicalOperator pTop = pGeoPolyline as ITopologicalOperator;
                //                 pTop.Simplify();
                //                 Path path = new PathClass();
                //                 object missing = Type.Missing;
                // 
                //                 path.AddPointCollection(Points);
                // 
                //                 IGeometryCollection pointPolyline = new PolylineClass();
                //                 pointPolyline.AddGeometry(path as IGeometry,ref missing,ref missing);
                //                 IPolyline polylineGeo = pointPolyline as IPolyline;
                //                 //清除长度为0的Segment,并重新组织Polyline中的Segment
                //                 polylineGeo.SimplifyNetwork();
                //               return polylineGeo as IGeometry;
            }
            catch (Exception ex)
            {
                string strErrInfo = "点集转Polyline图形失败！\n" + ex.Message;
                return null;
            }

        }

        #endregion
    }
}

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
    /// 点
    /// </summary>
    public class QPoint
    {
        #region ============= 10.  IPoint  =============

        /// <summary>
        /// 从 IPointCollection 获得 点集
        /// </summary>
        /// <param name="ptCol"></param>
        /// <returns></returns>
        public static List<IPoint> GetPoints(ref IPointCollection ptCol)
        {
            List<IPoint> listPts = new List<IPoint>();
            int iCount = ptCol.PointCount;
            IPoint pt = new PointClass();
            //            IPoint pt2;
            for (int i = 0; i < iCount; i++)
            {
                ptCol.QueryPoint(i, pt);
                //                pt2 = ptCol.get_Point(i);
                listPts.Add(pt);
            }
            return listPts;
        }


        //reference: https://blog.csdn.net/m1m2m3mmm/article/details/104961193?utm_source=app#1.%20%E8%AE%A1%E7%AE%97%E4%B8%A4%E4%B8%AA%E5%90%91%E9%87%8F%E4%B9%8B%E9%97%B4%E7%9A%84%E5%A4%B9%E8%A7%92
        /// <summary>
        /// 计算连续三点构成的两个向量的夹角
        /// </summary>
        /// <param name="pt1">起点1</param>
        /// <param name="pt2">连接点2</param>
        /// <param name="pt3">终点3</param>
        /// <returns>向量夹角</returns>
        public static double calAngleVectors(IPoint pt1, IPoint pt2, IPoint pt3)
        {
            IPoint vec1 = new ESRI.ArcGIS.Geometry.Point();
            vec1.X = pt2.X - pt1.X;
            vec1.Y = pt2.Y - pt1.Y;

            IPoint vec2 = new ESRI.ArcGIS.Geometry.Point();
            vec2.X = pt3.X - pt2.X;
            vec2.Y = pt3.Y - pt2.Y;

            double len1 = Math.Sqrt(vec1.X * vec1.X + vec1.Y * vec1.Y);
            double len2 = Math.Sqrt(vec2.X * vec2.X + vec2.Y * vec2.Y);
            double v12 = vec1.X * vec2.X + vec1.Y * vec2.Y;
            double cos = v12 / (len1 * len2);
            return Math.Acos(cos) * 180 / Math.PI;
        }


        /// <summary>
        /// 计算连续三点构成的两个向量的夹角
        /// </summary>
        /// <param name="pt1">起点1</param>
        /// <param name="pt2">连接点2</param>
        /// <param name="pt3">终点3</param>
        /// <returns>向量夹角</returns>
        //public static double calAngleVectors(System.Drawing.PointF pt1, System.Drawing.PointF pt2, System.Drawing.PointF pt3)
        //{
        //    System.Drawing.PointF vec1 = new System.Drawing.PointF();
        //    vec1.X = pt2.X - pt1.X;
        //    vec1.Y = pt2.Y - pt1.Y;

        //    System.Drawing.PointF vec2 = new System.Drawing.PointF();
        //    vec2.X = pt3.X - pt2.X;
        //    vec2.Y = pt3.Y - pt2.Y;

        //    double len1 = Math.Sqrt(vec1.X * vec1.X + vec1.Y * vec1.Y);
        //    double len2 = Math.Sqrt(vec2.X * vec2.X + vec2.Y * vec2.Y);
        //    double v12 = vec1.X * vec2.X + vec1.Y * vec2.Y;
        //    double cos = v12 / (len1 * len2);
        //    double arc = Math.Acos(cos);
        //    return Math.Acos(cos) * 180 / Math.PI;
        //}
        #endregion
    }
}

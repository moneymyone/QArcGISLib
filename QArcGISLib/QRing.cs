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
    /// Ring类
    /// </summary>
    public class QRing
    {

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
                        dicRings[currentExteriorRing] = GetInteriorRings(ref polygon4, ref currentExteriorRing);
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

    }
}

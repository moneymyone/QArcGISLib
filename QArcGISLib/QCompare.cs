/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib
{
    /// <summary>
    /// 比较类
    /// </summary>
    public class QCompare
    {

        /// <summary>
        /// 判断两个图形的关系
        /// </summary>
        /// <param name="pGeometryA"></param>
        /// <param name="pGeometryB"></param>
        /// <param name="method"></param>
        /// <returns></returns>
        public static bool CompareGeometry(IGeometry pGeometryA, IGeometry pGeometryB, EGeoCompareMethod method = EGeoCompareMethod.Equals)
        {
            IRelationalOperator pRelOperator = pGeometryA as IRelationalOperator;
            if (method == EGeoCompareMethod.Contains)
            {
                if (pRelOperator.Contains(pGeometryB)) return true;
                else return false;
            }
            else if (method == EGeoCompareMethod.Equals)
            {
                if (pRelOperator.Equals(pGeometryB)) return true;
                else return false;
            }
            else if (method == EGeoCompareMethod.Crosses)
            {
                if (pRelOperator.Crosses(pGeometryB)) return true;
                else return false;
            }
            else if (method == EGeoCompareMethod.Disjoint)
            {
                if (pRelOperator.Disjoint(pGeometryB)) return true;
                else return false;
            }
            else if (method == EGeoCompareMethod.Overlaps)
            {
                if (pRelOperator.Overlaps(pGeometryB)) return true;
                else return false;
            }
            else if (method == EGeoCompareMethod.Relation)
            {
                //relationDescription 根据需要修改!
                string relationDescription = "RELATE(G1, G2, 'T********')";
                if (pRelOperator.Relation(pGeometryB, relationDescription)) return true;
                else return false;
            }
            else if (method == EGeoCompareMethod.Touches)
            {
                if (pRelOperator.Touches(pGeometryB)) return true;
                else return false;
            }
            else if (method == EGeoCompareMethod.Within)
            {
                IRelationalOperator pRelOperatorB = pGeometryB as IRelationalOperator;
                if (pRelOperator.Within(pGeometryB) || pRelOperatorB.Within(pGeometryA)) return true;
                else return false;
            }
            return false;
        }
        /// <summary>
        /// 判断两个要素是否相同，字段比较和图形比较
        /// </summary>
        /// <param name="pFeaSrc"></param>
        /// <param name="pFeaTar"></param>
        /// <param name="fields">字段比较</param>
        /// <returns></returns>
        public static bool IsFeatureEqual(ref IFeature pFeaSrc, ref IFeature pFeaTar, params string[] fields)
        {
            if (pFeaSrc == null && pFeaTar == null) return true;
            for (int i = 0; i < fields.Length; i++)
            {
                string field = fields[i];
                if (pFeaSrc.get_Value(pFeaSrc.Fields.FindField(field)).ToString() != pFeaTar.get_Value(pFeaTar.Fields.FindField(field)).ToString())
                {
                    return false;
                }
            }
            if (CompareGeometry(pFeaSrc.ShapeCopy, pFeaTar.ShapeCopy, EGeoCompareMethod.Equals))
            {
                return true;
            }
            else return false;
        }

        /// <summary>
        /// 判断两个要素是否相同
        /// </summary>
        /// <param name="pFeaSrc"></param>
        /// <param name="pFeaTar"></param>
        /// <param name="fieldIndexSrc"></param>
        /// <param name="fieldIndexTar"></param>
        /// <returns></returns>
        public static bool IsFeatureEqual(ref IFeature pFeaSrc, ref IFeature pFeaTar, int[] fieldIndexSrc, int[] fieldIndexTar)
        {
            if (pFeaSrc == null && pFeaTar == null) return true;
            for (int i = 0; i < fieldIndexSrc.Length; i++)
            {
                int indexSrc = fieldIndexSrc[i];
                int indexTar = fieldIndexTar[i];
                if (pFeaSrc.get_Value(indexSrc).ToString() != pFeaTar.get_Value(indexTar).ToString())
                {
                    return false;
                }
            }
            if (pFeaSrc.Shape!= null && pFeaTar.Shape!=null && CompareGeometry(pFeaSrc.ShapeCopy, pFeaTar.ShapeCopy, EGeoCompareMethod.Equals))
            {
                return true;
            }
            else return false;
        }


    }
}

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

namespace QArcGISLib.GISCoordTrans
{
    /// <summary>
    /// 坐标转换
    /// </summary>
    public class GISCTrans
    {
        #region ========== 坐标转换 ==========

        #region 转换
        /// <summary>
        /// 
        /// </summary>
        /// <param name="x_src"></param>
        /// <param name="y_src"></param>
        /// <param name="x_tar"></param>
        /// <param name="y_tar"></param>
        private static void WGS84toMars(double x_src, double y_src, out double x_tar, out double y_tar)
        {
            double[] arrGCJ02 = new Coordtransform().Wgs84toGcj02(x_src, y_src);
            x_tar = arrGCJ02[0];
            y_tar = arrGCJ02[1];
        }

        /// <summary>
        /// WGS84toBaidu
        /// </summary>
        /// <param name="x_src"></param>
        /// <param name="y_src"></param>
        /// <param name="x_tar"></param>
        /// <param name="y_tar"></param>
        private static void WGS84toBaidu(double x_src, double y_src, out double x_tar, out double y_tar)
        {
            double[] arrBd09 = new Coordtransform().Wgs84tobd09(x_src, y_src);
            x_tar = arrBd09[0];
            y_tar = arrBd09[1];
        }

        /// <summary>
        /// Mars2WGS84
        /// </summary>
        /// <param name="x_src"></param>
        /// <param name="y_src"></param>
        /// <param name="x_tar"></param>
        /// <param name="y_tar"></param>
        private static void Mars2WGS84(double x_src, double y_src, out double x_tar, out double y_tar)
        {
            double[] arrWGS84 = new Coordtransform().Gcj02toWGS84(x_src, y_src);
            x_tar = arrWGS84[0];
            y_tar = arrWGS84[1];
        }

        /// <summary>
        /// Mars2Baidu
        /// </summary>
        /// <param name="x_src"></param>
        /// <param name="y_src"></param>
        /// <param name="x_tar"></param>
        /// <param name="y_tar"></param>
        private static void Mars2Baidu(double x_src, double y_src, out double x_tar, out double y_tar)
        {
            double[] arrBd09 = new Coordtransform().Gcj02toBd09(x_src, y_src);
            x_tar = arrBd09[0];
            y_tar = arrBd09[1];
        }

        /// <summary>
        /// Baidu2WGS84
        /// </summary>
        /// <param name="x_src"></param>
        /// <param name="y_src"></param>
        /// <param name="x_tar"></param>
        /// <param name="y_tar"></param>
        private static void Baidu2WGS84(double x_src, double y_src, out double x_tar, out double y_tar)
        {
            double[] arrWGS84 = new Coordtransform().Bd09towgs84(x_src, y_src);
            x_tar = arrWGS84[0];
            y_tar = arrWGS84[1];
        }

        /// <summary>
        /// Baidu2Mars
        /// </summary>
        /// <param name="x_src"></param>
        /// <param name="y_src"></param>
        /// <param name="x_tar"></param>
        /// <param name="y_tar"></param>
        private static void Baidu2Mars(double x_src, double y_src, out double x_tar, out double y_tar)
        {
            double[] arrGCJ02 = new Coordtransform().Bd09toGcj02(x_src, y_src);
            x_tar = arrGCJ02[0];
            y_tar = arrGCJ02[1];
        }


        /// <summary>
        /// WGS84转Mercator
        /// </summary>
        /// <param name="long_src"></param>
        /// <param name="lat_src"></param>
        /// <param name="x_tar"></param>
        /// <param name="y_tar"></param>
        public static void WGS84ToWebMercator(double long_src, double lat_src, out double x_tar, out double y_tar)
        {
            x_tar = 0;
            y_tar = 0;
            double[] xy = new Coordtransform().WGS84ToMercator(long_src, lat_src);
            x_tar = xy[0];
            x_tar = xy[1];
        }

        /// <summary>
        /// WGS84转Mercator
        /// </summary>
        /// <param name="x_src"></param>
        /// <param name="y_src"></param>
        /// <param name="long_tar"></param>
        /// <param name="lat_tar"></param>
        public static void WebMercator2WGS84(double x_src, double y_src, out double long_tar, out double lat_tar)
        {
            long_tar = 0;
            lat_tar = 0;
            double[] lonlat = new Coordtransform().WebMercator2WGS84(x_src, y_src);
            long_tar = lonlat[0];
            lat_tar = lonlat[1];
        }


        #endregion
        

        //单点坐标转换
        /// <summary>
        /// 单点坐标转换
        /// </summary>
        /// <param name="ptSrc">原始点</param>
        /// <param name="ptDest">转换之后的点</param>
        /// <param name="srcCoord">源坐标</param>
        /// <param name="tarCoord">转换之后的坐标</param>
        public static void CoorTrans(IPoint ptSrc, out IPoint ptDest, ECoordSys srcCoord, ECoordSys tarCoord)
        {
            ptDest = ptSrc;
            double xSrc = 0.0, ySrc = 0.0, xDest = 0.0, yDest = 0.0;
            xSrc = ptSrc.X;
            ySrc = ptSrc.Y;
            if (srcCoord == ECoordSys.WGS84 && tarCoord == ECoordSys.MARS)
            {
                WGS84toMars(xSrc, ySrc, out xDest, out yDest);
            }
            else if (srcCoord == ECoordSys.WGS84 && tarCoord == ECoordSys.BAIDU)
            {
                WGS84toBaidu(xSrc, ySrc, out xDest, out yDest);
            }
            else if (srcCoord == ECoordSys.MARS && tarCoord == ECoordSys.WGS84)
            {
                Mars2WGS84(xSrc, ySrc, out xDest, out yDest);
            }
            else if (srcCoord == ECoordSys.MARS && tarCoord == ECoordSys.BAIDU)
            {
                Mars2Baidu(xSrc, ySrc, out xDest, out yDest);
            }
            else if (srcCoord == ECoordSys.BAIDU && tarCoord == ECoordSys.WGS84)
            {
                Baidu2WGS84(xSrc, ySrc, out xDest, out yDest);
            }
            else if (srcCoord == ECoordSys.BAIDU && tarCoord == ECoordSys.MARS)
            {
                Baidu2Mars(xSrc, ySrc, out xDest, out yDest);
            }
            else if (srcCoord == ECoordSys.WEBMERCATOR && tarCoord == ECoordSys.WGS84)
            {
                WebMercator2WGS84(xSrc, ySrc, out xDest, out yDest);
            }
            else if (srcCoord == ECoordSys.WGS84 && tarCoord == ECoordSys.WEBMERCATOR)
            {
                WGS84ToWebMercator(xSrc, ySrc, out xDest, out yDest);
            }
            ptDest.PutCoords(xDest, yDest);
        }

        /// <summary>
        /// 对pointCollection进行坐标转换
        /// </summary>
        /// <param name="pointCollection"></param>
        /// <param name="coordSrc"></param>
        /// <param name="coordTar"></param>
        public static void CoordTransPointCollection(ref IPointCollection pointCollection, ref ECoordSys coordSrc, ref ECoordSys coordTar)
        {
            IPoint ptSrc;
            IPoint ptCal;
            //坐标转换
            for (int j2 = 0; j2 < pointCollection.PointCount; j2++)
            {
                ptSrc = new PointClass();
                pointCollection.QueryPoint(j2, ptSrc);
                CoorTrans(ptSrc, out ptCal, coordSrc, coordTar);
                pointCollection.UpdatePoint(j2, ptCal);
            }
        }

        /// <summary>
        /// 对Ring坐标转换
        /// </summary>
        /// <param name="ring"></param>
        /// <param name="coordSrc"></param>
        /// <param name="coordTar"></param>
        public static void CoordTransRing(ref IRing ring, ref ECoordSys coordSrc, ref ECoordSys coordTar)
        {
            IPointCollection ptCollection = ring as IPointCollection;
            CoordTransPointCollection(ref ptCollection, ref coordSrc, ref coordTar);
        }

        /// <summary>
        /// 对Polyline坐标转换
        /// </summary>
        /// <param name="polyline"></param>
        /// <param name="coordSrc"></param>
        /// <param name="coordTar"></param>
        /// <returns></returns>
        public static IGeometry CoordTransPolyline(ref IPolyline polyline, ref ECoordSys coordSrc, ref ECoordSys coordTar)
        {
            IGeometry pGeoPline = null;
            IPointCollection ptCollection = polyline as IPointCollection;
            CoordTransPointCollection(ref ptCollection, ref coordSrc, ref coordTar);
            pGeoPline = QPolyline.GetPolylineGeometry(ptCollection);
            return pGeoPline;
        }

        /// <summary>
        /// 转换面对象坐标
        /// </summary>
        /// <param name="pGeoPolygon"></param>
        /// <param name="pCoordSrc"></param>
        /// <param name="pCoordTar"></param>
        /// <returns></returns>
        public static IGeometry CoordTransPolygon(ref IGeometry pGeoPolygon, ref ECoordSys pCoordSrc, ref ECoordSys pCoordTar)
        {
            Dictionary<IRing, List<IRing>> dicRings = QPolygon.GetPolygonGeometry(pGeoPolygon);
            IGeometryCollection geoCollection = new PolygonClass();
            object missing = Type.Missing;
            if (dicRings.Count > 0)
            {
                var ge = dicRings.GetEnumerator();
                while (ge.MoveNext())
                {
                    IRing exteriorRing = ge.Current.Key;
                    CoordTransRing(ref exteriorRing, ref pCoordSrc, ref pCoordTar);
                    geoCollection.AddGeometry(exteriorRing as IGeometry, ref missing, ref missing);

                    if (ge.Current.Value != null)
                    {
                        List<IRing> listInterRings = ge.Current.Value;
                        for (int i = 0; i < listInterRings.Count; i++)
                        {
                            IRing interRing = listInterRings[i];
                            CoordTransRing(ref interRing, ref pCoordSrc, ref pCoordTar);
                            geoCollection.AddGeometry(interRing as IGeometry, ref missing, ref missing);
                        }
                    }
                }
            }

            IPolygon polygonNew = geoCollection as IPolygon;
            //polygonNew.SimplifyPreserveFromTo();
            return polygonNew as IGeometry;
        }

        /// <summary>
        /// 转换单个Feature
        /// </summary>
        /// <param name="pFeature"></param>
        /// <param name="pCoordSrc"></param>
        /// <param name="pCoordTar"></param>
        /// <param name="pGeoType"></param>
        public static bool TransFeature(IFeature pFeature, ECoordSys pCoordSrc, ECoordSys pCoordTar, esriGeometryType pGeoType)
        {
            try
            {
                IPoint ptSrc = null;
                IPoint ptCal = null;
                IPointCollection pointCollection = null;

                esriFeatureType type = pFeature.FeatureType;//获取要素的类型

                IGeometry pGeo = pFeature.ShapeCopy;

                if (pGeoType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPoint)
                {
                    ptSrc = pGeo as IPoint;
                    CoorTrans(ptSrc, out ptCal, pCoordSrc, pCoordTar);
                    pFeature.Shape = ptCal;
                }
                else if (pGeoType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolyline)
                {
                    pointCollection = new PolylineClass();
                    IPolyline polyline = pGeo as IPolyline;

                    pGeo = CoordTransPolyline(ref polyline, ref pCoordSrc, ref pCoordTar);
                    pFeature.Shape = pGeo;
                }
                else if (pGeoType == ESRI.ArcGIS.Geometry.esriGeometryType.esriGeometryPolygon)
                {
                    pFeature.Shape = CoordTransPolygon(ref pGeo, ref pCoordSrc, ref pCoordTar);
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }



        /// <summary>
        /// 转换原始FeatureClass,存储到目标FeatureClass中
        /// </summary>
        /// <param name="pSFeaCls"></param>
        /// <param name="pTFeaCls">目标feature class</param>
        /// <param name="pCoordSrc"></param>
        /// <param name="pCoordTar"></param>
        /// <param name="pGeoType"></param>
        /// <param name="dispalyProgress"></param>
        /// <returns></returns>
        public static bool TransFeatureClass(ref IFeatureClass pSFeaCls, ref IFeatureClass pTFeaCls, ref ECoordSys pCoordSrc, ref ECoordSys pCoordTar, esriGeometryType pGeoType,
            Func<int, int> dispalyProgress = null)
        {
            bool bTrans = false;
            bool hasError = false;
            string sMsg = "";
            string error = "";
            string sFeaClasNm = pSFeaCls.AliasName;
            IFeature pFeature = null;
            List<IFeature> listSFeature = QFeature.GetFeature(pSFeaCls);
            List<IFeature> listTFeature = new List<IFeature>();
            List<Task> listTask = new List<Task>();
            int iFeaCount = listSFeature.Count;
            ECoordSys coordSrc = pCoordSrc;
            ECoordSys coordTar = pCoordTar;
            esriGeometryType pShape = pSFeaCls.ShapeType;
            for (int j1 = 0; j1 < iFeaCount; j1++)
            {
                pFeature = listSFeature[j1];
                //                sMsg = string.Format("   > [{0}] -> 要素ID:{1}", pSFeaCls.AliasName, pFeature.OID);
                //                Console.WriteLine(sMsg);
                var t = Task.Factory.StartNew(() =>
                {
                    bTrans = TransFeature(pFeature, coordSrc, coordTar, pShape);

                    if (dispalyProgress != null) { dispalyProgress(Convert.ToInt32((j1 + 1) / (iFeaCount * 1.0) * 100)); }

                    if (!bTrans)
                    {
                        sMsg = string.Format("   > [{0}] 原要素类中要素[ID:{1}]坐标转换失败: {2}", sFeaClasNm, pFeature.OID, sMsg);
                        Console.WriteLine(sMsg);
                        hasError = true;
                    }
                    else
                    {
                        listTFeature.Add(pFeature);
                    }
                });
                listTask.Add(t);
                t.Wait();
            }
            //Task.WaitAll(listTask.ToArray());
            
            bool bWrite = QFeature.WriteFeature2FeatureClass(ref pTFeaCls, ref listTFeature,ref error);
            if (!bWrite)
            {
                sMsg = $"   > [{pTFeaCls.AliasName}]数据集写入要素列表失败！{error}";
                Console.WriteLine(sMsg);
            }
            return (!hasError) && bWrite;
        }

        /// <summary>
        /// 根据已有要素类和目标工作空间，写入目标要素类
        /// </summary>
        /// <param name="pSFeaCls"></param>
        /// <param name="pTWorkspace"></param>
        /// <param name="pCoordSrd"></param>
        /// <param name="pCoordTar"></param>
        /// <param name="pSpaRefSrc"></param>
        /// <returns></returns>
        public static bool TransFeatureClass(ref IFeatureClass pSFeaCls, ref IWorkspace pTWorkspace,
            ref ECoordSys pCoordSrd, ref ECoordSys pCoordTar, ISpatialReference pSpaRefSrc)
        {
            string pFeClsName = pSFeaCls.AliasName;
            string sMsg = "";
            //判断数据集下是否存在要素类
            if (QExist.IsFeatureClassInWorkspace(ref pTWorkspace, pFeClsName))
            {
                if (!QFeatureClass.DeleteFeatureClass(ref pTWorkspace, pFeClsName))
                {
                    sMsg = string.Format("   > [{0}] 工作空间删除要素类[{1}]失败！", System.IO.Path.GetFileName(pTWorkspace.PathName), pFeClsName);
                    Console.WriteLine(sMsg);
                    return false;
                }
            }
            sMsg = string.Format("\n   > 转换独立要素类: {0} ", pFeClsName);
            Console.WriteLine(sMsg);
            //新建数据集
            pSpaRefSrc = QSpatialReference.GetSpatialReference(pSFeaCls);
            IFeatureClass pTFeaCls = QFeatureClass.CreateFeatureClass(ref pTWorkspace, ref pSFeaCls, pSpaRefSrc);
            return TransFeatureClass(ref pSFeaCls, ref pTFeaCls, ref pCoordSrd, ref pCoordTar, pSFeaCls.ShapeType); ;
        }

        /// <summary>
        /// 批量转换FeatureClass列表，并写入文件
        /// </summary>
        /// <param name="listSFC"></param>
        /// <param name="pTWorkspace"></param>
        /// <param name="pTFeDataset"></param>
        /// <param name="pCoordSrd"></param>
        /// <param name="pCoordTar"></param>
        /// <param name="pSpaRefSrc"></param>
        /// <returns></returns>
        public static bool TransListFeatureClass(ref List<IFeatureClass> listSFC, ref IWorkspace pTWorkspace,
                                            ref IFeatureDataset pTFeDataset,
                                            ref ECoordSys pCoordSrd, ref ECoordSys pCoordTar, ISpatialReference pSpaRefSrc)
        {
            bool bTrans = false;
            bool hasError = false;
            string sMsg = "";
            string error = "";
            IFeatureClass pFeatureCls = null;
            IFeatureClass pTFeaCls = null;
            string pDSName = pTFeDataset.Name;
            string pFCName = "";
            if (listSFC.Count == 0)
            {
                sMsg = string.Format("   > [{0}] 数据集为空！", pDSName);
                Console.WriteLine(sMsg);
                return false;
            }
            IWorkspace2 pTWS2 = pTWorkspace as  IWorkspace2;
            for (int j = 0; j < listSFC.Count; j++)
            {
                pFeatureCls = listSFC[j];
                pFCName = pFeatureCls.AliasName;
                //判断数据集下是否存在要素类
                if (QExist.IsFeatureClassInDataset(ref pTWorkspace, pDSName, pFCName))
                {
                    bool bDel = QFeatureClass.DeleteFeatureClass(ref pTWorkspace, pDSName, pFCName);
                    if (!bDel)
                    {
                        sMsg = string.Format("   > [{0}] 删除数据集中已有要素类[{1}]失败！", pDSName, pFCName);
                        Console.WriteLine(sMsg);
                        hasError = true;
                        continue;
                    }
                }
                //新建数据集
                pTFeaCls = QFeatureClass.CreateFeatureClass(ref pTFeDataset, ref pFeatureCls,ref error);

                sMsg = string.Format("              >> {0} -> {1}", pDSName, pFCName);
                Console.WriteLine(sMsg);

                bTrans = TransFeatureClass(ref pFeatureCls, ref pTFeaCls, ref pCoordSrd, ref pCoordTar, pFeatureCls.ShapeType);
                if (!bTrans)
                {
                    sMsg = string.Format("   > [{0}] 数据集写入要素类[{1}]有误！", pDSName, pFCName);
                    Console.WriteLine(sMsg);
                    hasError = true;
                }
            }
            QRelease.ReleaseESRIObject(pFeatureCls, pTFeaCls);
            return (!hasError) && bTrans;
        }

        /// <summary>
        /// 转换featrueClass坐标
        /// </summary>
        /// <param name="listSFC"></param>
        /// <param name="pTWorkspace"></param>
        /// <param name="pCoordSrd"></param>
        /// <param name="pCoordTar"></param>
        /// <param name="pSpaRefSrc"></param>
        /// <returns></returns>
        public static bool TransListFeatureClass(ref List<IFeatureClass> listSFC, ref IWorkspace pTWorkspace,
                                    ref ECoordSys pCoordSrd, ref ECoordSys pCoordTar, ISpatialReference pSpaRefSrc)
        {
            bool bTrans = false;
            bool hasError = false;
            IFeatureClass pSFeaCls = null;
            IFeatureClass pTFeaCls = null;
            IFeatureWorkspace pTFeaWS = null;
            IWorkspace2 pTWS2 = pTWorkspace as IWorkspace2;
            string pFCName = "";
            string sMsg = "";
            if (listSFC.Count == 0) return false;
            for (int j = 0; j < listSFC.Count; j++)
            {
                pSFeaCls = listSFC[j];
                pFCName = pSFeaCls.AliasName;
                //判断数据集下是否存在要素类
                if (pTWS2.get_NameExists(esriDatasetType.esriDTFeatureClass, pFCName))
                {
                    bool bDel = QFeatureClass.DeleteFeatureClass(ref pTWorkspace, pFCName);
                    if (!bDel)
                    {
                        sMsg = string.Format("   > [{0}] 删除工作空间中已有要素类[{1}]失败！", System.IO.Path.GetFileName(pTWorkspace.PathName), pFCName);
                        Console.WriteLine(sMsg);
                        hasError = true;
                        continue;
                    }
                }

                //新建数据集
                pTFeaCls = QFeatureClass.CreateFeatureClass(ref pTWorkspace, ref pSFeaCls, pSpaRefSrc);

                sMsg = string.Format("   > 转换独立要素类: {0} ", pFCName);
                Console.WriteLine(sMsg);

                bTrans = TransFeatureClass(ref pSFeaCls, ref pTFeaCls, ref pCoordSrd, ref pCoordTar, pSFeaCls.ShapeType);
                if (!bTrans)
                {
                    sMsg = string.Format("   > [{0}] 要素类写入要素类[{1}]有误！", pSFeaCls.AliasName, pFCName);
                    Console.WriteLine(sMsg);
                    hasError = true;
                }
            }
            QRelease.ReleaseESRIObject(pSFeaCls, pTFeaCls, pTFeaWS);
            return bTrans && (!hasError);
        }
        #endregion
    }
}

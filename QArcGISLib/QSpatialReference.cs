/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.Carto;
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
    /// 空间参考
    /// </summary>
    public class QSpatialReference
    {
        #region ============= 空间参考（地理坐标）  =============
        #region =======  创建空间参考（地理坐标）  =======
        /// <summary>  
        /// 根据prj文件创建空间参考  
        /// </summary>  
        /// <param name="strProFile">空间参照文件</param>  
        /// <returns></returns>  
        public static ISpatialReference CreateSpatialReference(string strProFile)
        {
            ISpatialReference pSpatialReference = null;
            if (!System.IO.File.Exists(strProFile))
            {
                return pSpatialReference;
            }
            ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            pSpatialReference = pSpatialReferenceFactory.CreateESRISpatialReferenceFromPRJFile(strProFile);
            return pSpatialReference;
        }

        /// <summary>
        /// 创建空投影(Unknow)
        /// </summary>
        /// <param name="extent"></param>
        /// <returns></returns>
        public static ISpatialReference CreateUnKnownSpatialReference(params double[] extent)
        {
            ISpatialReference pSR = new UnknownCoordinateSystemClass();
            if (extent.Length != 4)
            {
                return pSR;
            }
            else
            {
                //设置空间范围  
                pSR.SetDomain(extent[0], extent[1], extent[2], extent[3]);
            }
            return pSR;
        }

        /// <summary>  
        /// 创建地理坐标系  
        /// </summary>  
        /// <param name="gcsType">esriSRProjCS4Type</param>  
        /// <returns></returns>  
        public static ISpatialReference CreateGeographicCoordinate(esriSRProjCS4Type gcsType)
        {
            ISpatialReferenceFactory pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateGeographicCoordinateSystem((int)gcsType);
            return pSpatialReference;
        }


        /// <summary>  
        /// 创建投影坐标系  
        /// </summary>  
        /// <param name="pcsType">esriSRProjCS4Type</param>  
        /// <returns></returns>  
        public static ISpatialReference CreateProjectedCoordinate(esriSRProjCS4Type pcsType)
        {
            ISpatialReferenceFactory2 pSpatialReferenceFactory = new SpatialReferenceEnvironmentClass();
            ISpatialReference pSpatialReference = pSpatialReferenceFactory.CreateProjectedCoordinateSystem((int)pcsType);
            return pSpatialReference;
        }
        #endregion

        #region =======  获取空间参考（地理坐标）  =======

        /// <summary>
        /// 获取数据集空间参考
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="strDatasetName"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static ISpatialReference GetSpatialReference(ref IWorkspace pWorkspace, string strDatasetName, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if(pWorkspace == null)
                {
                    strErrInfo = "工作空间为空！";
                    return null;
                }
                IFeatureWorkspace pFws = pWorkspace as IFeatureWorkspace;
                IFeatureDataset pFeatureDataset = pFws.OpenFeatureDataset(strDatasetName);
                IGeoDataset pGeoDataset = pFeatureDataset as IGeoDataset;
                return pGeoDataset.SpatialReference;
            }
            catch (Exception ex)
            {
                strErrInfo = "获取数据集空间参考失败！" + ex.Message;
                return null;
            }
        }

        /// <summary>  
        /// 获取要素集空间参考  
        /// </summary>  
        /// <param name="pFeatureDataset">要素集</param>  
        /// <returns></returns>  
        public static ISpatialReference GetSpatialReference(IFeatureDataset pFeatureDataset)
        {
            IGeoDataset pGeoDataset = pFeatureDataset as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            return pSpatialReference;
        }

        /// <summary>  
        /// 获取要素类空间参考  
        /// </summary>  
        /// <param name="pFeatureClass">要素类</param>  
        /// <returns></returns>  
        public static ISpatialReference GetSpatialReference(IFeatureClass pFeatureClass)
        {
            IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            return pSpatialReference;
        }

        /// <summary>  
        /// 获取要素层空间参考  
        /// </summary>  
        /// <param name="pFeatureLayer">要素层</param>  
        /// <returns></returns>  
        public static ISpatialReference GetSpatialReference(IFeatureLayer pFeatureLayer)
        {
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
            ISpatialReference pSpatialReference = pGeoDataset.SpatialReference;
            return pSpatialReference;
        }

        /// <summary>
        /// 获得源坐标系统和目标坐标系统，平面坐标系
        /// </summary>
        /// <param name="pEnumType">坐标转换类型</param>
        /// <param name="fromLo"></param>
        /// <param name="toLt"></param>
        /// <param name="iMark">1：源坐标系统有带号，2：目标坐标系统需要添加带号，3：源坐标和目标坐标均没有带号 </param>
        /// <param name="pSpaFrom"></param>
        /// <param name="pSpaTo"></param>
        public static void GetSourceTargetSpatialRef(ETransType pEnumType, double fromLo, double toLt, double iMark, ref ISpatialReference pSpaFrom, ref ISpatialReference pSpaTo)
        {
            string pathFrom = "";
            string pathTo = "";

            if (iMark == 3)
            {//源坐标和目标坐标均没有带号
                switch (pEnumType)
                {
                    case ETransType.NJ92XA80:
                        pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Xian80_118'50.prj";
                        if (toLt == 117)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "Xian80_117.prj";
                        else if (toLt == 120)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "Xian80_120.prj";
                        break;
                    case ETransType.NJ92WGS84:
                        pathFrom = AppDomain.CurrentDomain.BaseDirectory + "WGS84_118'50.prj";
                        if (toLt == 117)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "WGS84_117.prj";
                        else if (toLt == 120)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "WGS84_120.prj";
                        break;
                    case ETransType.NJ92BJ54:
                        pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_118'50.prj";
                        if (toLt == 117)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_117.prj";
                        else if (toLt == 120)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_120.prj";
                        break;
                    case ETransType.XA80NJ92:
                        if (fromLo == 117)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Xian80_117.prj";
                        else if (fromLo == 120)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Xian80_120.prj";
                        pathTo = AppDomain.CurrentDomain.BaseDirectory + "Xian80_118'50.prj";
                        break;
                    case ETransType.WGS84NJ92:
                        if (fromLo == 117)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "WGS84_117.prj";
                        else if (fromLo == 120)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "WGS84_120.prj";
                        pathTo = AppDomain.CurrentDomain.BaseDirectory + "WGS84_118'50.prj";
                        break;
                    case ETransType.BJ54NJ92:
                        if (fromLo == 117)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_117.prj";
                        else if (fromLo == 120)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_120.prj";
                        pathTo = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_118'50.prj";
                        break;
                    case ETransType.NJ2008NJ92:
                        pathFrom = AppDomain.CurrentDomain.BaseDirectory + "NJ08_118'50.prj";
                        pathTo = AppDomain.CurrentDomain.BaseDirectory + "NanJing92_118'50.prj";
                        break;
                    case ETransType.NJ92NJ2008:
                        pathFrom = AppDomain.CurrentDomain.BaseDirectory + "NanJing92_118'50.prj";
                        pathTo = AppDomain.CurrentDomain.BaseDirectory + "NJ08_118'50.prj";
                        break;
                }
            }

            else if (iMark == 1)
            {//源坐标系统有带号
                switch (pEnumType)
                {
                    case ETransType.XA80NJ92:
                        if (fromLo == 117)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Xian80_117_20.prj";
                        else if (fromLo == 120)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Xian80_120_40.prj";
                        pathTo = AppDomain.CurrentDomain.BaseDirectory + "Xian80_118'50.prj";
                        break;
                    case ETransType.WGS84NJ92:
                        if (fromLo == 117)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "WGS84_117_20.prj";
                        else if (fromLo == 120)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "WGS84_120_40.prj";
                        pathTo = AppDomain.CurrentDomain.BaseDirectory + "WGS84_118'50.prj";
                        break;
                    case ETransType.BJ54NJ92:
                        if (fromLo == 117)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_117_20.prj";
                        else if (fromLo == 120)
                            pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_120_40.prj";
                        pathTo = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_118'50.prj";
                        break;
                }
            }

            else if (iMark == 2)
            {//目标坐标系统需要添加带号
                switch (pEnumType)
                {
                    case ETransType.NJ92XA80:
                        pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Xian80_118'50.prj";
                        if (toLt == 117)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "Xian80_117_20.prj";
                        else if (toLt == 120)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "Xian80_120_40.prj";
                        break;
                    case ETransType.NJ92WGS84:
                        pathFrom = AppDomain.CurrentDomain.BaseDirectory + "WGS84_118'50.prj";
                        if (toLt == 117)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "WGS84_117_20.prj";
                        else if (toLt == 120)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "WGS84_120_40.prj";
                        break;
                    case ETransType.NJ92BJ54:
                        pathFrom = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_118'50.prj";
                        if (toLt == 117)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_117_20.prj";
                        else if (toLt == 120)
                            pathTo = AppDomain.CurrentDomain.BaseDirectory + "Beijing54_120_40.prj";
                        break;
                }
            }
            ISpatialReferenceFactory2 pFact = new SpatialReferenceEnvironmentClass();
            pSpaFrom = pFact.CreateESRISpatialReferenceFromPRJFile(pathFrom);
            pSpaTo = pFact.CreateESRISpatialReferenceFromPRJFile(pathTo);
        }
        #endregion

        #region =======  修改空间参考（地理坐标）  =======
        /// <summary>  
        /// 修改要素集空间参考  
        /// </summary>  
        /// <param name="pFeatureDataset">要素集</param>  
        /// <param name="pSpatialReference">新空间参考</param>  
        public static void AlterSpatialReference(IFeatureDataset pFeatureDataset, ISpatialReference pSpatialReference)
        {
            IGeoDataset pGeoDataset = pFeatureDataset as IGeoDataset;
            IGeoDatasetSchemaEdit pGeoDatasetSchemaEdit = pGeoDataset as IGeoDatasetSchemaEdit;
            if (pGeoDatasetSchemaEdit.CanAlterSpatialReference == true)
                pGeoDatasetSchemaEdit.AlterSpatialReference(pSpatialReference);
        }


        /// <summary>  
        /// 修改要素类空间参考  
        /// </summary>  
        /// <param name="pFeatureClass">要素类</param>  
        /// <param name="pSpatialReference">新空间参考</param>  
        public static void AlterSpatialReference(IFeatureClass pFeatureClass, ISpatialReference pSpatialReference)
        {
            IGeoDataset pGeoDataset = pFeatureClass as IGeoDataset;
            IGeoDatasetSchemaEdit pGeoDatasetSchemaEdit = pGeoDataset as IGeoDatasetSchemaEdit;
            if (pGeoDatasetSchemaEdit.CanAlterSpatialReference == true)
                pGeoDatasetSchemaEdit.AlterSpatialReference(pSpatialReference);
        }


        /// <summary>
        /// 修改数据集和要素类的投影设置
        /// </summary>
        /// <param name="pGeoDataset"></param>
        /// <param name="pEnumType"></param>
        /// <param name="ParaMeters"></param>
        public static void AlterSpatial(ref IGeoDataset pGeoDataset, ETransType pEnumType, double[] ParaMeters)
        {
            string pathTo = "";
            ISpatialReference pSpatialReferenceFrom = null;
            ISpatialReference pSpatialReferenceTo = null;
            IGeoDatasetSchemaEdit pGeoEdit = pGeoDataset as IGeoDatasetSchemaEdit;
            ISpatialReferenceFactory2 pFact = new SpatialReferenceEnvironmentClass();
            if (pEnumType == ETransType.UnKnown)
            {
                pSpatialReferenceTo = new UnknownCoordinateSystemClass();
                pGeoEdit.AlterSpatialReference(pSpatialReferenceTo);
            }
            else if (pEnumType == ETransType.NJ92NJ2008)
            {
                pathTo = AppDomain.CurrentDomain.BaseDirectory + "NJ08_118'50.prj";
                pSpatialReferenceTo = pFact.CreateESRISpatialReferenceFromPRJFile(pathTo);
                pGeoEdit.AlterSpatialReference(pSpatialReferenceTo);
            }
            else if ((pEnumType == ETransType.BJ54NJ92) || (pEnumType == ETransType.NJ2008NJ92) || (pEnumType == ETransType.WGS84NJ92) || (pEnumType == ETransType.XA80NJ92))
            {
                pathTo = AppDomain.CurrentDomain.BaseDirectory + "NanJing92_118'50.prj";
                pSpatialReferenceTo = pFact.CreateESRISpatialReferenceFromPRJFile(pathTo);
                pGeoEdit.AlterSpatialReference(pSpatialReferenceTo);
            }
            else if ((pEnumType == ETransType.WGS84TOMARS) || (pEnumType == ETransType.WGS84TOBAIDU) || (pEnumType == ETransType.BAIDU2WGS84) ||
                (pEnumType == ETransType.BAIDU2MARS) || (pEnumType == ETransType.MARS2BAIDU) || (pEnumType == ETransType.MARS2WGS84))
            {
                pathTo = AppDomain.CurrentDomain.BaseDirectory + "WGS84.prj";
                pSpatialReferenceTo = pFact.CreateESRISpatialReferenceFromPRJFile(pathTo);
                pGeoEdit.AlterSpatialReference(pSpatialReferenceTo);
            }
            else if ((pEnumType == ETransType.NJ92BJ54) || (pEnumType == ETransType.NJ92WGS84) || (pEnumType == ETransType.NJ92XA80))
            {
                GetSourceTargetSpatialRef(pEnumType, ParaMeters[5], ParaMeters[6], ParaMeters[7], ref pSpatialReferenceFrom, ref pSpatialReferenceTo);
                pGeoEdit.AlterSpatialReference(pSpatialReferenceTo);
            }

        }
        #endregion

        #endregion
    }
}

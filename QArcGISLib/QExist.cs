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
    /// 是否存在
    /// </summary>
    public class QExist
    {
        #region =============  判断 数据集、要素类、要素 是否存在  =============

        /// <summary>
        /// 判断对象是否存在于工作空间中
        /// </summary>
        /// <param name="pWorkspace">工作空间</param>
        /// <param name="objectType">对象类型</param>
        /// <param name="pObjectName">对象名称</param>
        /// <returns></returns>
        public static bool IsObjectInWorkspace(ref IWorkspace pWorkspace, esriDatasetType objectType, string pObjectName)
        {
            if (pWorkspace == null) return false;
            IWorkspace2 pWS2 = pWorkspace as IWorkspace2;
            bool bExist = false;    
            switch (objectType)
            {
                case esriDatasetType.esriDTAny:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTAny, pObjectName);
                    break;
                case esriDatasetType.esriDTContainer:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTContainer, pObjectName);
                    break;
                case esriDatasetType.esriDTGeo:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTGeo, pObjectName);
                    break;
                case esriDatasetType.esriDTFeatureDataset:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTFeatureDataset, pObjectName);
                    break;
                case esriDatasetType.esriDTFeatureClass:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTFeatureClass, pObjectName);
                    break;
                case esriDatasetType.esriDTPlanarGraph:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTPlanarGraph, pObjectName);
                    break;
                case esriDatasetType.esriDTGeometricNetwork:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTGeometricNetwork, pObjectName);
                    break;
                case esriDatasetType.esriDTTopology:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTTopology, pObjectName);
                    break;
                case esriDatasetType.esriDTText:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTText, pObjectName);
                    break;
                case esriDatasetType.esriDTTable:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTTable, pObjectName);
                    break;
                case esriDatasetType.esriDTRelationshipClass:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTRelationshipClass, pObjectName);
                    break;
                case esriDatasetType.esriDTRasterDataset:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTRasterDataset, pObjectName);
                    break;
                case esriDatasetType.esriDTRasterBand:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTRasterBand, pObjectName);
                    break;
                case esriDatasetType.esriDTTin:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTTin, pObjectName);
                    break;
                case esriDatasetType.esriDTCadDrawing:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTCadDrawing, pObjectName);
                    break;
                case esriDatasetType.esriDTRasterCatalog:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTRasterCatalog, pObjectName);
                    break;
                case esriDatasetType.esriDTToolbox:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTToolbox, pObjectName);
                    break;
                case esriDatasetType.esriDTTool:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTTool, pObjectName);
                    break;
                case esriDatasetType.esriDTNetworkDataset:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTNetworkDataset, pObjectName);
                    break;
                case esriDatasetType.esriDTTerrain:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTTerrain, pObjectName);
                    break;
                case esriDatasetType.esriDTRepresentationClass:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTRepresentationClass, pObjectName);
                    break;
                case esriDatasetType.esriDTCadastralFabric:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTCadastralFabric, pObjectName);
                    break;
                case esriDatasetType.esriDTSchematicDataset:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTSchematicDataset, pObjectName);
                    break;
                case esriDatasetType.esriDTLocator:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTLocator, pObjectName);
                    break;
                case esriDatasetType.esriDTMap:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTMap, pObjectName);
                    break;
                case esriDatasetType.esriDTLayer:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTLayer, pObjectName);
                    break;
                case esriDatasetType.esriDTStyle:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTStyle, pObjectName);
                    break;
                case esriDatasetType.esriDTMosaicDataset:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTMosaicDataset, pObjectName);
                    break;
                case esriDatasetType.esriDTLasDataset:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTLasDataset, pObjectName);
                    break;
                default:
                    bExist = pWS2.get_NameExists(esriDatasetType.esriDTFeatureClass, pObjectName);
                    break;
            }
            return bExist;
        }


        /// <summary>
        /// 要素数据集是否在工作空间中
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="pDatasetName"></param>
        /// <returns></returns>
        public static bool IsFeatureDatasetInWorkspace(ref IWorkspace pWorkspace, string pDatasetName)
        {
            IFeatureWorkspace pFeaWS = pWorkspace as IFeatureWorkspace;

            IEnumDatasetName enumDatasetName;
            IDatasetName datasetName;
            bool isExist = false;

            enumDatasetName = pWorkspace.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
            datasetName = enumDatasetName.Next();
            while (datasetName != null)
            {
                if (datasetName.Name == pDatasetName)
                {
                    isExist = true;
                    break;
                }
                datasetName = enumDatasetName.Next();
            }
            return isExist;
        }

        /// <summary>
        /// 判断数据集下是否已存在要素类
        /// </summary>
        /// <param name="pWorkspace">工作空间</param>
        /// <param name="pDatasetName">数据集名称</param>
        /// <param name="pFCName">要素类名称</param>
        /// <returns></returns>
        public static bool IsFeatureClassInDataset(ref IWorkspace pWorkspace, string pDatasetName, string pFCName)
        {
            bool isExist = false;
            try
            {
                IDataset pDataset;
                IEnumDataset pEnumDatasets = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                if (pEnumDatasets == null) return false;
                pDataset = pEnumDatasets.Next();

                while (pDataset != null)
                {
                    if (pDataset is IFeatureDataset)
                    {
                        //try to find class in dataset
                        string datasetName = pDataset.Name;
                        if (datasetName == pDatasetName)
                        {
                            IFeatureDataset pFeatureDS = pDataset as IFeatureDataset;
                            //获得数据集下中所有的独立要素类
                            IEnumDataset subEnumDataset = pDataset.Subsets as IEnumDataset;
                            IDataset subDataset = subEnumDataset.Next();
                            string strAliasName = "";
                            while (subDataset != null)
                            {
                                IFeatureClass pFeatureClass = subDataset as IFeatureClass;
                                //获得IFeatureClass的Name名称（IFeatureClass只有AliasName属性，没有Name属性。需要先转到IDataset接口）
                                strAliasName = pFeatureClass.AliasName;
                                if (strAliasName == pFCName)
                                {
                                    isExist = true;
                                    break;
                                }
                                subDataset = subEnumDataset.Next();
                            }
                            if (isExist)
                            {
                                break;
                            }
                        }
                    }
                    pDataset = pEnumDatasets.Next();
                }
                return isExist;
            }
            catch (Exception ex)
            {
                string strErrInfo = "判断要素类是否存在数据集中失败！\n" + ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 判断数据库中是否存在独立要素类
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="pFCName"></param>
        /// <returns></returns>
        public static bool IsFeatureClassInWorkspace(ref IWorkspace pWorkspace, string pFCName)
        {
            bool isExist = false;
            try
            {
                if (pWorkspace == null) return false;

                IEnumDatasetName enumDatasetNm = pWorkspace.get_DatasetNames(esriDatasetType.esriDTFeatureClass);
                if (enumDatasetNm == null) return false;
                enumDatasetNm.Reset();
                IDatasetName datasetName = enumDatasetNm.Next();
                while (datasetName != null)
                {
                    if (datasetName.Name == pFCName)
                    {
                        isExist = true;
                        break;
                    }
                    datasetName = enumDatasetNm.Next();
                }
                /*                ReleaseESRIObj(datasetName, enumDatasetNm);*/
                return isExist;
            }
            catch (Exception ex)
            {
                string strErrInfo = "判断要素类是否存在失败！\n" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 判断要素类是否存在于工作空间中，包括数据集和独立要素类
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="pFCName"></param>
        /// <returns></returns>
        public static bool IsFeatureClassInAllWorkspace(ref IWorkspace pWorkspace, string pFCName)
        {
            bool isExist = false;
            try
            {
                if (pWorkspace == null) return false;

                IEnumDataset pEDataset = pWorkspace.get_Datasets(esriDatasetType.esriDTAny);
                IDataset pDataset = pEDataset.Next();

                while (pDataset != null)
                {
                    if (pDataset.Type == esriDatasetType.esriDTFeatureClass)
                    {
                        if (pDataset.Name == pFCName)
                        {
                            isExist = true;
                            break;
                        }
                    }
                    //如果是数据集
                    else if (pDataset.Type == esriDatasetType.esriDTFeatureDataset)
                    {
                        IEnumDataset pESubDataset = pDataset.Subsets;
                        IDataset pSubDataset = pESubDataset.Next();
                        while (pSubDataset != null)
                        {
                            if (pSubDataset.Name == pFCName)
                            {
                                isExist = true;
                                break;
                            }
                            pSubDataset = pESubDataset.Next();
                        }
                    }
                    pDataset = pEDataset.Next();
                }
                return isExist;
            }
            catch (Exception ex)
            {
                string strErrInfo = "判断要素类是否存在失败！\n" + ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 判断要素是否在要素类中
        /// </summary>
        /// <param name="pFeaCls"></param>
        /// <param name="pFeatureTar"></param>
        /// <param name="pFindFeature"></param>
        /// <param name="ComparedFields">比较的字段</param>
        /// <returns></returns>
        public static bool IsFeatureInFeatureClass(IFeatureClass pFeaCls, IFeature pFeatureTar, out IFeature pFindFeature, params string[] ComparedFields)
        {
            bool isExist = false;
            bool bMatch = true;
            pFindFeature = null;
            if (pFeaCls == null || pFeatureTar == null) return false;
            try
            {
                IFeatureCursor featureCursor = pFeaCls.Search(null, false);  //true 为传址，false 为传值
                IFeature pFeaSrc = featureCursor.NextFeature();
                while (pFeaSrc != null)
                {
                    if (ComparedFields.Length > 0)
                    {
                        bMatch = true;
                        for (int i = 0; i < ComparedFields.Length; i++)
                        {
                            string field = ComparedFields[i];
                            if (pFeaSrc.get_Value(pFeaSrc.Fields.FindField(field)) != pFeatureTar.get_Value(pFeaSrc.Fields.FindField(field)))
                            {
                                bMatch = false;
                            }
                        }
                        if (bMatch && QCompare.CompareGeometry(pFeaSrc.ShapeCopy, pFeatureTar.ShapeCopy, EGeoCompareMethod.Equals))
                        {
                            isExist = true;
                            pFindFeature = pFeaSrc;
                            break;
                        }
                    }
                    pFeaSrc = featureCursor.NextFeature();
                }
                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor) > 0) ;
            }
            catch (Exception)
            {
                isExist = false;
            }
            return isExist;
        }

        /// <summary>
        /// 找出要素集合与要素类中要素相同的要素
        /// </summary>
        /// <param name="pFeaClsSrc">要比较的要素类</param>
        /// <param name="listFeatureTar">待比较的要素集合A</param>
        /// <param name="listFindFeaTar">集合A中存在要素类中的要素集合</param>
        /// <param name="fields">要素比较一样时，需要判断的字段</param>
        /// <returns>找出要素类中存在的要素</returns>
        public static List<IFeature> IsFeatureListInFeatureClass(ref IFeatureClass pFeaClsSrc, ref List<IFeature> listFeatureTar, ref List<IFeature> listFindFeaTar, params string[] fields)
        {
            List<IFeature> listFeaExistInFeaCls = new List<IFeature>();
            List<IFeature> listFeatureBak = listFeatureTar;
            bool isExist = true;
            if (pFeaClsSrc == null || listFeatureTar.Count == 0) return listFeaExistInFeaCls;
            try
            {
                IFeatureCursor featureCursor = pFeaClsSrc.Search(null, false);  //true 为传址，false 为传值
                IFeature pFeaSrc = featureCursor.NextFeature();
                while (pFeaSrc != null)
                {
                    for (int j = 0; j < listFeatureBak.Count; j++)
                    {
                        IFeature pFeaTar = listFeatureBak[j];
                        if (QCompare.IsFeatureEqual(ref pFeaSrc, ref pFeaTar, fields))
                        {
                            listFeaExistInFeaCls.Add(pFeaSrc);
                            listFindFeaTar.Add(pFeaTar);
                            listFeatureBak.Remove(pFeaTar);
                            isExist = true;
                            break;
                        }
                    }

                    pFeaSrc = featureCursor.NextFeature();
                }
                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor) > 0) ;

            }
            catch (Exception)
            {
                isExist = false;
            }
            return listFeaExistInFeaCls;
        }
        #endregion

    }
}

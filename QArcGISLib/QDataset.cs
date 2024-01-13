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
    /// 数据集类
    /// </summary>
    public class QDataset
    {
        #region =========  1.打开数据集  =========
        /// <summary>
        /// 打开数据集
        /// </summary>
        /// <param name="pWorkspace">工作空间</param>
        /// <param name="datasetName">数据集名称</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static IFeatureDataset OpenFeatureDataSet(ref IWorkspace pWorkspace, string datasetName,ref string error)
        {
            try
            {
                if (pWorkspace == null)
                {
                    error = "打开要素工作空间失败：工作空间为空！";
                    return null;
                }
                IFeatureWorkspace pFeaWS = pWorkspace as IFeatureWorkspace;
                IFeatureDataset pFeatureDataset = pFeaWS.OpenFeatureDataset(datasetName);
                return pFeatureDataset;

            }
            catch (Exception ex)
            {
                error = string.Format("打开要素集[{0}]失败:{1}",datasetName, ex.Message);
                return null;
            }
        }
        #endregion

        #region =========  2.获取数据集  =========
        /// <summary>
        /// 获取工作空间下所有要素数据集
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="listDataset"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static bool GetDataset(ref IWorkspace pWorkspace, ref List<IFeatureDataset> listDataset,
                    ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if (pWorkspace == null)
                {
                    strErrInfo = "工作空间为空！";
                    return false;
                }

                // 遍历要素集Dataset下的FeatrueClass
                IEnumDataset pEnumDatasets = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                pEnumDatasets.Reset();
                IDataset pDataset = pEnumDatasets.Next();
                while (pDataset != null)
                {
                    if (pDataset is IFeatureDataset)
                    {
                        string pDatasetName = pDataset.Name;
                        IFeatureDataset pFeatureDS = pDataset as IFeatureDataset;
                        if (pFeatureDS != null)
                        {
                            listDataset.Add(pFeatureDS);
                        }
                    }
                    pDataset = pEnumDatasets.Next();
                }
                return true;
            }
            catch (Exception ex)
            {
                strErrInfo = "读取空间要素集失败" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取数据集列表
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="listDataset"></param>
        /// <param name="strErrInfo"></param>
        /// <param name="arrDataset">指定数据集名称</param>
        /// <returns></returns>
        public static bool GetDataset(ref IWorkspace pWorkspace, ref List<IDataset> listDataset,
                    ref string strErrInfo, params string[] arrDataset)
        {
            strErrInfo = "";
            try
            {
                if (pWorkspace == null)
                {
                    strErrInfo = "工作空间为空！";
                    return false;
                }

                // 遍历要素集Dataset下的FeatrueClass
                IEnumDataset pEnumDatasets = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                pEnumDatasets.Reset();
                IDataset pDataset = pEnumDatasets.Next();
                while (pDataset != null)
                {
                    if (arrDataset.Length > 0 && !arrDataset.Contains(pDataset.Name))
                    {
                        continue;
                    }
                    listDataset.Add(pDataset);
                    pDataset = pEnumDatasets.Next();
                }
                return true;
            }
            catch (Exception ex)
            {
                strErrInfo = "读取要素集失败" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取工作空间里所有数据集名称
        /// </summary>
        /// <param name="pWorkSpace">工作空间</param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static List<string> GetDataSetName(ref IWorkspace pWorkSpace, ref string strErrInfo)
        {
            try
            {
                strErrInfo = "";
                List<string> strListDtSet = new List<string>();
                if (pWorkSpace == null)
                {
                    strErrInfo = "工作空间为空！";
                    return null;
                }

                //IFeatureWorkspace pFws = pWorkSpace as IFeatureWorkspace;
                IEnumDatasetName FeatureEnumDatasetName = pWorkSpace.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
                if (FeatureEnumDatasetName == null) return null;
                FeatureEnumDatasetName.Reset();
                IDatasetName pDatasetName = FeatureEnumDatasetName.Next();
                while (pDatasetName != null)
                {
                    strListDtSet.Add(pDatasetName.Name);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pDatasetName);
                    pDatasetName = FeatureEnumDatasetName.Next();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(FeatureEnumDatasetName);
                return strListDtSet;
            }
            catch (Exception ex)
            {
                strErrInfo = "获取GDB内数据集失败！" + ex.Message;
                return null;
            }
        }
        #endregion

        #region =========  3.创建数据集  =========
        /// <summary>
        /// 创建Feature数据集
        /// </summary>
        /// <param name="pWorkspace">目标工作空间</param>
        /// <param name="pDatasetName">数据集名称</param>
        /// <param name="pSpatialReference">投影</param>
        /// <param name="error"></param>
        /// <param name="bOverWrite">是否覆盖</param>
        /// <returns></returns>
        public static IFeatureDataset CreateFeatureDataset(ref IWorkspace pWorkspace,
            string pDatasetName, ISpatialReference pSpatialReference,
            ref string error, bool bOverWrite = false)
        {
            IFeatureDataset pTFeDataset = null;

            if (pWorkspace == null)
            {
                error = "工作空间为空！";
                return pTFeDataset;
            }

            IFeatureWorkspace pFeatureWS = pWorkspace as IFeatureWorkspace;

            if (string.IsNullOrEmpty(pDatasetName))
            {
                error = "数据集名称为空！";
                return pTFeDataset;
            }

            try
            {
                IWorkspace2 pWS2 = pFeatureWS as IWorkspace2;

                //先判断数据集是否存在，不存在则创建
                bool isExist = pWS2.get_NameExists(esriDatasetType.esriDTFeatureDataset, pDatasetName);

                if (isExist == true)  //删除已有数据集
                {
                    if (bOverWrite)
                    {
                        IFeatureDataset pFDS = OpenFeatureDataSet(ref pWorkspace, pDatasetName,ref error);
                        if (pFDS != null)
                        {
                            if (pFDS.CanDelete())
                            {
                                pFDS.Delete();
                                pTFeDataset = pFeatureWS.CreateFeatureDataset(pDatasetName, pSpatialReference);
                            }
                            else
                            {
                                error = string.Format("已存在数据集[{0}]不可删除", pDatasetName);
                                return null;
                            }
                        }
                    }
                    else pTFeDataset = OpenFeatureDataSet(ref pWorkspace, pDatasetName, ref error);
                }
                else pTFeDataset = pFeatureWS.CreateFeatureDataset(pDatasetName, pSpatialReference);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return pTFeDataset;
        }

        /// <summary>
        /// 创建数据集
        /// </summary>
        /// <param name="pWorkspace">目标工作空间</param>
        /// <param name="datasetSrc">参考数据集</param>
        /// <param name="suffix">后缀，即改名</param>
        /// <param name="error">错误</param>
        /// <param name="bOverWrite">是否重写</param>
        /// <returns></returns>
        public static IFeatureDataset CreateFeatureDataset(ref IWorkspace pWorkspace, ref IFeatureDataset datasetSrc, ref string error, string suffix = "", bool bOverWrite = false)
        {
            IFeatureDataset pTFeDataset = null;
            if (pWorkspace == null)
            {
                error = "工作空间为空！";
                return pTFeDataset;
            }

            IFeatureWorkspace pFeatureWS = pWorkspace as IFeatureWorkspace;

            if (datasetSrc == null)
            {
                error = "源数据集为空！";
                return pTFeDataset;
            }

            //要素集名称
            string pDSName = datasetSrc.Name;
            if (suffix != "") pDSName += suffix;

            //要素集空间参考
            ISpatialReference pSR = QSpatialReference.GetSpatialReference(datasetSrc);
            pSR.SetDomain(-9999999, 9999999.0, -9999999.0, 9999999.0);
            try
            {
                //先判断数据集是否存在，不存在则创建
                IWorkspace2 pWS2 = pFeatureWS as IWorkspace2;
                bool isExist = pWS2.get_NameExists(esriDatasetType.esriDTFeatureDataset, pDSName);

                if (isExist)
                {
                    if (bOverWrite)
                    {
                        IFeatureDataset pFDS = OpenFeatureDataSet(ref pWorkspace, pDSName,ref error);
                        if (pFDS != null)
                        {
                            if (pFDS.CanDelete())
                            {
                                pFDS.Delete();
                                pTFeDataset = pFeatureWS.CreateFeatureDataset(pDSName, pSR);
                            }
                            else
                            {
                                error = string.Format("已存在数据集[{0}]不可删除", pDSName);
                                return null;
                            }
                        }
                    }
                    else pTFeDataset = OpenFeatureDataSet(ref pWorkspace, pDSName, ref error);
                }
                else pTFeDataset = pFeatureWS.CreateFeatureDataset(pDSName, pSR);
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return pTFeDataset;
        }

        /// <summary>
        /// 创建数据集
        /// </summary>
        /// <param name="pWsSrc"></param>
        /// <param name="pWsTar"></param>
        /// <param name="error"></param>
        /// <param name="bContainFeaCls">是否包含数据集下要素类</param>
        /// <param name="suffix">名称后缀，即统一改名</param>
        /// <param name="datasetNames">指定数据集创建</param>
        /// <returns></returns>
        public static bool CreateFeatureDataset(ref IWorkspace pWsSrc, ref IWorkspace pWsTar, ref string error,
            bool bContainFeaCls = false, string suffix = "", params string[] datasetNames)
        {
            try
            {
                if (pWsSrc == null || pWsTar == null)
                {
                    error = "要素工作空间为空，请检查！";
                    return false;
                }

                IWorkspace2 pWs2Tar = pWsTar as IWorkspace2;
                IFeatureWorkspace pFeaWsTar = pWsTar as IFeatureWorkspace;
                List<IFeatureDataset> listDataset = new List<IFeatureDataset>();
                IFeatureDataset pFeaDsTar = null;
                //获取源空间要素集
                if (!GetDataset(ref pWsSrc, ref listDataset, ref error))
                {
                    return false;
                }
                if (listDataset.Count == 0) return true;

                for (int i = 0; i < listDataset.Count; i++)
                {
                    IFeatureDataset pFeaDsSrc = listDataset[i];
                    string dsName = pFeaDsSrc.Name + suffix;
                    if (datasetNames.Length > 0 && !datasetNames.Contains(dsName))
                    {
                        continue;
                    }
                    bool bExist = pWs2Tar.get_NameExists(esriDatasetType.esriDTFeatureDataset, dsName);
                    if (!bExist)
                    {
                        pFeaDsTar = CreateFeatureDataset(ref pWsTar, ref pFeaDsSrc, ref error, suffix);
                        if (!string.IsNullOrEmpty(error))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        pFeaDsTar = OpenFeatureDataSet(ref pWsTar, dsName,ref error);
                    }
                    if (bContainFeaCls)
                    {
                        if (!QFeatureClass.CreateFeatureClassFromDS2DS(ref pFeaDsSrc, ref pWsTar, ref pFeaDsTar, ref error, suffix))
                        {
                            return false;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion

        #region =========  4.删除数据集  =========
        /// <summary>
        /// 删除所有数据集
        /// </summary>
        /// <param name="pWorkspace">工作空间</param>
        /// <param name="pDataSetName">数据集名称，为空表示全部删除</param>
        /// <returns></returns>
        public static bool DeleteDataset(ref IWorkspace pWorkspace, string pDataSetName = "")
        {
            bool hasDelete = false;
            bool hasError = false;
            bool bDelAll = false;
            if (pDataSetName == "") //删除所有数据集
            {
                bDelAll = true;
            }
            if (pWorkspace == null) return false;
            try
            {
                IDataset pDataset;
                IEnumDataset pEnumDatasets = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                pDataset = pEnumDatasets.Next();

                while (pDataset != null)
                {
                    //try to find class in dataset
                    string datasetName = pDataset.Name;
                    if (bDelAll)
                    {
                        if (pDataset.CanDelete())
                        {
                            pDataset.Delete();
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                    else if (datasetName == pDataSetName)
                    {
                        if (pDataset.CanDelete())
                        {
                            pDataset.Delete();
                            hasDelete = true;
                        }
                        else
                        {
                            hasError = true;
                        }
                        break;
                    }

                    pDataset = pEnumDatasets.Next();
                }
                QRelease.ReleaseESRIObject(pDataset);
                if (hasError == true)
                {
                    hasDelete = false;
                }
                else if (bDelAll && hasDelete == false)
                {
                    hasDelete = true;
                }
                return hasDelete;
            }
            catch (Exception ex)
            {
                string strErrInfo = "判断要素类是否存在数据集中失败！\n" + ex.Message;
                return false;
            }
        }
        #endregion

        #region =========  5.判断数据集  =========
        /// <summary>
        /// 数据集是否存在于工作空间
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="pFeatureDatasetName"></param>
        /// <returns></returns>
        public static bool IsFeatureDatasetInWorkspace(ref IWorkspace pWorkspace, string pFeatureDatasetName)
        {
            if (pWorkspace == null) return false;
            IWorkspace2 pWS2 = pWorkspace as IWorkspace2;
            bool bExist = pWS2.get_NameExists(esriDatasetType.esriDTFeatureDataset, pFeatureDatasetName);
            return bExist;
        }
        #endregion


    }
}

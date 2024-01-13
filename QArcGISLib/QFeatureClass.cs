/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib
{
    /// <summary>
    /// 要素类
    /// </summary>
    public class QFeatureClass
    {
        #region =========  1.打开要素类  =========
        /// <summary>
        /// 打开要素类
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="featureClassName"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClass(ref IWorkspace pWorkspace, string featureClassName, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if (pWorkspace == null) return null;
                IFeatureWorkspace pFeatWorkSpace = pWorkspace as IFeatureWorkspace;
                IFeatureClass pFeatureClass = pFeatWorkSpace.OpenFeatureClass(featureClassName);
                return pFeatureClass;
            }
            catch (Exception ex)
            {
                strErrInfo = "打开要素类失败" + ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 打开要素集下的要素类
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="featureClassName"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClass(ref IFeatureDataset pFeatureDataset, string featureClassName, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                //打开数据集
                if (pFeatureDataset == null) return null;
                IDataset pDataset = pFeatureDataset as IDataset;
                //获得数据集下中所有的独立要素类
                IEnumDataset enumDataset = pDataset.Subsets as IEnumDataset;
                IDataset dataset = enumDataset.Next();
                string strName = "";
                string strAliasName = "";
                while (dataset != null)
                {
                    IFeatureClass pFeatureClass = dataset as IFeatureClass;
                    strName = dataset.Name;
                    strAliasName = pFeatureClass.AliasName;
                    if (featureClassName == strName)
                    {
                        System.Runtime.InteropServices.Marshal.ReleaseComObject(enumDataset);
                        return pFeatureClass;
                    }
                    dataset = enumDataset.Next();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(dataset);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(enumDataset);
                return null;
            }
            catch (Exception ex)
            {
                strErrInfo = "打开数据集内要素类失败！" + ex.Message.ToString();
                return null;
            }
        }

        /// <summary>
        /// 打开工作空间下要素集下指定要素类
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="datasetName"></param>
        /// <param name="featureClassName"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static IFeatureClass OpenFeatureClass(ref IWorkspace pWorkspace,string datasetName, string featureClassName, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if (pWorkspace == null)
                {
                    strErrInfo = "工作空间为空！";
                    return null;
                }

                if (string.IsNullOrEmpty(datasetName) || string.IsNullOrEmpty(featureClassName))
                {
                    strErrInfo = "数据集名称或者要素类名称为空！";
                    return null;
                }

                IFeatureWorkspace pFeatWorkSpace = pWorkspace as IFeatureWorkspace;
                IFeatureDataset pFeatureDataset = pFeatWorkSpace.OpenFeatureDataset(datasetName);
                if (pFeatureDataset == null)
                {
                    strErrInfo = "打开数据集失败！";
                    return null;
                }

                IFeatureClass pFeatureClass = OpenFeatureClass(ref pFeatureDataset,featureClassName,ref strErrInfo);
                return pFeatureClass;
            }
            catch (Exception ex)
            {
                strErrInfo = "打开要素类失败" + ex.Message;
                return null;
            }
        }
        #endregion

        #region =========  2.创建要素类  =========
        //新建要素类
        /// <summary>
        /// 根据已有FeatureClass的字段，在在Workspace内创建同字段的FeatureClass
        /// </summary>
        /// <param name="pWorkspace">Feature工作空间</param>
        /// <param name="pFeatCls">已有featureClass</param>
        /// <param name="pSpatialReference">空间参考</param>
        /// <param name="newFeaClsName">重命名，为空保持不变</param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClass(ref IWorkspace pWorkspace, ref IFeatureClass pFeatCls, ISpatialReference pSpatialReference, string newFeaClsName = "")
        {
            if(pWorkspace == null)
            {
                return null;
            }
            IField pField = null;
            IField pGeoField = null;
            IFeatureClass pNewFeatureClass = null;
            IFeatureWorkspace pFeaWorkspace = pWorkspace as IFeatureWorkspace;

            try
            {
                #region 获取属性字段 Method 1
                IGeometryDefEdit geomDefEdit = null;
                IFields pFields = pFeatCls.Fields;
                IFields pNewFields = new FieldsClass();
                IFieldsEdit pNewFieldsEdit = pNewFields as IFieldsEdit;
                IDataset pDS = pFeatCls as IDataset;
                if (string.IsNullOrEmpty(newFeaClsName)) newFeaClsName = pDS.Name;

                //保证源要素类与新要素类的字段结构一致，但空间范围不一样，即自己手动设计Geometry字段
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    pField = pFields.get_Field(i);
                    if (pField.Type != esriFieldType.esriFieldTypeGeometry)
                    {
                        pNewFieldsEdit.AddField(pField);
                    }
                    else
                    {
                        pGeoField = pField;
                        geomDefEdit = pGeoField.GeometryDef as IGeometryDefEdit;
                        geomDefEdit.SpatialReference_2 = pSpatialReference;
                        pNewFieldsEdit.AddField(pGeoField);
                    }
                }
                #endregion

                #region 获取属性字段 Method 2
                //属性信息
                // 创建字段检查对象
                //IWorkspace pTWorkspace = pWorkspace as IWorkspace;
                //IFieldChecker pFieldChecker = new FieldCheckerClass();
                //IEnumFieldError pEnumFieldError = null;
                //IFields fieldsTar;
                //pFieldChecker.ValidateWorkspace = pTWorkspace;
                //pFieldChecker.Validate(pFeatCls.Fields, out pEnumFieldError, out fieldsTar);
                #endregion

                if (pFeatCls.FeatureType == esriFeatureType.esriFTAnnotation)
                {//Annotation要素类
                    IFeatureWorkspaceAnno pWorkspaceAnno = pFeaWorkspace as IFeatureWorkspaceAnno;
                    IAnnoClass pAnnoClass = pFeatCls.Extension as IAnnoClass;
                    IGraphicsLayerScale pGraScale = new GraphicsLayerScaleClass();
                    pGraScale.ReferenceScale = pAnnoClass.ReferenceScale;
                    pGraScale.Units = pAnnoClass.ReferenceScaleUnits;
                    pNewFeatureClass = pWorkspaceAnno.CreateAnnotationClass(newFeaClsName, pNewFields, pFeatCls.CLSID, pFeatCls.EXTCLSID, "Shape", "", null, null, pAnnoClass.AnnoProperties, pGraScale, pAnnoClass.SymbolCollection, true);
                }
                else
                {//普通要素类
                    pNewFeatureClass = pFeaWorkspace.CreateFeatureClass(newFeaClsName, pNewFields, pFeatCls.CLSID, pFeatCls.EXTCLSID, pFeatCls.FeatureType, pFeatCls.ShapeFieldName, "");
                }
            }
            catch (Exception)
            {
                
            }
            return pNewFeatureClass;
        }


        //新建要素类
        /// <summary>
        /// 根据已有FeatureClass的字段，在数据集内创建同字段的FeatureClass
        /// </summary>
        /// <param name="pFeaDataset">目标数据集</param>
        /// <param name="pFeatCls">已有FeatureClass</param>
        /// <param name="error">错误信息</param>
        /// <param name="modifyFeaClassName">修改后的要素类名称</param>
        /// <returns></returns>
        public static IFeatureClass CreateFeatureClass(ref IFeatureDataset pFeaDataset, ref IFeatureClass pFeatCls, ref string error,
                string modifyFeaClassName = "")
        {
            IField pField = null;
            IField pGeoField = null;
            IFeatureClass pNewFeatureClass = null;
            IGeometryDefEdit geomDefEdit = null;
            try
            {
                IFields pFields = pFeatCls.Fields;
                IFields pNewFields = new FieldsClass();
                IFieldsEdit pNewFieldsEdit = pNewFields as IFieldsEdit;
                IDataset pDs = pFeatCls as IDataset;
                if (string.IsNullOrEmpty(modifyFeaClassName)) modifyFeaClassName = pDs.Name;

                //空间参考与Dataset保持一致，不用修改
                ISpatialReference pSpatialReference = QSpatialReference.GetSpatialReference(pFeaDataset);

                //保证源要素类与新要素类的字段结构一致，但空间范围不一样，即自己手动设计Geometry字段
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    pField = pFields.get_Field(i);
                    if (pField.Type != esriFieldType.esriFieldTypeGeometry)
                    {
                        pNewFieldsEdit.AddField(pField);
                    }
                    else
                    {
                        pGeoField = pField;
                        geomDefEdit = pGeoField.GeometryDef as IGeometryDefEdit;
                        geomDefEdit.SpatialReference_2 = pSpatialReference;
                        pNewFieldsEdit.AddField(pGeoField);
                    }
                }

                if (pFeatCls.FeatureType == esriFeatureType.esriFTAnnotation)
                {//Annotation要素类
                    IFeatureWorkspaceAnno pWorkspaceAnno = pFeaDataset.Workspace as IFeatureWorkspaceAnno;
                    IAnnoClass pAnnoClass = pFeatCls.Extension as IAnnoClass;
                    IGraphicsLayerScale pGraScale = new GraphicsLayerScaleClass();
                    pGraScale.ReferenceScale = pAnnoClass.ReferenceScale;
                    pGraScale.Units = pAnnoClass.ReferenceScaleUnits;

                    pNewFeatureClass = pWorkspaceAnno.CreateAnnotationClass(modifyFeaClassName, pNewFields, pFeatCls.CLSID, pFeatCls.EXTCLSID, "Shape", "", pFeaDataset, null, pAnnoClass.AnnoProperties, pGraScale, pAnnoClass.SymbolCollection, true);
                }
                else
                {//普通要素类
                    pNewFeatureClass = pFeaDataset.CreateFeatureClass(modifyFeaClassName, pNewFields, pFeatCls.CLSID, pFeatCls.EXTCLSID, pFeatCls.FeatureType, "Shape", "");
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }

            return pNewFeatureClass;
        }

        /// <summary>
        /// 将源数据集中要素类复制到目标工作空间指定数据集下
        /// suffix可给要素类名称添加后缀
        /// </summary>
        /// <param name="pFeaDsSrc">源数据集</param>
        /// <param name="pWsTar">目标工作空间</param>
        /// <param name="pFeaDsTar">指定数据集</param>
        /// <param name="error"></param>
        /// <param name="suffix">要素类名称添加后缀，为空保持不变</param>
        /// <returns></returns>
        public static bool CreateFeatureClassFromDS2DS(ref IFeatureDataset pFeaDsSrc,
            ref IWorkspace pWsTar, ref IFeatureDataset pFeaDsTar, ref string error, string suffix = "")
        {
            if (pFeaDsSrc == null)
            {
                error = "要素集为空,无法创建目标要素集";
                return false;
            }
            try
            {
                List<IFeatureClass> listFeaCls = new List<IFeatureClass>();
                if (!GetFeatureClassInDataset(ref pFeaDsSrc, ref listFeaCls, ref error))
                {
                    return false;
                }

                for (int i = 0; i < listFeaCls.Count; i++)
                {
                    IFeatureClass pFeatCls = listFeaCls[i];
                    IDataset pDS = pFeatCls as IDataset;
                    string feaClaName = pDS.Name + suffix;
                    IWorkspace2 pWS2Tar = pWsTar as IWorkspace2;
                    bool isExist = pWS2Tar.get_NameExists(esriDatasetType.esriDTFeatureClass, feaClaName);
                    if (!isExist)
                    {
                        IFeatureClass pNewFeaCls = CreateFeatureClass(ref pFeaDsTar, ref pFeatCls, ref error, feaClaName);
                        if (pNewFeaCls == null) return false;
                    }
                    else continue;
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 目标工作空间之间复制独立要素类
        /// </summary>
        /// <param name="pWsSrc">源工作空间</param>
        /// <param name="pWsTar">目标工作空间</param>
        /// <param name="error">错误</param>
        /// <param name="suffix">要素类添加统一后缀</param>
        /// <param name="feaClsNames">指定复制的要素类名称，为空表示所有</param>
        /// <returns></returns>
        public static bool CreateFeatureClass(ref IWorkspace pWsSrc, ref IWorkspace pWsTar,
            ref string error, string suffix = "", params string[] feaClsNames)
        {
            string fcName = string.Empty;
            try
            {
                if (pWsSrc == null || pWsTar == null)
                {
                    error = "要素工作空间为空，请检查！";
                    return false;
                }

                IWorkspace2 pWs2Tar = pWsTar as IWorkspace2;
                IFeatureWorkspace pFeaWsTar = pWsTar as IFeatureWorkspace;
                List<IFeatureClass> listFeaCls = new List<IFeatureClass>();
                IFeatureClass pFeaClsTar = null;
                IFeatureClass pFeaClsSrc = null;
                ISpatialReference sr = null;
                //获取源空间要素集
                if (!GetFeatureClass(ref pWsSrc, ref listFeaCls, ref error))
                {
                    return false;
                }
                if (listFeaCls.Count == 0) return true;

                for (int i = 0; i < listFeaCls.Count; i++)
                {
                    pFeaClsSrc = listFeaCls[i];
                    IDataset pDS = pFeaClsSrc as IDataset;
                    fcName = pDS.Name + suffix;
                    if (feaClsNames.Length > 0 && !feaClsNames.Contains(fcName))
                    {
                        continue;
                    }
                    bool bExist = pWs2Tar.get_NameExists(esriDatasetType.esriDTFeatureClass, fcName);
                    if (!bExist)
                    {
                        sr = QSpatialReference.GetSpatialReference(pFeaClsSrc);
                        pFeaClsTar = CreateFeatureClass(ref pWsTar, ref pFeaClsSrc, sr, fcName);
                    }
                    else
                    {
                        pFeaClsTar = OpenFeatureClass(ref pWsTar, fcName, ref error);
                    }
                    if (pFeaClsTar == null)
                    {
                        error = string.Format("创建要素类[{0}]失败！", fcName);
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                error = string.Format("目标工作空间内创建要素类失败:{0}", ex.Message);
                return false;
            }
        }

        #endregion

        #region =========  3.获取要素类  =========
        /// <summary>
        /// 获取要素类列表
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="listFeatureCls"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static bool GetFeatureClass(ref IWorkspace pWorkspace,
                    ref List<IFeatureClass> listFeatureCls, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if (pWorkspace == null) return false;

                //遍历工作空间下的featureclass
                IEnumDataset enumDatasets = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass) as IEnumDataset;
                enumDatasets.Reset();
                IDataset esriDataset = enumDatasets.Next() as IDataset;
                while (esriDataset is IFeatureClass)
                {
                    IFeatureClass featureClass = esriDataset as IFeatureClass;
                    string name = esriDataset.Name;
                    listFeatureCls.Add(featureClass);
                    esriDataset = enumDatasets.Next() as IDataset;
                }
                return true;
            }
            catch (Exception ex)
            {
                strErrInfo = "读取要素类列表失败" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取工作空间独立要素类名称集合
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="listFeatureClsNames"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static bool GetFeatureClassNames(ref IWorkspace pWorkspace,
            ref List<string> listFeatureClsNames, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if (pWorkspace == null)
                {
                    strErrInfo = "工作空间为空";
                    return false;
                }

                //遍历工作空间下的featureclass
                IEnumDatasetName enumDatasetName = pWorkspace.DatasetNames[esriDatasetType.esriDTFeatureClass];
                enumDatasetName.Reset();
                IDatasetName pDatasetName = enumDatasetName.Next();
                while (pDatasetName != null)
                {
                    listFeatureClsNames.Add(pDatasetName.Name);
                    pDatasetName = enumDatasetName.Next();
                }
                return true;
            }
            catch (Exception ex)
            {
                strErrInfo = "读取要素类名称列表失败@" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取 工作空间下所有数据集及独立要素类
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="dicDataSet">要素类字典</param>
        /// <param name="listFeatureCls">独立要素类集合</param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static bool GetFeatureClass(ref IWorkspace pWorkspace,
            ref Dictionary<IFeatureDataset, List<IFeatureClass>> dicDataSet,
            ref List<IFeatureClass> listFeatureCls,
            ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if (pWorkspace == null) return false;

                // 遍历要素集Dataset下的FeatrueClass
                IEnumDataset pEnumDatasets = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureDataset);
                pEnumDatasets.Reset();
                IDataset pDataset = pEnumDatasets.Next();
                while (pDataset != null)
                {
                    if (pDataset is IFeatureDataset)
                    {
                        //try to find class in dataset
                        string pDatasetName = pDataset.Name;
                        IFeatureDataset pFeatureDS = pDataset as IFeatureDataset;
                        //获得数据集下中所有的独立要素类
                        IEnumDataset subEnumDataset = pDataset.Subsets as IEnumDataset;
                        IDataset subDataset = subEnumDataset.Next();
                        string strName = "";
                        string strAliasName = "";
                        while (subDataset != null)
                        {
                            IFeatureClass pFeatureClass = subDataset as IFeatureClass;
                            //获得IFeatureClass的Name名称（IFeatureClass只有AliasName属性，没有Name属性。需要先转到IDataset接口）
                            strName = subDataset.Name;
                            strAliasName = pFeatureClass.AliasName;
                            if (!dicDataSet.ContainsKey(pFeatureDS))
                            {
                                dicDataSet[pFeatureDS] = new List<IFeatureClass>();
                            }
                            dicDataSet[pFeatureDS].Add(pFeatureClass);
                            subDataset = subEnumDataset.Next();
                        }
                    }
                    pDataset = pEnumDatasets.Next();
                }

                //遍历工作空间下的featureclass
                IEnumDataset enumDatasets = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass) as IEnumDataset;
                enumDatasets.Reset();
                IDataset esriDataset = enumDatasets.Next() as IDataset;
                while (esriDataset is IFeatureClass)
                {
                    IFeatureClass featureClass = esriDataset as IFeatureClass;
                    string name = esriDataset.Name;
                    listFeatureCls.Add(featureClass);
                    esriDataset = enumDatasets.Next() as IDataset;
                }
                return true;
            }
            catch (Exception ex)
            {
                strErrInfo = "读取要素类失败" + ex.Message;
                return false;
            }
        }




        /// <summary>
        /// 获取工作空间下所有数据集下要素类集合
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="listFeatureCls"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static bool GetFeatureClassInDataset(ref IWorkspace pWorkspace,
            ref List<IFeatureClass> listFeatureCls, ref string strErrInfo)
        {
            strErrInfo = "";
            listFeatureCls = new List<IFeatureClass>();
            try
            {
                if (pWorkspace == null)
                {
                    strErrInfo = "读取数据集下要素类：工作空间为空！";
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
                        List<IFeatureClass> listFeaCls = new List<IFeatureClass>();
                        //try to find class in dataset
                        string pDatasetName = pDataset.Name;
                        IFeatureDataset pFeatureDS = pDataset as IFeatureDataset;
                        //获得数据集下中所有的独立要素类
                        if (!GetFeatureClassInDataset(ref pFeatureDS, ref listFeaCls, ref strErrInfo))
                        {
                            return false;
                        }
                        listFeatureCls.AddRange(listFeaCls);
                    }
                    pDataset = pEnumDatasets.Next();
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrInfo = "读取要素集下要素类失败" + ex.Message;
                return false;
            }
        }


        /// <summary>
        /// 读取要素集下所有要素类
        /// </summary>
        /// <param name="pFeatureDS"></param>
        /// <param name="listFeatureCls"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static bool GetFeatureClassInDataset(ref IFeatureDataset pFeatureDS,
                ref List<IFeatureClass> listFeatureCls, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if (pFeatureDS == null)
                {
                    strErrInfo = "读取要素集下要素类:要素集为空！";
                    return false;
                }

                //try to find class in dataset
                string pDatasetName = pFeatureDS.Name;
                //获得数据集下中所有的独立要素类
                IEnumDataset subEnumDataset = pFeatureDS.Subsets;
                IDataset subDataset = subEnumDataset.Next();
                while (subDataset != null)
                {
                    IFeatureClass pFeatureClass = subDataset as IFeatureClass;
                    if (pFeatureClass != null)
                    {
                        listFeatureCls.Add(pFeatureClass);
                    }
                    subDataset = subEnumDataset.Next();
                }
                return true;
            }
            catch (Exception ex)
            {
                strErrInfo = "读取要素集下要素类失败" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 获取非空要素类
        /// </summary>
        /// <param name="listFeature"></param>
        public static void GetNotNullFeatureClass(ref List<IFeatureClass> listFeature)
        {
            if (listFeature.Count == 0)
            {
                return;
            }
            for (int i = listFeature.Count - 1; i >= 0; --i)
            {
                IFeatureClass pFeaCls = listFeature[i];
                //方法1 遍历Feature
                //                 if (IsFeatrueClassNull(ref pFeaCls) == 0)
                //                 {
                //                     listFeature.Remove(pFeaCls);
                //                 }
                //方法2 直接判断
                if (pFeaCls.FeatureCount(null) == 0)
                {
                    listFeature.Remove(pFeaCls);
                }
            }
        }

        /// <summary>
        /// 根据数据集名称获取该数据集下的要素类名称列表
        /// </summary>
        /// <param name="pWorkspace">工作空间</param>
        /// <param name="strDatasetName"></param>
        /// <returns></returns>
        public static Hashtable GetFeatureClassNamesInDataset(ref IWorkspace pWorkspace, string strDatasetName)
        {
            string strErrInfo = "";
            try
            {
                if (pWorkspace == null)
                {
                    //strErrInfo = "根据数据集名称获取该数据集下的要素类名称列表：工作空间为空！";
                    return null;
                }
                Hashtable LayerListFromDataset = new Hashtable();
                //打开数据集
                IFeatureDataset pFeatureDataset =QDataset.OpenFeatureDataSet(ref pWorkspace, strDatasetName, ref strErrInfo);
                if (pFeatureDataset == null) return null;

                IDataset pDataset = pFeatureDataset as IDataset;
                //获得数据集下中所有的独立要素类
                IEnumDataset enumDataset = pDataset.Subsets as IEnumDataset;
                IDataset dataset = enumDataset.Next();
                string strName = "";
                string strAliasName = "";
                while (dataset != null)
                {
                    IFeatureClass pFeatureClass = dataset as IFeatureClass;
                    strName = dataset.Name;
                    strAliasName = pFeatureClass.AliasName;
                    LayerListFromDataset.Add(strName, strAliasName);
                    dataset = enumDataset.Next();
                }
                //释放com对象
                System.Runtime.InteropServices.Marshal.ReleaseComObject(enumDataset);
                return LayerListFromDataset;
            }
            catch (Exception ex)
            {
                strErrInfo = "获取数据集内要素类失败：" + ex.Message.ToString();
                return null;
            }
        }

        /// <summary>
        /// 获取Dataset下所有Feature的名称列表
        /// </summary>
        /// <param name="pWorkspace">工作空间</param>
        /// <param name="strDatasetName"></param>
        /// <param name="strErrInfo"></param>
        /// <param name="bGetAliasName">是否获取别名</param>
        /// <returns></returns>
        public static List<string> GetFeatureClassNamesInDataset(ref IWorkspace pWorkspace, string strDatasetName, ref string strErrInfo, bool bGetAliasName = false)
        {
            strErrInfo = "";
            List<string> listLyrInDS = new List<string>();
            try
            {
                if (pWorkspace == null)
                {
                    strErrInfo = "根据数据集名称获取该数据集下的要素类名称列表：工作空间为空！";
                    return null;
                }
                //打开数据集
                IFeatureDataset pFeatureDataset = QDataset.OpenFeatureDataSet(ref pWorkspace, strDatasetName, ref strErrInfo);
                IDataset pDataset = pFeatureDataset as IDataset;
                //获得数据集下中所有的独立要素类
                IEnumDataset enumDataset = pDataset.Subsets as IEnumDataset;
                IDataset dataset = enumDataset.Next();
                string strName = "";
                string strAliasName = "";
                while (dataset != null)
                {
                    if (bGetAliasName)
                    {
                        IFeatureClass pFeatureClass = dataset as IFeatureClass;
                        strName = dataset.Name;
                        strAliasName = pFeatureClass.AliasName;
                        listLyrInDS.Add(strAliasName);
                    }
                    else
                    {
                        listLyrInDS.Add(dataset.Name);
                    }
                    dataset = enumDataset.Next();
                }
                //释放com对象
                System.Runtime.InteropServices.Marshal.ReleaseComObject(enumDataset);
                return listLyrInDS;
            }
            catch (Exception ex)
            {
                strErrInfo = "获取数据集内要素类失败！" + ex.Message.ToString();
                return null;
            }
        }

        /// <summary>
        /// 获取数据集内要素类名称
        /// </summary>
        /// <param name="pFeatureDataset"></param>
        /// <param name="strErrInfo"></param>
        /// <param name="bGetAliasName">是否获取别名</param>
        /// <returns></returns>
        public static List<string> GetFeatureClassNamesInDataset(ref IFeatureDataset pFeatureDataset, ref string strErrInfo, bool bGetAliasName = false)
        {
            strErrInfo = "";
            List<string> listLyrInDS = new List<string>();
            try
            {
                if (pFeatureDataset == null)
                {
                    strErrInfo = "获取数据集下的要素类名称列表：要素数据集为空！";
                    return null;
                }

                //打开数据集
                IDataset pDataset = pFeatureDataset as IDataset;
                //获得数据集下中所有的独立要素类
                IEnumDataset enumDataset = pDataset.Subsets as IEnumDataset;
                IDataset dataset = enumDataset.Next();
                string strName = "";
                string strAliasName = "";
                while (dataset != null)
                {
                    if (bGetAliasName)
                    {
                        IFeatureClass pFeatureClass = dataset as IFeatureClass;
                        strName = dataset.Name;
                        strAliasName = pFeatureClass.AliasName;
                        listLyrInDS.Add(strAliasName);
                    }
                    else
                    {
                        listLyrInDS.Add(dataset.Name);
                    }
                    dataset = enumDataset.Next();
                }
                //释放com对象
                System.Runtime.InteropServices.Marshal.ReleaseComObject(enumDataset);
                return listLyrInDS;
            }
            catch (Exception ex)
            {
                strErrInfo = "获取数据集内要素类失败！" + ex.Message.ToString();
                return null;
            }
        }

        /// <summary>
        /// 获取 数据集-要素类集合 名称字典
        /// </summary>
        /// <param name="pWorkSpace"></param>
        /// <param name="strErrInfo"></param>
        /// <param name="bGetAliasName">是否获取别名，默认为false</param>
        /// <returns></returns>
        public static Dictionary<string, List<string>> GetFeatrueClassInDataset(ref IWorkspace pWorkSpace, ref string strErrInfo, bool bGetAliasName = false)
        {
            try
            {
                strErrInfo = "";
                string sDSName = "";
                Dictionary<string, List<string>> dicLyrsInDS = new Dictionary<string, List<string>>();

                if (pWorkSpace == null) return null;

                //IFeatureWorkspace pFws = pWorkSpace as IFeatureWorkspace;
                IEnumDatasetName FeatureEnumDatasetName = pWorkSpace.get_DatasetNames(esriDatasetType.esriDTFeatureDataset);
                if (FeatureEnumDatasetName == null) return null;
                FeatureEnumDatasetName.Reset();
                IDatasetName pDatasetName = FeatureEnumDatasetName.Next();
                while (pDatasetName != null)
                {
                    sDSName = pDatasetName.Name;
                    if (!dicLyrsInDS.ContainsKey(sDSName))
                    {
                        dicLyrsInDS[sDSName] = GetFeatureClassNamesInDataset(ref pWorkSpace, sDSName,ref strErrInfo, bGetAliasName);
                    }
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(pDatasetName);
                    pDatasetName = FeatureEnumDatasetName.Next();
                }
                System.Runtime.InteropServices.Marshal.ReleaseComObject(FeatureEnumDatasetName);
                return dicLyrsInDS;
            }
            catch (Exception ex)
            {
                strErrInfo = "获取数据集失败！" + ex.Message;
                return null;
            }
        }

        /// <summary>
        /// 获取要素类名称
        /// </summary>
        /// <param name="pFeaCls"></param>
        /// <param name="bAliasName"></param>
        /// <returns></returns>
        public static string GetFeatureClassName(ref IFeatureClass pFeaCls,bool bAliasName = false)
        {
            if (pFeaCls == null) return "";
            try
            {
                if (bAliasName)
                {
                    return pFeaCls.AliasName;
                }
                else
                {
                    IDataset pDS = pFeaCls as IDataset;
                    return pDS.Name;
                }
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #region =========  4.删除要素类  =========
        /// <summary>
        /// 删除工作空间下要素类
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="pFeatureClsName">要素类名称。为空时，全部删除</param>
        /// <returns></returns>
        public static bool DeleteFeatureClass(ref IWorkspace pWorkspace, string pFeatureClsName = "")
        {
            bool hasDelete = false;
            bool hasError = false;
            bool bDelAll = false;
            if (pFeatureClsName == "")
            {
                bDelAll = true;
            }
            if (pWorkspace == null) return false;
            try
            {
                //遍历工作空间下的featureclass
                IEnumDataset enumDatasets = pWorkspace.get_Datasets(esriDatasetType.esriDTFeatureClass) as IEnumDataset;
                if (enumDatasets == null) return true;
                enumDatasets.Reset();
                IDataset esriDataset = enumDatasets.Next() as IDataset;
                while (esriDataset is IFeatureClass)
                {
                    if (bDelAll)
                    {
                        if (esriDataset.CanDelete())
                        {
                            esriDataset.Delete();
                        }
                        else
                        {
                            hasError = true;
                        }
                    }
                    else if (esriDataset.Name == pFeatureClsName)
                    {
                        if (esriDataset.CanDelete())
                        {
                            esriDataset.Delete();
                            hasDelete = true;
                            break;
                        }
                        else
                        {
                            hasError = true;
                            break;
                        }
                    }
                    esriDataset = enumDatasets.Next() as IDataset;
                }
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
                string strErrInfo = "删除数据集下所有要素类失败！\n" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 删除数据集下所有要素类
        /// </summary>
        /// <param name="pWorkspace">工作空间</param>
        /// <param name="pDataSetName">数据集名称</param>
        /// <param name="pFeatureClassName">要素类名称。为空时，全部删除</param>
        /// <returns></returns>
        public static bool DeleteFeatureClass(ref IWorkspace pWorkspace, string pDataSetName, string pFeatureClassName = "")
        {
            bool hasDelete = false;
            bool hasError = false;
            bool bDelAll = false;
            if (pFeatureClassName == "") //删除所有图层
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
                    if (pDataset is IFeatureDataset)
                    {
                        //try to find class in dataset
                        string datasetName = pDataset.Name;
                        if (datasetName == pDataSetName)
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
                                if (bDelAll)
                                {
                                    if (subDataset.CanDelete())
                                    {
                                        subDataset.Delete();
                                    }
                                    else
                                    {
                                        hasError = true;
                                    }
                                }
                                else if (strAliasName == pFeatureClassName)
                                {
                                    if (subDataset.CanDelete())
                                    {
                                        subDataset.Delete();
                                        hasDelete = true;
                                        break;
                                    }
                                    else
                                    {
                                        hasError = true;
                                    }
                                }
                                subDataset = subEnumDataset.Next();
                            }
                            QRelease.ReleaseESRIObject(subDataset);
                            if (hasDelete)
                            {
                                break;
                            }
                        }
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
                string strErrInfo = "删除数据集下所有要素类失败！\n" + ex.Message;
                return false;
            }
        }
        #endregion

        #region ============= 5.  [通过文件(dwg)获取空间数据]  =============
        /// <summary>
        /// 通过dwg获取Featureclass
        /// </summary>
        /// <param name="sCADFilePath"></param>
        /// <param name="fileName"></param>
        /// <param name="type">polygon等</param>
        /// <returns></returns>
        public static IFeatureClass OpenCadDrawingDataset(string sCADFilePath, string fileName, string type)
        {

            IWorkspaceFactory pWSF = null;
            IFeatureWorkspace pFeatureWorkspace = null;
            IFeatureClass pFeatureClass = null;
            try
            {
                //设置workspace
                pWSF = new CadWorkspaceFactoryClass();
                //Open the Workspace 
                pFeatureWorkspace = pWSF.OpenFromFile(sCADFilePath, 0) as IFeatureWorkspace;
                pFeatureClass = pFeatureWorkspace.OpenFeatureClass(fileName + ":" + type);

                return pFeatureClass;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                QRelease.ReleaseESRIObject(pWSF, pFeatureWorkspace);
            }
        }
        #endregion

        #region ============= 6.判断要素类  =============
        /// <summary>
        /// 判断要素类是否为空的要素类，不含任何要素返回0,有要素，返回1，出错，返回-1;
        /// </summary>
        /// <param name="pFeaCls"></param>
        /// <returns></returns>
        public static int IsFeatrueClassNull(ref IFeatureClass pFeaCls)
        {
            if (pFeaCls == null) return 0;
            int iRes = -1;
            try
            {
                IFeatureCursor featureCursor = pFeaCls.Search(null, false);
                IFeature pFeature = featureCursor.NextFeature();
                if (pFeature != null)
                {
                    iRes = 1;
                }
                else iRes = 0;
                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor) > 0) ;
                return iRes;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        #endregion

        #region ============= 7.复制要素类  =============

        /// <summary>
        /// 拷贝要素类
        /// </summary>
        /// <param name="pSrcFeaCls"></param>
        /// <param name="pTarFeaCls"></param>
        /// <param name="strError"></param>
        /// <param name="bAppendFields">字段不一致时，是否追加字段</param>
        /// <returns></returns>
        public static bool CopyFeatureClass(ref IFeatureClass pSrcFeaCls, ref IFeatureClass pTarFeaCls,
            out string strError, bool bAppendFields = true)
        {
            strError = "";
            if (pSrcFeaCls == null || pTarFeaCls == null)
            {
                strError = "复制要素类失败：要素类对象为空！";
                return false;
            }

            long lCount = 0;
            //获取到IWorkspaceEdit接口，IWorkspaceEdit是编辑必须的接口
            IField fieldSrc = new FieldClass();
            IDataset pTarDataset = pTarFeaCls as IDataset;
            IWorkspace pTarWS = pTarDataset.Workspace;
            IWorkspaceEdit wsEditTar = pTarWS as IWorkspaceEdit;

            if (wsEditTar != null)
            {
                //判断要素类是否包含要素
                int iIsNull = QFeatureClass.IsFeatrueClassNull(ref pSrcFeaCls);
                if (iIsNull == 0) return true;
                else if (iIsNull == -1)
                {
                    strError = $"要素类[{GetFeatureClassName(ref pSrcFeaCls)}]:判断是否为空错误！";
                    return false;
                }

                wsEditTar.StartEditing(true);
                wsEditTar.StartEditOperation();

                try
                {
                    IFeatureCursor pFeaCursorTar = null;
                    IFeatureCursor pFeaCursorSrc = pSrcFeaCls.Search(null, false); //true 为传址，false 为传值
                    IFeature pFeaSrc = pFeaCursorSrc.NextFeature();
                    while (pFeaSrc != null)
                    {
                        ++lCount;
                        //创建featureBuffer
                        IFeatureBuffer featureBuffer = pTarFeaCls.CreateFeatureBuffer();
                        //获取插入游标
                        pFeaCursorTar = pTarFeaCls.Insert(true);

                        IFields fieldsSrc = pFeaSrc.Fields;
                        int count = fieldsSrc.FieldCount;

                        for (int i = 0; i < count; i++)
                        {
                            int indexTar = -1;
                            fieldSrc = fieldsSrc.Field[i];
                            string fieldName = fieldSrc.Name;
                            //写入前判断字段是否可编辑
                            indexTar = featureBuffer.Fields.FindField(fieldName);
                            if (fieldSrc.Editable && pFeaSrc.Value[i] != DBNull.Value)
                            {
                                if (indexTar == -1 && bAppendFields) //追加字段
                                {
                                    if (!QField.AddField(ref pTarFeaCls, ref strError, fieldName, fieldSrc.Type))
                                    {
                                        strError =
                                            $"目标要素类{GetFeatureClassName(ref pTarFeaCls)}新建字段{fieldName}失败:{strError}";
                                        return false;
                                    }

                                    indexTar = fieldsSrc.FindField(fieldName);
                                }
                                else if (indexTar == -1 && !bAppendFields)
                                {
                                    continue;
                                }

                                featureBuffer.Value[indexTar] = pFeaSrc.Value[i];
                            }
                        }

                        pFeaCursorTar.InsertFeature(featureBuffer);
                        if ((lCount + 1) % 1000 == 0) pFeaCursorTar.Flush();
                        // Stop editing.
                        pFeaSrc = pFeaCursorSrc.NextFeature();
                    }

                    if (lCount % 1000 != 0 && pFeaCursorTar != null) pFeaCursorTar.Flush();
                    wsEditTar.StopEditOperation();
                    wsEditTar.StopEditing(true);
                    QRelease.ReleaseESRIObject(fieldSrc, pFeaCursorSrc);
                    return true;
                }
                catch (Exception ex)
                {
                    wsEditTar.StopEditOperation();
                    wsEditTar.StopEditing(false); //不保存数据
                    strError = ex.Message;
                    return false;
                }
            }
            else return false;
        }
        #endregion
    }
}

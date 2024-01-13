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
    /// 要素
    /// </summary>
    public class QFeature
    {
        #region =========  1.查找要素  =========
        /// <summary>
        /// 查找要素
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="envelope"></param>
        /// <param name="spatialRel"></param>
        /// <returns></returns>
        public static IFeature SearchFeature(ref IFeatureClass featureClass, IGeometry envelope, esriSpatialRelEnum spatialRel = esriSpatialRelEnum.esriSpatialRelIntersects)
        {
            try
            {
                //空间查询
                ISpatialFilter spatialFilter = new SpatialFilterClass();
                spatialFilter.Geometry = envelope;//指定几何体
                String shpFld = featureClass.ShapeFieldName;
                spatialFilter.GeometryField = shpFld;
                spatialFilter.SpatialRel = spatialRel;//相交
                IQueryFilter queryFilter = new QueryFilterClass();
                queryFilter = (IQueryFilter)spatialFilter;
                IFeatureCursor searchCursor = featureClass.Search(queryFilter, true);
                IFeature feature = searchCursor.NextFeature();
                int n = 0;
                while (feature != null)
                {
                    n++;
                    feature = searchCursor.NextFeature();
                }
                return feature;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFeaClass"></param>
        /// <param name="strWhereClause">条件字符串："字段名 = '字段值' "</param>
        /// <returns></returns>
        public static List<IFeature> SearchFeature(ref IFeatureClass pFeaClass, string strWhereClause)
        {
            try
            {
                List<IFeature> listFea = new List<IFeature>();
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = strWhereClause;
                IFeatureCursor pFeatureCursor = pFeaClass.Search(pQueryFilter, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    listFea.Add(pFeature);
                    pFeature = pFeatureCursor.NextFeature();
                }
                return listFea;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 找出两个要素集中相同的要素集
        /// </summary>
        /// <param name="listFeatureSrc">源要素列表</param>
        /// <param name="listFeatureTar">待比较的要素列表</param>
        /// <param name="listFindFeaSrc">源要素集合中与待比较的要素列表相同要素集合</param>
        /// <param name="fields">比较的字段</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static bool FindSameFeature(ref List<IFeature> listFeatureSrc, ref List<IFeature> listFeatureTar, ref List<IFeature> listFindFeaSrc, ref string error, params string[] fields)
        {
            if (listFeatureSrc.Count == 0 && listFeatureTar.Count == 0) return true;
            if (listFeatureSrc.Count != 0 && listFeatureTar.Count == 0) return true;
            if (listFeatureSrc.Count == 0 && listFeatureTar.Count != 0) return false;

            bool isExist = false;
            int iField = fields.Length;
            int iFeaTarCount = listFeatureTar.Count;
            int[] indexSrc = new int[iField];
            int[] indexTar = new int[iField];
            IFeature pFeaSrc = null;
            IFeature pFeaTar = null;
            try
            {
                pFeaSrc = listFeatureSrc[0];
                pFeaTar = listFeatureTar[0];
                for (int i = 0; i < iField; i++)
                {
                    string field = fields[i];
                    int idxSrc = pFeaSrc.Fields.FindField(field);
                    int idxTar = pFeaTar.Fields.FindField(field);
                    if (idxSrc != -1 && idxTar != -1)
                    {
                        indexSrc[i] = idxSrc;
                        indexTar[i] = idxTar;
                    }
                    else if (idxSrc == -1 && idxTar == -1)
                    {
                        continue;
                    }
                    else return false;
                }
                for (int i = listFeatureSrc.Count - 1; i >= 0; --i)
                {
                    pFeaSrc = listFeatureSrc[i];
                    if (pFeaSrc != null)
                    {
                        for (int j = iFeaTarCount - 1; j >= 0; --j)
                        {
                            pFeaTar = listFeatureTar[j];
                            if (QCompare.IsFeatureEqual(ref pFeaSrc, ref pFeaTar, indexSrc, indexTar))
                            {
                                listFindFeaSrc.Add(pFeaTar);
                                --iFeaTarCount;
                                isExist = true;
                                break;
                            }
                        }
                    }
                }
                return isExist;
            }
            catch (Exception ex)
            {
                error = string.Format("查找要素类中相同的要素集合失败：{0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 查找两个要素类中相同的要素
        /// </summary>
        /// <param name="pFeaClsSrc">源要素类</param>
        /// <param name="pFeaClsTar">新要素类</param>
        /// <param name="listFindFeaSrc">源要素类中已存在的要素集合</param>
        /// <param name="error"></param>
        /// <param name="fields">比较的字段</param>
        /// <returns></returns>
        public static bool FindSameFeature(ref IFeatureClass pFeaClsSrc, ref IFeatureClass pFeaClsTar, ref List<IFeature> listFindFeaSrc, ref string error, params string[] fields)
        {
            if (pFeaClsSrc == null && pFeaClsTar == null) return true;
            if (pFeaClsSrc != null && pFeaClsTar == null) return true;
            if (pFeaClsSrc == null && pFeaClsTar != null)
            {
                error = "FindFeatureListSame：源要素类为空";
                return false;
            }

            int iField = fields.Length;
            int[] indexSrc = new int[iField];
            int[] indexTar = new int[iField];
            try
            {
                List<IFeature> listFeaSrc = GetFeature(pFeaClsSrc);
                int iFeaSrcCount = listFeaSrc.Count;
                if (iFeaSrcCount == 0) return true;
                IFeature pFeaSrc = listFeaSrc[0];
                IFeature pFeaTar = null;
                if (pFeaClsTar.FeatureCount(null) == 0) return true;

                //获取字段对应的索引
                for (int i = 0; i < iField; i++)
                {
                    string field = fields[i];
                    int idxSrc = pFeaSrc.Fields.FindField(field);
                    int idxTar = pFeaClsTar.Fields.FindField(field);
                    if (idxSrc != -1 && idxTar != -1)
                    {
                        indexSrc[i] = idxSrc;
                        indexTar[i] = idxTar;
                    }
                    else if (idxSrc == -1 && idxTar == -1)
                    {
                        continue;
                    }
                    else return false;
                }

                IFeatureCursor featureCursor = pFeaClsTar.Search(null, false);  //true 为传址，false 为传值
                pFeaTar = featureCursor.NextFeature();
                while (pFeaTar != null)
                {
                    for (int i = iFeaSrcCount - 1; i >= 0; --i)
                    {
                        pFeaSrc = listFeaSrc[i];
                        if (pFeaSrc != null)
                        {
                            if (QCompare.IsFeatureEqual(ref pFeaSrc, ref pFeaTar, indexSrc, indexTar))
                            {
                                listFindFeaSrc.Add(pFeaTar);
                                --iFeaSrcCount;
                                listFeaSrc.RemoveAt(i);
                                break;
                            }

                        }
                    }
                    if (iFeaSrcCount < 0)
                    {
                        break;
                    }
                    pFeaTar = featureCursor.NextFeature();
                }
                while (System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor) > 0) ;
                return true;
            }
            catch (Exception ex)
            {
                error = string.Format("找出两个要素类中相同的要素失败：{0}", ex.Message);
                return false;
            }
        }

        #endregion

        #region =========  2.获取要素  =========
        /// <summary>
        /// 要素类中获得指定图形的对象
        /// </summary>
        /// <param name="pGeometry"></param>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static List<IFeature> GetFeature(IGeometry pGeometry, IFeatureClass featureClass)
        {
            List<IFeature> listFeature = new List<IFeature>();
            ISpatialFilter spatialFilter = new SpatialFilterClass();
            spatialFilter.Geometry = pGeometry;
            spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            IFeatureCursor featureCursor = featureClass.Search(spatialFilter, false);
            IFeature pFeature = featureCursor.NextFeature();
            while (pFeature != null)
            {
                listFeature.Add(pFeature);
                pFeature = featureCursor.NextFeature();
            }
            System.Runtime.InteropServices.Marshal.ReleaseComObject(pFeature);
            while (System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor) > 0) ;
            System.Runtime.InteropServices.Marshal.ReleaseComObject(spatialFilter);
            return listFeature;
        }

        /// <summary>
        /// 获取要素类下所有要素
        /// </summary>
        /// <param name="featureClass"></param>
        /// <returns></returns>
        public static List<IFeature> GetFeature(IFeatureClass featureClass)
        {
            List<IFeature> listFeature = new List<IFeature>();
            IFeatureCursor featureCursor = featureClass.Search(null, false);  //true 为传址，false 为传值
            IFeature pFeature = featureCursor.NextFeature();
            while (pFeature != null)
            {
                listFeature.Add(pFeature);
                pFeature = featureCursor.NextFeature();
            }
            while (System.Runtime.InteropServices.Marshal.ReleaseComObject(featureCursor) > 0) ;
            return listFeature;
        }

        /// <summary>
        /// 获取要素类名称
        /// </summary>
        /// <param name="pFeature"></param>
        /// <returns></returns>
        public static string GetFeatureName(ref IFeature pFeature)
        {
            if (pFeature == null) return "";
            try
            {
                IDataset pDS = pFeature as IDataset;
                return pDS.Name;
            }
            catch (Exception)
            {
                return "";
            }
        }

        /// <summary>
        /// 获取要素类中要素的个数
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <returns></returns>
        public static int GetFeatureNumber(ref IFeatureClass pFeatureClass)
        {
            if (pFeatureClass == null)
            {
                return 0;
            }

            return pFeatureClass.FeatureCount(null);
        }
        #endregion

        #region =========  3.删除要素  =========
        /// <summary>  
        /// 通过IFeature.Delete方法删除要素  
        /// </summary>  
        /// <param name="pFeatureclass">要素类</param>  
        /// <param name="strWhereClause">查询条件</param>  
        public static bool DeleteFeatureByIFeature(ref IFeatureClass pFeatureclass, string strWhereClause)
        {
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = strWhereClause;
                IFeatureCursor pFeatureCursor = pFeatureclass.Search(pQueryFilter, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    pFeature.Delete();
                    pFeature = pFeatureCursor.NextFeature();
                }
                QRelease.ReleaseESRIObject(pQueryFilter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>  
        /// 通过IFeatureCursor.DeleteFeature方法删除要素  
        /// </summary>  
        /// <param name="pFeatureclass">要素类</param>  
        /// <param name="strWhereClause">查询条件: example ：objectID 小于 8 </param>  
        public static bool DeleteFeatureByIFeatureCursor(ref IFeatureClass pFeatureclass, string strWhereClause)
        {
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = strWhereClause;
                IFeatureCursor pFeatureCursor = pFeatureclass.Update(pQueryFilter, false);
                IFeature pFeature = pFeatureCursor.NextFeature();
                while (pFeature != null)
                {
                    pFeatureCursor.DeleteFeature();
                    pFeature = pFeatureCursor.NextFeature();
                }
                QRelease.ReleaseESRIObject(pQueryFilter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }



        /// <summary>
        /// 根据SQL语句删除要素
        /// </summary>
        /// <param name="PFeatureclass"></param>
        /// <param name="strWhereClause">sql语句的where部分</param>
        public static bool DeleteFeatureBySQL(ref IFeatureClass PFeatureclass, string strWhereClause)
        {
            try
            {
                IDataset pDataset = PFeatureclass as IDataset;
                pDataset.Workspace.ExecuteSQL("delete from " + PFeatureclass.AliasName + " " + strWhereClause);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 根据行删除要素
        /// </summary>
        /// <param name="PFeatureclass"></param>
        /// <param name="strWhereClause"></param>
        /// <returns></returns>
        public static bool DeleteFeatureBySerchedRows(IFeatureClass PFeatureclass, string strWhereClause)
        {
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = strWhereClause;
                ITable pTable = PFeatureclass as ITable;
                pTable.DeleteSearchedRows(pQueryFilter);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(pQueryFilter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>  
        /// 通过ITable.DeleteSearchedRows方法删除要素  
        /// </summary>  
        /// <param name="pFeatureclass">要素类</param>  
        /// <param name="strWhereClause">查询条件</param>  
        public static bool DeleteFeatureByITable(IFeatureClass pFeatureclass, string strWhereClause)
        {
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = strWhereClause;   //"ZONING_S = 'R'"
                ITable pTable = pFeatureclass as ITable;
                pTable.DeleteSearchedRows(pQueryFilter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// 通过ITable.DeleteSearchedRows方法删除要素 
        /// </summary>
        /// <param name="pLayer"></param>
        /// <param name="queryFilter"></param>
        public static bool DeleteFeatureByITable(IFeatureLayer pLayer, IQueryFilter queryFilter)
        {
            try
            {
                ITable pTable = pLayer.FeatureClass as ITable;
                pTable.DeleteSearchedRows(queryFilter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion


        #region =========  4.写要素  =========
        /// <summary>
        /// 插入单个数据
        /// </summary>
        /// <param name="pFeatureCls"></param>
        /// <param name="pFeature"></param>
        /// <param name="featureOID">插入数据之后该数据的OID</param>
        /// <param name="sError">错误信息</param>
        /// <param name="bAppendFields">字段不一致时，是否追加字段</param>
        /// <returns></returns>
        public static bool WriteFeature2FeatureClass(ref IFeatureClass pFeatureCls, ref IFeature pFeature, out object featureOID,out string sError,bool bAppendFields = true)
        {
            sError = "";
            featureOID = null;
            string fieldName = "";

            if (pFeatureCls == null || pFeature == null)
            {
                sError = "写入要素至要素类失败：参数为空！";
                return false;   
            }

            //获取到IWorkspaceEdit接口，IWorkspaceEdit是编辑必须的接口
            IField fieldSrc = new FieldClass();
            IDataset dataset = pFeatureCls as IDataset;
            IWorkspace workspace = dataset.Workspace;
            IWorkspaceEdit wsEdit = workspace as IWorkspaceEdit;

            // Start an edit session and edit operation.第一个参数是是否允许Undo，Redo（重做，撤销）
            wsEdit.StartEditing(true);
            //构成一个EditOperation有StartEditOperation和StopEditOperation方法，Undo，Redo是针对一个EditOperation的
            wsEdit.StartEditOperation();
            try
            {
                //创建featureBuffer
                IFeatureBuffer featureBuffer = pFeatureCls.CreateFeatureBuffer();
                //获取插入游标
                IFeatureCursor featureCursor = pFeatureCls.Insert(true);
                int count = pFeature.Fields.FieldCount;

                IFields fieldsSrc = pFeature.Fields;
                for (int i = 0; i < count; i++)
                {
                    fieldSrc = fieldsSrc.Field[i];
                    fieldName = fieldSrc.Name;
                    //写入前判断字段是否可编辑
                    if (fieldSrc.Editable && pFeature.Value[i] != DBNull.Value)
                    {
                        int indexTar = featureBuffer.Fields.FindField(fieldName);
                        if (indexTar == -1 && bAppendFields)
                        {
                            if (!QField.AddField(ref pFeatureCls, ref sError, fieldName, fieldSrc.Type))
                            {
                                sError = $"目标要素类{QFeatureClass.GetFeatureClassName(ref pFeatureCls)}新建字段{fieldName}失败！";
                                return false;
                            }
                            indexTar = pFeatureCls.Fields.FindField(fieldName);
                        }
                        else if (indexTar == -1 && !bAppendFields) continue;

                        if (indexTar != -1) featureBuffer.Value[indexTar] = pFeature.Value[i];
                    }
                }
                int indexCD = featureBuffer.Fields.FindField("CREATE_DATE");
                if (indexCD != -1)
                {
                    IField fieldTar = featureBuffer.Fields.Field[indexCD];
                    if (fieldTar.Editable)
                    {
                        featureBuffer.Value[indexCD] = DateTime.Now;
                    }
                }

                featureOID = featureCursor.InsertFeature(featureBuffer);
                featureCursor.Flush();
                // Stop editing.
                wsEdit.StopEditOperation();
                wsEdit.StopEditing(true);
                QRelease.ReleaseESRIObject(fieldSrc, fieldsSrc, featureBuffer, featureCursor);
            }
            catch (Exception ex)
            {
                string strErrInfo = $"要素类[{pFeatureCls.AliasName}]插入字段{fieldName}数据失败！\n {ex.Message}";
                // Stop editing.
                wsEdit.StopEditOperation();
                wsEdit.StopEditing(false);   //不保存数据
                return false;
            }
            return true;
        }


        /// <summary>
        /// 将Feature的集合写入到FeatureClass中
        /// </summary>
        /// <param name="pFeatureCls"></param>
        /// <param name="pListFeatures"></param>
        /// <param name="sError">错误信息</param>
        /// <param name="bAppendFields">字段不一致时，是否追加字段</param>
        /// <returns></returns>
        public static bool WriteFeature2FeatureClass(ref IFeatureClass pFeatureCls, ref List<IFeature> pListFeatures, ref string sError, bool bAppendFields = true)
        {
            if (pListFeatures.Count == 0)
            {
                sError = "待写入的要素为空！";
                return false;
            }
            if (pFeatureCls == null)
            {
                sError = "待写入的要素类为空！";
                return false;
            }

            //获取到IWorkspaceEdit接口，IWorkspaceEdit是编辑必须的接口
            long lCount = 0;
            IFeatureCursor featureCursor = null;
            IField field = new FieldClass();
            IDataset dataset = pFeatureCls as IDataset;
            IWorkspace workspace = dataset.Workspace;
            IWorkspaceEdit wsEdit = workspace as IWorkspaceEdit;
            // Start an edit session and edit operation.第一个参数是是否允许Undo，Redo（重做，撤销）
            wsEdit.StartEditing(true);
            //构成一个EditOperation有StartEditOperation和StopEditOperation方法，Undo，Redo是针对一个EditOperation的
            wsEdit.StartEditOperation();

            try
            {
                for (int i = 0; i < pListFeatures.Count; i++)
                {
                    ++lCount;
                    IFeature pFeature = pListFeatures[i];
                    //创建featureBuffer
                    IFeatureBuffer featureBuffer = pFeatureCls.CreateFeatureBuffer();
                    //获取插入游标
                    featureCursor = pFeatureCls.Insert(true);
                    int count = pFeature.Fields.FieldCount;

                    IFields fields = pFeature.Fields;
                    for (int j = 0; j < count; j++)
                    {
                        field = fields.Field[j];
                        string fieldName = field.Name;
                        //写入前判断字段是否可编辑
                        int index = featureBuffer.Fields.FindField(fieldName);
                        if (field.Editable && pFeature.Value[j] != DBNull.Value)
                        {
                            if (index == -1 && bAppendFields)
                            {
                                if (!QField.AddField(ref pFeatureCls, ref sError, fieldName, field.Type))
                                {
                                    sError = $"目标要素类{QFeatureClass.GetFeatureClassName(ref pFeatureCls)}新建字段{fieldName}失败！";
                                    return false;
                                }
                                index = pFeatureCls.Fields.FindField(fieldName);
                            }
                            else if(index == -1 && !bAppendFields) continue;
                            if (index != -1) featureBuffer.Value[index] = pFeature.Value[j];
                        }
                    }

                    int indexCD = featureBuffer.Fields.FindField("CREATE_DATE");
                    if (indexCD != -1)
                    {
                        field = featureBuffer.Fields.Field[indexCD];
                        if (field.Editable) featureBuffer.Value[indexCD] = DateTime.Now;
                    }

                    if (pFeature.Shape != null)
                    {
                        featureBuffer.Shape = pFeature.ShapeCopy;
                    }
                    featureCursor.InsertFeature(featureBuffer);
                    if (lCount % 1000 == 0) featureCursor.Flush();
                }
                if (lCount % 1000 != 0 && featureCursor != null) featureCursor.Flush();
                // Stop editing.
                wsEdit.StopEditOperation();
                wsEdit.StopEditing(true);
                QRelease.ReleaseESRIObject(field);
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                //停止编辑
                wsEdit.StopEditOperation();
                //不保存数据
                wsEdit.StopEditing(false);   
                return false;
            }
            return true;
        }
        #endregion
    }
}

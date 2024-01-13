/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib
{
    /// <summary>
    /// 表格
    /// </summary>
    public class QTable
    {
        #region =========  打开表格  =========
        /// <summary>
        /// 打开表格
        /// </summary>
        /// <param name="pWorkSpace"></param>
        /// <param name="tableName"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static ITable OpenTable(ref IWorkspace pWorkSpace, string tableName, ref string error)
        {
            error = "";
            try
            {
                if (pWorkSpace == null)
                {
                    error = "打开属性表失败：工作空间为空！";
                    return null;
                }
                IFeatureWorkspace pFws = pWorkSpace as IFeatureWorkspace;
                if (pFws != null)
                {
                    ITable pTable = pFws.OpenTable(tableName);
                    return pTable;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                error = string.Format("打开表[{0}]出错:{1}",tableName, ex.Message);
                return null;
            }
        }
        #endregion

        #region =========  获取表格  =========
        /// <summary>
        /// 获取工作空间下所有表格
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="listTable"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static bool GetTable(ref IWorkspace pWorkspace,
    ref List<ITable> listTable, ref string strErrInfo)
        {
            strErrInfo = "";
            listTable = new List<ITable>();
            try
            {
                if (pWorkspace == null)
                {
                    strErrInfo = "读取要素集下要素类：工作空间为空！";
                    return false;
                }
                // 遍历要素集Dataset下的FeatrueClass
                IEnumDataset pEnumTables = pWorkspace.get_Datasets(esriDatasetType.esriDTTable);
                pEnumTables.Reset();
                IDataset pDataset = pEnumTables.Next();
                while (pDataset != null)
                {
                    if (pDataset is ITable)
                    {
                        //try to find class in dataset
                        string pDatasetName = pDataset.Name;
                        ITable pTable = pDataset as ITable;
                        listTable.Add(pTable);
                    }
                    pDataset = pEnumTables.Next();
                }

                return true;
            }
            catch (Exception ex)
            {
                strErrInfo = "读取表格列表失败" + ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 根据表格名称获取表格
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="sTableName"></param>
        /// <returns></returns>
        public static ITable GetTable(ref IWorkspace pWorkspace,string sTableName)
        {
            ITable pTable = null;
            if (pWorkspace == null || string.IsNullOrEmpty(sTableName))
            {
                return null;
            }
            try
            {
                // 遍历要素集Dataset下的FeatrueClass
                IEnumDataset pEnumTables = pWorkspace.get_Datasets(esriDatasetType.esriDTTable);
                pEnumTables.Reset();
                IDataset pDataset = pEnumTables.Next();
                while (pDataset != null)
                {
                    if (pDataset is ITable)
                    {
                        //try to find class in dataset
                        string pDatasetName = pDataset.Name;
                        if (pDatasetName == sTableName)
                        {
                            pTable = pDataset as ITable;
                            return pTable;
                        }
                    }
                    pDataset = pEnumTables.Next();
                }

                return pTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 获取GDB里的所有纯表
        /// </summary>
        /// <param name="pWorkSpace"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static List<string> GetTableNames(ref IWorkspace pWorkSpace, ref string strErrInfo)
        {
            try
            {
                strErrInfo = "";
                List<string> strListTblNm = new List<string>();
                if (pWorkSpace == null)
                {
                    strErrInfo = "获取表名列表失败：工作空间为空！";
                    return null;
                }
                //IFeatureWorkspace pFws = pWorkSpace as IFeatureWorkspace;
                IEnumDatasetName FeatureEnumDatasetName = pWorkSpace.get_DatasetNames(esriDatasetType.esriDTTable);
                if (FeatureEnumDatasetName == null) return null;
                FeatureEnumDatasetName.Reset();
                IDatasetName pDatasetName = FeatureEnumDatasetName.Next();
                while (pDatasetName != null)
                {
                    strListTblNm.Add(pDatasetName.Name);
                    pDatasetName = FeatureEnumDatasetName.Next();
                }
                return strListTblNm;
            }
            catch (Exception ex)
            {
                strErrInfo = "获取表名集合出错：" + ex.Message.ToString();
                return null;
            }
        }
       
        /// <summary>
        /// 获取表格名称
        /// </summary>
        /// <param name="pTable"></param>
        /// <returns></returns>
        public static string GetTableName(ref ITable pTable)
        {
            if (pTable == null) return "";
            try
            {
                IDataset pDS = pTable as IDataset;
                return pDS.Name;
            }
            catch (Exception)
            {
                return "";
            }
        }
        #endregion

        #region =========  获取非空表格  =========
        /// <summary>
        /// 获取非空表格列表
        /// </summary>
        /// <param name="listTable"></param>
        public static void GetNotNullTable(ref List<ITable> listTable)
        {
            if (listTable.Count == 0)
            {
                return;
            }
            for (int i = listTable.Count - 1; i >= 0; --i)
            {
                ITable pTable = listTable[i];
                if (pTable.RowCount(null) == 0)
                {
                    listTable.Remove(pTable);
                }
            }
        }

        /// <summary>
        /// 是否是空表
        /// </summary>
        /// <param name="pTable"></param>
        /// <returns></returns>
        public static bool IsNullTable(ref ITable pTable)
        {
            if (pTable == null)
            {
                return true;
            }
            if (pTable.RowCount(null) == 0) return true;
            else return false;
        }

        /// <summary>
        /// 判断表是否为空表
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static bool IsNullTable(ref IWorkspace pWorkspace, string tableName)
        {
            if (pWorkspace == null || string.IsNullOrEmpty(tableName)) return true;
            ITable pTable = GetTable(ref pWorkspace, tableName);
            if (pTable == null)
            {
                return true;
            }
            if (pTable.RowCount(null) == 0) return true;
            else return false;
        }
        #endregion

        #region =========  创建表格  =========
        /// <summary>
        /// 根据源工作空间给目标工作空间创建相同字段的空表格
        /// </summary>
        /// <param name="pWsSrc">源工作空间</param>
        /// <param name="pWsTar">目标工作空间</param>
        /// <param name="error"></param>
        /// <param name="suffix">表名添加后缀</param>
        /// <param name="tableNames">指定创建的表名</param>
        /// <returns></returns>
        public static bool CreateTable(ref IWorkspace pWsSrc, ref IWorkspace pWsTar, ref string error,
    string suffix = "", params string[] tableNames)
        {
            try
            {
                if (pWsSrc == null || pWsTar == null)
                {
                    error = "工作空间为空，请检查！";
                    return false;
                }

                IWorkspace2 pWs2Tar = pWsTar as IWorkspace2;
                IFeatureWorkspace pFeaWsTar = pWsTar as IFeatureWorkspace;
                List<ITable> listTable = new List<ITable>();
                ITable pTblSrc = null;
                ITable pTblTar = null;
                //获取源空间要素集
                if (!GetTable(ref pWsSrc, ref listTable, ref error))
                {
                    return false;
                }
                GetNotNullTable(ref listTable);
                int iTblNO = listTable.Count;
                if (iTblNO == 0) return true;

                for (int i = 0; i < iTblNO; i++)
                {
                    pTblSrc = listTable[i];
                    IDataset pDataset = pTblSrc as IDataset;
                    string tblName = pDataset.Name + suffix;
                    if (tableNames.Length > 0 && !tableNames.Contains(tblName))
                    {
                        continue;
                    }
                    bool bExist = pWs2Tar.get_NameExists(esriDatasetType.esriDTTable, tblName);
                    if (!bExist)
                    {
                        pTblTar = CreateTable(ref pWsTar, ref pTblSrc, tblName);
                        if (pTblTar == null)
                        {
                            error = string.Format("创建新表[{0}]失败", tblName);
                            return false;
                        }
                    }
                    else
                    {
                        pTblTar = OpenTable(ref pWsTar, tblName, ref error);
                    }
                }
                return pTblTar != null;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 创建新表
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="pTable"></param>
        /// <param name="tableName"></param>
        /// <returns></returns>
        public static ITable CreateTable(ref IWorkspace pWorkspace, ref ITable pTable, string tableName = "")
        {
            IField pField = null;
            ITable pNewTable = null;
            if (pWorkspace == null || pTable == null)
            {
                return null;
            }
            try
            {
                IFeatureWorkspace pFeaWorkspace = pWorkspace as IFeatureWorkspace;
                #region 获取属性字段 
                IFields pFields = pTable.Fields;
                IFields pNewFields = new FieldsClass();
                IFieldsEdit pFieldEdit = pNewFields as IFieldsEdit;
                if (tableName == "")
                {
                    IDataset pDataset = pTable as IDataset;
                    tableName = pDataset.Name;
                }
                //保证源要素类与新要素类的字段结构一致，但空间范围不一样，即自己手动设计Geometry字段
                for (int i = 0; i < pFields.FieldCount; i++)
                {
                    pField = pFields.get_Field(i);
                    if (pField.Type != esriFieldType.esriFieldTypeGeometry)
                    {
                        pFieldEdit.AddField(pField);
                    }

                }
                #endregion
                pNewTable = pFeaWorkspace.CreateTable(tableName, pNewFields, null, null, "");
            }
            catch (Exception)
            {
            }
            return pNewTable;
        }
        #endregion


        #region =========  复制表格  =========
        /// <summary>
        /// 复制表格到目标工作空间
        /// </summary>
        /// <param name="pSrcTable"></param>
        /// <param name="pTWorkspace"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool CopyTable(ref ITable pSrcTable, ref IWorkspace pTWorkspace, out string error)
        {
            error = "";
            try
            {
                ITable pTarTable = null;
                IWorkspace2 pTWS2 = pTWorkspace as IWorkspace2;
                string sTblName = GetTableName(ref pSrcTable);
                if (!pTWS2.get_NameExists(esriDatasetType.esriDTTable,sTblName))
                {
                    pTarTable = CreateTable(ref pTWorkspace, ref pSrcTable);
                    if(pTarTable == null)
                    {
                        error = $"创建表格[{sTblName}]失败！";
                        return false;
                    }
                }
                if (!CopyTable(ref pSrcTable,ref pTarTable,out error))
                {
                    return false;
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
        /// 拷贝表格数据
        /// </summary>
        /// <param name="pSrcTable"></param>
        /// <param name="pTarTable"></param>
        /// <param name="error"></param>
        /// <param name="bAppendFields">字段不一致时，是否追加字段</param>
        /// <returns></returns>
        public static bool CopyTable(ref ITable pSrcTable,ref ITable pTarTable,out string error,bool bAppendFields = true)
        {
            error = "";
            if (pSrcTable == null || pTarTable == null)
            {
                error = "复制表格失败：表格对象为空！";
                return false;
            }
            try
            {
                if (IsNullTable(ref pSrcTable)) return true;

                int indexTar = -1;
                IRow pRowTar = null;
                IField field = null;
                IFields fieldsSrc = pSrcTable.Fields;
                IFields fieldsTar = pTarTable.Fields;
                ICursor pCursor = pSrcTable.Search(null, false);
                IRow pRowSrc = pCursor.NextRow();
                while (pRowSrc != null)
                {
                    pRowTar = pTarTable.CreateRow();
                    for (int j = 0; j < fieldsSrc.FieldCount; j++)
                    {
                        field = fieldsSrc.get_Field(j);
                        string fieldName = field.Name;
                        //写入前判断字段是否可编辑
                        if (field.Editable && pRowSrc.get_Value(j) != DBNull.Value)
                        {
                            indexTar = fieldsTar.FindField(fieldName);
                            if (indexTar == -1 && bAppendFields)
                            {
                                if (!QField.AddField(ref pTarTable,ref error, fieldName, field.Type))
                                {
                                    error = $"目标表格{GetTableName(ref pTarTable)}新建字段{fieldName}失败！";
                                    return false;
                                }
                                indexTar = fieldsTar.FindField(fieldName);
                            }
                            else if (indexTar == -1 && !bAppendFields)
                            {
                                continue;
                            }
                            if (indexTar != -1) pRowTar.set_Value(indexTar, pRowSrc.get_Value(j));
                        }
                    }
                    pRowTar.Store();
                    pRowSrc = pCursor.NextRow();
                }
                QRelease.ReleaseESRIObject(pCursor);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion 

        #region ==================  表格行数据  ==================
        #region =========  获取表格行数据  =========
        /// <summary>
        /// 获取表格所有行；
        /// </summary>
        /// <param name="pTable"></param>
        /// <param name="listRows"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool GetRowsInTable(ref ITable pTable, ref List<IRow> listRows, ref string error)
        {
            try
            {
                if (pTable == null)
                {
                    error = "获取表数据失败：表格对象为空！";
                    return false;
                }
                ICursor pCursor = pTable.Search(null, false);
                IRow pRow = pCursor.NextRow();
                while (pRow != null)
                {
                    listRows.Add(pRow);
                    pRow = pCursor.NextRow();
                }
                return true;
            }
            catch (Exception ex)
            {
                error = "读取表格信息失败" + ex.Message;
                return false;
            }
        }

        #endregion

        #region =========  删除表格行数据  =========
        /// <summary>
        /// 删除表格行数据
        /// </summary>
        /// <param name="pTable"></param>
        /// <param name="strWhereClause"></param>
        /// <returns></returns>
        public static bool DeleteRow(ITable pTable, string strWhereClause)
        {
            try
            {
                IQueryFilter pQueryFilter = new QueryFilterClass();
                pQueryFilter.WhereClause = strWhereClause;
                pTable.DeleteSearchedRows(pQueryFilter);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion

        #region =========  查找相同行数据  =========
        /// <summary>
        /// 查找两个表格之间相同的数据
        /// </summary>
        /// <param name="pTblSrc">源表格</param>
        /// <param name="pTblTar">目标表格</param>
        /// <param name="listFindRowTar">目标表格中已存在的相同的行数据</param>
        /// <param name="error">错误信息</param>
        /// <param name="fields">比较的字段，若为空则比较每一个字段</param>
        /// <returns></returns>
        public static bool FindSameRow(ref ITable pTblSrc, ref ITable pTblTar, ref List<IRow> listFindRowTar, ref string error, params string[] fields)
        {
            if (pTblSrc == null && pTblTar == null) return true;
            if (pTblSrc != null && pTblTar == null) return true;
            if (pTblSrc == null && pTblTar != null) return false;

            bool isExist = false;
            IRow pRowSrc = null;
            IRow pRowTar = null;
            List<IRow> listRowSrc = new List<IRow>();
            List<IRow> listRowTar = new List<IRow>();
            List<string> listField = new List<string>();
            try
            {
                if (!GetRowsInTable(ref pTblSrc, ref listRowSrc, ref error)) return false;
                int iRowSrcNO = listRowSrc.Count;
                if (iRowSrcNO == 0) return true;

                if (fields.Length == 0)
                {
                    pRowSrc = listRowSrc[0];
                    IFields iFields = pRowSrc.Fields;
                    for (int j = 0; j < iFields.FieldCount; j++)
                    {
                        IField ifield = iFields.get_Field(j);
                        if (ifield.Editable)
                        {
                            listField.Add(ifield.Name);
                        }
                    }
                    fields = listField.ToArray();
                }
                int iField = fields.Length;
                int[] indexSrc = new int[iField];
                int[] indexTar = new int[iField];
                for (int i = 0; i < iField; i++)
                {
                    string field = fields[i];
                    int idxSrc = pTblSrc.Fields.FindField(field);
                    int idxTar = pTblTar.Fields.FindField(field);
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

                ICursor pCursor = pTblTar.Search(null, false);
                pRowTar = pCursor.NextRow();
                while (pRowTar != null)
                {
                    for (int i = iRowSrcNO - 1; i >= 0; --i)
                    {
                        pRowSrc = listRowSrc[i];
                        if (pRowSrc != null)
                        {
                            for (int k = 0; k < indexSrc.Length; k++)
                            {
                                if (pRowSrc.get_Value(indexSrc[k]).ToString() != pRowTar.get_Value(indexTar[k]).ToString())
                                {
                                    isExist = false;
                                    break;
                                }
                            }
                            if (isExist)
                            {
                                --iRowSrcNO;
                                listRowSrc.RemoveAt(i);
                                listFindRowTar.Add(pRowTar);
                                break;
                            }
                            isExist = true;
                        }
                    }
                    if (iRowSrcNO < 0)
                    {
                        break;
                    }
                    pRowTar = pCursor.NextRow();
                }

                return true;
            }
            catch (Exception ex)
            {
                error = string.Format("查找两表相同的数据失败：{0}", ex.Message);
                return false;
            }
        }

        /// <summary>
        /// 从两个行集合中找出相同的行数据
        /// </summary>
        /// <param name="listRowSrc">源行数据集合</param>
        /// <param name="listRowTar">目标行数据集合</param>
        /// <param name="listFindRowTar">目标行数据集合中，找到的相同的行数据集合</param>
        /// <param name="error">错误信息</param>
        /// <param name="fields">比较的字段，若为空则比较每一个字段</param>
        /// <returns></returns>
        public static bool FindSameRow(ref List<IRow> listRowSrc, List<IRow> listRowTar, ref List<IRow> listFindRowTar, ref string error, params string[] fields)
        {
            if (listRowSrc.Count == 0 && listRowTar.Count == 0) return true;
            if (listRowSrc.Count != 0 && listRowTar.Count == 0) return true;
            if (listRowSrc.Count == 0 && listRowTar.Count != 0) return false;

            bool isExist = false;
            IRow pRowSrc = null;
            IRow pRowTar = null;

            List<string> listField = new List<string>();
            try
            {
                int iRowTarNO = listRowTar.Count;
                int iRowSrcNO = listRowSrc.Count;

                if (fields.Length == 0)
                {
                    pRowSrc = listRowSrc[0];
                    IFields iFields = pRowSrc.Fields;
                    for (int j = 0; j < iFields.FieldCount; j++)
                    {
                        IField ifield = iFields.get_Field(j);
                        if (ifield.Editable)
                        {
                            listField.Add(ifield.Name);
                        }
                    }
                    fields = listField.ToArray();
                }
                int iField = fields.Length;
                int[] indexSrc = new int[iField];
                int[] indexTar = new int[iField];
                for (int i = 0; i < iField; i++)
                {
                    string field = fields[i];
                    int idxSrc = listRowSrc[0].Fields.FindField(field);
                    int idxTar = listRowTar[0].Fields.FindField(field);
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

                for (int i = iRowSrcNO - 1; i >= 0; --i)
                {
                    pRowSrc = listRowSrc[i];
                    if (pRowSrc != null)
                    {
                        for (int j = iRowTarNO - 1; j >= 0; --j)
                        {
                            pRowTar = listRowTar[j];
                            for (int k = 0; k < indexSrc.Length; k++)
                            {
                                if (pRowSrc.get_Value(indexSrc[k]).ToString() != pRowTar.get_Value(indexTar[k]).ToString())
                                {
                                    isExist = false;
                                    break;
                                }
                            }
                            if (isExist)
                            {
                                --iRowTarNO;
                                listFindRowTar.Add(pRowTar);
                                break;
                            }
                        }
                    }
                }
                return isExist;
            }
            catch (Exception ex)
            {
                error = string.Format("查找两个行集合相同的数据失败：{0}", ex.Message);
                return false;
            }
        }
        #endregion

        #region =========  写入表格行数据  =========
        /// <summary>
        /// 表格写入数据
        /// </summary>
        /// <param name="pTarTable">目标表格</param>
        /// <param name="listRow"></param>
        /// <param name="error"></param>
        /// <param name="bAppendFields">字段不一致时，是否追加字段</param>
        /// <returns></returns>
        public static bool WriteRow2Table(ref ITable pTarTable, ref List<IRow> listRow, ref string error,bool bAppendFields = true)
        {
            if (listRow.Count == 0) return true;
            int index = 0;
            int indexTar = 0;
            IField fieldSrc = null;
            IFields fieldsSrc = null;
            IFields fieldsTar = pTarTable.Fields;
           IRow pRowTar = null;
            IRow pRowSrc = null;
            try
            {
                for (int i = 0; i < listRow.Count; i++)
                {
                    pRowTar = pTarTable.CreateRow();
                    pRowSrc = listRow[i];
                    fieldsSrc = pRowSrc.Fields;
                    for (int j = 0; j < fieldsSrc.FieldCount; j++)
                    {
                        fieldSrc = fieldsSrc.get_Field(j);
                        string fieldName = fieldSrc.Name;
                        //写入前判断字段是否可编辑
                        index = fieldsSrc.FindField(fieldName);
                        if (index != -1 && fieldSrc.Editable && pRowSrc.Value[index] != DBNull.Value)
                        {
                            indexTar = fieldsTar.FindField(fieldName);
                            if (indexTar == -1 && bAppendFields)
                            {
                                if (!QField.AddField(ref pTarTable, ref error, fieldName, fieldSrc.Type))
                                {
                                    error = $"目标表格{GetTableName(ref pTarTable)}新建字段{fieldName}失败！";
                                    return false;
                                }
                                indexTar = fieldsTar.FindField(fieldName);
                            }
                            else if (indexTar == -1 && !bAppendFields)
                            {
                                continue;
                            }

                            if(indexTar != -1) pRowTar.Value[index] = pRowSrc.Value[index];
                        }
                    }
                    int indexCD = pRowTar.Fields.FindField("CREATE_DATE");
                    if (indexCD != -1)
                    {
                        fieldSrc = pRowTar.Fields.Field[indexCD];
                        if (fieldSrc.Editable)
                        {
                            pRowTar.Value[indexCD] = DateTime.Now;
                        }
                    }

                    pRowTar.Store();
                }
                QRelease.ReleaseESRIObject(fieldSrc, fieldsSrc,fieldsTar);
                return true;
            }
            catch (Exception ex)
            {
                error = string.Format("表格写入数据失败:{0}", ex.Message);
                return false;
            }
        }
        #endregion
        #endregion

        #region =========  表格转换  =========
        /// <summary>
        /// 将ITable转换为DataTable
        /// </summary>
        /// <param name="mTable"></param>
        /// <returns></returns>
        public static System.Data.DataTable ToDataTable(ITable mTable)
        {
            try
            {
                System.Data.DataTable pTable = new System.Data.DataTable();
                for (int i = 0; i < mTable.Fields.FieldCount; i++)
                {
                    pTable.Columns.Add(mTable.Fields.get_Field(i).Name);
                }

                ICursor pCursor = mTable.Search(null, false);
                IRow pRrow = pCursor.NextRow();
                while (pRrow != null)
                {
                    System.Data.DataRow pRow = pTable.NewRow();
                    string[] StrRow = new string[pRrow.Fields.FieldCount];
                    for (int i = 0; i < pRrow.Fields.FieldCount; i++)
                    {
                        StrRow[i] = pRrow.get_Value(i).ToString();
                    }
                    pRow.ItemArray = StrRow;
                    pTable.Rows.Add(pRow);
                    pRrow = pCursor.NextRow();
                }

                return pTable;
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// 把DataTable转为ITable ,tempPath 不含的
        /// </summary>
        /// <param name="dataTable"></param>
        /// <param name="foldPath">文件夹路径</param>
        /// <param name="filename">文件名</param>
        /// <param name="pNewTable">返回的表</param>
        /// <param name="error">错误信息</param>
        /// <returns></returns>
        public static bool ToITable(System.Data.DataTable dataTable, string foldPath,string filename,ref ITable pNewTable,ref string error)
        {
            try
            {
                #region 新建表字段
                IField pField = null;

                IFields fields = new FieldsClass();
                IFieldsEdit fieldsEdit = (IFieldsEdit)fields;
                fieldsEdit.FieldCount_2 = dataTable.Columns.Count;

                for (int i = 0; i < dataTable.Columns.Count; i++)
                {
                    System.Data.DataColumn col = dataTable.Columns[i];
                    string columnName = col.ColumnName;

                    pField = new FieldClass();
                    IFieldEdit fieldEdit = (IFieldEdit)pField;
                    fieldEdit.Name_2 = columnName;
                    fieldEdit.AliasName_2 = columnName;
                    if (col.DataType == System.Type.GetType("System.String"))
                    {
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                    }
                    else if (col.DataType == System.Type.GetType("System.Int32") ||
                        col.DataType == System.Type.GetType("System.Int16") ||
                        col.DataType == System.Type.GetType("System.Int64") ||
                        col.DataType == System.Type.GetType("System.UInt32") ||
                        col.DataType == System.Type.GetType("System.UInt16") ||
                        col.DataType == System.Type.GetType("System.UInt64"))
                    {
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeInteger;
                    }
                    else if (col.DataType == System.Type.GetType("System.Double"))
                    {
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                    }
                    else if (col.DataType == System.Type.GetType("System.DateTime"))
                    {
                        fieldEdit.Type_2 = esriFieldType.esriFieldTypeDate;
                    }
                    fieldEdit.Editable_2 = true;
                    fieldsEdit.set_Field(i, pField);
                }
                #endregion

                ShapefileWorkspaceFactoryClass class2 = new ShapefileWorkspaceFactoryClass();
                ESRI.ArcGIS.Geodatabase.IWorkspace pWorkspace = class2.OpenFromFile(foldPath, 0);
                IFeatureWorkspace pFWS = pWorkspace as IFeatureWorkspace;

                //删除已有的
                string dbfPath = foldPath + "\\" + filename + ".dbf";
                if (System.IO.File.Exists(dbfPath))
                {
                    System.IO.File.Delete(dbfPath);
                }

                //创建空表
                pNewTable = pFWS.CreateTable(filename, fieldsEdit, null, null, "");

                //获取表中记录数
                int count = dataTable.Rows.Count;

                //转换为ITable中的数据
                for (int k = 0; k < count; k++)
                {
                    //ITable 的记录
                    IRow row = pNewTable.CreateRow();

                    System.Data.DataRow pRrow = dataTable.Rows[k];
                    //列元素
                    int rowNum = pRrow.ItemArray.Length;

                    // 添加记录
                    for (int n = 1; n < rowNum + 1; n++)
                    {
                        row.set_Value(n, pRrow.ItemArray.GetValue(n - 1));
                        row.Store();
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
    }
}

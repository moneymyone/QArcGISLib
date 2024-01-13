/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib
{
    /// <summary>
    /// 字段
    /// </summary>
    public class QField
    {
        #region =========  获取字段  =========
        /// <summary>
        /// 根据图层名获取字段名以及字段别名
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="featureClassName"></param>
        /// <param name="listFieldName">字段名集合</param>
        /// <param name="listFieldAliasName">字段别名集合</param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static bool GetFields(IWorkspace pWorkspace, string featureClassName, ref List<string> listFieldName, ref List<string> listFieldAliasName, ref string strErrInfo)
        {
            try
            {
                if (pWorkspace == null)
                {
                    strErrInfo = "根据图层名获取字段名以及字段别名：工作空间为空！";
                    return false;
                }
                IFeatureClass featureClass = QArcGISLib.QFeatureClass.OpenFeatureClass(ref pWorkspace, featureClassName, ref strErrInfo);
                if (featureClass == null)
                {
                    strErrInfo = string.Format("要素类[{0}]获取失败！", featureClassName);
                    return false;
                }
                for (int i = 0; i < featureClass.Fields.FieldCount; i++)
                {
                    if (featureClass.Fields.get_Field(i).Name.ToUpper() != "OBJECTID" &&
                        !featureClass.Fields.get_Field(i).Name.ToUpper().Contains("SHAPE"))
                    {
                        string strFieldName = featureClass.Fields.get_Field(i).Name; //字段名称
                        string strFieldAliasName = featureClass.Fields.get_Field(i).AliasName; //字段别名
                        listFieldName.Add(strFieldName);
                        listFieldAliasName.Add(strFieldAliasName);
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                strErrInfo =  ex.Message;
                return false;
            }
        }
        #endregion

        #region =========  添加字段  =========
        /// <summary>
        /// 表格添加字段
        /// </summary>
        /// <param name="pTable"></param>
        /// <param name="error"></param>
        /// <param name="fieldname"></param>
        /// <param name="fieldtype"></param>
        /// <param name="aliasname"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public static bool AddField(ref ITable pTable, ref string error, string fieldname, esriFieldType fieldtype = esriFieldType.esriFieldTypeString, string aliasname = "", bool isNullable = true)
        {
            try
            {
                //在新增字段之前，先修改SchemaLock的状态
                ISchemaLock pSchemaLock = pTable as ISchemaLock;
                pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                //Add New Field
                int index = pTable.Fields.FindField(fieldname);
                if (index == -1)
                {
                    IField pField = new FieldClass();
                    IFieldEdit pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = fieldname;
                    pFieldEdit.Type_2 = fieldtype;
                    pFieldEdit.IsNullable_2 = isNullable;
                    pFieldEdit.AliasName_2 = aliasname == "" ? fieldname : aliasname;
                    pFieldEdit.Editable_2 = true;
                    pTable.AddField(pField);
                }
                //最后进行恢复处理
                pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 要素类添加字段
        /// </summary>
        /// <param name="featrueClass"></param>
        /// <param name="error"></param>
        /// <param name="fieldname"></param>
        /// <param name="fieldtype"></param>
        /// <param name="aliasname"></param>
        /// <param name="isNullable"></param>
        /// <returns></returns>
        public static bool AddField(ref IFeatureClass featrueClass, ref string error, string fieldname, esriFieldType fieldtype = esriFieldType.esriFieldTypeString, string aliasname = "", bool isNullable = true)
        {
            try
            {
                //在新增字段之前，先修改SchemaLock的状态
                ISchemaLock pSchemaLock = featrueClass as ISchemaLock;
                pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                //Add New Field
                int index = featrueClass.Fields.FindField(fieldname);
                if (index == -1)
                {
                    IField pField = new FieldClass();
                    IFieldEdit pFieldEdit = pField as IFieldEdit;
                    pFieldEdit.Name_2 = fieldname;
                    pFieldEdit.Type_2 = fieldtype;
                    pFieldEdit.IsNullable_2 = isNullable;
                    pFieldEdit.AliasName_2 = aliasname == "" ? fieldname : aliasname;
                    pFieldEdit.Editable_2 = true;
                    featrueClass.AddField(pField);
                }
                //最后进行恢复处理
                pSchemaLock.ChangeSchemaLock(esriSchemaLock.esriExclusiveSchemaLock);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }
        }
        #endregion

        #region =========  设置字段值  =========
        /// <summary>
        /// 统一设置要素类字段值
        /// </summary>
        /// <param name="featureClass"></param>
        /// <param name="field"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static bool SetFieldValue(ref IFeatureClass featureClass, string field, object value)
        {
            try
            {
                IFeatureCursor pCursor = featureClass.Update(null, false);
                IFeature pFeature = pCursor.NextFeature();
                while (pFeature != null)
                {
                    pFeature.set_Value(featureClass.Fields.FindField(field), value);
                    pCursor.UpdateFeature(pFeature); //将更新的内容保存
                    pFeature = pCursor.NextFeature();
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        
        /// <summary>
        /// 根据指定条件，设置字段值
        /// </summary>
        /// <param name="featureClass">要素类</param>
        /// <param name="field">字段名称</param>
        /// <param name="value">值</param>
        /// <param name="strWhereClause">条件语句</param>
        /// <returns></returns>
        public static bool SetFieldValue(ref IFeatureClass featureClass, string field, object value, string strWhereClause)
        {
            try
            {
                IFeatureCursor pCursor = featureClass.Update(null, false);
                List<IFeature> listFeature = QFeature.SearchFeature(ref featureClass, strWhereClause);
                IFeature pFeature = null;
                if (listFeature.Count > 0)
                {
                    for (int i = 0; i < listFeature.Count; i++)
                    {
                        pFeature = listFeature[i];
                        pFeature.set_Value(featureClass.Fields.FindField(field), value);
                        pFeature.Store();
                    }
                    return true;
                }
                else return false;
            }
            catch (Exception)
            {
                return false;
            }
        }
        #endregion
    }
}

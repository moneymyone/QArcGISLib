/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib
{
    /// <summary>
    /// 工作空间
    /// </summary>
    public class QWorkspace
    {
        #region 常规数据库操作
        /// <summary>
        /// 打开gdb mdb工作空间
        /// </summary>
        /// <param name="arg_DbPath">文件路径</param>
        /// <param name="arg_DbType">PersonGeodatabase:gdb,FileGeodatabase:mdb,Shapefile:shapefile</param>
        /// <param name="strErrInfo">错误信息</param>
        /// <returns></returns>
        public static IWorkspace OpenWorkSpace(string arg_DbPath, EGdbType arg_DbType, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                IWorkspaceFactory pFactory = null;
                if (arg_DbType == EGdbType.FileGeodatabase)
                {
                    pFactory = new FileGDBWorkspaceFactoryClass();
                }
                else if (arg_DbType == EGdbType.PersonGeodatabase)
                {
                    pFactory = new AccessWorkspaceFactory();
                }
                else if (arg_DbType == EGdbType.Shapefile)
                {
                    pFactory = new ShapefileWorkspaceFactoryClass();
                    arg_DbPath = System.IO.Path.GetDirectoryName(arg_DbPath);  //shapefile 必须读取文件夹路径
                }
                IWorkspace pWorkspace = pFactory.OpenFromFile(arg_DbPath, 0);
                if (pWorkspace != null)
                {
                    return pWorkspace;
                }
                else strErrInfo = "未获取到有效工作空间";
            }
            catch (Exception ex)
            {
                strErrInfo = "打开工作空间失败！" + ex.ToString();
            }
            return null;
        }


        /// <summary>
        /// 打开工作空间
        /// </summary>
        /// <param name="arg_DbPath">数据库完整路径</param>
        /// <param name="strErrInfo">错误信息</param>
        /// <returns></returns>
        public static IWorkspace OpenWorkSpace(string arg_DbPath, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if (string.IsNullOrEmpty(arg_DbPath))
                {
                    strErrInfo = "文件路径为空！";
                    return null;
                }

                EGdbType type = EGdbType.FileGeodatabase;
                if (!GetDatabaseType(arg_DbPath,ref type))
                {
                    strErrInfo = "数据类型暂不支持！";
                    return null;
                }

                return OpenWorkSpace(arg_DbPath, type, ref strErrInfo);
            }
            catch (Exception ex)
            {
                strErrInfo = "打开工作空间失败！" + ex.ToString();
            }
            return null;
        }
        #endregion

        #region 创建工作空间
        /// <summary>
        /// 创建工作空间,支持FileGeoDatabase,PersonalGeoDatabase,Shapefile
        /// </summary>
        /// <param name="strGdbPath"></param>
        /// <param name="strErrInfo"></param>
        /// <returns></returns>
        public static IWorkspace CreateWorkSpace(string strGdbPath, ref string strErrInfo)
        {
            strErrInfo = "";
            IWorkspace pWorkspace = null;
            try
            {
                if (string.IsNullOrEmpty(strGdbPath))
                {
                    strErrInfo = "文件路径为空！";
                    return null;
                }
                int iEGdbType = -1;
                IWorkspaceFactory pFactory = null;
                IWorkspaceName pWSName = null;
                string pFilePath = "", pFileName = "";
                pFileName = System.IO.Path.GetFileName(strGdbPath);
                pFilePath = System.IO.Path.GetDirectoryName(strGdbPath);
                if (!GetDatabaseType(strGdbPath,ref iEGdbType))
                {
                    strErrInfo = "数据类型暂不支持！";
                    return null;
                }

                if (iEGdbType == 0)
                {
                    pFactory = new FileGDBWorkspaceFactoryClass();
                    pWSName = pFactory.Create(pFilePath, pFileName, null, 0);
                    pWorkspace = pFactory.OpenFromFile(pWSName.PathName, 0);
                }
                else if (iEGdbType == 1)
                {
                    pFactory = new AccessWorkspaceFactory();
                    pWSName = pFactory.Create(pFilePath, pFileName, null, 0);
                    pWorkspace = pFactory.OpenFromFile(pWSName.PathName, 0);
                }
                else if (iEGdbType == 2)
                {
                    pFactory = new ShapefileWorkspaceFactoryClass();
                    pFileName = System.IO.Path.GetFileNameWithoutExtension(strGdbPath);  //shapefile 必须读取文件夹路径
                    pWorkspace = pFactory.OpenFromFile(pFilePath, 0);
                }

                if (pWorkspace != null)
                {
                    return pWorkspace;
                }
            }
            catch (Exception ex)
            {
                strErrInfo = "打开工作空间失败！" + ex.ToString();
            }
            return null;
        }
        #endregion

        #region 获取文件类型
        /// <summary>
        /// 获取文件类型
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        private static bool GetDatabaseType(string filepath, ref int type)
        {
            string sExt = System.IO.Path.GetExtension(filepath);
            if (sExt.Equals(".gdb", StringComparison.OrdinalIgnoreCase))
            {
                type = 0;
            }
            else if (sExt.Equals(".mdb", StringComparison.OrdinalIgnoreCase))
            {
                type = 1;
            }
            else if (sExt.Equals(".shp", StringComparison.OrdinalIgnoreCase))
            {
                type = 2;
            }
            else
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 获取数据库类型
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static bool GetDatabaseType(string filepath, ref EGdbType type)
        {
            string sExt = System.IO.Path.GetExtension(filepath);
            if (sExt.Equals(".gdb", StringComparison.OrdinalIgnoreCase))
            {
                type = EGdbType.FileGeodatabase;
            }
            else if (sExt.Equals(".mdb", StringComparison.OrdinalIgnoreCase))
            {
                type = EGdbType.PersonGeodatabase;
            }
            else if (sExt.Equals(".shp", StringComparison.OrdinalIgnoreCase))
            {
                type = EGdbType.Shapefile;
            }
            else
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}

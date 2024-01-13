/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib.SDE
{
    /// <summary>
    /// SDE工作空间
    /// </summary>
    public class QSDEWorkspace
    {
        private static IWorkspace m_sdeWorkspace = null; 

        /// <summary>
        /// 获取SDE工作空间
        /// </summary>
        public static IWorkspace SdeWorkspace
        {
            get { return m_sdeWorkspace; }
        }

        /// <summary>
        /// 打开oracle工作空间
        /// </summary>
        /// <param name="arg_server">服务器地址，如：192.168.34.152</param>
        /// <param name="arg_userName"></param>
        /// <param name="arg_passWord"></param>
        /// <param name="arg_error"></param>
        /// <param name="dbType">数据库类型</param>
        /// <param name="arg_dbName">数据库名称，为空时arg_server要加上数据库名称 /database </param>
        /// <returns></returns>
        public static IWorkspace OpenOracleSdeWorkspace(string arg_server, string arg_userName, string arg_passWord, ref string arg_error,EDatabaseType dbType = EDatabaseType.oracle11g, string arg_dbName = "")
        {
            IWorkspaceFactory pWorkspaceFactory;
            string Instance = string.Empty;
            if (dbType == EDatabaseType.oracle11g) 
                Instance = string.Format("sde:oracle11g:{0}", arg_server);
            else if (dbType == EDatabaseType.postgresql)
                Instance = string.Format("sde:postgresql:{0}", arg_server);
            else if (dbType == EDatabaseType.sqlserver)
                Instance = string.Format("sde:sqlserver:{0}", arg_server);
            try
            {
                IPropertySet pPropSet = new PropertySetClass();
                pPropSet.SetProperty("INSTANCE", Instance);
                pPropSet.SetProperty("USER", arg_userName);
                pPropSet.SetProperty("PASSWORD", arg_passWord);
                if (!string.IsNullOrEmpty(arg_dbName)) pPropSet.SetProperty("DATABASE", arg_dbName);
                pWorkspaceFactory = new SdeWorkspaceFactory();
                return OpenSdeWorkspace(pPropSet, ref arg_error);
            }
            catch (System.Exception ex)
            {
                arg_error = ex.Message.ToString() + "，请重新设置参数 ！";
                return null;
            }
        }

        /// <summary>
        /// 打开sde工作空间
        /// </summary>
        /// <param name="arg_Server">服务器地址，如：192.168.34.152</param>
        /// <param name="arg_Instance">连接实例，如sde:sqlserver:192.168.34.152</param>
        /// <param name="arg_userName">用户名</param>
        /// <param name="arg_passWord">密码</param>
        /// <param name="arg_database">要连接的数据库名称</param>
        /// <param name="arg_error"></param>
        /// <param name="arg_version">//数据库的拥有者也就是数据库的架构，一般是DBO或SDE</param>
        /// <returns></returns>
        public static IWorkspace OpenSdeWorkspace(string arg_Server, string arg_Instance, string arg_userName, string arg_passWord,string arg_database,  ref string arg_error, string arg_version = "sde.DEFAULT")
        {
            IWorkspaceFactory pWorkspaceFactory;
            try
            {
                IPropertySet pPropSet = new PropertySetClass();
                pPropSet.SetProperty("SERVER", arg_Server);
                pPropSet.SetProperty("INSTANCE", arg_Instance);
                pPropSet.SetProperty("USER", arg_userName);
                pPropSet.SetProperty("PASSWORD", arg_passWord);
                pPropSet.SetProperty("DATABASE", arg_database);
                pPropSet.SetProperty("VERSION", arg_version);
                pWorkspaceFactory = new SdeWorkspaceFactory();
                return OpenSdeWorkspace(pPropSet, ref arg_error);
            }
            catch (System.Exception ex)
            {
                arg_error = ex.Message.ToString() + "，请重新设置参数 ！";
                return null;
            }
        }

        /// <summary>
        /// 打开SDE工作空间
        /// </summary>
        /// <param name="pPropSet">属性设置</param>
        /// <param name="arg_error"></param>
        /// <returns></returns>
        public static IWorkspace OpenSdeWorkspace(IPropertySet pPropSet, ref string arg_error)
        {
            try
            {
                IWorkspaceFactory pWorkspaceFactory = new SdeWorkspaceFactory();
                m_sdeWorkspace = pWorkspaceFactory.Open(pPropSet, 0);
                if (m_sdeWorkspace != null)
                {
                    return m_sdeWorkspace;
                }
                else
                    return null;
            }
            catch (System.Exception ex)
            {
                if (ex.ToString().Contains("SDE") == true)
                    arg_error = "连接失败，SDE服务未启动,，请重新设置参数！";
                else if (ex.ToString().Contains("Bad login user") == true)
                    arg_error = "SDE用户名或密码错误，请重新设置参数！";
                else
                    arg_error = ex.Message.ToString() + "，请重新设置参数 ！";
                return null;
            }
        }

        /// <summary>
        /// 从sde文件打开工作空间
        /// </summary>
        /// <param name="connectionFile"></param>
        /// <param name="pWorkspace"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public static bool OpenSdeWorkspace(string connectionFile,out IWorkspace pWorkspace,out string sError)
        {
            sError = "";
            pWorkspace = null;
            if (!System.IO.File.Exists(connectionFile))
            {
                sError = $"sde文件不存在！{connectionFile}";
                return false;
            }
            try
            {
                IWorkspaceFactory workspaceFactory = new SdeWorkspaceFactoryClass();
                pWorkspace = workspaceFactory.OpenFromFile(connectionFile, 0);
                if (pWorkspace != null)
                {
                    m_sdeWorkspace = pWorkspace;
                    return true;
                }
                else
                    return false;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
            
        
        }

        /// <summary>
        /// 关闭SDE工作空间
        /// </summary>
        /// <returns></returns>
        public static bool CloseSDEWorkspace()
        {
            try
            {
                if (m_sdeWorkspace != null)
                {
                    while (m_sdeWorkspace != null && System.Runtime.InteropServices.Marshal.ReleaseComObject(SdeWorkspace) > 0) { }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}

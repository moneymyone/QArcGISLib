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

namespace QArcGISLib.SHP
{
    /// <summary>
    /// shp FeatureClass
    /// </summary>
    public class QSHP_FeatureClass
    {
        /// <summary>
        /// 读取 shapefile 要素类
        /// </summary>
        /// <param name="arg_SHPPath">shapefile文件路径</param>
        /// <param name="strErrInfo">错误信息</param>
        /// <returns></returns>
        public static IFeatureClass OpenShpFeatureClass(string arg_SHPPath, ref string strErrInfo)
        {
            strErrInfo = "";
            try
            {
                if (string.IsNullOrEmpty(arg_SHPPath) ||  !System.IO.File.Exists(arg_SHPPath))
                {
                    strErrInfo = string.Format("打开shp文件失败，文件不存在：{0}", arg_SHPPath);
                    return null;
                }
                string strFileName = System.IO.Path.GetFileName(arg_SHPPath);
                string strPath = System.IO.Path.GetDirectoryName(arg_SHPPath);
                IWorkspaceFactory pFactory = null;
                IFeatureWorkspace pFW = null;
                IFeatureClass pFC = null;
                pFactory = new ShapefileWorkspaceFactoryClass();
                pFW = pFactory.OpenFromFile(strPath, 0) as IFeatureWorkspace;

                if (pFW != null)
                {
                    pFC = pFW.OpenFeatureClass(strFileName);
                    return pFC;
                }
            }
            catch (Exception ex)
            {
                strErrInfo = "读取Shapefile要素类失败！" + ex.ToString();
            }
            return null;
        }
    }
}

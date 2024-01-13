/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib
{
    /// <summary>
    /// 释放对象类
    /// </summary>
    public class QRelease
    {
        /// <summary>
        /// 释放ESRI对象
        /// </summary>
        /// <param name="obj"></param>
        public static void ReleaseESRIObject(params object[] obj)
        {
            try
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    if (obj[i] != null)
                    {
                        while (obj[i] != null && System.Runtime.InteropServices.Marshal.ReleaseComObject(obj[i]) > 0) { }
                    }
                }
            }
            catch
            {

            }
        }
    }
}

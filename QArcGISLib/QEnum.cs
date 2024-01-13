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
    /// 坐标转换枚举值
    /// </summary>
    public enum ETransType
    {
        /// <summary>
        /// 未知
        /// </summary>
        UnKnown = 1,
        /// <summary>
        /// WGS84转火星坐标
        /// </summary>
        WGS84TOMARS = 2,
        /// <summary>
        /// WGS84转百度
        /// </summary>
        WGS84TOBAIDU = 3,
        /// <summary>
        /// 火星坐标转WGS84
        /// </summary>
        MARS2WGS84 = 4,
        /// <summary>
        /// 火星坐标转百度
        /// </summary>
        MARS2BAIDU = 5,
        /// <summary>
        /// 百度转WGS84
        /// </summary>
        BAIDU2WGS84 = 6,
        /// <summary>
        /// 百度转火星
        /// </summary>
        BAIDU2MARS = 7,
        /// <summary>
        /// NJ92转NJ2008
        /// </summary>
        NJ92NJ2008 = 8,
        /// <summary>
        /// BJ54转NJ92
        /// </summary>
        BJ54NJ92 = 9,
        /// <summary>
        /// NJ2008转NJ92
        /// </summary>
        NJ2008NJ92 = 10,
        /// <summary>
        /// WGS84转NJ92
        /// </summary>
        WGS84NJ92 = 11,
        /// <summary>
        /// XA80转NJ92
        /// </summary>
        XA80NJ92 = 12,
        /// <summary>
        /// NJ92转BJ54
        /// </summary>
        NJ92BJ54 = 13,
        /// <summary>
        /// NJ92转WGS84
        /// </summary>
        NJ92WGS84 = 14,
        /// <summary>
        /// NJ92转XA80
        /// </summary>
        NJ92XA80 = 15
    }

    /// <summary>
    /// 要素比较方法枚举值
    /// </summary>
    public enum EGeoCompareMethod
    {
        /// <summary>
        /// 相等
        /// </summary>
        Equals,
        /// <summary>
        /// 
        /// </summary>
        Touches,
        /// <summary>
        /// 
        /// </summary>
        Contains,
        /// <summary>
        /// 
        /// </summary>
        Within,
        /// <summary>
        /// 
        /// </summary>
        Disjoint,
        /// <summary>
        /// 
        /// </summary>
        Crosses,
        /// <summary>
        /// 
        /// </summary>
        Overlaps,
        /// <summary>
        /// 
        /// </summary>
        Relation
    }

    /// <summary>
    /// GDB数据库类型
    /// </summary>
    public enum EGdbType
    {
        /// <summary>
        /// 个人数据库
        /// </summary>
        PersonGeodatabase = 0,
        /// <summary>
        /// 文件数据库
        /// </summary>
        FileGeodatabase = 1,
        /// <summary>
        /// shapefile
        /// </summary>
        Shapefile =2
    }


    /// <summary>
    /// 坐标系统
    /// </summary>
    public enum ECoordSys
    {
        /// <summary>
        /// WGS84坐标
        /// </summary>
        WGS84 = 1,
        /// <summary>
        /// 火星坐标系
        /// </summary>
        MARS = 2,
        /// <summary>
        /// 百度09
        /// </summary>
        BAIDU = 3,
        /// <summary>
        /// 网络墨卡托
        /// </summary>
        WEBMERCATOR = 4
    }

    /// <summary>
    /// 数据库类型
    /// </summary>
    public enum EDatabaseType
    {
        /// <summary>
        /// oracle数据库
        /// </summary>
        oracle11g = 1,
        /// <summary>
        /// pgsql
        /// </summary>
        postgresql = 2,
        /// <summary>
        /// sqlserver
        /// </summary>
        sqlserver = 3
    }

    /// <summary>
    /// CAD数据类型
    /// </summary>
    public enum ECADDataType
    {
        /// <summary>
        /// 点
        /// </summary>
        Point = 1,
        /// <summary>
        /// 多段线
        /// </summary>
        Polyline = 2,
        /// <summary>
        /// 面
        /// </summary>
        Polygon = 3,
        /// <summary>
        /// 注记
        /// </summary>
        Annotation = 4,
        /// <summary>
        /// 
        /// </summary>
        MultiPathch
    }
}

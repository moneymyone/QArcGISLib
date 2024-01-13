/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.Geometry;
using NetTopologySuite.IO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib
{
    /// <summary>
    /// 转换
    /// </summary>
    public class QConvert
    {
        /// <summary>
        /// 转换图形为WKB
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static byte[] Geometry2WKB(IGeometry geometry)
        {
            geometry.SnapToSpatialReference();

            var oper = geometry as ITopologicalOperator2;
            if (oper != null) oper.Simplify();
            Debug.WriteLine(oper.IsSimple.ToString());
            var factory = new GeometryEnvironment() as IGeometryFactory3;
            var xxx = factory.CreateWkbVariantFromGeometry(geometry);
            var b = xxx as byte[];
            return b;
        }

        /// <summary>
        /// WKT转WKB
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        public static byte[] WKT2WKB(string wkt)
        {
            var writer = new WKBWriter();
            var reader = new WKTReader();
            return writer.Write(reader.Read(wkt));
        }

        /// <summary>
        /// WKB转WKT
        /// </summary>
        /// <param name="wkb"></param>
        /// <returns></returns>
        public static string WKB2WKT(byte[] wkb)
        {
            var writer = new WKTWriter();
            var reader = new WKBReader();
            return writer.Write(reader.Read(wkb));
        }

        /// <summary>
        /// Geometry转WKT
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static string Geometry2WKT(IGeometry geometry)
        {
            var b = Geometry2WKB(geometry);
            var reader = new WKBReader();
            var g = reader.Read(b);
            var writer = new WKTWriter();
            return writer.Write(g);
        }

        /// <summary>
        /// WKB转Geometry
        /// </summary>
        /// <param name="wkb"></param>
        /// <returns></returns>
        public static IGeometry WKB2Geometry(byte[] wkb)
        {
            IGeometry geom;
            var countin = wkb.GetLength(0);
            var factory = new GeometryEnvironment() as IGeometryFactory3;
            factory.CreateGeometryFromWkbVariant(wkb, out geom, out countin);
            return geom;
        }

        /// <summary>
        /// GeoAPI Geometry转ESRI Geometry
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static IGeometry GeoAPI2ESRI(GeoAPI.Geometries.IGeometry geometry)
        {
            var writer = new WKBWriter();
            var bytes = writer.Write(geometry);
            return WKB2Geometry(bytes);
        }

        /// <summary>
        /// ESRI Geometry 转 GeoAPI Geometry
        /// </summary>
        /// <param name="geometry"></param>
        /// <returns></returns>
        public static GeoAPI.Geometries.IGeometry ESRI2GeoAPI(IGeometry geometry)
        {
            try
            {
                var wkb = Geometry2WKB(geometry);
                var reader = new WKBReader();
                return reader.Read(wkb);
            }
            catch (Exception exception)
            {
                Debug.WriteLine(new StackTrace().GetFrame(0).GetMethod().Name + exception);
            }
            return null;
        }

        /// <summary>
        /// WKT转GeoAPI Geometry
        /// </summary>
        /// <param name="wkt"></param>
        /// <returns></returns>
        public static GeoAPI.Geometries.IGeometry WKT2GeoAPI(string wkt)
        {
            try
            {
                WKTReader reader = new WKTReader();
                GeoAPI.Geometries.IGeometry tempGeometry = reader.Read(wkt);
                return tempGeometry;
            }
            catch (Exception exception)
            {
                Debug.WriteLine(new StackTrace().GetFrame(0).GetMethod().Name + exception);
            }

            return null;
        }
    }
}

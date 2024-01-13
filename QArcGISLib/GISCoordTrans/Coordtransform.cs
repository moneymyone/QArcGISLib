/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace QArcGISLib.GISCoordTrans
{
    /// <summary>
    /// 坐标转换
    /// </summary>
    public class Coordtransform
    {
        //定义一些常量
        private const double x_PI = Math.PI * 3000.0 / 180.0;
        private const double PI = Math.PI;// π 3.1415926535897932384626
        private const double a = 6378245.0;// 长半轴
        private const double ee = 0.00669342162296594323;// 偏心率平方

        /// <summary>
        ///  百度坐标系 (BD-09) 与 火星坐标系 (GCJ-02)的转换
        ///  即 百度 转 谷歌、高德
        /// </summary>
        /// <param name="bd_lon"></param>
        /// <param name="bd_lat"></param>
        /// <returns></returns>
        public  double[] Bd09toGcj02(double bd_lon, double bd_lat)
        {
            double x = bd_lon - 0.0065;
            double y = bd_lat - 0.006;
            double z = Math.Sqrt(x * x + y * y) - 0.00002 * Math.Sin(y * x_PI);
            double theta = Math.Atan2(y, x) - 0.000003 * Math.Cos(x * x_PI);
            double gg_lng = z * Math.Cos(theta);
            double gg_lat = z * Math.Sin(theta);

            return new[] { gg_lng, gg_lat };
        }

        /// <summary>
        /// 火星坐标系 (GCJ-02) 与百度坐标系 (BD-09) 的转换
        /// 即谷歌、高德 转 百度
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public double[] Gcj02toBd09(double lng, double lat)
        {
            double z = Math.Sqrt(lng * lng + lat * lat) + 0.00002 * Math.Sin(lat * x_PI);
            double theta = Math.Atan2(lat, lng) + 0.000003 * Math.Cos(lng * x_PI);
            double bd_lng = z * Math.Cos(theta) + 0.0065;
            double bd_lat = z * Math.Sin(theta) + 0.006;

            return new[] { bd_lng, bd_lat };
        }

        /// <summary>
        /// WGS84转GCj02
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public double[] Wgs84toGcj02(double lng, double lat)
        {
            if (Out_of_china(lng, lat))
            {
                return new[] { lng, lat };
            }
            else
            {
                double dlat = TransformLatitude(lng - 105.0, lat - 35.0);
                double dlng = TransformLongitude(lng - 105.0, lat - 35.0);
                double radlat = lat / 180.0 * PI;
                double magic = Math.Sin(radlat);
                magic = 1 - ee * magic * magic;
                double sqrtmagic = Math.Sqrt(magic);
                dlat = (dlat * 180.0) / ((a * (1 - ee)) / (magic * sqrtmagic) * PI);
                dlng = (dlng * 180.0) / (a / sqrtmagic * Math.Cos(radlat) * PI);
                double mglat = lat + dlat;
                double mglng = lng + dlng;

                return new[] { mglng, mglat };
            }
        }

        /// <summary>
        ///  GCJ02 转换为 WGS84
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public double[] Gcj02toWGS84(double longitude, double latitude)
        {
            if (Out_of_china(longitude, latitude))
            {
                return new[] { longitude, latitude };
            }
            else
            {
                double dlat = TransformLatitude(longitude - 105.0, latitude - 35.0);
                double dlng = TransformLongitude(longitude - 105.0, latitude - 35.0);
                double radlat = latitude / 180.0 * PI;
                double magic = Math.Sin(radlat);
                magic = 1 - ee * magic * magic;
                double sqrtmagic = Math.Sqrt(magic);
                dlat = (dlat * 180.0) / ((a * (1 - ee)) / (magic * sqrtmagic) * PI);
                dlng = (dlng * 180.0) / (a / sqrtmagic * Math.Cos(radlat) * PI);
                double mglat = latitude + dlat;
                double mglng = longitude + dlng;

                return new[] { longitude * 2 - mglng, latitude * 2 - mglat };
            }
        }

        /// <summary>
        /// 高德坐标转换求纬度
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public double TransformLatitude(double longitude, double latitude)
        {
            double ret = -100.0 + 2.0 * longitude + 3.0 * latitude + 0.2 * latitude * latitude + 0.1 * longitude * latitude + 0.2 * Math.Sqrt(Math.Abs(longitude));
            ret += (20.0 * Math.Sin(6.0 * longitude * PI) + 20.0 * Math.Sin(2.0 * longitude * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(latitude * PI) + 40.0 * Math.Sin(latitude / 3.0 * PI)) * 2.0 / 3.0;
            ret += (160.0 * Math.Sin(latitude / 12.0 * PI) + 320 * Math.Sin(latitude * PI / 30.0)) * 2.0 / 3.0;

            return ret;
        }

        /// <summary>
        /// 高德坐标转换求经度
        /// </summary>
        /// <param name="longitude"></param>
        /// <param name="latitude"></param>
        /// <returns></returns>
        public double TransformLongitude(double longitude, double latitude)
        {
            double ret = 300.0 + longitude + 2.0 * latitude + 0.1 * longitude * longitude + 0.1 * longitude * latitude + 0.1 * Math.Sqrt(Math.Abs(longitude));
            ret += (20.0 * Math.Sin(6.0 * longitude * PI) + 20.0 * Math.Sin(2.0 * longitude * PI)) * 2.0 / 3.0;
            ret += (20.0 * Math.Sin(longitude * PI) + 40.0 * Math.Sin(longitude / 3.0 * PI)) * 2.0 / 3.0;
            ret += (150.0 * Math.Sin(longitude / 12.0 * PI) + 300.0 * Math.Sin(longitude / 30.0 * PI)) * 2.0 / 3.0;

            return ret;
        }

        /// <summary>
        /// 判断是否在国内，不在国内则不做偏移
        /// 纬度3.86~53.55,经度73.66~135.05 
        /// </summary>
        /// <param name="lng"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public bool Out_of_china(double lng, double lat)
        {
            return !(lng > 73.66 && lng < 135.05 && lat > 3.86 && lat < 53.55); ;
        }

        /// <summary>
        /// WGS84坐标系->百度坐标系
        /// </summary>
        /// <param name="lon"></param>
        /// <param name="lat"></param>
        /// <returns></returns>
        public  double[] Wgs84tobd09(double lon, double lat)
        {
            var gcj02 = Wgs84toGcj02(lon, lat);

            return Gcj02toBd09(gcj02[0], gcj02[1]);
        }

        /// <summary>
        /// 百度坐标系->WGS84坐标系
        /// </summary>
        /// <param name="bd_lon">百度经度</param>
        /// <param name="bd_lat">百度纬度</param>
        /// <returns></returns>
        public double[] Bd09towgs84(double bd_lon, double bd_lat)
        {
            var gcj02 = Bd09toGcj02(bd_lon, bd_lat);

            return Gcj02toWGS84(gcj02[0], gcj02[1]);
        }


        /// <summary>
        /// WGS84经纬度转web Mercator
        /// </summary>
        /// <param name="lon">经度</param>
        /// <param name="lat">纬度</param>
        /// <returns></returns>
        public double[] WGS84ToMercator(double lon, double lat)
        {
            double[] xy = new double[2];
            double x = lon * 20037508.342789 / 180.0;
            double y = Math.Log(Math.Tan((90.0 + lat) * PI / 360.0)) / (PI / 180.0);
            y = y * 20037508.34789 / 180.0;
            xy[0] = x;
            xy[1] = y;
            return xy;
        }

        /// <summary>
        /// web Mercator转WGS84经纬度
        /// </summary>
        /// <param name="mercatorX"></param>
        /// <param name="mercatorY"></param>
        /// <returns></returns>
        public double[] WebMercator2WGS84(double mercatorX, double mercatorY)
        {
            double[] xy = new double[2];
            double x = mercatorX / 20037508.34 * 180.0;
            double y = mercatorY / 20037508.34 * 180.0;
            y = 180 / PI * (2 * Math.Atan(Math.Exp(y * PI / 180.0)) - PI / 2.0);
            xy[0] = x;
            xy[1] = y;
            return xy;
        }
    }
}

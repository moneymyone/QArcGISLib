/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Geodatabase;
using GeoAPI.Geometries;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WW.Cad.IO;
using WW.Cad.Model;
using WW.Cad.Model.Entities;
using WW.Cad.Model.Tables;
using static WW.Cad.Model.DxfValueFormat;

namespace QArcGISLib.CAD
{
    /// <summary>
    /// cad操作
    /// </summary>
    public class QCAD
    {
        /// <summary>
        /// 打开CAD 工作空间
        /// </summary>
        /// <param name="sCADPath"></param>
        /// <param name="pWorkspace"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public static bool OpenWorkSpace(string sCADPath,ref IWorkspace pWorkspace,out string sError)
        {
            sError = "";
            if (string.IsNullOrEmpty(sCADPath) || !System.IO.File.Exists(sCADPath))
            {
                sError = string.Format("CAD文件不存在：{0}", sCADPath);
                return false;
            }

            pWorkspace = null;
            try
            {
                string sPath = System.IO.Path.GetDirectoryName(sCADPath);
                IWorkspaceFactory pWSF = new CadWorkspaceFactoryClass();
                pWorkspace = pWSF.OpenFromFile(sPath, 0);
                return pWorkspace == null;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 打开CAD要素工作空间
        /// </summary>
        /// <param name="sCADPath"></param>
        /// <param name="pFeatureWorkspace"></param>
        /// <param name="sError"></param>
        /// <returns></returns>
        public static bool OpenFeatureWorkSpace(string sCADPath, ref IFeatureWorkspace pFeatureWorkspace, out string sError)
        {
            sError = "";
            if (string.IsNullOrEmpty(sCADPath) || !System.IO.File.Exists(sCADPath))
            {
                sError = string.Format("CAD文件不存在：{0}", sCADPath);
                return false;
            }

            pFeatureWorkspace = null;
            try
            {
                string sPath = System.IO.Path.GetDirectoryName(sCADPath);
                IWorkspaceFactory pWSF = new CadWorkspaceFactoryClass();
                pFeatureWorkspace = pWSF.OpenFromFile(sPath, 0) as IFeatureWorkspace;
                return pFeatureWorkspace == null;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取要素类
        /// </summary>
        /// <param name="pWorkspace"></param>
        /// <param name="sCadPath"></param>
        /// <param name="dataType"></param>
        /// <param name="sError"></param>
        /// <param name="pFeatrueClass"></param>
        /// <returns></returns>
        public static bool ReadFeatureClass(ref IWorkspace pWorkspace,string sCadPath, ECADDataType dataType,
            out string sError,ref IFeatureClass pFeatrueClass)
        {
            sError = "";
            if(pWorkspace == null)
            {
                sError = "工作空间为空！";
                return false;
            }
            if (string.IsNullOrEmpty(sCadPath) || !System.IO.File.Exists(sCadPath) )
            {
                sError = $"CAD 文件不存在！{sCadPath}";
                return false;
            }
            string filename = System.IO.Path.GetFileName(sCadPath);
            IFeatureWorkspace pFeaWS = pWorkspace as IFeatureWorkspace;
            try
            {
                pFeatrueClass = pFeaWS.OpenFeatureClass($"{filename}:{dataType.ToString()}");
                return pFeatrueClass == null;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 读取cad
        /// </summary>
        /// <param name="sCadPath"></param>
        /// <param name="dataType"></param>
        /// <param name="sError"></param>
        /// <param name="pFeatrueClass"></param>
        /// <returns></returns>
        public static bool ReadFeatureClass(string sCadPath, ECADDataType dataType,
            out string sError, ref IFeatureClass pFeatrueClass)
        {
            sError = "";
            string filename = System.IO.Path.GetFileName(sCadPath);
            try
            {
                IFeatureWorkspace pFeaWS = null;
                if (!OpenFeatureWorkSpace(sCadPath,ref pFeaWS,out sError)) return false;
                pFeatrueClass = pFeaWS.OpenFeatureClass($"{filename}:{dataType.ToString()}");
                return pFeatrueClass == null;
            }
            catch (Exception ex)
            {
                sError = ex.Message;
                return false;
            }
        }

        /// <summary>
        /// 创建dwg模型
        /// </summary>
        /// <param name="DwgPath"></param>
        /// <param name="model"></param>
        /// <param name="appIdPeUrl"></param>
        /// <param name="error"></param>
        /// <param name="dxfVersion"></param>
        /// <returns></returns>
        public static bool CrateDwgModel(string DwgPath, out DxfModel model, out DxfAppId appIdPeUrl,out string error, DxfVersion dxfVersion = DxfVersion.Dxf21)
        {
            model = null;
            appIdPeUrl = null;
            error = "";
            try
            {
                if (File.Exists(DwgPath))
                {
                    model = DwgReader.Read(DwgPath);

                    if (model.AppIds.Keys.Contains("DX_INFO"))
                    {
                        appIdPeUrl = model.AppIds["DX_INFO"];
                    }
                    else
                    {
                        appIdPeUrl = new DxfAppId("DX_INFO");
                        model.AppIds.Add(appIdPeUrl);
                    }
                }
                else
                {
                    model = new DxfModel(DxfVersion.Dxf21);
                    model.Header.DrawingCodePage = DrawingCodePage.Gb2312;
                    appIdPeUrl = new DxfAppId("DX_INFO");
                    model.AppIds.Add(appIdPeUrl);
                }
                return true;
            }
            catch (Exception exception)
            {
                File.Decrypt(DwgPath);
                model = new DxfModel(DxfVersion.Dxf21);
                model.Header.DrawingCodePage = DrawingCodePage.Gb2312;
                appIdPeUrl = new DxfAppId("DX_INFO");
                model.AppIds.Add(appIdPeUrl);
                error = exception.Message;
                return false;
            }
        }

        /// <summary>
        /// 保存DWG模型
        /// </summary>
        /// <param name="DwgPath"></param>
        /// <param name="model"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool SaveDwgModel(string DwgPath, DxfModel model,out string error)
        {
            error = "";
            try
            {
                DwgWriter.Write(DwgPath, model);
                return true;
            }
            catch (Exception exception)
            {
                error = exception.Message;
                return false;
            }
        }

        /// <summary>
        /// 添加图层
        /// </summary>
        /// <param name="LayerName"></param>
        /// <param name="model"></param>
        /// <param name="dxfLayer"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool AddLayer(string LayerName, DxfModel model, out DxfLayer dxfLayer,out string error)
        {
            error = "";
            try
            {
                if (model.Layers.Keys.Contains(LayerName))
                {
                    dxfLayer = model.Layers[LayerName];
                }
                else
                {
                    dxfLayer = new DxfLayer(LayerName);
                    model.Layers.Add(dxfLayer);
                }
                return true;    
            }
            catch (Exception exception)
            {
                error = exception.Message;
                dxfLayer = null;
                return false;
            }
        }


        /// <summary>
        /// 空间对象和扩展属性添加到cadlib model中
        /// </summary>
        /// <param name="model">The model.</param>
        /// <param name="dxfLayer">The DXF layer.</param>
        /// <param name="SHAPE">The shape.</param>
        /// <param name="extendedData">The extended data.</param>
        /// <param name="cadlibfeatureinfoColor">Color of the cadlibfeatureinfo.</param>
        /// <param name="error">错误信息</param>
        public static bool Add2DwgByGeoExtendedData(DxfModel model, DxfLayer dxfLayer, string SHAPE, DxfExtendedData extendedData, EntityColor cadlibfeatureinfoColor,out string error)
        {
            error = "";
            var tempGeoAPI_Geometry = QConvert.WKT2GeoAPI(SHAPE);

            if (tempGeoAPI_Geometry == null)
            {
                error = "图形为空";
                return false;
            }

            try
            {
                if (tempGeoAPI_Geometry is IMultiLineString)
                {
                    #region 多线

                    var ddMultiLineString = (IMultiLineString)tempGeoAPI_Geometry;

                    foreach (var geometry in ddMultiLineString.Geometries)
                    {
                        var tempVertices = Coordinates2Vertices(geometry.Coordinates);
                        var p2dExteriorRing = new DxfLwPolyline
                        {
                            Color = cadlibfeatureinfoColor,
                            Closed = false,
                            Layer = dxfLayer
                        };
                        p2dExteriorRing.Vertices.AddRange(tempVertices);
                        if (extendedData != null)
                        {
                            p2dExteriorRing.ExtendedDataCollection.Add(extendedData);
                        }
                        model.Entities.Add(p2dExteriorRing);
                    }

                    #endregion
                }
                else if (tempGeoAPI_Geometry is ILineString)
                {
                    #region 简单线

                    var tempVertices = Coordinates2Vertices(tempGeoAPI_Geometry.Coordinates);
                    var p2dExteriorRing = new DxfLwPolyline
                    {
                        Color = cadlibfeatureinfoColor,
                        Closed = false,
                        Layer = dxfLayer
                    };
                    p2dExteriorRing.Vertices.AddRange(tempVertices);
                    if (extendedData != null)
                    {
                        p2dExteriorRing.ExtendedDataCollection.Add(extendedData);
                    }

                    model.Entities.Add(p2dExteriorRing);

                    #endregion
                }
                else if (tempGeoAPI_Geometry is IPolygon)
                {
                    #region 多边形

                    var xxxPolygon = (IPolygon)tempGeoAPI_Geometry;


                    #region 多边形外部节点

                    var ddVerticesExteriorRing = Coordinates2Vertices(xxxPolygon.ExteriorRing.Coordinates);
                    var p2dExteriorRing = new DxfLwPolyline
                    {
                        Color = cadlibfeatureinfoColor,
                        Closed = true,
                        Layer = dxfLayer
                    };
                    p2dExteriorRing.Vertices.AddRange(ddVerticesExteriorRing);



                    #endregion

                    #region 多边形内部节点

                    if (xxxPolygon.NumInteriorRings > 0)
                    {
                        DxfBlock block = new DxfBlock("TEST_BLOCK" + System.Guid.NewGuid().ToString("N"));
                        if (extendedData != null)
                        {
                            block.ExtendedDataCollection.Add(extendedData);
                        }

                        if (!model.Blocks.Contains(block))
                        {
                            model.Blocks.Add(block);
                        }
                        block.Entities.Add(p2dExteriorRing);
                        foreach (var xxxPolygonInteriorRing in xxxPolygon.InteriorRings)
                        {
                            var ddVerticesInteriorRing = Coordinates2Vertices(xxxPolygonInteriorRing.Coordinates);
                            var p2dInteriorRing = new DxfLwPolyline
                            {
                                Color = EntityColors.Red,
                                Closed = true,
                                Layer = dxfLayer
                            };
                            p2dInteriorRing.Vertices.AddRange(ddVerticesInteriorRing);
                            block.Entities.Add(p2dInteriorRing);

                        }
                        DxfInsert tempDxfInsert = new DxfInsert(block);
                        if (extendedData != null)
                        {
                            tempDxfInsert.ExtendedDataCollection.Add(extendedData);
                        }

                        model.Entities.Add(tempDxfInsert);
                    }
                    else
                    {
                        if (extendedData != null)
                        {
                            p2dExteriorRing.ExtendedDataCollection.Add(extendedData);
                        }

                        model.Entities.Add(p2dExteriorRing);

                    }
                    #endregion
                    #endregion
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
        /// 扩展属性字段添加值
        /// </summary>
        /// <param name="model"></param>
        /// <param name="ExtendedData"></param>
        /// <param name="cadlibfeatureinfoColor"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool AddText2Dwg(DxfModel model, DxfExtendedData ExtendedData, EntityColor cadlibfeatureinfoColor ,out string error)
        {
            error = "";

            double x = 1;
            double y = 1;
            double z = 0;
            double height = 1;
            string text = "";
            try
            {
                foreach (IExtendedDataValue extendedDataValue in ExtendedData.Values)
                {
                    var tempValue = extendedDataValue.ValueObject.ToString().Split(':');
                    if (tempValue.Length != 2) continue;
                    var fieldName = tempValue[0];
                    var fieldVale = tempValue[1];

                    if (fieldName == "设施名称")
                    {
                        text = fieldVale;
                    }
                    if (fieldName == "中心点_X" || fieldName == "中心点X")
                    {
                        x = Convert.ToDouble(fieldVale);
                    }

                    if (fieldName == "中心点_Y" || fieldName == "中心点Y")
                    {
                        y = Convert.ToDouble(fieldVale);
                    }   
                }

                var textStyle = new DxfTextStyle("MYSTYLE", "arial.ttf");
                model.TextStyles.Add(textStyle);
                var text3 = new DxfText(text, new WW.Math.Point3D(x, y, z), height)
                {
                    Thickness = 0.4d,
                    Style = textStyle,
                    Color = cadlibfeatureinfoColor
                };
                model.Entities.Add(text3);
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// 将GeoAPI节点转换为Cadlib节点
        /// </summary>
        /// <param name="Coordinates">The coordinates.</param>
        /// <returns>DxfLwPolyline.Vertex[].</returns>
        public static DxfLwPolyline.Vertex[] Coordinates2Vertices(Coordinate[] Coordinates)
        {
            var tempVertices = new DxfLwPolyline.Vertex[Coordinates.Length];
            for (var index = 0; index < Coordinates.Length; index++)
            {
                var coordinate = Coordinates[index];
                tempVertices[index] = new DxfLwPolyline.Vertex(coordinate.X, coordinate.Y);
            }
            return tempVertices;
        }

    }
}

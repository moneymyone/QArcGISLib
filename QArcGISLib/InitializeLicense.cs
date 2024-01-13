/******************************************************************************
 * Copyright (C), 2024, Randy.
 * All rights reserved.
 *
 *****************************************************************************/
using ESRI.ArcGIS.esriSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QArcGISLib
{
    public class InitializeLicense
    {
        private static bool bInitialized = false;
        public static bool IsInitialized
        {
            get { return bInitialized; }
        }
        public static void IniLicense()
        {
            try
            {
                ESRI.ArcGIS.RuntimeManager.Bind(ESRI.ArcGIS.ProductCode.EngineOrDesktop);
                AoInitialize aoi = new AoInitializeClass();
                //Additional license choices can be included here.
                esriLicenseProductCode productCode1 = esriLicenseProductCode.esriLicenseProductCodeEngineGeoDB;
                esriLicenseProductCode productCode2 = esriLicenseProductCode.esriLicenseProductCodeEngine;
                if (aoi.IsProductCodeAvailable(productCode1) == esriLicenseStatus.esriLicenseAvailable)
                {
                    aoi.Initialize(productCode1);
                }
                if (aoi.IsProductCodeAvailable(productCode2) == esriLicenseStatus.esriLicenseAvailable)
                {
                    aoi.Initialize(productCode2);
                }
                bInitialized = true;
            }
            catch
            {
                bInitialized = false;
            }
        }
    }
}

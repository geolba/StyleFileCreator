using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;
using StyleFileCreator.Internal;

namespace StyleFileCreator
{
    internal static class ArcMap
    {
        #region fields:

        private static ESRI.ArcGIS.Framework.IApplication m_app = null;
        private static IDocumentEvents_Event m_docEvent;

        #endregion

        public static ESRI.ArcGIS.Framework.IApplication Application
        {
            get
            {
                if (ArcMap.m_app == null)
                {
                    //var test = AddInStartupObject.GetHook<IMxApplication>();
                    ArcMap.m_app = (AddInStartupObject.GetHook<IMxApplication>() as ESRI.ArcGIS.Framework.IApplication);
                }
                return ArcMap.m_app;
            }
        }

        public static ESRI.ArcGIS.ArcMapUI.IMxDocument Document
        {
            get
            {
                ESRI.ArcGIS.ArcMapUI.IMxDocument result;
                if (ArcMap.Application != null)
                {
                    result = (ArcMap.Application.Document as ESRI.ArcGIS.ArcMapUI.IMxDocument);
                }
                else
                {
                    result = null;
                }
                return result;
            }
        }

        public static IMxApplication ThisApplication
        {
            get
            {
                return ArcMap.Application as IMxApplication;
            }
        }

        public static IDockableWindowManager DockableWindowManager
        {
            get
            {
                return ArcMap.Application as IDockableWindowManager;
            }
        }

        public static IDocumentEvents_Event Events
        {
            get
            {
                ArcMap.m_docEvent = (ArcMap.Document as IDocumentEvents_Event);
                return ArcMap.m_docEvent;
            }
        }
    }
}

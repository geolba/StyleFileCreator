using ESRI.ArcGIS.esriSystem;

namespace StyleFileCreator
{
    internal static class ThisAddIn
    {
        internal class IDs
        {
            internal static string MdbStyleFileCreatorButton
            {
                get
                {
                    return "Geologische_Bundesanstalt_StyleFileCreator_MdbStyleFileCreatorButton";
                }
            }
        }

        internal static string Name
        {
            get
            {
                return "StyleFileCreator";
            }
        }

        internal static string AddInID
        {
            get
            {
                return "{ed58afb7-2337-4a4e-b132-82dcba0e717a}";
            }
        }

        internal static string Company
        {
            get
            {
                return "Geologische Bundesanstalt";
            }
        }

        internal static string Version
        {
            get
            {
                return "1.5";
            }
        }

        internal static string Description
        {
            get
            {
                return "The \"MdbStyleFileCreator\" is atool for dumpung rgb- or hex-values to ESRI-FillSymbols.";
            }
        }

        internal static string Author
        {
            get
            {
                return "Arno Kaimbacher";
            }
        }

        internal static string Date
        {
            get
            {
                return "07.08.2017";
            }
        }

        internal static UID ToUID(this string id)
        {
            return new UIDClass
            {
                Value = id
            };
        }
    }
}

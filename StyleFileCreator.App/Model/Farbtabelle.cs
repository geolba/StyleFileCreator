using System;
using ESRI.ArcGIS.Display;
using System.Globalization;
using ESRI.ArcGIS.Framework;
using System.Reflection;
using System.Linq;
using System.Runtime.InteropServices;
using StyleFileCreator.App.Utils;
using System.ComponentModel.DataAnnotations;

namespace StyleFileCreator.App.Model
{
    public class Farbtabelle : ObservableModel
    {
        private int _id;
        private string _hexwert;
        private string _name;
        private string _tag;

        public Farbtabelle()
        { }

        //public int Id { get; set; }
        //public string Hexwert { get; set; }
        //public string Name { get; set; }
        //public string Tag { get; set; }
             
        [Key]
        public int Id
        {
            get { return _id; }
            set { base.SetProperty(ref _id, value, "Id"); }
        }
     
        [Required(ErrorMessage = "Das Attribut Hexwert muss in der Datenbank angegeben sein!")]
        //[RegularExpression("^((0x){0,1}|#{0,1})([0-9A-F]{8}|[0-9A-F]{6})$", ErrorMessage = "Invalid Hex Value")]
        [RegularExpression("^((0x){0,1}|#{0,1})([0-9A-F]{8}|[0-9A-F]{6}|[0-9a-f]{8}|[0-9a-f]{6})$", ErrorMessage = "Invalid Hex Value")]
        public string Hexwert
        {
            get { return _hexwert; }
            set 
            { 
                base.SetProperty<string>(ref this._hexwert, value, "Hexwert"); 
            }
        }
      
        [Required(ErrorMessage = "Das Attribut 'Name' muss in der Datenbank angegeben sein!")]
        [MaxLength(255, ErrorMessage = "Das Attribute 'Name' kann maximal nur 255 Zeichen enthalten.")]
        public string Name
        {
            get { return _name; }
            set
            {
                base.SetProperty<string>(ref this._name, value, "Name");
            }
        }

        //Tag is optional:            
        [MaxLength(255, ErrorMessage = "Das Attribute 'Tag' kann maximal nur 255 Zeichen enthalten.")]
        public string Tag
        {
            get { return _tag; }
            set
            {
                base.SetProperty<string>(ref this._tag, value, "Tag");
            }
        }

        public string ShortName
        {
            get 
            {
                string result;
                if (this.Name != null)
                {
                    int pos = Name.LastIndexOf("/") + 1;
                    result = this.Name.Substring(pos, Name.Length - pos);
                }
                else
                {
                    result = string.Empty;
                }
                return result;
            }
            set
            {
                this.ShortName = value;
            }
    
        }

        public IStyleGalleryItem GetStyleGalleryItem(string category, GeometryType geometrySelection, bool truncateName)
        {
            //IStyleGalleryClass styleGalleryClass = new ESRI.ArcGIS.Carto.FillSymbolStyleGalleryClass();
            Type factoryTypeDisplay = Type.GetTypeFromProgID("esriCarto.FillSymbolStyleGalleryClass");
            IStyleGalleryClass styleGalleryClass = (ESRI.ArcGIS.Carto.FillSymbolStyleGalleryClass)Activator.CreateInstance(factoryTypeDisplay);
            //stdole.IUnknown newObject = styleGalleryClass.get_NewObject("Fill Symbol") as stdole.IUnknown;
          
            ISymbol generalSymbol = null;
            if (geometrySelection == GeometryType.Fill)
            {
                //ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbol();
                ISimpleFillSymbol simpleFillSymbol = (SimpleFillSymbol)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("7914E603-C892-11D0-8BB6-080009EE4E41")));
                simpleFillSymbol.Color = this.BuildRgbOutOfHex();
                simpleFillSymbol.Outline = null;
                generalSymbol = (ISymbol)simpleFillSymbol;
            }
            else if (geometrySelection == GeometryType.Line)
            {
                //ISimpleLineSymbol simpleLineSymbol = new SimpleLineSymbol();
                ISimpleLineSymbol simpleLineSymbol = (SimpleLineSymbol)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("7914E5F9-C892-11D0-8BB6-080009EE4E41")));
                simpleLineSymbol.Color = this.BuildRgbOutOfHex();
                simpleLineSymbol.Style = esriSimpleLineStyle.esriSLSSolid;
                simpleLineSymbol.Width = 1;
                generalSymbol = (ISymbol)simpleLineSymbol;
            }
            else if (geometrySelection == GeometryType.Marker)
            {
                //ISimpleMarkerSymbol simpleMarkerSymbol = new SimpleMarkerSymbol();
                ISimpleMarkerSymbol simpleMarkerSymbol = (SimpleMarkerSymbol)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("7914E5FE-C892-11D0-8BB6-080009EE4E41")));
                simpleMarkerSymbol.Color = this.BuildRgbOutOfHex();
                simpleMarkerSymbol.Size = 3.5;
                generalSymbol = (ISymbol)simpleMarkerSymbol;
            }
            
            //Create a new style item using the object.
            //IStyleGalleryItem2 
            //ESRI.ArcGIS.Display.IStyleGalleryItem styleGalleryItem = new StyleGalleryItem();
            IStyleGalleryItem styleGalleryItem = (StyleGalleryItem)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("AC0E9829-91CB-11D1-8813-080009EC732A")));
            IStyleGalleryItem2 styleGalleryItem2 = styleGalleryItem as IStyleGalleryItem2;
             //styleGalleryItem2.Item = createFillSymbol == true ? (ISymbol)simpleFillSymbol : (ISymbol)simpleLineSymbol;// simpleFillSymbol;
            styleGalleryItem2.Item = generalSymbol;
            styleGalleryItem2.Name = (truncateName == true ? this.ShortName : this.Name);
            styleGalleryItem2.Category = category;
            styleGalleryItem2.Tags = this.Tag;  
            return styleGalleryItem2;
        }
        
        public static string[] GetTypePropertyNames(object classObject, BindingFlags bindingFlags)
        {
            //if (classObject == null)
            //{
            //    throw new ArgumentNullException(nameof(classObject));
            //}

            var type = classObject.GetType();
            var propertyInfos = type.GetProperties(bindingFlags);

            return propertyInfos.Select(propertyInfo => propertyInfo.Name).ToArray();
        }
        
        public System.Type GetByParameterName(string ParameterName)
        {
            try
            {
                return this.GetType().GetProperties().FirstOrDefault(x => x.Name == ParameterName).PropertyType;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        #region private methods

        private IColor BuildRgbOutOfHex()
        {
            //für IRgbColor ESRI.ArcGIS.esrisSystem hinzufügen:
            //IRgbColor rgbColor = new RgbColor();
            IRgbColor rgbColor = (RgbColor)Activator.CreateInstance(Marshal.GetTypeFromCLSID(new Guid("7EE9C496-D123-11D0-8383-080009B996CC")));
            //Remove # if present
            if (this.Hexwert.IndexOf('#') != -1)
            {
                this.Hexwert = this.Hexwert.Replace("#", "");
            }
            int red = 0;
            int green = 0;
            int blue = 0;
            if (this.Hexwert.Length == 6)
            {
                //#RRGGBB
                red = int.Parse(this.Hexwert.Substring(0, 2), NumberStyles.AllowHexSpecifier);
                green = int.Parse(this.Hexwert.Substring(2, 2), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(this.Hexwert.Substring(4, 2), NumberStyles.AllowHexSpecifier);
            }
            else if (this.Hexwert.Length == 3)
            {
                //#RGB
                red = int.Parse(this.Hexwert[0].ToString() + this.Hexwert[0].ToString(), NumberStyles.AllowHexSpecifier);
                green = int.Parse(this.Hexwert[1].ToString() + this.Hexwert[1].ToString(), NumberStyles.AllowHexSpecifier);
                blue = int.Parse(this.Hexwert[2].ToString() + this.Hexwert[2].ToString(), NumberStyles.AllowHexSpecifier);
            }
            rgbColor.Red = red;
            rgbColor.Green = green;
            rgbColor.Blue = blue;
            return rgbColor;
        }

        #endregion

    }

}

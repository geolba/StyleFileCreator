using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StyleFileCreator.App.Model;
using StyleFileCreator.App.Resources;

namespace StyleFileCreator.App.ViewModel
{
    public class GeometryTypeViewModel : ViewVM
    {
        #region Fields

        ReadOnlyCollection<OptionViewModel<GeometryType>> _availableGeometryTypes;

        #endregion // Fields

        public GeometryTypeViewModel(FormularData formData)
            : base(formData)
        {            
            //_formData = formData;
        }

        #region properties

        public override string DisplayName
        {
            get
            {
                return Strings.PageDisplayName_GeometryType;// "Konfigurationen";
            }
        }

        #region AvailableDrinkSizes
        /// <summary>
        /// Returns a read-only collection of all drink sizes that the user can select.
        /// </summary>
        public ReadOnlyCollection<OptionViewModel<GeometryType>> AvailableGeometryTypes
        {
            get
            {
                if (_availableGeometryTypes == null)
                {
                    this.CreateAvailableDrinkSizes();
                }

                return _availableGeometryTypes;
            }
        }

        private void CreateAvailableDrinkSizes()
        {
            List<OptionViewModel<GeometryType>> list = new List<OptionViewModel<GeometryType>>();
            list.Add(new OptionViewModel<GeometryType>(Strings.GeometryType_Fill, GeometryType.Fill, 0));
            list.Add(new OptionViewModel<GeometryType>(Strings.GeometryType_Line, GeometryType.Line, 1));
            list.Add(new OptionViewModel<GeometryType>(Strings.GeometryType_Marker, GeometryType.Marker, 2));

            foreach (OptionViewModel<GeometryType> option in list)
            {
                option.PropertyChanged += this.OnGeometryTypeOptionPropertyChanged;
                //option.PropertyChanged += new PropertyChangedEventHandler(this.OnGeometryTypeOptionPropertyChanged);
            }

            list.Sort();

            _availableGeometryTypes = new ReadOnlyCollection<OptionViewModel<GeometryType>>(list);
        }

        private void OnGeometryTypeOptionPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            OptionViewModel<GeometryType> option = sender as OptionViewModel<GeometryType>;
            if (option.IsSelected)
            {
                this.FormData.GeometryType = option.GetValue();
            }
            //else
            //{
            //    this.FormData.GeometryType = null;
            //}
        }

        #endregion // AvailableDrinkSizes

        #endregion // Properties

        internal override bool IsValid()
        {
            // The wizard can navigate to the next page once the user has selected the required value.
            return base.FormData.GeometryType.HasValue;
        }

        internal override void Reset()
        {           
            CreateAvailableDrinkSizes();
            base.FormData.GeometryType = null;
           
        }

        public override void Cleanup()
        {         
            base.Cleanup();
        }

    }
}

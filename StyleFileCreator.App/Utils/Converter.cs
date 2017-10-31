using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Windows;
using System.Collections.ObjectModel;
using StyleFileCreator.App.Model;

namespace StyleFileCreator.App.Utils
{


    public class AnalysenConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null)
            {
                return 0;
            }
            else
            {
                var colorTableList = (ObservableCollection<Farbtabelle>)value;
                if (parameter.ToString() == "IsNotValid")
                {
                    //return List.Where(a_vm => a_vm.IsDeleted == true).Count();
                    var deleted = colorTableList
                        .Where(colort => colort.IsValid() == false)                     
                       .ToList();
                    return deleted.Count;
                }
                if (parameter.ToString() == "IsValid")
                {                    
                    var valid = colorTableList
                        .Where(colort => colort.IsValid() == true)                       
                       .ToList();
                    return valid.Count;
                }
             
                return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility)
            {
                Visibility vis = (Visibility)value;
                if (vis == Visibility.Collapsed)
                    return true;
            }
            return false;
        }
    }

    public class BooleanToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (parameter != null && parameter.ToString() == "negate")
            {
                if (value is Boolean && (bool)value == true)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
            else
            {
                if (value is Boolean && (bool)value == true)
                {
                    return Visibility.Visible;
                }
                return Visibility.Collapsed;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is Visibility && (Visibility)value == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }

}
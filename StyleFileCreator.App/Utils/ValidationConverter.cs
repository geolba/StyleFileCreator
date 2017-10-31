using System;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;

namespace StyleFileCreator.App.Utils
{
    public class ValidationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ReadOnlyObservableCollection<ValidationError> errors = value as ReadOnlyObservableCollection<ValidationError>;
            if (errors == null) return value;
            if (errors.Count > 0)
            {
                return errors[0].ErrorContent;
            }
            return "";
        }


        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (string.IsNullOrEmpty(value.ToString()))
            {
                return null;
            }

            return value;
        }
    }
}

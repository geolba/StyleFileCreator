﻿using System;
using System.Windows.Data;

namespace StyleFileCreator.App.Utils
{
    [ValueConversion(typeof(Enum), typeof(bool))]
    public class EnumToBoolConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            string enumValue = value.ToString();
            string targetValue = parameter.ToString();
            bool outputValue = enumValue.Equals(targetValue, StringComparison.InvariantCultureIgnoreCase);
            return outputValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value == null || parameter == null) return null;
            bool useValue = (bool)value;
            string targetValue = parameter.ToString();
            if (useValue) return Enum.Parse(targetType, targetValue);
            return null;
        }

        #endregion
    }
    

}

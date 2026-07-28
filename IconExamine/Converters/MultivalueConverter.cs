using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace FontExamine.Converters
{
    public class MultiParameterConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return values.Clone(); // Combine parameters into an array
        }
        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}

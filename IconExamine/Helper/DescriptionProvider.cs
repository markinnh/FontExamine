using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Data;

namespace FontExamine.Helper
{
    public class DescriptionProvider:IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if(value is Enum item)
            {
                var descriptionAttribute = item.GetType().GetField(item.ToString())
                    .GetCustomAttributes(typeof(System.ComponentModel.DescriptionAttribute), false)
                    as System.ComponentModel.DescriptionAttribute[];
                if (descriptionAttribute != null && descriptionAttribute.Length > 0)
                {
                    return descriptionAttribute[0].Description;
                }
                else
                {
                    return item.ToString();
                }
            }
            return value?.ToString() ?? string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }
    }
}

using System;
using System.Globalization;
using System.Windows.Data;

public class ByteToStringConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is byte byteValue) {
            return byteValue.ToString();
        }
        return string.Empty;
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is string strValue && byte.TryParse(strValue, out byte byteValue)) {
            return byteValue;
        }
        return 0;
    }
}
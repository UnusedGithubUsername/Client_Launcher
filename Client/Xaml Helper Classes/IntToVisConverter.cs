using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Client {
    public class IntToVisConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if(parameter == null)
                return (int)value == -1 ? Visibility.Hidden : Visibility.Visible;//could be prettier

            if (Int32.Parse((string)parameter)==0)
                return (int)value == 0 ? Visibility.Hidden : Visibility.Visible;

            return (int)value == -1 ? Visibility.Hidden : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

}

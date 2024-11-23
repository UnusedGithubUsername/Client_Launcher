using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Client {
    public class IntToVisConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            Visibility vis = Visibility.Collapsed;
            if(parameter == null)
                vis = (int)value == -1 ? Visibility.Hidden : Visibility.Visible;//could be prettier
            
            if(parameter != null)
                if (Int32.Parse((string)parameter)==0)
                    vis = (int)value == 0 ? Visibility.Hidden : Visibility.Visible;

            if(vis == Visibility.Collapsed)
                vis = (int)value == -1 ? Visibility.Hidden : Visibility.Visible;

            return vis;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }

}

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data; 
using Client.Models;

namespace Client.Xaml_Helper_Classes {
    public class IsDecreasableConverter : IValueConverter {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {

            if (value == null)
                return Visibility.Hidden;

            CharacterStat_Model CurrentUIStats = (CharacterStat_Model)value;//wpf does not naturally support bindings as parameters, so we are gonne do a this thing hardcoded...

            int availablePoints = CurrentUIStats.DecreasableStatpoints;
            int statIncrease = CurrentUIStats.CurrentStatDelta[Int32.Parse((string)parameter)];

            if ((int)statIncrease > 0)//if the stat is increased, it can always be decreased,
                return Visibility.Visible;

            return availablePoints > 0 ? Visibility.Visible: Visibility.Hidden; //if available decreasor-points are 0, decreasing sis hidden
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
            throw new NotImplementedException();
        }
    }
}

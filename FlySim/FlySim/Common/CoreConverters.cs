using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace FlySim.Common
{  

    public class MilesDisplayLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double distance = System.Convert.ToDouble(value);

            return (Math.Abs(distance) == 0.0) ? "--" : $"{System.Convert.ToDouble(value):N0} miles";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public sealed class StatusToStrokeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((FlightStatus)value == FlightStatus.Ok) ? App.Current.Resources["AppStatusOkStrokeBrush"] as SolidColorBrush : App.Current.Resources["AppStatusAtRiskStrokeBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }

    public sealed class StatusToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((FlightStatus)value == FlightStatus.Ok) ? App.Current.Resources["AppStatusOkFillBrush"] as SolidColorBrush : App.Current.Resources["AppStatusAtRiskFillBrush"] as SolidColorBrush;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }

    public sealed class AirspeedLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double speed = System.Convert.ToDouble(value);

            return (Math.Abs(speed) == 0.0) ? "--" : $"{speed:N0} mph";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }

    public sealed class HeadingLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double heading = System.Convert.ToDouble(value);

            return (Math.Abs(heading) == 0.0) ? "0° N" : $"{heading:N0}° {heading.AsDirectionLabel().GetSuffix()}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }

    public sealed class HumidityLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double humidity = System.Convert.ToDouble(value);

            return (Math.Abs(humidity) == 0.0) ? "--" : $"{humidity:N0}%";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }

    public sealed class TemperatureLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double temperature = System.Convert.ToDouble(value);

            return (Math.Abs(temperature) == 0.0) ? "--" : $"{((temperature * 9 / 5) + 32):N1} ° F";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }

    public sealed class LocalTimeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var time = (DateTime)value;

            return (time.ToLocalTime().ToString("hh:mm:ss"));
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return System.Convert.ToBoolean(value) ? parameter : null;
        }
    }

    public class ObjectTooltipLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Windows.Devices.Geolocation.Geopoint position = (Windows.Devices.Geolocation.Geopoint)value;

            return $"{System.Convert.ToDouble(position.Position.Altitude):N0} ft";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class NumberDisplayLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            double number = System.Convert.ToDouble(value);

            return (Math.Abs(number) == 0.0) ? "--" : $"{System.Convert.ToDouble(value):N0} ft";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PaddedNumberDisplayLabelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return $"{((int)(value)):00#}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

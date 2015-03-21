using System;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MASACore.Core.Converters
{
    public class IntegerStringToLeftMarginConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int[] parameterValue = new int[] { 0, 0, 0, 0 };

            if (!String.IsNullOrWhiteSpace(parameter as String))
            {
                String parameterString = parameter as String;
                parameterValue = parameterString.Split(new char[] {','}).Select(int.Parse).ToArray();
            }

            int leftMarginValue = Math.Abs(Math.Min((int)value, 7)) * 50 + 5;
            parameterValue[0] = leftMarginValue;
            Thickness margin = new Thickness(parameterValue[0], parameterValue[1], parameterValue[2], parameterValue[3]);
            return margin;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

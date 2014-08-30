using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace HushWindowsPhoneLibrary.Views.Converters
{
    public class StringCaseConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {

            var parameterValue = parameter as String;
            bool toLower = false;
            bool toUpper = false;
            
            if (parameterValue != null)
            {
                toLower = parameterValue.ToLower().Equals("lower");
                toUpper = parameterValue.ToLower().Equals("upper");
            } 

            var inputValue = value as String;
            String returnValue = inputValue;
            if (!String.IsNullOrWhiteSpace(inputValue))
            {
                if (toLower)
                {
                    returnValue = inputValue.ToLower();
                }
                else if (toUpper)
                {
                    returnValue = inputValue.ToUpper();
                }
            }

            return returnValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

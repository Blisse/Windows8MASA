using System;
using System.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MASACore.Core.Converters
{
    public class ObjectToVisibilityConverter : IValueConverter
    {
        private const String NegateString = "negate";

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            // value
            Visibility returnVisibility;
            if (value != null)
            {
                if (value is IEnumerable)
                {
                    var enumerable = value as IList;
                    if (enumerable != null)
                    {
                        if (enumerable.Count > 0)
                        {
                            returnVisibility = Visibility.Visible;
                        }
                        else
                        {
                            returnVisibility = Visibility.Collapsed;
                        }
                    }
                    else
                    {
                        returnVisibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    returnVisibility = Visibility.Visible;   
                }
            }
            else
            {
                returnVisibility = Visibility.Collapsed;
            }

            // parameter
            String negateParameter = parameter as String;
            Boolean negate = negateParameter == NegateString;

            // negation
            if (negate)
            {
                if (returnVisibility == Visibility.Visible)
                {
                    returnVisibility = Visibility.Collapsed;
                }
                else
                {
                    returnVisibility = Visibility.Visible;
                }
            }

            return returnVisibility;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

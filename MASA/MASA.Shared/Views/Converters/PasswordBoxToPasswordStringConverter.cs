using System;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace HushWindowsPhoneLibrary.Views.Converters
{
    public class PasswordBoxToPasswordStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            PasswordBox passwordBox = (PasswordBox) value;
            return passwordBox.Password;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}

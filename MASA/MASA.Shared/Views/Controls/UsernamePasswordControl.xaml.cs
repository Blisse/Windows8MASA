using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using GalaSoft.MvvmLight.Ioc;
using MASA.ViewModels.Controls;

namespace MASA.Views.Controls
{
    public sealed partial class UsernamePasswordControl : UserControl
    {
        public static readonly DependencyProperty IdProperty = DependencyProperty.Register("Id", typeof (String),
            typeof (UsernamePasswordControl), new PropertyMetadata(String.Empty, IdPropertyChanged));

        private static void IdPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            ((UsernamePasswordControl) dependencyObject).IdPropertyChanged(dependencyPropertyChangedEventArgs);
        }

        private void IdPropertyChanged(DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            String oldId = (String) dependencyPropertyChangedEventArgs.OldValue;
            String newId = (String) dependencyPropertyChangedEventArgs.NewValue;

            if (SimpleIoc.Default.IsRegistered<UsernamePasswordControlViewModel>(oldId))
            {
                SimpleIoc.Default.Unregister(oldId);
            }
            if (!SimpleIoc.Default.IsRegistered<UsernamePasswordControlViewModel>(newId))
            {
                SimpleIoc.Default.Register(() => UsernamePasswordControlViewModel, newId);
            }
        }

        public String Id
        {
            get { return (String) GetValue(IdProperty); }
            set { SetValue(IdProperty, value); }
        }

        private UsernamePasswordControlViewModel _usernamePasswordControlViewModel = new UsernamePasswordControlViewModel();
        public UsernamePasswordControlViewModel UsernamePasswordControlViewModel
        {
            get { return _usernamePasswordControlViewModel; }
            set { _usernamePasswordControlViewModel = value; }
        }

        public UsernamePasswordControl()
        {
            InitializeComponent();

            Loaded += (sender, args) =>
            {
                if (!SimpleIoc.Default.IsRegistered<UsernamePasswordControlViewModel>(Id))
                {
                    SimpleIoc.Default.Register(() => UsernamePasswordControlViewModel, Id);   
                }
            };

            Unloaded += (sender, args) =>
            {
                if (SimpleIoc.Default.IsRegistered<UsernamePasswordControlViewModel>(Id))
                {
                    SimpleIoc.Default.Unregister(Id);
                }
            };
        }
    }
}

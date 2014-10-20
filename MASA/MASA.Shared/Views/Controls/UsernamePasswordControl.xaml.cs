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
        private String _id;
        public String Id
        {
            get { return _id; }
            set { _id = value; }
        }

        private UsernamePasswordControlViewModel _controlViewModel = new UsernamePasswordControlViewModel();
        public UsernamePasswordControlViewModel ControlViewModel
        {
            get { return _controlViewModel; }
            set { _controlViewModel = value; }
        }

        public UsernamePasswordControl()
        {
            InitializeComponent();
            ControlViewModel = new UsernamePasswordControlViewModel();

            Loaded += (sender, args) =>
            {
                SimpleIoc.Default.Register(() => ControlViewModel, Id);
            };

            Unloaded += (sender, args) =>
            {
                SimpleIoc.Default.Unregister(Id);
            };
        }
    }
}

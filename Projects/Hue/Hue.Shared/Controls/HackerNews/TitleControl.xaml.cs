using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace MASA.Views.Controls.HackerNews
{
    public sealed partial class TitleControl : UserControl
    {
        private static readonly DependencyProperty PageNameProperty = DependencyProperty.Register("PageName", typeof (String),
            typeof (TitleControl), new PropertyMetadata("News"));

        public String PageName
        {
            get { return (String) GetValue(PageNameProperty); }
            set { SetValue(PageNameProperty, value); }
        }

        public TitleControl()
        {
            this.InitializeComponent();
        }
    }
}

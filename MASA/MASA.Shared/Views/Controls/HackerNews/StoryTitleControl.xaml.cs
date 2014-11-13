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
using GalaSoft.MvvmLight.Command;
using MASA.DataModel.HackerNews;
using MASA.ViewModels.Pages.HackerNews;

namespace MASA.Views.Controls.HackerNews
{
    public sealed partial class StoryTitleControl : UserControl
    {
        private static readonly DependencyProperty StoryProperty = DependencyProperty.Register("Story", typeof(StoryViewModel),
            typeof(StoryTitleControl), new PropertyMetadata(null));

        public StoryViewModel Story
        {
            get { return (StoryViewModel)GetValue(StoryProperty); }
            set { SetValue(StoryProperty, value); }
        }

        private static readonly DependencyProperty CommentCommandProperty = DependencyProperty.Register(
            "CommentCommand", typeof(RelayCommand<StoryViewModel>),
            typeof (StoryTitleControl), new PropertyMetadata(null));

        public RelayCommand<StoryViewModel> CommentCommand
        {
            get { return (RelayCommand<StoryViewModel>)GetValue(CommentCommandProperty); }
            set { SetValue(CommentCommandProperty, value); }
        }

        private static readonly DependencyProperty CommentCommandParameterProperty =
            DependencyProperty.Register("CommentCommandParameter", typeof(StoryViewModel),
                typeof (StoryTitleControl), new PropertyMetadata(null));

        public StoryViewModel CommentCommandParameter
        {
            get { return (StoryViewModel)GetValue(CommentCommandParameterProperty); }
            set { SetValue(CommentCommandParameterProperty, value); }
        }

        public StoryTitleControl()
        {
            this.InitializeComponent();
        }
    }
}

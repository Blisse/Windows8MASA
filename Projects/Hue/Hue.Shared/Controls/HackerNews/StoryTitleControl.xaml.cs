using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236
using GalaSoft.MvvmLight.Command;
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

        private static readonly DependencyProperty ShowCommentsProperty = DependencyProperty.Register("ShowComments", typeof(StoryViewModel),
            typeof(Boolean), new PropertyMetadata(true, ShowCommentsPropertyChanged));

        private static void ShowCommentsPropertyChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var storyTitleControl = dependencyObject as StoryTitleControl;
            if (storyTitleControl != null)
            {
                storyTitleControl.ShowCommentsChanged((bool)dependencyPropertyChangedEventArgs.OldValue,
                    (bool)dependencyPropertyChangedEventArgs.NewValue);
            }
        }

        private void ShowCommentsChanged(Boolean oldValue, Boolean newValue)
        {
            if (newValue == false)
            {
                Delimiter.Visibility = Visibility.Collapsed;
                CommentsLink.Visibility = Visibility.Collapsed;
            }
            else
            {
                Delimiter.Visibility = Visibility.Visible;
                CommentsLink.Visibility = Visibility.Visible;
            }
        }

        public Boolean ShowComments
        {
            get { return (Boolean)GetValue(ShowCommentsProperty); }
            set { SetValue(ShowCommentsProperty, value); }
        }

        public StoryTitleControl()
        {
            this.InitializeComponent();
        }
    }
}

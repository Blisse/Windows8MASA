using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MASA.Views.Pages.HackerNews
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StoryPage : BaseViewModelPage
    {
        public StoryPage()
        {
            this.InitializeComponent();
        }

        private void WebView_OnFrameNavigationStarting(WebView sender, WebViewNavigationStartingEventArgs args)
        {
            CurrentPageViewModel.IsLoading = true;
        }

        private void WebView_OnFrameNavigationCompleted(WebView sender, WebViewNavigationCompletedEventArgs args)
        {
            CurrentPageViewModel.IsLoading = false;
        }

        private void WebView_OnFrameContentLoading(WebView sender, WebViewContentLoadingEventArgs args)
        {
            CurrentPageViewModel.IsLoading = true;
        }

        private void WebView_OnFrameDOMContentLoaded(WebView sender, WebViewDOMContentLoadedEventArgs args)
        {
            CurrentPageViewModel.IsLoading = true;
        }
    }
}

/*
  In App.xaml:
  <Application.Resources>
      <vm:Locator xmlns:vm="clr-namespace:HushWP8"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/


namespace MASA.ViewModels.Locator
{

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Ioc;
    using Services.Implementations;
    using Services.Implementations.HackerNews;
    using Services.Interfaces;
    using Pages;
    using Pages.HackerNews;

    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
    {
        /// <summary>
        /// Initializes a new instance of the Locator class.
        /// </summary>
        public ViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
;
            }

            SimpleIoc.Default.Register<IStorageService, StorageService>();
            SimpleIoc.Default.Register<IHackerNewsService, HackerNewsService>();

            SimpleIoc.Default.Register<LogInPageViewModel>();
            SimpleIoc.Default.Register<RegisterPageViewModel>();

            SimpleIoc.Default.Register<NewsPageViewModel>();
            SimpleIoc.Default.Register<StoryPageViewModel>();
            SimpleIoc.Default.Register<CommentsPageViewModel>();
        }

        public LogInPageViewModel LogInPage
        {
            get { return SimpleIoc.Default.GetInstance<LogInPageViewModel>(); }
        }

        public RegisterPageViewModel RegisterPage
        {
            get { return SimpleIoc.Default.GetInstance<RegisterPageViewModel>(); }
        }

        public NewsPageViewModel NewsPage
        {
            get { return SimpleIoc.Default.GetInstance<NewsPageViewModel>(); }
        }

        public StoryPageViewModel StoryPage
        {
            get { return SimpleIoc.Default.GetInstance<StoryPageViewModel>(); }
        }

        public CommentsPageViewModel CommentsPage
        {
            get { return SimpleIoc.Default.GetInstance<CommentsPageViewModel>(); }
        }
        
        public static void Cleanup()
        {
            SimpleIoc.Default.Unregister<IHackerNewsService>();
            SimpleIoc.Default.Unregister<IStorageService>();
            SimpleIoc.Default.Unregister<LogInPageViewModel>();
            SimpleIoc.Default.Unregister<RegisterPageViewModel>();
            SimpleIoc.Default.Unregister<NewsPageViewModel>();
            SimpleIoc.Default.Unregister<StoryPageViewModel>();
            SimpleIoc.Default.Unregister<CommentsPageViewModel>();
        }
    }
}
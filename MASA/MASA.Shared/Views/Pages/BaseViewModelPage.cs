using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;
using MASA.Common.LifeCycle;
using MASA.Services.Interfaces;
using MASA.ViewModels.Pages;

namespace MASA.Views.Pages
{
    public abstract class BaseViewModelPage : Page, IDialogService, IUiDispatchService, INavigationService
    {
        protected CoreDispatcher UiDispatcher;

        protected Boolean IsDialogOpen;

        public static readonly Dictionary<Type, Type> PageDictionary;

        private readonly NavigationHelper _navigationHelper;

        static BaseViewModelPage()
        {
            PageDictionary = new Dictionary<Type, Type>();
        }

        protected BaseViewModelPage()
        {
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
            
            NavigationCacheMode = NavigationCacheMode.Required;
        }

        public NavigationHelper NavigationHelper
        {
            get { return _navigationHelper; }
        }

        protected ResourceLoader CurrentResourceLoader
        {
            get { return ResourceLoader.GetForCurrentView("Resources"); }
        }

        protected BasePageViewModel CurrentPageViewModel
        {
            get { return DataContext as BasePageViewModel; }
        }

        #region Navigation Registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UiDispatcher = Dispatcher;
            SimpleIoc.Default.Register<INavigationService>(() => this);
            SimpleIoc.Default.Register<IUiDispatchService>(() => this);
            SimpleIoc.Default.Register<IDialogService>(() => this);
            NavigationHelper.OnNavigatedTo(e);
        }
 
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            NavigationHelper.OnNavigatedFrom(e);
            SimpleIoc.Default.Unregister<INavigationService>();
            SimpleIoc.Default.Unregister<IUiDispatchService>();
            SimpleIoc.Default.Unregister<IDialogService>();
            UiDispatcher = null;
        }

        protected virtual void LoadState(LoadStateEventArgs e)
        {
            if (CurrentPageViewModel != null)
            {
                CurrentPageViewModel.LoadState(e);
            }
        }

        protected virtual void SaveState(SaveStateEventArgs e)
        {
            if (CurrentPageViewModel != null)
            {
                CurrentPageViewModel.SaveState(e);
            }
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            LoadState(e);
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            SaveState(e);
        }

        #endregion

        #region DialogService Implementation

        public async Task ShowCustomMessageDialogAsync(string title, string content)
        {
            await ShowCustomMessageDialogAsync(title, content, CurrentResourceLoader.GetString("DialogDefaultButtonText"));
        }

        public async Task ShowCustomMessageDialogAsync(string title, string content, string cancelButtonText)
        {
            if (Dispatcher != null)
            {
                await UiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                {
                    if (!IsDialogOpen)
                    {
                        IsDialogOpen = true;

                        var dialog = new MessageDialog(content, title)
                        {
                            CancelCommandIndex = 0,
                            DefaultCommandIndex = 0,
                            Options = MessageDialogOptions.AcceptUserInputAfterDelay
                        };
                        dialog.Commands.Add(new UICommand(cancelButtonText));
                        var dialogResult = await dialog.ShowAsync();

                        IsDialogOpen = false;
                    }
                });
            }
        }

        public async Task ShowLogInFailedMessageDialogAsync()
        {
            await ShowCustomMessageDialogAsync(CurrentResourceLoader.GetString("LogInFailedDialogTitle"), CurrentResourceLoader.GetString("LogInFailedDialogMessage"));
        }

        public async Task ShowRegisterUserFailedMessageDialogAsync()
        {
            await ShowCustomMessageDialogAsync(CurrentResourceLoader.GetString("RegisterUserDialogTitle"), CurrentResourceLoader.GetString("RegisterUserDialogMessage"));
        }

        public virtual bool IsProgressDialogOpen()
        {
            return CurrentPageViewModel.IsLoading;
        }

        public virtual void ShowProgressDialog()
        {
            CurrentPageViewModel.IsLoading = true;
        }

        public virtual void CloseProgressDialog()
        {
            CurrentPageViewModel.IsLoading = false;
        }

        #endregion

        #region DispatchService Implementation

        public async Task DispatchOnUiThread(Action uiAction)
        {
            if (UiDispatcher != null)
            {
                await UiDispatcher.RunAsync(CoreDispatcherPriority.Normal, uiAction.Invoke);
            }
        }

        public async Task DispatchOnUiThread(Func<Task> asyncUiAction)
        {
            if (UiDispatcher != null)
            {
                await UiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await asyncUiAction.Invoke());
            }
        }

        #endregion

        #region NavigationService Implementation

        public void Navigate(Type pageViewModelType, Object parameter = null)
        {
            if (PageDictionary.ContainsKey(pageViewModelType))
            {
                if (parameter != null)
                {
                    Frame.Navigate(PageDictionary[pageViewModelType], parameter);
                }
                else
                {
                    Frame.Navigate(PageDictionary[pageViewModelType]);
                }
            }
        }

        public bool GoBack()
        {
            if (CanGoBack())
            {
                Frame.GoBack();
            }

            return false;
        }

        public bool CanGoBack()
        {
            return Frame.CanGoBack;
        }

        public void NavigateAndRemoveSelf(Type pageViewModelType, object parameter = null)
        {
            Navigate(pageViewModelType, parameter);
            if (Frame.CanGoBack)
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }
        }

        public void NavigateAndRemoveAll(Type pageViewModelType, object parameter = null)
        {
            Navigate(pageViewModelType, parameter);
            while (Frame.CanGoBack)
            {
                Frame.BackStack.RemoveAt(Frame.BackStackDepth - 1);
            }
        }

        #endregion
    }
}

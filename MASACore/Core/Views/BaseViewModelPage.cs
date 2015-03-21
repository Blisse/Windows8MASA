using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.ApplicationModel.Resources;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Ioc;
using MASACore.Core.LifeCycle;
using MASACore.Core.Services.Interfaces;
using MASACore.Core.ViewModels;

namespace MASACore.Core.Views
{
    public abstract class BaseViewModelPage : Page, IViewDialogService, IViewUiDispatchService, IViewNavigationService, IViewProgressService
    {
        public Boolean IsDialogOpen;
        public CoreDispatcher UiDispatcher;

        protected static readonly Dictionary<Type, Type> PageDictionary = new Dictionary<Type, Type>();
        protected ResourceLoader CurrentResourceLoader
        {
            get { return ResourceLoader.GetForCurrentView("Resources"); }
        }
        protected BasePageViewModel CurrentPageViewModel
        {
            get { return DataContext as BasePageViewModel; }
        }

        private readonly NavigationHelper _navigationHelper;

        protected BaseViewModelPage()
        {
            _navigationHelper = new NavigationHelper(this);
            _navigationHelper.LoadState += navigationHelper_LoadState;
            _navigationHelper.SaveState += navigationHelper_SaveState;
            
           NavigationCacheMode = NavigationCacheMode.Required;
        }

        ~BaseViewModelPage()
        {
            _navigationHelper.LoadState -= navigationHelper_LoadState;
            _navigationHelper.SaveState -= navigationHelper_SaveState;
        }

        #region Navigation Registration

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            UiDispatcher = Dispatcher;
            SimpleIoc.Default.Register<IViewNavigationService>(() => this);
            SimpleIoc.Default.Register<IViewProgressService>(() => this);
            SimpleIoc.Default.Register<IViewUiDispatchService>(() => this);
            SimpleIoc.Default.Register<IViewDialogService>(() => this);
            _navigationHelper.OnNavigatedTo(e);
        }
 
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            _navigationHelper.OnNavigatedFrom(e);
            SimpleIoc.Default.Unregister<IViewNavigationService>();
            SimpleIoc.Default.Unregister<IViewProgressService>();
            SimpleIoc.Default.Unregister<IViewUiDispatchService>();
            SimpleIoc.Default.Unregister<IViewDialogService>();
            UiDispatcher = null;
        }

        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
            if (CurrentPageViewModel != null)
            {
                CurrentPageViewModel.navigationHelper_LoadState(e);
            }
            LoadState(e);
        }

        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
            if (CurrentPageViewModel != null)
            {
                CurrentPageViewModel.navigationHelper_SaveState(e);
            }
            SaveState(e);
        }

        protected abstract void LoadState(LoadStateEventArgs e);

        protected abstract void SaveState(SaveStateEventArgs e);

        #endregion

        #region DialogService Implementation

        public async Task ShowMessageDialogAsync(string title, string content)
        {
            await ShowMessageDialogAsync(title, content, "OK");
        }

        public async Task ShowMessageDialogAsync(string title, string content, string cancelButtonText)
        {
            if (Dispatcher != null)
            {
                if (IsDialogOpen == false)
                {
                    await UiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                    {
                        var dialog = new MessageDialog(content, title)
                        {
                            CancelCommandIndex = 0,
                            DefaultCommandIndex = 0,
                            Options = MessageDialogOptions.AcceptUserInputAfterDelay
                        };
                        dialog.Commands.Add(new UICommand(cancelButtonText));
                        await dialog.ShowAsync();
                    });

                    IsDialogOpen = false;
                }
            }
        }

        #endregion

        #region ProgressService Implementation

        public virtual bool IsLoading()
        {
            return CurrentPageViewModel.IsLoading;
        }

        public virtual void ShowIndeterminateProgress()
        {
            CurrentPageViewModel.IsLoading = true;
        }

        public virtual void HideIndeterminateProgress()
        {
            CurrentPageViewModel.IsLoading = false;
        }

        #endregion

        #region UiDispatchService Implementation

        public async Task DispatchOnUiThreadAsync(Action uiAction)
        {
            if (UiDispatcher != null)
            {
                await UiDispatcher.RunAsync(CoreDispatcherPriority.Normal, uiAction.Invoke);
            }
        }

        public async Task DispatchOnUiThreadAsync(Func<Task> asyncUiAction)
        {
            if (UiDispatcher != null)
            {
                Task task = asyncUiAction.Invoke();
                if (task.Status == TaskStatus.Created)
                {
                    await UiDispatcher.RunAsync(CoreDispatcherPriority.Normal, async () => await task);
                }

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
            if (Frame.CanGoBack)
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

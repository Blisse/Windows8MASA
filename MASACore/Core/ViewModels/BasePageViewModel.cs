using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MASACore.Core.LifeCycle;
using MASACore.Core.Services.Interfaces;

namespace MASACore.Core.ViewModels
{
    public abstract class BasePageViewModel : ViewModelBase
    {
        /// <summary>
        /// This CancellationTokenSource generates a token that will be Cancelled when the Page's OnNavigatedFrom is called.
        /// </summary>
        protected CancellationTokenSource ActivePageCancellationTokenSource;

        private bool _isLoading = false;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }

            set
            {
                if (_isLoading == value)
                {
                    return;
                }

                _isLoading = value;
                RaisePropertyChanged();
            }
        }

        protected async Task ExecuteWithProgressDialogAsync(Func<CancellationToken, Task> asyncFunc)
        {
            if (ViewUiDispatchService != null)
            {
                await ViewUiDispatchService.DispatchOnUiThreadAsync(async () =>
                {
                    if (ViewProgressService.IsLoading() == false)
                    {
                        try
                        {
                            const int halfSecond = 5*100;
                            const int tenSeconds = 10*1000;

                            var timeoutCts = new CancellationTokenSource(tenSeconds);
                            var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ActivePageCancellationTokenSource.Token, timeoutCts.Token);
                            var invokeTask = asyncFunc.Invoke(linkedCts.Token);
                            await Task.WhenAny(Task.Delay(halfSecond, linkedCts.Token), invokeTask);

                            if (invokeTask.IsCompleted == false)
                            {
                                ViewProgressService.ShowIndeterminateProgress();
                            }

                            await Task.WhenAny(Task.Delay(tenSeconds, linkedCts.Token), invokeTask);
                            if (invokeTask.IsFaulted)
                            {
                                throw invokeTask.Exception.InnerException;
                            }
                        }
                        finally
                        {
                            ViewProgressService.HideIndeterminateProgress();
                        }
                    }
                });
            }
        }

        protected IViewDialogService ViewDialogService
        {
            get { return SimpleIoc.Default.GetInstance<IViewDialogService>(); }
        }

        protected IViewUiDispatchService ViewUiDispatchService
        {
            get { return SimpleIoc.Default.GetInstance<IViewUiDispatchService>(); }
        }

        protected IViewProgressService ViewProgressService
        {
            get { return SimpleIoc.Default.GetInstance<IViewProgressService>(); }
        }

        protected IViewNavigationService ViewNavigationService
        {
            get { return SimpleIoc.Default.GetInstance<IViewNavigationService>(); }
        }

        public void navigationHelper_LoadState(LoadStateEventArgs e)
        {
            ActivePageCancellationTokenSource = new CancellationTokenSource();
            LoadState(e);
        }

        public void navigationHelper_SaveState(SaveStateEventArgs e)
        {
            ActivePageCancellationTokenSource.Cancel();
            ActivePageCancellationTokenSource = null;
            IsLoading = false;
            SaveState(e);
        }

        protected abstract void LoadState(LoadStateEventArgs e);
        protected abstract void SaveState(SaveStateEventArgs e);
    }
}

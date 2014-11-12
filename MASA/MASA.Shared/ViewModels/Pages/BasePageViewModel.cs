using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MASA.Common.LifeCycle;
using MASA.Services.Interfaces;

namespace MASA.ViewModels.Pages
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

        protected async Task ExecuteWithProgressDialogAsync(Func<CancellationToken, Task> asyncFunc, CancellationToken pageInactivatedCancellationToken)
        {
            await UiDispatchService.DispatchOnUiThread(async () =>
            {
                IDialogService dialogService;
                try
                {
                    dialogService = DialogService;
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    return;
                }

                if (dialogService.IsProgressDialogOpen())
                {
                    return;
                }

                try
                {
                    var timeOutCancellationTokenSource = new CancellationTokenSource();
                    var linkedCancellationTokenSource =
                        CancellationTokenSource.CreateLinkedTokenSource(timeOutCancellationTokenSource.Token,
                            pageInactivatedCancellationToken);

                    var invokeTask = asyncFunc.Invoke(linkedCancellationTokenSource.Token);
                    if (pageInactivatedCancellationToken.IsCancellationRequested)
                    {
                        timeOutCancellationTokenSource.Cancel();
                        pageInactivatedCancellationToken.ThrowIfCancellationRequested();
                    }

                    await Task.WhenAny(Task.Delay(5 * 100, pageInactivatedCancellationToken), invokeTask);
                    if (pageInactivatedCancellationToken.IsCancellationRequested)
                    {
                        timeOutCancellationTokenSource.Cancel();
                        pageInactivatedCancellationToken.ThrowIfCancellationRequested();
                    }

                    if (!invokeTask.IsCompleted)
                    {
                        dialogService.ShowProgressDialog();
                    }

                    await Task.WhenAny(Task.Delay(10 * 1000, pageInactivatedCancellationToken), invokeTask);
                    if (pageInactivatedCancellationToken.IsCancellationRequested)
                    {
                        timeOutCancellationTokenSource.Cancel();
                        pageInactivatedCancellationToken.ThrowIfCancellationRequested();
                    }

                    if (invokeTask.IsFaulted)
                    {
                        throw invokeTask.Exception.InnerException;
                    }
                }
                finally
                {
                    dialogService.CloseProgressDialog();
                }
            });
        }

        protected IDialogService DialogService
        {
            get { return SimpleIoc.Default.GetInstance<IDialogService>(); }
        }

        protected IUiDispatchService UiDispatchService
        {
            get { return SimpleIoc.Default.GetInstance<IUiDispatchService>(); }
        }

        protected INavigationService NavigationService
        {
            get { return SimpleIoc.Default.GetInstance<INavigationService>(); }
        }

        public virtual void LoadState(LoadStateEventArgs e)
        {
            ActivePageCancellationTokenSource = new CancellationTokenSource();
        }

        public virtual void SaveState(SaveStateEventArgs e)
        {
            ActivePageCancellationTokenSource.Cancel();
            ActivePageCancellationTokenSource = null;

            IsLoading = false;
        }
    }
}

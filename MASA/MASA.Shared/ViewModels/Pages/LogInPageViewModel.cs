using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using GalaSoft.MvvmLight.Command;
using MASA.Common.Commands;
using MASA.Common.LifeCycle;

namespace MASA.ViewModels.Pages
{
    public class LogInPageViewModel : BasePageViewModel
    {
        #region Fields



        #endregion

        #region Properties
        
        public AwaitableDelegateCommand LogInCommand { get; set; }
        public RelayCommand NavigateToRegisterCommand { get; set; }

        #endregion

        public LogInPageViewModel()
        {
        }

        #region Command Methods


        #endregion

        #region Navigation Methods

        public override void LoadState(LoadStateEventArgs e)
        {
            base.LoadState(e);
        }

        public override void SaveState(SaveStateEventArgs e)
        {
            base.SaveState(e);
        }

        #endregion
    }
}

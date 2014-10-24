using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using MASA.Common.Commands;
using MASA.Common.LifeCycle;
using MASA.ViewModels.Controls;

namespace MASA.ViewModels.Pages
{
    public class LogInPageViewModel : BasePageViewModel
    {
        #region Fields



        #endregion

        #region Properties
        
        public AwaitableDelegateCommand LogInCommand { get; set; }
        public RelayCommand NavigateToRegisterCommand { get; set; }

        public String LogInUsernamePasswordControlId
        {
            get { return "LogInUsernamePasswordControlId"; }
        } 

        #endregion

        public LogInPageViewModel()
        {
            LogInCommand = new AwaitableDelegateCommand(ExecuteLogIn);
        }

        #region Command Methods

        private async Task ExecuteLogIn()
        {
            UsernamePasswordControlViewModel usernamePasswordControl =
                SimpleIoc.Default.GetInstance<UsernamePasswordControlViewModel>(LogInUsernamePasswordControlId);

            await Task.Delay(1000);
        }

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

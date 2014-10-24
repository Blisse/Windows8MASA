using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using MASA.Common.Commands;

namespace MASA.ViewModels.Pages
{
    public class RegisterPageViewModel : BasePageViewModel
    {
        
        #region Fields



        #endregion

        #region Properties
        
        public AwaitableDelegateCommand SignUpCommand { get; set; }

        public String RegisterUsernamePasswordControlId
        {
            get { return "RegisterUsernamePasswordControlId"; }
        } 

        #endregion

        public RegisterPageViewModel()
        {
            SignUpCommand = new AwaitableDelegateCommand(ExecuteSignUp);
        }

        private async Task ExecuteSignUp()
        {
            await Task.Delay(500);
        }
    }
}

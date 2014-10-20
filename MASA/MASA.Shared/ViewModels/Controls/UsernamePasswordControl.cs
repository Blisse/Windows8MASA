using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace MASA.ViewModels.Controls
{
    public class UsernamePasswordControlViewModel : ObservableObject
    {

        #region Properties

        private String _username;
        public String Username
        {
            get
            {
                return _username;
            }

            set
            {
                if (_username == value)
                {
                    return;
                }

                _username = value;
                RaisePropertyChanged();
            }
        }

        private String _password;
        public String Password
        {
            get
            {
                return _password;
            }

            set
            {
                if (_password == value)
                {
                    return;
                }

                _password = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        public UsernamePasswordControlViewModel()
        {
            
        }
    }
}

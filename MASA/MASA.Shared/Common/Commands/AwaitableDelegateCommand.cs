using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace MASA.Common.Commands
{
    public class AwaitableDelegateCommand : AwaitableDelegateCommand<object>, IAsyncCommand
    {
        public AwaitableDelegateCommand(Func<Task> executeMethod)
            : base(o => executeMethod())
        {

        }

        public AwaitableDelegateCommand(Func<Task> executeMethod, Func<bool> canExecuteMethod)
            : base(o => executeMethod(), o => canExecuteMethod())
        {

        }
    }

    public class AwaitableDelegateCommand<T> : IAsyncCommand<T>, ICommand
    {
        private readonly Func<T, Task> _executeMethod;
        private readonly RelayCommand<T> _underlyingCommand;
        private bool _isExecuting;

        #region Constructor

        public AwaitableDelegateCommand(Func<T, Task> executeMethod)
            : this(executeMethod, _ => true)
        {
        }

        public AwaitableDelegateCommand(Func<T, Task> executeMethod, Func<T, bool> canExecuteMethod)
        {
            _executeMethod = executeMethod;
            _underlyingCommand = new RelayCommand<T>(x => { }, canExecuteMethod);
        }

        #endregion

        #region IAsyncCommand Implementation

        public async Task ExecuteAsync(T obj)
        {
            try
            {
                if (CanExecute(obj))
                {
                    _isExecuting = true;
                    RaiseCanExecuteChanged();
                    await _executeMethod(obj);   
                }
            }
            finally
            {
                _isExecuting = false;
                RaiseCanExecuteChanged();
            }
        }

        public ICommand Command { get { return this; } }

        #endregion
        
        public bool CanExecute(object obj)
        {
            return !_isExecuting && _underlyingCommand.CanExecute((T)obj);
        }

        #region ICommand Implementation

        /// <summary>
        /// Execute the Command synchronously.
        /// </summary>
        /// <param name="parameter">Parameter to pass to the Execute method. Can be null if type param not specified.</param>
        public async void Execute(object parameter)
        {
            await ExecuteAsync((T)parameter);
        }

        #endregion

        public event EventHandler CanExecuteChanged
        {
            add { _underlyingCommand.CanExecuteChanged += value; }
            remove { _underlyingCommand.CanExecuteChanged -= value; }
        }

        public void RaiseCanExecuteChanged()
        {
            _underlyingCommand.RaiseCanExecuteChanged();
        }
    }
}

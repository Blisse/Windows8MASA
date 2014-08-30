using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MASA.Common.Commands
{
    /* RelayCommands and DelegateCommands accept an Execute method of type Action and Action<T>. When we call
     * Command.Execute, because the return type of the Execute method is void, we cannot await it. For Unit
     * Testing, because we only have a reference to the public ICommand method, we require that the Execute
     * method completes before continuing the Unit Test. This means we need to await the Execute method, 
     * requiring an IAsyncCommand.
     */

    public interface IAsyncCommand : IAsyncCommand<object>
    {
    }

    public interface IAsyncCommand<in T> : IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Execute the Command asynchronously.
        /// </summary>
        /// <param name="obj">Parameter of the Execute method. Can be null if type not specified.</param>
        /// <returns>An awaitable Task</returns>
        Task ExecuteAsync(T obj);

        /// <summary>
        /// Check whether the Command can be executed. Will be false if Execute is still completing 
        /// or if the CanExecute method is false. 
        /// </summary>
        /// <param name="obj">Parameter of the Execute method. Can be null if type not specified.</param>
        /// <returns>Whether the Command can be executed.</returns>
        bool CanExecute(object obj);

        ICommand Command { get; }
    }
}

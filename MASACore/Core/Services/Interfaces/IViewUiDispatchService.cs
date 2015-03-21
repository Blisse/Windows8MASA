using System;
using System.Threading.Tasks;

namespace MASACore.Core.Services.Interfaces
{
    public interface IViewUiDispatchService
    {
        /// <summary>
        /// Run the action on the UI thread.
        /// </summary>
        /// <param name="uiAction">Any action.</param>
        /// <returns></returns>
        Task DispatchOnUiThreadAsync(Action uiAction);

        /// <summary>
        /// Run the asynchronous Task on the UI thread.
        /// </summary>
        /// <param name="asyncUiAction">An asynchronous Task.</param>
        /// <returns></returns>
        Task DispatchOnUiThreadAsync(Func<Task> asyncUiAction);
    }
}

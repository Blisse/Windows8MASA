namespace MASA.Services.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public interface IUiDispatchService
    {
        /// <summary>
        /// Run the action on the UI thread.
        /// </summary>
        /// <param name="uiAction">Any action.</param>
        /// <returns></returns>
        Task DispatchOnUiThread(Action uiAction);

        /// <summary>
        /// Run the asynchronous Task on the UI thread.
        /// </summary>
        /// <param name="asyncUiAction">An asynchronous Task.</param>
        /// <returns></returns>
        Task DispatchOnUiThread(Func<Task> asyncUiAction);
    }
}

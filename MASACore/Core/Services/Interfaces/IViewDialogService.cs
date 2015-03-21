using System;
using System.Threading.Tasks;

namespace MASACore.Core.Services.Interfaces
{
    /// <summary>
    /// Service to manage presenting dialogs on the screen.
    /// </summary>
    public interface IViewDialogService
    {
        /// <summary>
        /// Present a Message Dialog with a single button for action. Used to give information to the user.
        /// </summary>
        /// <param name="title">Title of the message.</param>
        /// <param name="content">Contents of the message.</param>
        /// <returns></returns>
        Task ShowMessageDialogAsync(String title, String content);

        /// <summary>
        /// Present a Message Dialog with a single button for action. Used to give information to the user.
        /// </summary>
        /// <param name="title">Title of the message.</param>
        /// <param name="content">Contents of the message.</param>
        /// <param name="cancelButtonText">Text on the button dismissing the dialog.</param>
        /// <returns></returns>
        Task ShowMessageDialogAsync(String title, String content, String cancelButtonText);
    }
}

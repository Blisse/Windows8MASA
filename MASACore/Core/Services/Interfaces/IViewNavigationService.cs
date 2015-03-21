using System;

namespace MASACore.Core.Services.Interfaces
{
    public interface IViewNavigationService
    {
        /// <summary>
        /// Navigate to the Page associated with the PageViewModel type parameter.
        /// </summary>
        /// <param name="pageViewModelType">The PageViewModel type associated to the Page to navigate to.</param>
        /// <param name="parameter">Optional parameter to pass to the page.</param>
        void Navigate(Type pageViewModelType, Object parameter = null);

        /// <summary>
        /// Navigate back in the app's navigation stack.
        /// </summary>
        /// <returns></returns>
        bool GoBack();

        /// <summary>
        /// Check if the app can navigate back in the app's navigation stack.
        /// </summary>
        /// <returns></returns>
        bool CanGoBack();

        /// <summary>
        /// Navigate to a new page like the Navigate method, but remove the current page
        /// from the navigation stack.
        /// </summary>
        /// <param name="pageViewModelType"></param>
        /// <param name="parameter"></param>
        void NavigateAndRemoveSelf(Type pageViewModelType, Object parameter = null);

        /// <summary>
        /// Navigate to a new page like the Navigate method, but remove all pages
        /// from the navigation stack.
        /// </summary>
        /// <param name="pageViewModelType"></param>
        /// <param name="parameter"></param>
        void NavigateAndRemoveAll(Type pageViewModelType, Object parameter = null);
    }
}

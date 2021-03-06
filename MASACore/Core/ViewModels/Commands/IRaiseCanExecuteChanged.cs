﻿using System.Windows.Input;

namespace MASACore.Core.ViewModels.Commands
{
    public interface IRaiseCanExecuteChanged
    {
        void RaiseCanExecuteChanged();
    }

    public static class CommandExtensions
    {
        /// <summary>
        /// Helper method to make it easy to raise changed events
        /// </summary>
        /// <param name="command"></param>
        public static void RaiseCanExecuteChanged(this ICommand command)
        {
            var canExecuteChanged = command as IRaiseCanExecuteChanged;

            if (canExecuteChanged != null)
                canExecuteChanged.RaiseCanExecuteChanged();
        }
    }
}

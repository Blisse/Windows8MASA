using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MASACore.Core.Services.Interfaces
{
    public interface IViewProgressService
    {
        /// <summary>
        /// Check whether a Progress Dialog is loading. 
        /// Only one Progress Dialog can be displayed at a time.
        /// </summary>
        /// <returns></returns>
        bool IsLoading();

        /// <summary>
        /// Show a Progress Dialog.
        /// </summary>
        void ShowIndeterminateProgress();

        /// <summary>
        /// Close the currently shown Progress Dialog.
        /// </summary>
        void HideIndeterminateProgress();
    }
}

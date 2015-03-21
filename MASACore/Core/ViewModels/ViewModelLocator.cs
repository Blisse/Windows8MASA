/*
  In App.xaml:
  <Application.Resources>
      <vm:Locator xmlns:vm="clr-namespace:HushWP8"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/


using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using MASACore.Core.Services.Implementations;
using MASACore.Core.Services.Interfaces;

namespace MASACore.Core.ViewModels
{
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    public abstract class BaseViewModelLocator
    {
        public BaseViewModelLocator()
        {
            if (ViewModelBase.IsInDesignModeStatic)
            {
                // Create design time view services and models
            }
            else
            {
                // Create run time view services and models
                SimpleIoc.Default.Register<IStorageService, StorageService>();
            }
        }

        public virtual void Deinitialize()
        {
            SimpleIoc.Default.Unregister<IStorageService>();
        }
    }
}
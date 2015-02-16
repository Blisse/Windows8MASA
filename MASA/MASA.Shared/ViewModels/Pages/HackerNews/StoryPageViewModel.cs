using System;
using GalaSoft.MvvmLight.Command;
using MASA.Common.LifeCycle;
using MASA.Services.Implementations.HackerNews;
using MASA.DataModel.HackerNews;

namespace MASA.ViewModels.Pages.HackerNews
{
    public class StoryPageViewModel : BasePageViewModel
    {
        #region Fields

        private readonly IHackerNewsService _hackerNewsService;
        private StoryViewModel _story = null;

        #endregion

        #region Properties

        public StoryViewModel Story
        {
            get
            {
                return _story;
            }

            set
            {
                if (_story == value)
                {
                    return;
                }

                _story = value;
                RaisePropertyChanged();
            }
        }

        public RelayCommand<StoryViewModel> NavigateToCommentsCommand { get; set; } 

        #endregion

        public StoryPageViewModel(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;

            NavigateToCommentsCommand = new RelayCommand<StoryViewModel>(ExecuteNavigateToCommentsCommand);
        }

        private void ExecuteNavigateToCommentsCommand(StoryViewModel storyViewModel)
        {
            NavigationService.Navigate(typeof(CommentsPageViewModel), storyViewModel);
        }

        #region Command Methods



        #endregion

        #region Navigation Methods

        public override void LoadState(LoadStateEventArgs e)
        {
            base.LoadState(e);

            if (e.NavigationParameter != null && e.NavigationParameter as StoryViewModel != null)
            {
                Story = e.NavigationParameter as StoryViewModel;
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        public override void SaveState(SaveStateEventArgs e)
        {
            base.SaveState(e);

            Story = null;
        }

        #endregion
    }
}

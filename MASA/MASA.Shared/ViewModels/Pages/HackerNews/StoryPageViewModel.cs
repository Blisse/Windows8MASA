using MASA.Common.LifeCycle;
using MASA.DataModel.HackerNews;
using MASA.Services.Implementations.HackerNews;

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

        #endregion

        public StoryPageViewModel(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;
        }

        #region Command Methods



        #endregion

        #region Navigation Methods

        public override void LoadState(LoadStateEventArgs e)
        {
            base.LoadState(e);

            if (e.NavigationParameter != null && e.NavigationParameter as StoryModel != null)
            {
                Story = StoryViewModel.ViewModelFromModel(e.NavigationParameter as StoryModel);
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using MASA.Common.Commands;
using MASA.Common.LifeCycle;
using MASA.DataModel.HackerNews;
using MASA.Services.Implementations.HackerNews;

namespace MASA.ViewModels.Pages.HackerNews
{
    #region View Models

    public class StoryViewModel : ViewModelBase
    {
        public StoryModel StoryModel { get; set; }

        private int _id = 0;
        public int Id
        {
            get
            {
                return _id;
            }

            set
            {
                if (_id == value)
                {
                    return;
                }

                _id = value;
                RaisePropertyChanged();
            }
        }

        private int _number = 0;
        public int Number
        {
            get
            {
                return _number;
            }

            set
            {
                if (_number == value)
                {
                    return;
                }

                _number = value;
                RaisePropertyChanged();
            }
        }

        private int _score = 0;
        public int Score
        {
            get
            {
                return _score;
            }

            set
            {
                if (_score == value)
                {
                    return;
                }

                _score = value;
                RaisePropertyChanged();
            }
        }

        private String _title = String.Empty;
        public String Title
        {
            get
            {
                return _title;
            }

            set
            {
                if (_title == value)
                {
                    return;
                }

                _title = value;
                RaisePropertyChanged();
            }
        }

        private String _author = String.Empty;
        public String Author
        {
            get
            {
                return _author;
            }

            set
            {
                if (_author == value)
                {
                    return;
                }

                _author = value;
                RaisePropertyChanged();
            }
        }

        private DateTimeOffset _createTime;
        public DateTimeOffset CreateTime
        {
            get
            {
                return _createTime;
            }

            set
            {
                if (_createTime == value)
                {
                    return;
                }

                _createTime = value;
                RaisePropertyChanged();
            }
        }

        private String _text = String.Empty;
        public String Text
        {
            get
            {
                return _text;
            }

            set
            {
                if (_text == value)
                {
                    return;
                }

                _text = value;
                RaisePropertyChanged();
            }
        }

        private String _linkUrl = String.Empty;
        public String LinkUrl
        {
            get
            {
                return _linkUrl;
            }

            set
            {
                if (_linkUrl == value)
                {
                    return;
                }

                _linkUrl = value;
                RaisePropertyChanged();
            }
        }

        private List<int> _comments = new List<int>();
        public List<int> Comments
        {
            get
            {
                return _comments;
            }

            set
            {
                if (_comments == value)
                {
                    return;
                }

                _comments = value;
                RaisePropertyChanged();
            }
        }

        public static StoryViewModel ViewModelFromModel(StoryModel storyModel)
        {
            return new StoryViewModel()
            {
                StoryModel = storyModel,

                Id = storyModel.Id,
                Number = storyModel.Number,
                Score = storyModel.Score,
                Title = storyModel.Title,
                Author = storyModel.Author,
                CreateTime = storyModel.CreateTime,
                Text = storyModel.Text,
                LinkUrl = storyModel.LinkUrl,
                Comments = storyModel.Comments
            };
        }
    }

    #endregion

    public class NewsPageViewModel :  BasePageViewModel

    {
        #region Fields

        private readonly IHackerNewsService _hackerNewsService;
        private ObservableCollection<StoryViewModel> _stories = new ObservableCollection<StoryViewModel>();

        #endregion

        #region Properties

        public AwaitableDelegateCommand RefreshNewsCommand { get; set; }
        public RelayCommand<StoryViewModel> NavigateToStoryCommand { get; set; }
        public RelayCommand<StoryViewModel> NavigateToCommentsCommand { get; set; }

        public ObservableCollection<StoryViewModel> Stories
        {
            get
            {
                return _stories;
            }

            set
            {
                if (_stories == value)
                {
                    return;
                }

                _stories = value;
                RaisePropertyChanged();
            }
        }

        #endregion

        public NewsPageViewModel(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;

            RefreshNewsCommand = new AwaitableDelegateCommand(ExecuteRefreshNewsAsync, CanExecuteRefreshNews);
            NavigateToStoryCommand = new RelayCommand<StoryViewModel>(ExecuteNavigateToStory);
            NavigateToCommentsCommand = new RelayCommand<StoryViewModel>(ExecuteNavigateToComments);
        }

        private bool CanExecuteRefreshNews()
        {
            return !IsLoading;
        }

        #region Command Methods

        private async Task ExecuteRefreshNewsAsync()
        {
            await ExecuteWithProgressDialogAsync(async delegate
            {
                List<StoryModel> topStories = await _hackerNewsService.GetTopStoriesAsync();
                IEnumerable<StoryViewModel> filteredTopStories = topStories.Select(StoryViewModel.ViewModelFromModel);
                Stories = new ObservableCollection<StoryViewModel>(filteredTopStories);
            }, ActivePageCancellationTokenSource.Token);
        }

        private void ExecuteNavigateToStory(StoryViewModel storyModel)
        {
            if (String.IsNullOrWhiteSpace(storyModel.LinkUrl))
            {
                NavigationService.Navigate(typeof(CommentsPageViewModel), storyModel);
            }
            else
            {
                NavigationService.Navigate(typeof(StoryPageViewModel), storyModel);   
            }
        }

        private void ExecuteNavigateToComments(StoryViewModel storyModel)
        {
            NavigationService.Navigate(typeof(CommentsPageViewModel), storyModel);
        }

        #endregion

        #region Navigation Methods

        public override async void LoadState(LoadStateEventArgs e)
        {
            base.LoadState(e);

            await RefreshNewsCommand.ExecuteAsync(null);
        }

        public override void SaveState(SaveStateEventArgs e)
        {
            base.SaveState(e);
        }

        #endregion
    }
}

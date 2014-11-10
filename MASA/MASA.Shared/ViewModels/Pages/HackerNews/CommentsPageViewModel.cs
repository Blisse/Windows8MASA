﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Html;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Views;
using MASA.Common.Commands;
using MASA.Common.LifeCycle;
using MASA.DataModel.HackerNews;
using MASA.Services.Implementations.HackerNews;

namespace MASA.ViewModels.Pages.HackerNews
{
    public class CommentsPageViewModel : BasePageViewModel
    {
        
        #region Fields

        private readonly IHackerNewsService _hackerNewsService;
        private ObservableCollection<CommentModel> _comments = new ObservableCollection<CommentModel>();
        private StoryViewModel _story = null;

        #endregion

        #region Properties
        
        public AwaitableDelegateCommand RefreshCommentsCommand { get; set; }
        public RelayCommand<StoryModel> NavigateToStoryCommand { get; set; }

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
        public ObservableCollection<CommentModel> Comments
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

        #endregion

        public CommentsPageViewModel(IHackerNewsService hackerNewsService)
        {
            _hackerNewsService = hackerNewsService;

            RefreshCommentsCommand = new AwaitableDelegateCommand(ExecuteRefreshCommentsAsync, CanExecuteRefreshComments);
            NavigateToStoryCommand = new RelayCommand<StoryModel>(ExecuteNavigateToStory);
        }

        private bool CanExecuteRefreshComments()
        {
            return !IsLoading;
        }

        #region Command Methods

        private async Task ExecuteRefreshCommentsAsync()
        {
            var comments = await _hackerNewsService.GetCommentsAsync(Story.StoryModel);
            var orderedComments =
                comments.Where(comment => !comment.Deleted)
                    .OrderBy(comment => comment.ParentId)
                    .ThenBy(comment => comment.Level);

            Comments = new ObservableCollection<CommentModel>(orderedComments);
        }

        private void ExecuteNavigateToStory(StoryModel storyModel)
        {
            NavigationService.Navigate(typeof(StoryPageViewModel), storyModel);
        }

        #endregion

        #region Navigation Methods

        public override async void LoadState(LoadStateEventArgs e)
        {
            base.LoadState(e);

            if (e.NavigationParameter != null && e.NavigationParameter as StoryModel != null)
            {
                Story = StoryViewModel.ViewModelFromModel(e.NavigationParameter as StoryModel);

                await RefreshCommentsCommand.ExecuteAsync(null);
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        public override void SaveState(SaveStateEventArgs e)
        {
            base.SaveState(e);
        }

        #endregion
    }
}
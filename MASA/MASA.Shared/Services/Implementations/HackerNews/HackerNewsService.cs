using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.Web.Http;
using MASA.Common.Utilities;
using MASA.DataModel.HackerNews;
using Newtonsoft.Json.Linq;

namespace MASA.Services.Implementations.HackerNews
{
    public interface IHackerNewsService
    {
        Task<List<StoryModel>> GetTopStoriesAsync(int numberOfResults = 20, int paginate = 0);

        Task<List<CommentModel>> GetCommentsAsync(StoryModel parentComment);
    }

    public class HackerNewsService : IHackerNewsService
    {
        private const String HackerNewsItemUrl = "https://hacker-news.firebaseio.com/v0/item/{0}.json";
        private const String HackerNewsTopStoriesUrl = "https://hacker-news.firebaseio.com/v0/topstories.json";

        private async Task<JToken> GetResponseForIdAsync(int id)
        {
            String requestUrl = String.Format(HackerNewsItemUrl, id);
            HttpClient httpClient = new HttpClient();
            String resultString = await httpClient.GetStringAsync(new Uri(requestUrl, UriKind.Absolute));
            JToken resultToken = JToken.Parse(resultString);
            return resultToken;
        }

        private async Task<List<int>> GetTopStoryIdsAsync()
        {
            String requestUrl = HackerNewsTopStoriesUrl;
            HttpClient httpClient = new HttpClient();
            String resultString = await httpClient.GetStringAsync(new Uri(requestUrl, UriKind.Absolute));
            JArray resultArray = JArray.Parse(resultString);
            List<int> resultList = resultArray.ToObject<List<int>>();
            return resultList;
        }

        public async Task<List<StoryModel>> GetTopStoriesAsync(int numberOfResults = 20, int paginate = 0)
        {
            Debug.Assert(numberOfResults * (paginate + 1) <= 100);
            int lowerRange = numberOfResults * paginate;

            List<int> ids = await GetTopStoryIdsAsync();
            List<int> filteredIds = ids.GetRange(lowerRange, numberOfResults);

            JToken[] jsonStories = await Task.WhenAll(filteredIds.Select(GetResponseForIdAsync));

            List<StoryModel> stories = new List<StoryModel>();
            foreach (JToken jsonStory in jsonStories)
            {
                if ("story".Equals(jsonStory.Value<String>("type")))
                {
                    JToken kids = jsonStory["kids"] ?? JToken.FromObject(new List<int>());
                    long unixSeconds = jsonStory.Value<long>("time");

                    StoryModel story = new StoryModel
                    {
                        Number = (numberOfResults * paginate) + Array.IndexOf(jsonStories, jsonStory),
                        Id = jsonStory.Value<int>("id"),
                        Comments = kids.ToObject<List<int>>(),
                        Title = jsonStory.Value<String>("title"),
                        Author = jsonStory.Value<String>("by"),
                        Score = jsonStory.Value<int>("score"),
                        CreateTime = DateTimeOffsetUtility.DateTimeFromUnixTimeStamp(unixSeconds),
                        Text = jsonStory.Value<String>("text"),
                        LinkUrl = jsonStory.Value<String>("url")
                    };

                    stories.Add(story);
                }
            }

            return stories;
        }

        public async Task<List<CommentModel>> GetCommentsAsync(CommentModel parentComment, int level = 1)
        {
            Debug.Assert(parentComment != null);

            List<int> ids = parentComment.Comments;
            JToken[] jsonComments = await Task.WhenAll(ids.Select(GetResponseForIdAsync));

            List<CommentModel> comments = new List<CommentModel>();
            foreach (JToken jsonStory in jsonComments)
            {
                if ("comment".Equals(jsonStory.Value<String>("type")))
                {
                    JToken kids = jsonStory["kids"] ?? JToken.FromObject(new List<int>());
                    long unixSeconds = jsonStory.Value<long>("time");

                    CommentModel comment = new CommentModel
                    {
                        Deleted = jsonStory["deleted"] != null,
                        Id = jsonStory.Value<int>("id"),
                        ParentId = jsonStory.Value<int>("parent"),
                        Level = level,
                        Comments = kids.ToObject<List<int>>(),
                        Author = jsonStory.Value<String>("by"),
                        CreateTime = DateTimeOffsetUtility.DateTimeFromUnixTimeStamp(unixSeconds),
                        Text = jsonStory.Value<String>("text")
                    };

                    comments.Add(comment);
                }
            }

            List<CommentModel>[] innerComments =
                await Task.WhenAll(comments.Select(async comment => await GetCommentsAsync(comment, level + 1)));

            foreach (List<CommentModel> commentModels in innerComments)
            {
                comments.AddRange(commentModels);
            }

            return comments;
        }

        public async Task<List<CommentModel>> GetCommentsAsync(StoryModel story)
        {
            Debug.Assert(story != null);

            List<int> ids = story.Comments;
            JToken[] jsonComments = await Task.WhenAll(ids.Select(GetResponseForIdAsync));

            List<CommentModel> comments = new List<CommentModel>();
            foreach (JToken jsonStory in jsonComments)
            {
                if ("comment".Equals(jsonStory.Value<String>("type")))
                {
                    JToken kids = jsonStory["kids"] ?? JToken.FromObject(new List<int>());
                    long unixSeconds = jsonStory.Value<long>("time");

                    CommentModel comment = new CommentModel
                    {
                        Deleted = jsonStory["deleted"] != null,
                        Id = jsonStory.Value<int>("id"),
                        ParentId = jsonStory.Value<int>("parent"),
                        Level = 0,
                        Comments = kids.ToObject<List<int>>(),
                        Author = jsonStory.Value<String>("by"),
                        CreateTime = DateTimeOffsetUtility.DateTimeFromUnixTimeStamp(unixSeconds),
                        Text = jsonStory.Value<String>("text")
                    };

                    comments.Add(comment);
                }
            }

            List<CommentModel>[] innerComments =
                await Task.WhenAll(comments.Select(async comment => await GetCommentsAsync(comment)));

            foreach (List<CommentModel> commentModels in innerComments)
            {
                comments.AddRange(commentModels);
            }

            return comments;
        }

        //
        //            private const String NewsPageUrl = "https://news.ycombinator.com/news?p={0}";
        //            String requestUrl = String.Format(NewsPageUrl, page);
        //            HttpClient httpClient = new HttpClient();
        //            IInputStream resultStream = await httpClient.GetInputStreamAsync(new Uri(requestUrl, UriKind.Absolute));
        //
        //            HtmlDocument htmlDocument = new HtmlDocument();
        //            htmlDocument.Load(resultStream.AsStreamForRead());
        //
        //            if (htmlDocument.DocumentNode != null)
        //            {
        //                var rows = htmlDocument.DocumentNode.Descendants("tr")
        //                    .Where(
        //                        node =>
        //                            node.Descendants("td").Count() != 0 &&
        //                            node.Descendants("td").FirstOrDefault() != null &&
        //                            node.Descendants("td").FirstOrDefault().HasAttributes &&
        //                            node.Descendants("td").FirstOrDefault().Attributes != null &&
        //                            node.Descendants("td").FirstOrDefault().Attributes["class"] != null &&
        //                            node.Descendants("td").FirstOrDefault().Attributes["class"].Value == "title");
        //            }
    }
}

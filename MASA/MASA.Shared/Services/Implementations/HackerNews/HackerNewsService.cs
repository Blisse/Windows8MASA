using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
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

        private async Task<List<CommentModel>> GetCommentsAsync(List<int> commentIds, int level = 0)
        {
            List<CommentModel> comments = new List<CommentModel>();

            Barrier barrier = new Barrier(commentIds.Count);
            foreach (int id in commentIds)
            {
                JToken jsonComment = await GetResponseForIdAsync(id);
                if (jsonComment.HasValues && "comment".Equals(jsonComment.Value<String>("type")))
                {
                    JToken kids = jsonComment["kids"] ?? JToken.FromObject(new List<int>());
                    long unixSeconds = jsonComment.Value<long>("time");

                    CommentModel comment;
                    if (jsonComment["deleted"] != null)
                    {
                        comment = new CommentModel
                        {
                            Deleted = true,
                            Id = jsonComment.Value<int>("id"),
                            ParentId = jsonComment.Value<int>("parent"),
                            Level = level,
                            Comments = kids.ToObject<List<int>>(),
                            CreateTime = DateTimeOffsetUtility.DateTimeFromUnixTimeStamp(unixSeconds),
                        };
                    }
                    else
                    {
                        comment = new CommentModel
                        {
                            Deleted = false,
                            Id = jsonComment.Value<int>("id"),
                            ParentId = jsonComment.Value<int>("parent"),
                            Level = level,
                            Comments = kids.ToObject<List<int>>(),
                            Author = jsonComment.Value<String>("by"),
                            CreateTime = DateTimeOffsetUtility.DateTimeFromUnixTimeStamp(unixSeconds),
                            Text = jsonComment.Value<String>("text")
                        };
                    }
                    comments.Add(comment);

                    List<CommentModel> innerComments = await GetCommentsAsync(comment, level);
                    comments.AddRange(innerComments);
                }
            }
//
//            foreach (JToken jsonStory in jsonComments.Where(j => j.HasValues 
//                && "comment".Equals(j.Value<String>("type"))))
//            {
//                JToken kids = jsonStory["kids"] ?? JToken.FromObject(new List<int>());
//                long unixSeconds = jsonStory.Value<long>("time");
//
//                CommentModel comment;
//                if (jsonStory["deleted"] != null)
//                {
//                    comment = new CommentModel
//                    {
//                        Deleted = true,
//                        Id = jsonStory.Value<int>("id"),
//                        ParentId = jsonStory.Value<int>("parent"),
//                        Level = level,
//                        Comments = kids.ToObject<List<int>>(),
//                        CreateTime = DateTimeOffsetUtility.DateTimeFromUnixTimeStamp(unixSeconds),
//                    };
//                }
//                else
//                {
//                    comment = new CommentModel
//                    {
//                        Deleted = false,
//                        Id = jsonStory.Value<int>("id"),
//                        ParentId = jsonStory.Value<int>("parent"),
//                        Level = level,
//                        Comments = kids.ToObject<List<int>>(),
//                        Author = jsonStory.Value<String>("by"),
//                        CreateTime = DateTimeOffsetUtility.DateTimeFromUnixTimeStamp(unixSeconds),
//                        Text = jsonStory.Value<String>("text")
//                    };
//                }
//                comments.Add(comment);
//
//                List<CommentModel> innerComments = await GetCommentsAsync(comment, level);
//                comments.AddRange(innerComments);
//            }

            return comments;
        }

        public async Task<List<CommentModel>> GetCommentsAsync(CommentModel parentComment, int level = 1)
        {
            Debug.Assert(parentComment != null);

            List<int> ids = parentComment.Comments;
            List<CommentModel> comments = await GetCommentsAsync(ids, level + 1);

            return comments;
        }

        public async Task<List<CommentModel>> GetCommentsAsync(StoryModel story)
        {
            Debug.Assert(story != null);

            List<int> ids = story.Comments;
            List<CommentModel> comments = await GetCommentsAsync(ids);

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

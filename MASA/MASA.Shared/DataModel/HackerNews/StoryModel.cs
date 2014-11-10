using System;
using System.Collections.Generic;
using System.Text;
using GalaSoft.MvvmLight;

namespace MASA.DataModel.HackerNews
{
    public class StoryModel
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Score { get; set; }
        public String Title { get; set; }
        public String Author { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        public String Text { get; set; }
        public String LinkUrl { get; set; }
        public List<int> Comments { get; set; }
    }
}

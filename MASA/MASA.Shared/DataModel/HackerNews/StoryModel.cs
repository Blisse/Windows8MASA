using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;
using GalaSoft.MvvmLight;

namespace MASA.DataModel.HackerNews
{
    [DataContract]
    public class StoryModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int Number { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public String Title { get; set; }
        [DataMember]
        public String Author { get; set; }
        [DataMember]
        public DateTimeOffset CreateTime { get; set; }
        [DataMember]
        public String Text { get; set; }
        [DataMember]
        public String LinkUrl { get; set; }
        [DataMember]
        public List<int> Comments { get; set; }
    }
}

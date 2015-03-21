using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MASA.DataModel.HackerNews
{
    [DataContract]
    public class CommentModel
    {
        [DataMember]
        public int Id { get; set; }
        [DataMember]
        public int ParentId { get; set; }
        [DataMember]
        public int Level { get; set; }
        [DataMember]
        public String Author { get; set; }
        [DataMember]
        public DateTimeOffset CreateTime { get; set; }
        [DataMember]
        public String Text { get; set; }
        [DataMember]
        public List<int> Comments { get; set; }
        [DataMember]
        public bool Deleted { get; set; }
    }
}

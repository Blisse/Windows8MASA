using System;
using System.Collections.Generic;
using System.Text;

namespace MASA.DataModel.HackerNews
{
    public class CommentModel
    {
        public int Id { get; set; }
        public int ParentId { get; set; }
        public int Level { get; set; }
        public String Author { get; set; }
        public DateTimeOffset CreateTime { get; set; }
        public String Text { get; set; }
        public List<int> Comments { get; set; }
        public bool Deleted { get; set; }
    }
}

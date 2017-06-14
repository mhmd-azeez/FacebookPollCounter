using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookPollCounter
{
    public class PagedList<T>
    {
        public T this[int i]
        {
            get => Children[i];
        }

        [JsonProperty("data")]
        public List<T> Children { get; set; }
        public Paging Paging { get; set; }
        public Summary Summary { get; set; }
    }

    public class Cursors
    {
        public string Before { get; set; }
        public string After { get; set; }
    }

    public class Paging
    {
        public Cursors Cursors { get; set; }
        public string Next { get; set; }
    }

    public class Summary
    {
        public string Order { get; set; }
        [JsonProperty("total_count")]
        public int TotalCount { get; set; }
        [JsonProperty("can_comment")]
        public bool CanComment { get; set; }
    }
}

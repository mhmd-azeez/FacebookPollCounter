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
            get => Data[i];
        }

        public List<T> Data { get; set; }
        public Paging Paging { get; set; }
        public Summary Summary { get; set; }
    }

    public class Cursors
    {
        public string before { get; set; }
        public string after { get; set; }
    }

    public class Paging
    {
        public Cursors cursors { get; set; }
        public string next { get; set; }
    }

    public class Summary
    {
        public string order { get; set; }
        public int total_count { get; set; }
        public bool can_comment { get; set; }
    }
}

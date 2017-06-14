using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookPollCounter
{
    public class Comment
    {
        [JsonProperty("created_time")]
        public DateTime CreatedTime { get; set; }
        public Item From { get; set; }
        public string Message { get; set; }
        public string Id { get; set; }
    }
}

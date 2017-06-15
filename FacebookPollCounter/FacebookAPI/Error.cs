using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FacebookPollCounter.FacebookAPI
{
    public class Error
    {
        public string Message { get; set; }
        public string Type { get; set; }
        public int Code { get; set; }
        [JsonProperty("Fbtrace_id")]
        public string FbTraceId { get; set; }
    }

    public class ErrorResponse
    {
        public Error Error { get; set; }
    }

}

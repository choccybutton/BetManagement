using Newtonsoft.Json;
using System.Collections.Generic;
using FootballAPIWrapper.Converters;

namespace FootballAPIWrapper.Models
{
    public class ApiResponse<T>
    {
        [JsonProperty("get")]
        public string Get { get; set; }

        [JsonProperty("parameters")]
        public Dictionary<string, object> Parameters { get; set; }

        [JsonProperty("errors")]
        [JsonConverter(typeof(ErrorsJsonConverter))]
        public List<string> Errors { get; set; }

        [JsonProperty("results")]
        public int Results { get; set; }

        [JsonProperty("paging")]
        public Paging Paging { get; set; }

        [JsonProperty("response")]
        public List<T> Response { get; set; }
    }

    public class Paging
    {
        [JsonProperty("current")]
        public int Current { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }
    }
}
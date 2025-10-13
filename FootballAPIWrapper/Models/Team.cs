using Newtonsoft.Json;

namespace FootballAPIWrapper.Models
{
    public class Team
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("founded")]
        public int? Founded { get; set; }

        [JsonProperty("national")]
        public bool National { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("venue")]
        public Venue Venue { get; set; }
    }

    public class Venue
    {
        [JsonProperty("id")]
        public int? Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("capacity")]
        public int? Capacity { get; set; }

        [JsonProperty("surface")]
        public string Surface { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }
    }
}
using Newtonsoft.Json;
using System;

namespace FootballAPIWrapper.Models
{
    public class Fixture
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("referee")]
        public string Referee { get; set; }

        [JsonProperty("timezone")]
        public string Timezone { get; set; }

        [JsonProperty("date")]
        public DateTime Date { get; set; }

        [JsonProperty("timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty("periods")]
        public Periods Periods { get; set; }

        [JsonProperty("venue")]
        public Venue Venue { get; set; }

        [JsonProperty("status")]
        public FixtureStatus Status { get; set; }
    }

    public class FixtureDetails
    {
        [JsonProperty("fixture")]
        public Fixture Fixture { get; set; }

        [JsonProperty("league")]
        public League League { get; set; }

        [JsonProperty("teams")]
        public Teams Teams { get; set; }

        [JsonProperty("goals")]
        public Goals Goals { get; set; }

        [JsonProperty("score")]
        public Score Score { get; set; }
    }

    public class Periods
    {
        [JsonProperty("first")]
        public long? First { get; set; }

        [JsonProperty("second")]
        public long? Second { get; set; }
    }

    public class FixtureStatus
    {
        [JsonProperty("long")]
        public string Long { get; set; }

        [JsonProperty("short")]
        public string Short { get; set; }

        [JsonProperty("elapsed")]
        public int? Elapsed { get; set; }
    }

    public class Teams
    {
        [JsonProperty("home")]
        public TeamInfo Home { get; set; }

        [JsonProperty("away")]
        public TeamInfo Away { get; set; }
    }

    public class TeamInfo
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("logo")]
        public string Logo { get; set; }

        [JsonProperty("winner")]
        public bool? Winner { get; set; }
    }

    public class Goals
    {
        [JsonProperty("home")]
        public int? Home { get; set; }

        [JsonProperty("away")]
        public int? Away { get; set; }
    }

    public class Score
    {
        [JsonProperty("halftime")]
        public Goals Halftime { get; set; }

        [JsonProperty("fulltime")]
        public Goals Fulltime { get; set; }

        [JsonProperty("extratime")]
        public Goals Extratime { get; set; }

        [JsonProperty("penalty")]
        public Goals Penalty { get; set; }
    }
}
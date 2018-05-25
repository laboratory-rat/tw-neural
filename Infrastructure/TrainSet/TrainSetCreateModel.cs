using Domain.Neural;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using Tools.Json;

namespace Infrastructure.TrainSet
{
    public class TrainSetCreateModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("source_id")]
        public string SourceId { get; set; }

        [JsonProperty("min_count")]
        [JsonConverter(typeof(StrictIntConverter))]
        public int MinCount { get; set; }

        [JsonProperty("max_count")]
        [JsonConverter(typeof(StrictIntConverter))]
        public int MaxCount { get; set; }


        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public TrainSetSourceType Type { get; set; }
    }
}

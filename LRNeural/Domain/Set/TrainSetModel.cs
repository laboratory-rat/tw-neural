using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Domain.Set
{
    public class TrainSetModel
    {
        [JsonProperty("data")]
        public Tuple<double[], double[]>[] Data { get; set; }

        [JsonProperty("string_source")]
        public Tuple<string[], string[]>[] StringSource { get; set; }
    }
}

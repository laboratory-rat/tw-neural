using Domain.General;
using Domain.Neural;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;
using Tools.Json;

namespace Infrastructure.Neural
{
    public class NetCreateModel
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("skrew")]
        public float Skrew { get; set; }

        [JsonProperty("seed")]
        [JsonConverter(typeof(StrictIntConverter))]
        public int? Seed { get; set; }

        [JsonProperty("use_seed")]
        public bool UseSeed { get; set; }

        [JsonProperty("input_length")]
        [JsonConverter(typeof(StrictIntConverter))]
        public int InputLength { get; set; }

        [JsonProperty("output_length")]
        [JsonConverter(typeof(StrictIntConverter))]
        public int OutputLength { get; set; }

        [JsonProperty("hidden_layers_count")]
        [JsonConverter(typeof(StrictIntConverter))]
        public int HiddenLayersCount { get; set; }

        [JsonProperty("hidden_neurons_count")]
        [JsonConverter(typeof(StrictIntConverter))]
        public int HiddenNeuronsCount { get; set; }


        [JsonProperty("type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public NeuralNetType Type { get; set; }


        [JsonProperty("func_type")]
        [JsonConverter(typeof(StringEnumConverter))]
        public NeuralNetFuncType FuncType { get; set; }
    }

    public class NeuralNetDisplayModel : NetCreateModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("type_string")]
        public string TypeString { get; set; }

        [JsonProperty("func_type_string")]
        public string FuncTypeString { get; set; }
    }

    public class NeuralNetShortDisplayModel : ShortInfoModel
    {
        [JsonProperty("type_string")]
        public string TypeString { get; set; }

        [JsonProperty("type_string")]
        public string FuncTypeString { get; set; }

    }
}

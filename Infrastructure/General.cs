using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure
{
    public class AppDataModel
    {
        public string FacebookAppId { get; set; }
        public string FacebookAppSecret { get; set; }
    }

    public class ListModel<T>
    {
        [JsonProperty("take")]
        public int Take { get; set; }

        [JsonProperty("skip")]
        public int Skip { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("data")]
        public List<T> Data { get; set; }

        [JsonProperty("next")]
        public int Next { get; set; }

        public ListModel()
        {

        }

        public ListModel(int take, int skip, int total, List<T> data)
        {
            Take = take;
            Skip = skip;
            Total = total;

            if(data == null)
                Data = new List<T>();
            else
                Data = data;

            Next = Take + Skip;
        }
    }

    public class IdNameModel : IdNameModel<string, string> { }

    public class IdNameModel<T> : IdNameModel<string, T> { }

    public class IdNameModel<T, J>
    {
        [JsonProperty("id")]
        public T Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }
    }

    public class ShortInfoModel : IdNameModel
    {
        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty("input_length")]
        public int InputLength { get; set; }

        [JsonProperty("output_length")]
        public int OutputLength { get; set; }

    }

}

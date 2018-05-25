using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Api.Common
{
    public class ApiResponse
    {
        [JsonProperty("data")]
        public object Data { get; set; }

        [JsonProperty("is_success")]
        public bool IsSuccess { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("inner_message")]
        public string InnerMessage { get; set; }
    }

    public class ApiResponse<T>
        where T : class
    {
        [JsonProperty("data")]
        public T Data { get; set; }

        [JsonProperty("Message")]
        public string Message { get; set; }

        [JsonProperty("is_success")]
        public bool IsSuccess { get; set; }

        [JsonProperty("inner_message")]
        public string InnerMessage { get; set; }

        public static implicit operator ApiResponse<T>(ApiResponse response)
        {
            return new ApiResponse<T>
            {
                Data = (T)response.Data,
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                InnerMessage = response.InnerMessage
            };
        }

        public static implicit operator ApiResponse(ApiResponse<T> response)
        {
            return new ApiResponse
            {
                Data = response.Data,
                Message = response.Message,
                IsSuccess = response.IsSuccess,
                InnerMessage = response.InnerMessage
            };
        }
    }
}

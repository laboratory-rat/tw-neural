using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Twitter
{
    public class TwitterUserResponseModel
    {
        [JsonProperty("screenName")]
        public string ScreenName { get; set; }

        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("statusesCount")]
        public int StatusesCount { get; set; }

        [JsonProperty("accountCreatedAt")]
        public DateTime AccountCreatedAt { get; set; }

        [JsonProperty("language")]
        public string Language { get; set; }

        [JsonProperty("location")]
        public string Location { get; set; }

        [JsonProperty("geoEnabled")]
        public bool GeoEnabled { get; set; }

        [JsonProperty("timeZone")]
        public string TimeZone { get; set; }

        [JsonProperty("profileImageUrl")]
        public string ProfileImageUrl { get; set; }

        [JsonProperty("profileBackgroundImageUrl")]
        public string ProfileBackgroundImageUrl { get; set; }


        [JsonProperty("profileSidebarFillColor")]
        public string ProfileSidebarFillColor { get; set; }

        [JsonProperty("profileSidebarBorderColor")]
        public string ProfileSidebarBorderColor { get; set; }

        [JsonProperty("profileLinkColor")]
        public string ProfileLinkColor { get; set; }

        [JsonProperty("profileTextColor")]
        public string ProfileTextColor { get; set; }
    }
}

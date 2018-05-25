using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Facebook
{
    public class FacebookUserProfileModel : IFacebookModel
    {
        [JsonProperty("id")]
        public long Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("locale")]
        public string Locale { get; set; }

        [JsonProperty("birthday")]
        public DateTime Birthday { get; set; }

        [JsonProperty("first_name")]
        public string FirstName { get; set; }

        [JsonProperty("last_name")]
        public string LastName { get; set; }

        [JsonProperty("picture")]
        public FacebookSingleDataModel<FacebookUserProfilePictureModel> Picture { get; set; }

    }

    public class FacebookUserProfilePictureModel : IFacebookModel
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Infrastructure.Collections
{
    public class CollectionUpdateModel
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("title")]
        [Required]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Title must be from 5 to 100 chars")]
        public string Title { get; set; }

        [JsonProperty("Comments")]
        public string Comments { get; set; }


        [JsonProperty("sourcesIds")]
        public List<long> SourcesIds { get; set; }

        [JsonProperty("blackListIds")]
        public List<long> BlackListIds { get; set; }
    }


}

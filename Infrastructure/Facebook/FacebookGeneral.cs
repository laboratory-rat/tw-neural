using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Facebook
{
    public interface IFacebookModel { }

    public class FacebookSingleDataModel<T> : IFacebookModel
        where T : IFacebookModel
    {
        [JsonProperty("data")]
        public T Data { get; set; }
    }

    public class FacebookParser
    {
        public static T Parse<T>(object obj)
            where T: class, new()
        {
            if (obj == null) return null;

            var result = new T();

            try
            {
                string objString = JsonConvert.SerializeObject(obj);
                result = JsonConvert.DeserializeObject<T>(objString);
            }
            catch(Exception ex)
            {
                result = default(T);
            }

            return result;
        }
    }

}

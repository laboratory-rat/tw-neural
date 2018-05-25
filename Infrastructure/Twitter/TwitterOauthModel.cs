using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Twitter
{
    public class TwitterOauthModel
    {
        public string OauthToken { get; set; }
        public string OauthVerifier { get; set; }
        public string AuthorizationId { get; set; }
    }
}

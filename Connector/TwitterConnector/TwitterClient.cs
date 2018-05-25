using Microsoft.AspNetCore.Http;
using System;
using Tweetinvi;
using Tweetinvi.Models;

namespace TwitterConnector
{
    public class TwitterClient
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
        public string RedirectUrl { get; set; }

        public ConsumerCredentials AppCredentials { get; set; }

        public TwitterClient(string key, string secret, string redirectUrl)
        {
            ApiKey = key;
            ApiSecret = secret;
            RedirectUrl = redirectUrl;

            AppCredentials = new ConsumerCredentials(key, secret);
        }

        public string GetTwitterAuthUrl(HttpRequest request)
        {
            var context = AuthFlow.InitAuthentication(AppCredentials, RedirectUrl);
            return context.AuthorizationURL;
        }

        public IAuthenticatedUser ValidateTwitterAuth(string token, string verifier, string authId)
        {
            IAuthenticatedUser user = null;

            try
            {
                var userCredentials = AuthFlow.CreateCredentialsFromVerifierCode(verifier, authId);
                user = User.GetAuthenticatedUser(userCredentials);
            }
            catch(Exception ex)
            {
                var e = ex;
            }


            return user;
        }

        public void OAuthUser(string consumerKey, string consumerSecret, string token, string tokenSecret)
        {
            Auth.SetUserCredentials(consumerKey, consumerSecret, token, tokenSecret);
        }
    }
}

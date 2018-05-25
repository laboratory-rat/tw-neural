using AutoMapper;
using Domain.User;
using Infrastructure.Api.Common;
using Infrastructure.Twitter;
using Manager.General;
using Repository;
using Repository.Store;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Models;
using TwitterConnector;

namespace Manager
{
    public class TwitterManager : BaseManager
    {
        protected TwitterClient _client;
        protected IUserSocialsStore _userSocialsStore;

        public TwitterManager(IPrincipal principal, ApiUserStore userStore, IMapper mapper, TwitterClient client, IUserSocialsStore userSocialsStore) : base(principal, userStore, mapper)
        {
            _client = client;
            _userSocialsStore = userSocialsStore;
        }

        public async Task<ApiResponse<TwitterUserResponseModel>> SearchUser(string screenName)
        {
            var user = await CurrentUser();
            var twitter = await _userSocialsStore.GetTwitter(user.Id);
            if (twitter == null) return Failed();

            OAuth(twitter);
            var twitterUser = User.GetUserFromScreenName(screenName);

            return Ok(_mapper.Map<TwitterUserResponseModel>(twitterUser));
        }



        protected void OAuth(EntityUserSocial twitterSocial)
        {
            _client.OAuthUser(twitterSocial.ConsumerKey, twitterSocial.ConsumerSecret, twitterSocial.Token, twitterSocial.TokenSecret);
        }

    }
}

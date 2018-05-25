using AutoMapper;
using Domain.User;
using Infrastructure.Api.Common;
using Infrastructure.Error;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Repository;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TwitterConnector;

namespace Manager.General
{
    public abstract class BaseManager
    {
        protected readonly ApiUserStore _userStore;

        protected readonly ClaimsPrincipal _principal;
        public string CurrentUserEmail => _principal?.Identity.Name;

        protected EntityUser _currentUser;
        public async Task<EntityUser> CurrentUser()
        {
            if (_currentUser == null) 
               _currentUser = await _userStore.FindByEmailAsync(CurrentUserEmail?.ToLower());

            return _currentUser;
        }

        protected readonly ILogger _logger;
        protected readonly IMapper _mapper;

        public BaseManager(IPrincipal principal, ApiUserStore userStore, IMapper mapper)
        {
            _principal = principal as ClaimsPrincipal;
            _userStore = userStore;
            _logger = new LoggerFactory().CreateLogger(GetType().FullName);
            _mapper = mapper;
        }

        protected void OAuthTwitter(TwitterClient client, EntityUserSocial social)
        {
            client.OAuthUser(social.ConsumerKey, social.ConsumerSecret, social.Token, social.TokenSecret);
        }

        public ApiResponse Ok(object data, string message, string innerMessage)
        {
            return new ApiResponse
            {
                Data = data,
                Message = message,
                IsSuccess = true,
                InnerMessage = innerMessage
            };
        }
        public ApiResponse Ok(object data) => Ok(data, string.Empty, string.Empty);
        public ApiResponse Ok(string message) => Ok(null, message, string.Empty);
        public ApiResponse Ok(string message, string innerMessage) => Ok(null, message, innerMessage);
        public ApiResponse Ok() => Ok(null, string.Empty, string.Empty);


        public ApiResponse Failed(object data, string message, string innerMessage)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Message = message,
                Data = data,
                InnerMessage = innerMessage
            };
        }
        public ApiResponse Failed(object data) => Failed(data, string.Empty, string.Empty);
        public ApiResponse Failed(string message) => Failed(null, message, string.Empty);
        public ApiResponse Failed(string message, string innerMessage) => Failed(null, message, innerMessage);
        public ApiResponse Failed() => Failed(null, string.Empty, string.Empty);


        protected void Info(string message) => _logger?.LogInformation(message);
        protected void Warning(string message) => _logger?.LogWarning(message);
        protected void Error(string message) => _logger?.LogError(message);

    }
}

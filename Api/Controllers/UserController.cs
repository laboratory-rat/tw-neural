using Domain.User;
using Manager;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwitterConnector;

namespace Api.Controllers
{
    [Route("api/user")]
    [Authorize]
    public class UserController : BaseController
    {
        protected TwitterManager _twitterManager { get; set; }

        public UserController(UserManager managerUser, TwitterManager twitterManager) : base(managerUser)
        {
            _twitterManager = twitterManager;
        }



        [Route("is_auth")]
        [HttpGet]
        public IActionResult IsAuthorized()
        {
            return Ok(true);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("auth/facebook")]
        public async Task<IActionResult> AuthFacebook([FromQuery]string code)
        {
            var response = await _userManager.OAuthByFacebook(code);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("auth/twitter_url")]
        public async Task<IActionResult> GetOauthTwitterUrl()
        {
            var url = _userManager.GetTwitterAuthUrl(Request);
            return Ok(url);
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("auth/twitter")]
        public async Task<IActionResult> OAuthTwitter([FromQuery]string oauthToken, [FromQuery]string oauthVerifier, [FromQuery]string authorizationId)
        {
            var result = await _userManager.OAuthByTwitter(oauthToken, oauthVerifier, authorizationId);
            return Ok(result);
        }

    }
}

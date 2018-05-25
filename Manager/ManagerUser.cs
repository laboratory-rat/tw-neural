using Api.Models;
using AutoMapper;
using Domain.User;
using Facebook;
using Infrastructure;
using Infrastructure.Api.Common;
using Infrastructure.Api.Response.User;
using Infrastructure.Facebook;
using Manager.General;
using Microsoft.AspNetCore.Authentication.OAuth;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Repository;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TwitterConnector;

namespace Manager
{
    public class UserManager : BaseManager
    {
        protected readonly AppDataModel _appData;
        protected FacebookClient GetFacebookClient(string token) => new FacebookClient(token);

        protected TwitterClient _twitterClient { get; set; }

        protected ApiDbContext _context;

        public UserManager(IPrincipal principal, ApiUserStore userStore, IMapper mapper, ApiDbContext context, AppDataModel appData, TwitterClient twitterClient) : base(principal, userStore, mapper)
        {
            _appData = appData;
            _twitterClient = twitterClient;
            _context = context;
        }

        public async Task<ApiResponse<AccessTokenModel>> OAuthByFacebook(string fbShortToken)
        {
            var client = GetFacebookClient(fbShortToken);

            object userModelResponse = null;
            try
            {
                userModelResponse = await client.GetTaskAsync("me", new { fields = new[] { "id", "name", "email", "birthday", "picture", "locale", "first_name", "last_name" } });
            }
            catch (Exception e) { }

            var uModel = FacebookParser.Parse<FacebookUserProfileModel>(userModelResponse);

            if (uModel == null)
                return Failed();

            var dbUser = await _userStore.FindByEmailAsync(uModel.Email.ToLower());
            if (dbUser == null)
            {
                dbUser = new EntityUser
                {
                    UserName = uModel.Email,
                    FirstName = uModel.FirstName,
                    LastName = uModel.LastName,
                    Email = uModel.Email,
                    PictureUrl = uModel.Picture.Data.Url,
                    Birthday = uModel.Birthday,
                    Locale = uModel.Locale,
                    EmailConfirmed = true,
                    NormalizedEmail = uModel.Email.ToLower(),
                    NormalizedUserName = uModel.Email.ToLower()
                };

                await _userStore.CreateAsync(dbUser);
                await _userStore.AddToRoleAsync(dbUser, "user");
            }

            var dbSocial = await _context.UserSocials.FirstOrDefaultAsync(x => x.UserId == dbUser.Id && x.Type == EntityUserSocialType.Facebook && x.ExternalId == uModel.Id);
            if (dbSocial == null)
            {
                dbSocial = new EntityUserSocial
                {
                    ExternalId = uModel.Id,
                    Type = EntityUserSocialType.Facebook,
                    UserId = dbUser.Id,
                };

                await _context.UserSocials.AddAsync(dbSocial);
                await _context.SaveChangesAsync();
            }

            // get long live token
            using (HttpClient http = new HttpClient())
            {
                var longTokenResponse = await http.GetAsync($"https://graph.facebook.com/oauth/access_token?grant_type=fb_exchange_token&client_id=" + _appData.FacebookAppId + "&client_secret=" + _appData.FacebookAppSecret + "&fb_exchange_token=" + fbShortToken);
                if (!longTokenResponse.IsSuccessStatusCode)
                {
                    return Failed();
                }

                var longTokenResponseContent = await longTokenResponse.Content.ReadAsStringAsync();
                var facebookToken = JsonConvert.DeserializeObject<FacebookAccessTokenModel>(longTokenResponseContent);

                if (facebookToken == null)
                {
                    return Failed();
                }

                dbSocial.Token = facebookToken.AccessToken;
                dbSocial.TokenExpires = DateTime.UtcNow.AddSeconds(facebookToken.ExpiresIn);

                _context.UserSocials.Update(dbSocial);
                await _context.SaveChangesAsync();
            }


            var token = await AuthUser(dbUser, EntityUserSocialType.Facebook);

            return Ok(token);
        }

        public string GetTwitterAuthUrl(HttpRequest request)
        {
            return _twitterClient.GetTwitterAuthUrl(request);
        }

        public async Task<ApiResponse<AccessTokenModel>> OAuthByTwitter(string token, string verifier, string authorizationId)
        {
            var uModel = _twitterClient.ValidateTwitterAuth(token, verifier, authorizationId);
            if (uModel == null) return null;

            var dbUser = await _userStore.FindByEmailAsync(uModel.Email.ToLower());
            if (dbUser == null)
            {
                dbUser = new EntityUser
                {
                    UserName = uModel.Email,
                    FirstName = uModel.ScreenName,
                    LastName = "",
                    Email = uModel.Email,
                    PictureUrl = uModel.ProfileImageUrl400x400,
                    Birthday = DateTime.UtcNow,
                    Locale = uModel.Language.ToString(),
                    EmailConfirmed = true,
                    NormalizedEmail = uModel.Email.ToLower(),
                    NormalizedUserName = uModel.Email.ToLower()
                };

                await _userStore.CreateAsync(dbUser);
                await _userStore.AddToRoleAsync(dbUser, "user");
            }

            var dbSocial = await _context.UserSocials.FirstOrDefaultAsync(x => x.UserId == dbUser.Id && x.Type == EntityUserSocialType.Twitter && x.ExternalId == uModel.Id);
            if (dbSocial == null)
            {
                dbSocial = new EntityUserSocial
                {
                    ExternalId = uModel.Id,
                    Type = EntityUserSocialType.Twitter,
                    UserId = dbUser.Id,
                };

                await _context.UserSocials.AddAsync(dbSocial);
                await _context.SaveChangesAsync();
            }

            dbSocial.Token = uModel.Credentials.AccessToken;
            dbSocial.TokenSecret = uModel.Credentials.AccessTokenSecret;
            dbSocial.ConsumerKey = uModel.Credentials.ConsumerKey;
            dbSocial.ConsumerSecret = uModel.Credentials.ConsumerSecret;
            dbSocial.TokenExpires = DateTime.UtcNow.AddYears(100);

            _context.UserSocials.Update(dbSocial);
            await _context.SaveChangesAsync();

            var accessToken = await AuthUser(dbUser, EntityUserSocialType.Twitter);

            return Ok(accessToken);
        }

        public async Task<AccessTokenModel> AuthUser(EntityUser user, EntityUserSocialType provider)
        {
            var identity = await GetUserIdentity(user, provider);
            var now = DateTime.UtcNow;
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                claims: identity.Claims,
                expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
                signingCredentials: new SigningCredentials(AuthOptions.GetKey(), SecurityAlgorithms.HmacSha256));

            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

            return new AccessTokenModel
            {
                Token = encodedJwt,
                Email = user.Email,
                Expires = jwt.ValidTo.Subtract(now).Ticks,
                FirstName = user.FirstName,
                LastName = user.LastName,
                OAuthProvider = provider.ToString(),
                ProfileImageUrl = user.PictureUrl
            };
        }

        protected async Task<ClaimsIdentity> GetUserIdentity(EntityUser user, EntityUserSocialType provider)
        {
            var list = new List<Claim>();

            if (await _userStore.IsInRoleAsync(user, "user"))
            {
                list.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "user"));
            }

            if (await _userStore.IsInRoleAsync(user, "admin"))
            {
                list.Add(new Claim(ClaimsIdentity.DefaultRoleClaimType, "admin"));
            }

            list.Add(new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email));
            list.Add(new Claim("Provider", provider.ToString()));

            return new ClaimsIdentity(list, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}

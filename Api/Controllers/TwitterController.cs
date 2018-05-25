using System.Threading.Tasks;
using Manager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Route("api/twitter")]
    public class TwitterController : BaseController
    {
        protected readonly TwitterManager _manager;

        public TwitterController(UserManager managerUser, TwitterManager manager) : base(managerUser)
        {
            _manager = manager;
        }

        [HttpGet]
        [Route("search/{screenName}")]
        public async Task<IActionResult> Search(string screenName)
        {
            return Ok(await _manager.SearchUser(screenName));
        }

    }
}

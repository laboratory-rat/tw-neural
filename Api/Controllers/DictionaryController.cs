using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Api.Common;
using Manager;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/collections")]
    public class DictionaryController : BaseController
    {
        public DictionaryController(UserManager managerUser) : base(managerUser)
        {
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Get(int count = 10, int page = 1)
        {
            return null;
        }
    }
}

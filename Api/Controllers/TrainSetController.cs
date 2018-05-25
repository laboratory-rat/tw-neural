using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Api.Common;
using Infrastructure.TrainSet;
using Manager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/trainset")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TrainSetController : BaseController
    {
        protected readonly TrainSetManager _trainSet;

        public TrainSetController(UserManager managerUser, TrainSetManager trainSetManager) : base(managerUser)
        {
            _trainSet = trainSetManager;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApiResponse<IdNameModel>))]
        public async Task<IActionResult> Create([FromBody] TrainSetCreateModel model)
        {
            return Ok(await _trainSet.Create(model));
        }

        [HttpGet]
        [Route("{skip}/{take}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<ListModel<TrainSetDisplayModel>>))]
        public async Task<IActionResult> GetList(int take, int skip)
        {
            return Ok(await _trainSet.Get(take, skip));
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<TrainSetDisplayModel>))]
        public async Task<IActionResult> Get(string id)
        {
            return Ok();
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok(await _trainSet.Delete(id));
        }
    }
}

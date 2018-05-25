using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Api.Common;
using Infrastructure.Neural;
using Manager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Route("api/neural")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NeuralController : BaseController
    {
        protected readonly NeuralManager _neuralManager;

        public NeuralController(UserManager managerUser, NeuralManager neuralManager) : base(managerUser)
        {
            _neuralManager = neuralManager;
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApiResponse<IdNameModel>))]
        public async Task<IActionResult> Create([FromBody] NetCreateModel model)
        {
            return Ok(await _neuralManager.Create(model));
        }

        [Route("list/{skip}/{count}")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse<ShortInfoModel>))]
        public async Task<IActionResult> Get(int count, int skip)
        {
            return Ok(await _neuralManager.Get(count, skip));
        }

        [Route("{id}")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(ApiResponse<NeuralNetDisplayModel>))]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _neuralManager.Get(id));
        }

        [Route("{id}")]
        [HttpDelete]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok(await _neuralManager.Delete(id));
        }
    }
}

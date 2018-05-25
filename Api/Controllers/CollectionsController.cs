using Infrastructure;
using Infrastructure.Api.Common;
using Infrastructure.Collections;
using Manager;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/collections")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CollectionsController : BaseController
    {
        protected ICollectionsManager _manager;

        public CollectionsController(UserManager managerUser, ICollectionsManager manager) : base(managerUser)
        {
            _manager = manager;
        }

        [HttpGet]
        [Route("{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<List<CollectionUpdateModel>>))]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(await _manager.Get(id));
        }

        [HttpGet]
        [Route("{skip}/{take}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<ListModel<CollectionDisplayShortModel>>))]
        public async Task<IActionResult> GetCollection(int skip, int take)
        {
            return Ok(await _manager.Get(skip, take));
        }

        [HttpPost]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Create([FromBody]CollectionUpdateModel model)
        {
            if (!ModelState.IsValid)
                return null;

            return Ok(await _manager.Create(model));
        }

        [HttpPut]
        [Route("{id}")]
        [ProducesResponseType(200, Type = typeof(ApiResponse))]
        public async Task<IActionResult> Update(string id, CollectionUpdateModel model)
        {
            if (!ModelState.IsValid)
                return null;

            return Ok(await _manager.Create(model));
        }

        [HttpDelete]
        [Route("{id}")]
        [ProducesResponseType(200, Type =  typeof(ApiResponse))]
        public async Task<IActionResult> Delete(string id)
        {
            return Ok(await _manager.Delete(id));
        }
    }
}

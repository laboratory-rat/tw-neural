using Manager;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Controllers
{
    [Route("api/schedule")]
    [AllowAnonymous]
    public class ScheduleController : Controller
    {
        protected readonly TrainSetManager _trainSetManager;

        public static bool IsTrainSetInProgress = false;

        public ScheduleController(TrainSetManager trainSetManager)
        {
            _trainSetManager = trainSetManager;
        }

        [Route("train_set")]
        [HttpGet]
        [ProducesResponseType(200, Type = typeof(string))]
        public async Task<IActionResult> TrainSet()
        {
            if (IsTrainSetInProgress)
                return Ok("Task in progress");

            IsTrainSetInProgress = true;

            try
            {
                await _trainSetManager.ScheduleUpdate();
            }
            catch (Exception _)
            {
                var logger = new LoggerFactory().CreateLogger("Schedule.TrainSet");
                logger.LogError(_, "Can not schedule update train sets");
                return StatusCode(500, "Server error");
            }
            finally
            {
                IsTrainSetInProgress = false;
            }

            return Ok();
        }
    }
}

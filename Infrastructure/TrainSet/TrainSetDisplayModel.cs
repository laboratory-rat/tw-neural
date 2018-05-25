using Domain.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.TrainSet
{
    public class TrainSetDisplayModel : IdNameModel
    {
        [JsonProperty("update_time")]
        public DateTime UpdateTime { get; set; }

        [JsonProperty("schedule_status")]
        public ScheduleStatus ScheduleStatus { get; set; }

        [JsonProperty("schedule_status_string")]
        public string ScheduleStatusString { get; set; }

        [JsonProperty("schedule_message")]
        public string ScheduleMessage { get; set; }

        [JsonProperty("examples_count")]
        public int ExamplesCount { get; set; }


        public static implicit operator TrainSetDisplayModel(Domain.Neural.TrainSet data)
        {
            if (data == null)
                return null;
            return new TrainSetDisplayModel
            {
                Id = data.Id,
                Name = data.Name,
                ScheduleMessage = data.ScheduleMessage,
                ScheduleStatus = data.ScheduleStatus,
                ScheduleStatusString = data.ScheduleStatus.ToString(),
                UpdateTime = data.UpdatedTime,
                ExamplesCount = data.ExamplesCount
            };
        }
    }
}

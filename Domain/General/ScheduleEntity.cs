using Domain.Enums;
using System;

namespace Domain.General
{
    public class ScheduleEntity : BaseEntity
    {
        public ScheduleStatus ScheduleStatus { get; set; } = ScheduleStatus.New;
        public DateTime LastCheckTime { get; set; } = DateTime.UtcNow;
        public string ScheduleMessage { get; set; }

        public void SetFailed(string message = null) => SetStatus(ScheduleStatus.Failed, message);
        public void SetReady(string message = null) => SetStatus(ScheduleStatus.Ready, message);
        public void SetPending(string message = null) => SetStatus(ScheduleStatus.Pending, message);
        public void SetFreeze(string message = null) => SetStatus(ScheduleStatus.Freeze, message);


        public void SetStatus(ScheduleStatus status, string message = null)
        {
            ScheduleStatus = status;
            ScheduleMessage = message;
            LastCheckTime = DateTime.UtcNow;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.General
{
    public interface IEntityGeneral
    {
        string Id { get; set; }
        DateTime CreatedTime { get; set; }
        DateTime UpdatedTime { get; set; }
        EntityState State { get; set; }
    }

    public class BaseEntity : IEntityGeneral
    {
        public BaseEntity() { }

        public string Id { get; set; }
        public DateTime CreatedTime { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedTime { get; set; } = DateTime.UtcNow;
        public EntityState State { get; set; } = EntityState.Active;
    }

    public enum EntityState
    {
        Active,
        Archived,
        Deleted
    }
}

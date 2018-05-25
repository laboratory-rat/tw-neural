using Domain.Enums;
using Domain.General;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Neural
{
    public class TrainSet : ScheduleEntity
    {
        public string Name { get; set; }

        public string SourceId { get; set; }

        public TrainSetSourceType Type { get; set; }

        public int MaxCount { get; set; }
        public int MinCount { get; set; }

        public int InputWordsCount { get; set; }

        public int ExamplesCount { get; set; } = 0;

        public string StorageKey { get; set; }
        public List<NeuralNet> NeuralNets { get; set; }

        public string UserId { get; set; }
        public EntityUser User { get; set; }
    }

    public enum TrainSetSourceType
    {
        Twitter,
        File,
        RawText
    }
}

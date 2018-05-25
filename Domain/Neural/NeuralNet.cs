using Domain.General;
using Domain.User;
using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Neural
{
    public class NeuralNet : BaseEntity
    {
        public string Name { get; set; }

        public float Skrew { get; set; }


        public NeuralNetType Type { get; set; }
        public NeuralNetFuncType NetFuncType { get; set; }

        public int? Seed { get; set; }

        public int InputCount { get; set; }
        public int HiddenLayersCount { get; set; }
        public int HiddenCount { get; set; }
        public int OutputCount { get; set; }

        public bool IsTrained { get; set; }

        public string StorageKey { get; set; }

        public string TrainSetId { get; set; }  
        public TrainSet TrainSet { get; set; }

        public string UserId { get; set; }
        public EntityUser User { get; set; }
    }

    public enum NeuralNetType
    {
        WORD_SPLIT,
        CUSTOM
    }

    public enum NeuralNetFuncType
    {
        SIGMOID_SIGMOID,
        THAN_THAN
    }
}

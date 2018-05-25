using MRNeural.Interface.Bias;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Domain.Bias
{
    public class NeuralNullBias : NeuralBias, INeuralBias
    {
        [JsonIgnore]
        public override double Signal => 0d;
        public override void Adjust(double delta) { }
    }
}

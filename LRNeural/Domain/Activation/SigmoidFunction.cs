using MRNeural.Interface.Activation;
using MRNeural.Interface.Neuron;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Domain.Activation
{
    public class SigmoidFunction : IActivationFunction
    {
        public SigmoidFunction() { }

        [JsonProperty("skew")]
        protected double _skew { get; set; }

        [JsonIgnore]
        public double Skew => _skew;

        public virtual double Activate(double input) => (1 / (1 + Math.Pow(Math.E, -6 * input)));

        public virtual double Der(double x) => x * (1 - x);

        public virtual IActivationFunction Create(INeuralCell cell, double skew) => new SigmoidFunction
        {
            _skew = skew
        };

        public virtual IActivationFunction OnSerialize() => this;

        public virtual void OnDeserialize(INeuralCell layer) { }
    }
}

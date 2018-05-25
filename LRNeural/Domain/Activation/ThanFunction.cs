using MRNeural.Interface.Activation;
using MRNeural.Interface.Neuron;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Domain.Activation
{
    public class ThanFunction : IActivationFunction
    {
        [JsonIgnore]
        protected INeuralCell _cell { get; set; }

        public double Activate(double input)
        {
            var dInput = input * 2;
            return (Math.Pow(Math.E, dInput) - 1) / (Math.Pow(Math.E, dInput) + 1);
        }

        public IActivationFunction Create(INeuralCell cell, double skew)
        {
            return new ThanFunction
            {
                _cell = cell
            };
        }

        public double Der(double x) => 1 - Math.Pow(x, 2);

        public void OnDeserialize(INeuralCell cell)
        {
            _cell = cell;
        }

        public IActivationFunction OnSerialize() => this;
    }
}

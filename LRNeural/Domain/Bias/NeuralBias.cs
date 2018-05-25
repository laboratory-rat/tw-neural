using MRNeural.Interface.Bias;
using MRNeural.Interface.Layer;
using MRNeural.Interface.Neuron;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Domain.Bias
{
    public class NeuralBias : INeuralBias
    {
        [JsonIgnore]
        protected INeuralLayer _layer;

        [JsonIgnore]
        public INeuralLayer Layer => _layer;

        [JsonIgnore]
        public Random Entropy => _layer?.Entropy;

        [JsonProperty("signal")]
        protected double _signal { get; set; }

        [JsonIgnore]
        public virtual double Signal => _signal * _weight;

        [JsonProperty("weight")]
        protected double _weight { get; set; }

        [JsonIgnore]
        public double Weight => _weight;

        public NeuralBias() { }

        public NeuralBias(INeuralLayer layer)
        {
            _layer = layer;
        }

        public virtual void Adjust(double delta)
        {
            _weight += delta;
        }

        public virtual void Randomize()
        {
            _signal = 1d;
            _weight = Entropy.NextDouble() * 2 - 1d;
        }

        public virtual INeuralBias Create(INeuralLayer layer) => new NeuralBias(layer);

        public INeuralBias OnSerialize() => this;

        public void OnDeserialize(INeuralLayer layer)
        {
            _layer = layer;
        }
    }
}

using MRNeural.Domain.Activation;
using MRNeural.Domain.Bias;
using MRNeural.Domain.ExceptionObject;
using MRNeural.Exceptions;
using MRNeural.Interface.Activation;
using MRNeural.Interface.Bias;
using MRNeural.Interface.Layer;
using MRNeural.Interface.Neuron;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRNeural.Domain.Neuron
{
    public class NeuralCell : INeuralCell
    {

        [JsonIgnore]
        protected INeuralLayer _layer;
        [JsonIgnore]
        public INeuralLayer Layer => _layer;

        [JsonIgnore]
        public Random Entropy => _layer?.Entropy;

        [JsonProperty("weights")]
        protected double[] _weights;
        [JsonIgnore]
        public double[] Weights => _weights;

        [JsonProperty("axons_count")]
        protected int _axonsCount;
        [JsonIgnore]
        public int AxonsCount => _axonsCount;

        [JsonProperty("last_sygnal")]
        protected double[] _lastSygnal;
        [JsonIgnore]
        public double[] LastSygnal => _lastSygnal;

        [JsonProperty("last_out")]
        protected double _lastOut;
        [JsonIgnore]
        public double LastOut => _lastOut;

        [JsonProperty("activation_function")]
        protected IActivationFunction _function;
        [JsonIgnore]
        public IActivationFunction Function =>_function;

        public NeuralCell() { }

        public NeuralCell(INeuralLayer layer, Type func, int axonsCount, double skew)
        {
            _layer = layer;
            _axonsCount = axonsCount;

            _weights = new double[axonsCount];
            for(var i = 0; i < axonsCount; i++)
            {
                _weights[i] = 0;
            }

            _function = ((IActivationFunction)Activator.CreateInstance(func))?.Create(this, skew) ?? throw new NeuralTypeException(typeof(IActivationFunction), func);
        }

        public virtual double Activate(double[] input)
        {
            if (input.Length != _weights.Length)
                throw new NeuronException(this, $"Incorrenct length of input vector ({_weights.Length} but {input.Length})");

            _lastSygnal = input;

            var summ = 0d;
            for (var i = 0; i < input.Length; i++) summ += input[i] * _weights[i];

            _lastOut = Function.Activate(summ);
            return _lastOut;


            /*
            _lastOut = Function.Activate(summ) + _bias.Signal;
            return _lastOut;
            */
        }

        public virtual double Der() => _function.Der(LastOut);

        public virtual double[] Adjust(double delta, double learnReate)
        {
            for(var i = 0; i < AxonsCount; i++)
            {
                _weights[i] += delta * Der() * LastSygnal[i] * learnReate;
            }

            return _weights;
        }


        public virtual void Randomize()
        {
            for(var i = 0; i < _axonsCount; i++)
            {
                _weights[i] = Entropy.NextDouble() - 0.5d;
            }

        }

        public virtual INeuralCell Create(INeuralLayer layer, Type func, int axonsCount, double skew) => new NeuralCell(layer, func, axonsCount, skew);

        public virtual INeuralCell OnSerialize()
        {
            if (_function != null) _function.OnSerialize();

            return this;
        }

        public virtual void OnDeserialize(INeuralLayer layer)
        {
            _layer = layer;

            if (_function != null)
                _function.OnDeserialize(this);
        }
    }
}

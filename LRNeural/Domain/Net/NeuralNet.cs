using MRNeural.Domain.Activation;
using MRNeural.Domain.Bias;
using MRNeural.Domain.ExceptionObject;
using MRNeural.Domain.Layer;
using MRNeural.Domain.Neuron;
using MRNeural.Infrastructure;
using MRNeural.Interface.Activation;
using MRNeural.Interface.Bias;
using MRNeural.Interface.Layer;
using MRNeural.Interface.Net;
using MRNeural.Interface.Neuron;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MRNeural.Domain.Net
{
    public class SNeuralNet : NeuralNet<NeuralLayer, NeuralCell, NeuralBias, SigmoidFunction, NeuralLayer, NeuralCell, SigmoidFunction>, INeuralNet
    {
        public SNeuralNet() { }
        public SNeuralNet(int inputLength, int hiddenLayersCount, int hiddenCount, int outputCount, double skew = -4d, int? seed = null) : base(inputLength, hiddenLayersCount, hiddenCount, outputCount, skew, seed) { }
    }

    public class TNeuralNet : NeuralNet<NeuralLayer, NeuralCell, NeuralBias, ThanFunction, NeuralLayer, NeuralCell, ThanFunction>, INeuralNet
    {
        public TNeuralNet() { }
        public TNeuralNet(int inputLength, int hiddenLayersCount, int hiddenCount, int outputCount, int? seed = null) : base(inputLength, hiddenLayersCount, hiddenCount, outputCount, 0, seed) { }
    }

    public class NeuralNet<HLayer, HCell, HBias, HFunc, OLayer, OCell, OFunc> : INeuralNet
        where HLayer : class, INeuralLayer, new()
        where HCell : class, INeuralCell, new()
        where HBias : class, INeuralBias, new()
        where HFunc : class, IActivationFunction, new()
        where OLayer : class, INeuralLayer, new()
        where OCell : class, INeuralCell, new()
        where OFunc : class, IActivationFunction, new()
    {
        protected int _seed;

        [JsonIgnore]
        public int Seed => _seed;

        protected Random _entropy { get; set; }

        [JsonIgnore]
        public Random Entropy => _entropy;

        [JsonProperty("input_length")]
        protected int _inputLength { get; set; }

        [JsonIgnore]
        public int InputLength => _inputLength;

        [JsonProperty("hidden_layers")]
        protected INeuralLayer[] _hiddenLayers { get; set; }

        [JsonProperty("output_layer")]
        protected INeuralLayer _outputLayer { get; set; }

        [JsonIgnore]
        public INeuralLayer[] Hidden => _hiddenLayers;
        [JsonIgnore]
        public INeuralLayer Output => _outputLayer;

        [JsonIgnore]
        public int HiddenLayersCount => _hiddenLayers?.Count() ?? 0;

        public NeuralNet() { }

        public NeuralNet(int inputLength, int hiddenLayersCount, int hiddenCount, int outputCount, double skew = -4d, int? seed = null) 
        {
            _inputLength = inputLength;

            _hiddenLayers = new INeuralLayer[hiddenLayersCount];

            INeuralLayer clearHLayer = (INeuralLayer)Activator.CreateInstance(typeof(HLayer)) ?? throw new NeuralTypeException(typeof(INeuralLayer), typeof(HLayer));
            INeuralLayer clearOLayer = (INeuralLayer)Activator.CreateInstance(typeof(OLayer)) ?? throw new NeuralTypeException(typeof(INeuralLayer), typeof(OLayer));


            for (var i = 0; i < hiddenLayersCount; i++)
            {
                var z = i == 0 ? InputLength : _hiddenLayers[i - 1].NeuronsCount + 1;
                _hiddenLayers[i] = clearHLayer.Create(this, typeof(HCell), typeof(HBias), typeof(HFunc), hiddenCount, z, skew);
            }

            _outputLayer = clearOLayer.Create(this, typeof(OCell), typeof(HBias), typeof(OFunc), outputCount, _hiddenLayers.Last().NeuronsCount + 1, skew);

            if (seed.HasValue)
            {
                _entropy = new Random(seed.Value);
            }
            else
            {
                _entropy = new Random();
            }
        }

        public virtual void Randomize()
        {
            if(_hiddenLayers != null)
            {
                foreach(var h in _hiddenLayers)
                {
                    h.Randomize();
                }
            }

            if(_outputLayer != null)
            {
                _outputLayer.Randomize();
            }
        }

        public virtual double[] Activate(double[] input)
        {
            input = ActivateHidden(input);
            return ActivateOutput(input);
        }

        public void Adjust(double[] correct, double rate)
        {
            var adjOut = _outputLayer.AdjustOutput(correct, rate);

            for(var i = HiddenLayersCount - 1; i >= 0; i--)
            {
                adjOut = Hidden[i].AdjustHidden(adjOut.Item1, adjOut.Item2, rate);
            }
        }

        public INeuralNet OnSerialize()
        {
            if(Hidden != null)
            {
                foreach (var h in Hidden)
                {
                    h.OnSerialize();
                }
            }
            
            if(_outputLayer != null)
            {
                _outputLayer.OnDeserialize(this);
            }

            return this;
        }

        public INeuralNet OnDeserialize()
        {
            if(_seed != 0)
            {
                _entropy = new Random(_seed);
            }
            else
            {
                _entropy = new Random();
            }

            if(_hiddenLayers != null)
            {
                foreach(var l in _hiddenLayers)
                {
                    l.OnDeserialize(this);
                }
            }

            if(_outputLayer != null)
            {
                _outputLayer.OnDeserialize(this);
            }

            return this;
        }

        protected virtual double[] ActivateHidden(double[] input)
        {
            foreach(var i in _hiddenLayers)
            {
                input = i.Activate(input);
            }

            return input;
        }

        protected virtual double[] ActivateOutput(double[] input)
        {
            return _outputLayer.ActivateOutput(input);
        }

    }
}

using MRNeural.Domain.ExceptionObject;
using MRNeural.Domain.Neuron;
using MRNeural.Exceptions;
using MRNeural.Interface.Activation;
using MRNeural.Interface.Bias;
using MRNeural.Interface.Layer;
using MRNeural.Interface.Net;
using MRNeural.Interface.Neuron;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MRNeural.Domain.Layer
{
    public class NeuralLayer : INeuralLayer
    {
        [JsonProperty("self_type")]
        protected string _selfType { get; set; }

        [JsonIgnore]
        protected INeuralNet _net;

        [JsonIgnore]
        public INeuralNet Net => _net;

        [JsonIgnore]
        public Random Entropy => _net?.Entropy;

        [JsonProperty("input_length")]
        protected int _inputLength;

        [JsonIgnore]
        public int InputLength => _inputLength;

        [JsonProperty("neural_cells")]
        protected INeuralCell[] _neurons;

        [JsonIgnore]
        public INeuralCell[] Neurons => _neurons;
        [JsonIgnore]
        public int NeuronsCount => Neurons?.Count() ?? 0;

        [JsonProperty("answer_history")]
        public List<double[]> AnswerHistory { get; set; }

        [JsonIgnore]
        public double[] LastAnswer => AnswerHistory == null || !AnswerHistory.Any() ? new double[0] : AnswerHistory.Last();

        [JsonProperty("save_hystory_length")]
        public int SaveHystoryLength { get; set; } = 3;

        protected INeuralBias _bias;
        public INeuralBias Bias => _bias;

        public NeuralLayer() { }

        public NeuralLayer(INeuralNet net, Type cell, Type bias, Type func, int neuronsCount, int connectionsCount, double skew)
        {
            _net = net;
            _neurons = new INeuralCell[neuronsCount];
            AnswerHistory = new List<double[]>();
            _inputLength = neuronsCount;

            var clearCell = (INeuralCell)Activator.CreateInstance(cell);

            _bias = ((INeuralBias)Activator.CreateInstance(bias))?.Create(this) ?? throw new NeuralTypeException(typeof(INeuralBias), bias);

            for (var i = 0; i < neuronsCount; i++)
            {
                _neurons[i] = clearCell.Create(this, func, connectionsCount, skew);
            }
        }

        public double[] Activate(double[] input)
        {
            double[] answer = new double[NeuronsCount + 1];
            for(var i = 0; i < NeuronsCount; i++)
            {
                answer[i] = _neurons[i].Activate(input);
            }

            answer[answer.Length - 1] = _bias.Signal;

            AnswerHistory.Add(answer);

            if(AnswerHistory.Count > 3)
            {
                AnswerHistory.RemoveAt(0);
            }

            return answer;
        }

        public double[] ActivateOutput(double[] input)
        {
            double[] answer = new double[NeuronsCount];
            for (var i = 0; i < NeuronsCount; i++)
            {
                answer[i] = _neurons[i].Activate(input);
            }

            AnswerHistory.Add(answer);

            if (AnswerHistory.Count > 3)
            {
                AnswerHistory.RemoveAt(0);
            }

            return answer;
        }

        public virtual Tuple<double[], double[][]> AdjustOutput(double[] correct, double rate)
        {
            var weights = new double[NeuronsCount][];
            var deltas = new double[correct.Length];

            for (var i = 0; i < correct.Length; i++)
            {
                deltas[i] = correct[i] - _neurons[i].LastOut;
                weights[i] = _neurons[i].Adjust(deltas[i], rate);
            }

            return new Tuple<double[], double[][]>(deltas, weights);
        }

        public virtual Tuple<double[], double[][]> AdjustHidden(double[] deltas, double[][] forwardWeights, double rate)
        {
            var d = new double[NeuronsCount];
            var w = new double[NeuronsCount][];

            for(var i = 0; i < NeuronsCount; i++)
            {
                var neuron = _neurons[i];
                var gamma = 0d;

                for(var j = 0; j < forwardWeights.Length; j++)
                {
                    gamma += deltas[j] * forwardWeights[j][i];
                }

                d[i] = gamma;
                w[i] = neuron.Adjust(gamma, rate);
            }


            _bias.Adjust(d.Sum() / d.Count() * rate);

            return new Tuple<double[], double[][]>(d, w);
        }
        
        public virtual void Randomize()
        {
            if(_neurons != null)
            {
                foreach(var n in _neurons)
                {
                    n.Randomize();
                }
            }
        }

        public virtual INeuralLayer Create(INeuralNet net, Type cell, Type bias, Type func, int neuronsCount, int connectionsCount, double skew) => new NeuralLayer(net, cell, bias, func, neuronsCount, connectionsCount, skew);

        public virtual INeuralLayer OnSerialize()
        {
            _selfType = GetType().ToString();
            if(_neurons != null)
            {
                foreach(var n in _neurons)
                {
                    n.OnSerialize();
                }
            }

            return this;
        }
        public virtual void OnDeserialize(INeuralNet net)
        {
            _net = net;

            if(_neurons != null)
            {
                foreach(var n in _neurons)
                {
                    n.OnDeserialize(this);
                }
            }
        }
    }
}

using MRNeural.Interface.Activation;
using MRNeural.Interface.Bias;
using MRNeural.Interface.Layer;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Interface.Neuron
{
    public interface INeuralCell
    {
        Random Entropy { get; }

        INeuralLayer Layer { get; }

        double[] Weights { get; }
        int AxonsCount { get; }

        IActivationFunction Function { get; }

        double[] LastSygnal { get; }
        double LastOut { get; }

        void Randomize();
        double Activate(double[] input);
        double Der();
        double[] Adjust(double gamma, double learnRate);

        INeuralCell Create(INeuralLayer layer, Type func, int axonsCount, double skew);

        INeuralCell OnSerialize();
        void OnDeserialize(INeuralLayer layer);
    }
}

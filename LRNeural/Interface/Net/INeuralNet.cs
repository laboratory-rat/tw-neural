using MRNeural.Interface.Layer;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Interface.Net
{
    public interface INeuralNet
    {
        Random Entropy { get; }
        int Seed { get; }
        int HiddenLayersCount { get; }
        int InputLength { get; }

        INeuralLayer[] Hidden { get; }
        INeuralLayer Output { get; }

        double[] Activate(double[] input);
        void Adjust(double[] delta, double rate);
        void Randomize();

        INeuralNet OnSerialize();
        INeuralNet OnDeserialize();
    }
}

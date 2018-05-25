using MRNeural.Interface.Layer;
using MRNeural.Interface.Neuron;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Interface.Bias
{
    public interface INeuralBias
    {
        INeuralLayer Layer { get; }
        Random Entropy { get; }
        double Signal { get; }
        double Weight { get; }

        void Adjust(double delta);
        void Randomize();

        INeuralBias Create(INeuralLayer layer);

        INeuralBias OnSerialize();
        void OnDeserialize(INeuralLayer layer);
    }
}

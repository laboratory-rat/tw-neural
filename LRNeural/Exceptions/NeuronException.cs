using MRNeural.Interface.Neuron;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Exceptions
{
    class NeuronException : Exception
    {
        public INeuralCell Neuron { get; set; }

        public NeuronException(INeuralCell neuron, string message) : base(message)
        {
            Neuron = neuron;
        }
    }
}

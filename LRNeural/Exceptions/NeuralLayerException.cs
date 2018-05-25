using MRNeural.Interface.Layer;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Exceptions
{
    public class NeuralLayerException : Exception
    {
        public INeuralLayer Layer { get; set; }
        public double[] Input { get; set; }
        public NeuralLayerException(INeuralLayer layer, double[] input, string message) : base(message)
        {
            Layer = layer;
        }
    }
}

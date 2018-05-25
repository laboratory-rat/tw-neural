using MRNeural.Interface.Neuron;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Interface.Activation
{
    public interface IActivationFunction
    {
        double Activate(double input);
        double Der(double x);

        IActivationFunction Create(INeuralCell cell, double skew);

        IActivationFunction OnSerialize();
        void OnDeserialize(INeuralCell layer);
    }
}

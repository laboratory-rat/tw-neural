using MRNeural.Interface.Activation;
using MRNeural.Interface.Bias;
using MRNeural.Interface.Net;
using MRNeural.Interface.Neuron;
using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Interface.Layer
{
    public interface INeuralLayer
    {
        INeuralNet Net { get; }

        Random Entropy { get; }

        int InputLength { get; }

        INeuralBias Bias { get; }

        INeuralCell[] Neurons { get; }
        int NeuronsCount { get; }

        List<double[]> AnswerHistory { get; set; }
        double[] LastAnswer { get; }
        int SaveHystoryLength { get; set; }

        double[] Activate(double[] input);
        double[] ActivateOutput(double[] input);
        void Randomize();

        Tuple<double[], double[][]> AdjustOutput(double[] deltas, double rate);

        Tuple<double[], double[][]> AdjustHidden(double[] delltasForfard, double[][] weightsForward, double rate);

        INeuralLayer Create(INeuralNet net, Type cell, Type bias, Type func, int neuronsCount, int connectionsCount, double skew);

        INeuralLayer OnSerialize();
        void OnDeserialize(INeuralNet net);
    }
}

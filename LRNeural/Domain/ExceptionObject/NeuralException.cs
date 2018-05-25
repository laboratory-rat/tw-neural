using System;
using System.Collections.Generic;
using System.Text;

namespace MRNeural.Domain.ExceptionObject
{
    public class NeuralException : Exception
    {
        public NeuralException() { }

        public NeuralException(string Message)
        {

        }
    }

    public class NeuralTypeException : NeuralException
    {
        public NeuralTypeException(Type expected, Type get) : base($"Bad type get. Expected: {expected.Name} / Get: {get.Name}") { }
    }

}

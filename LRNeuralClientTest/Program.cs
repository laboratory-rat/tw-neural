using System;
using System.Threading.Tasks;

namespace LRNeuralClientTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var dic = new VocabularyTests(Log);
            var n = new NeuralTest();
            n.TestWords((string log) => Log(log));

            Console.WriteLine();
            Console.WriteLine("Program finished");
            Console.ReadKey();
        }

        public static void Log(string message)
        {
            Console.WriteLine(message);
        }
    }
}

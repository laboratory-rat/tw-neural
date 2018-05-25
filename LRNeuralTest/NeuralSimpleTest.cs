using Microsoft.VisualStudio.TestTools.UnitTesting;
using MRNeural.Domain.Activation;
using MRNeural.Domain.Bias;
using MRNeural.Domain.Layer;
using MRNeural.Domain.Net;
using MRNeural.Domain.Neuron;
using MRNeural.Domain.Trainer;
using MRNeural.Interface.Net;
using MRNeural.Tools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Word2vec.Tools;

namespace MRNeuralTest
{
    [TestClass]
    public class NeuralSimpleTest
    {
        const string FILE_PATH = "d://neural.txt";

        public SNeuralNet[] Nets { get; set; } = new SNeuralNet[]
        {
            new SNeuralNet(2, 1, 2, 1),
            new SNeuralNet(2, 2, 2, 1),
            new SNeuralNet(2, 1, 10, 1),
            new SNeuralNet(2, 2, 10, 1),
            new SNeuralNet(2, 3, 2, 1),
            new SNeuralNet(2, 3, 10, 1),
        };

        public INeuralNet NetSimple11000 = new TNeuralNet(2, 1, 50000, 1); // new NeuralNet<NeuralLayer, NeuralCell, NeuralNullBias, SigmoidFunction, NeuralLayer, NeuralCell, SigmoidFunction>(2, 2, 100, 1, -8);

        public TNeuralNet[] NetsWordTest = new TNeuralNet[]
        {
            new TNeuralNet(900, 2, 900, 300),
            new TNeuralNet(900, 4, 900, 300),
            new TNeuralNet(900, 2, 2000, 300),
            new TNeuralNet(900, 4, 2000, 300),
        };

        [TestMethod]
        public void TestSimpleTask()
        {
            foreach (var net in Nets)
            {
                Trace.WriteLine($"layers: {net.HiddenLayersCount}\tneurons: {net.Hidden.First().NeuronsCount}");
                var trainer = new NeuralNetTrainer(net, Consts.XorSet, 10000, 0.01, 0.01, 5, Consts.TraceLog);
                trainer.SimpleTrain();

                foreach (var e in Consts.XorSet)
                {
                    var response = net.Activate(e.Item1);
                    Trace.WriteLine($"{e.Item1[0]} - {e.Item1[1]}\tResponse: {response[0]}\tCorrect: {e.Item2[0]}");
                }
            }
        }

        [TestMethod]
        public async Task TestSimple1100Task()
        {
            var net = NetSimple11000;
            var trainer = new NeuralNetTrainer(net, Consts.XorSet, 50, 0.001,  0.0001, 1, Consts.TraceLog);
            var result = trainer.SimpleTrain();

            foreach (var e in Consts.XorSet)
            {
                var response = net.Activate(e.Item1);
                Trace.WriteLine($"{e.Item1[0]} - {e.Item1[1]}\tResponse: {response[0]}\tCorrect: {e.Item2[0]}");
            }

            if(result.ResultError < 0.2)
            {
                await MRSerializer.ToFile(FILE_PATH, result.TargetNet, true);
            }
        }

        [TestMethod]
        public async Task DeserializeNet()
        {
            try
            {
                var net = await MRSerializer.FromFile<NeuralNet<NeuralLayer, NeuralCell, NeuralBias, SigmoidFunction, NeuralLayer, NeuralCell, SigmoidFunction>>(FILE_PATH);

            }
            catch(Exception ex)
            {
                var e = ex;
            }
        }

        [TestMethod]
        public async Task NeuralWordsTest()
        {
            // get GAME OF THRONES

            string regexText = string.Empty;

            try
            {
                var fullText = await File.ReadAllTextAsync(Consts.GAME_OF_THRONES_PATH);
                regexText = new Regex("Page [0-9]+").Replace(fullText, string.Empty);
            }
            catch(Exception ex)
            {
                var e = ex;
            }


            var vReader = new VReader(Consts.VOCAB_PATH);
            vReader.UploadBinary();

            var bag = MRWordBag.CreateToWords(regexText, 4);

            // create traine vectors
            var allSet = new List<Tuple<double[], double[]>>();

            foreach(var step in bag.Read())
            {
                bool isValid = true;
                foreach(var v in step)
                {
                    if (!vReader.Vocab.ContainsWord(v) || !vReader.Vocab.ContainsWord(v))
                    {
                        isValid = false;
                        break;
                    }
                }

                if (!isValid)
                    continue;

                var forInput = step.Take(3);
                List<double> input = new List<double>();
                foreach(var i in forInput)
                {
                    input.AddRange(vReader.Vocab.GetRepresentationFor(i).NumericVector.Select(x => (double)x).ToList());
                }

                var forOut = step.Last();
                double[] output = vReader.Vocab.GetRepresentationFor(forOut).NumericVector.Select(x => (double)x).ToArray();

                allSet.Add(new Tuple<double[], double[]>(input.ToArray(), output));
            }

            var trainSet = allSet.Take(allSet.Count - 10).ToArray();
            var checkSet = allSet.TakeLast(10).ToArray();

            var trainRates = new double[] { 0.00005d, 0.00001d };

            foreach(var rate in trainRates)
            {
                foreach(var net in NetsWordTest)
                {
                    Trace.WriteLine($"Train net: layers: {net.HiddenLayersCount} | neurons: {net.Hidden.First().NeuronsCount}\tRate: {rate}");
                    var trainer = new NeuralNetTrainer(net, trainSet, 500, 1, rate, 1, Consts.TraceLog);
                    var trainResult = trainer.SimpleTrain();

                    Trace.WriteLine("-- check net --");
                    foreach(var s in checkSet)
                    {
                        var response = net.Activate(s.Item1);

                        var responseR = new Representation(response.Select(x => (float)x).ToArray());
                        var responseWord = vReader.Vocab.Distance(responseR, 1)?.FirstOrDefault()?.Representation;

                        var correct = vReader.Vocab.Distance(new Representation(s.Item2.Select(x => (float)x).ToArray()), 1)?.FirstOrDefault()?.Representation;

                        Trace.WriteLine($"Correct: {correct.WordOrNull}\tResponse: {responseWord.WordOrNull}");
                    }

                    var name = $"Neural net ({net.HiddenLayersCount}-{net.Hidden.First().NeuronsCount}-epochs-{trainResult.EpochFinished}-error-{trainResult.ResultError}-time-{trainResult.TotalTimeMs})";
                    await MRSerializer.ToFile($"d://{name}.txt", net, true);
                }
            }


        }

        [TestMethod]
        public void CryptDecryptWord()
        {
            var reader = new VReader(Consts.VOCAB_PATH);
            reader.UploadBinary();

            var w = reader.Vocab.GetRepresentationFor("Hello");
            var ww = new Representation(w.NumericVector);


            var www = reader.Vocab.Distance(ww, 1).FirstOrDefault()?.Representation.WordOrNull;

            Trace.WriteLine(www);
        }
    }
}

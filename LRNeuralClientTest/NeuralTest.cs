using MRNeural;
using MRNeural.Domain.Net;
using MRNeural.Domain.Trainer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Word2vec.Tools;

namespace LRNeuralClientTest
{
    public class NeuralTest
    {


        List<string> Phrases = new List<string> { "Silent march in Paris after 'anti-Semitic' murder", "Aziza Oubaita on the fight of her life", "UK police probing poisoning of ex-spy and daughter believe first contact with nerve agent came from their front door", "Hey guys. Repeat after me, please! Data is not the new oil. Data is not the new oil. Data is not the new oil. Data is not the new oil." };

        public void TestNeuralCreate()
        {
            var sets = new Tuple<double[], double[]>[]
                {
                    new Tuple<double[], double[]>(new double[]{ -0.066 }, new double[]{ 0.09 }),
                    new Tuple<double[], double[]>(new double[]{ -0.1 }, new double[]{ 0.18 }),
                    new Tuple<double[], double[]>(new double[]{ 0.5 }, new double[]{ 0.27 }),
                    new Tuple<double[], double[]>(new double[]{ 0.7 }, new double[]{ 0.36 }),
                    new Tuple<double[], double[]>(new double[]{ 0.04 }, new double[]{ 0.45 }),
                    new Tuple<double[], double[]>(new double[]{ 0.035 }, new double[]{ 0.54 }),
                    new Tuple<double[], double[]>(new double[]{ 0.03 }, new double[]{ 0.63 }),
                    new Tuple<double[], double[]>(new double[]{ 0.038 }, new double[]{ 0.72 }),
                    new Tuple<double[], double[]>(new double[]{ 0.06 }, new double[]{ 0.81 }),
                    new Tuple<double[], double[]>(new double[]{ 0.02 }, new double[]{ 0.90 }),
                    new Tuple<double[], double[]>(new double[]{ 0.1 }, new double[]{ 0.99 }),
                    new Tuple<double[], double[]>(new double[]{ -0.2 }, new double[]{ 1.08 })
                };

            var set2 = new Tuple<double[], double[]>[]
            {
                    new Tuple<double[], double[]>(new double[]{ 0, 1 }, new double[]{ 1 }),
                    new Tuple<double[], double[]>(new double[]{ 1, 0 }, new double[]{ 1 }),
                    new Tuple<double[], double[]>(new double[]{ 0, 0 }, new double[]{ 0 }),
                    new Tuple<double[], double[]>(new double[]{ 1, 1 }, new double[]{ 0 }),
            };

            var net = new SNeuralNet(1, 1, 100, 1);
            var trainer = new NeuralNetTrainer()
                .SetNet(net)
                .SetDataSets(sets);

            trainer.EpochsCount = 100000;
            trainer.LearnRate = 0.01;

            trainer.SimpleTrain();

            Console.WriteLine("");
            Console.WriteLine("Test");

            foreach(var t in sets)
            {
                var input = t.Item1;
                var correctOut = t.Item2;

                var @out = trainer.Net.Activate(input);

                Console.WriteLine($"Input: {input[0]}\tOut: {@out[0]}\tCorrect: {correctOut[0]}");
            }


        }

        public void TestWords(Log log)
        {
            var net = new SNeuralNet(6, 2, 300, 1);
            var wordBag = WordBag.CreateToWords(string.Join(". ", Configuration.RawDataList), 1);

            var trainSets = new List<Tuple<double[], double[]>>();

            var wordsHistory = new List<string>();
            var vocab = new LRVocab().Create(Configuration.VocabularyPath, (string s) => log(s));

            log("Prepare tests list");

            foreach(var word in wordBag.Read())
            {
                var w = word[0];

                if (!vocab.Vocabulary.ContainsWord(w)) continue;

                wordsHistory.Add(w);
                if (wordsHistory.Count < 4) continue;
                if (wordsHistory.Count > 4) wordsHistory.RemoveAt(0);

                double[] input = new double[6];
                double[] correct = new double[1];

                input[0] = vocab.Vocabulary.GetRepresentationOrNullFor(wordsHistory[0]).MetricLength;
                input[1] = vocab.Vocabulary.GetRepresentationOrNullFor(wordsHistory[1]).MetricLength;
                input[2] = vocab.Vocabulary.GetRepresentationOrNullFor(wordsHistory[2]).MetricLength;

                input[3] = vocab.Vocabulary.GetSummRepresentationOrNullForPhrase(wordsHistory.Take(2).ToArray())?.MetricLength ?? 0d;
                input[4] = vocab.Vocabulary.GetSummRepresentationOrNullForPhrase(wordsHistory.Skip(1).Take(2).ToArray())?.MetricLength ?? 0d;
                input[5] = vocab.Vocabulary.GetSummRepresentationOrNullForPhrase(wordsHistory.Take(3).ToArray())?.MetricLength ?? 0d;

                correct = new double[] { vocab.Vocabulary.GetRepresentationFor(wordsHistory[3]).MetricLength };

                trainSets.Add(new Tuple<double[], double[]>(input, correct));
            }

            if (trainSets.Count == 0)
            {
                log("No train sets");
                return;
            }

            log($"Train sets count: {trainSets.Count}");
            log($"Train starts");

            var trainer = new NeuralNetTrainer()
                .SetDataSets(trainSets.ToArray())
                .SetNet(net);

            trainer.EpochsCount = 150;
            trainer.LearnRate = 0.001;

            trainer.SimpleTrain();
            log("Train end");

            foreach(var set in trainSets)
            {
                log($"Input: {set.Item1[0]} {set.Item1[1]} {set.Item1[2]}\t Result: {net.Activate(set.Item1)}");
            }
        }

        public void Test(Log log)
        {
            var voc = new LRVocab().Create("D:/vectors/google_vokab.bin", (data) => log(data));
            foreach (var p in Phrases)
            {
                var wb = WordBag.CreateToWords(p, 3);
                var pb = WordBag.CreateToPhrases(p, 1);

                log($"Process phrase '{p}'");

                List<Representation> wordsResults = new List<Representation>();
                List<Representation> phrasesResults = new List<Representation>();


                log("ToWords representation");

                foreach (var data in wb.Read())
                {
                    var s1 = voc.Vocabulary.GetSummRepresentationOrNullForPhrase(data[0]);
                    var s2 = voc.Vocabulary.GetSummRepresentationOrNullForPhrase(data[1]);
                    var s3 = voc.Vocabulary.GetSummRepresentationOrNullForPhrase(data[2]);

                    Representation[] s = { s1, s2, s3 };
                    Representation result = null;

                    foreach (var ss in s)
                    {
                        if (ss == null) continue;
                        if (result == null)
                        {
                            result = ss;
                        }
                        else
                        {
                            result.Add(ss);
                        }
                    }



                    if (result == null) continue;

                    log($"{data[0]} + {data[1]} + {data[2]} => {result.WordOrNull} ({result.MetricLength})");

                    if (!string.IsNullOrWhiteSpace(result.WordOrNull))
                    {
                        wordsResults.Add(result);
                    }
                }

                log("words results:");
                log(string.Join(" ", wordsResults.Select(x => x.WordOrNull)));
                log("");

                log("phrases represuntation");
                foreach (var data in pb.Read())
                {
                    var result = voc.Vocabulary.GetSummRepresentationOrNullForPhrase(data);
                    if (result == null) continue;

                    log($"{data[0]} => {result.WordOrNull} ({result.MetricLength})");
                    if (!string.IsNullOrWhiteSpace(result.WordOrNull))
                    {
                        phrasesResults.Add(result);
                    }
                }
                log("phrase results:");
                log(string.Join(" ", phrasesResults.Select(x => x.WordOrNull)));
                log("");
                log("------");
            }
        }
    }
}

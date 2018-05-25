using MRNeural;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LRNeuralClientTest
{
    public class VocabularyTests
    {
        const string VOCAB_PATH = "D:/vectors/google_vokab.bin";

        public Log log { get; set; }

        public VocabularyTests(Log log)
        {
            this.log = log;
        }

        public async Task Test()
        {
            var vocab = new LRVocab().Create(VOCAB_PATH, (x) => log.Invoke(x));

            if (!vocab.IsReady) return;

            log($"Words count: {vocab.Vocabulary.Words}");
            log($"Size: {vocab.Vocabulary.VectorDimensionsCount}");

            var boyToGirl = vocab.Vocabulary.GetSummRepresentationOrNullForPhrase("An unusual galaxy far, far away is stumping astronomers not because of what’s there, but because of what’s missing");
            log($"Analogy: {boyToGirl.WordOrNull}");
        }
    }

    public delegate void Log(string log);
}

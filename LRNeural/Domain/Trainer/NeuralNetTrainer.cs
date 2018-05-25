using MRNeural.Interface.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace MRNeural.Domain.Trainer
{
    public delegate void ProgressLog(int TotalEpochs, int currentEpoch, double tolerableError, double learnRate, double erorr);

    public class NeuralNetTrainer
    {
        public INeuralNet Net { get; set; }
        public Tuple<double[], double[]>[] DataSets { get; set; }
        public int EpochsCount { get; set; } = 1000;
        public double TolerableError { get; set; } = 0.01d;
        public double LearnRate { get; set; } = 0.001d;

        public int ErrorWriteCycle { get; set; } = 10;
        public ProgressLog Log { get; set; }

        public NeuralNetTrainer() { }

        public NeuralNetTrainer(INeuralNet net)
        {
            Net = net;
        }

        public NeuralNetTrainer(Tuple<double[], double[]>[] dataSets)
        {
            DataSets = dataSets;
        }

        public NeuralNetTrainer(INeuralNet net, Tuple<double[], double[]>[] dataSets) : this(net)
        {
            DataSets = dataSets;
        }

        public NeuralNetTrainer(INeuralNet net, Tuple<double[], double[]>[] dataSets, int? epochCount, double? tolerableError, double? learnRate) : this(net, dataSets)
        {
            if (epochCount.HasValue)
                EpochsCount = epochCount.Value;

            if (tolerableError.HasValue)
                TolerableError = tolerableError.Value;

            if (learnRate.HasValue)
                LearnRate = learnRate.Value;
        }

        public NeuralNetTrainer(INeuralNet net, Tuple<double[], double[]>[] dataSets, int? epochCount, double? tolerableError, double? learnRate, int? logCycle, ProgressLog logMethod) : this(net, dataSets, epochCount, tolerableError, learnRate)
        {
            if (logCycle.HasValue)
                ErrorWriteCycle = logCycle.Value;

            Log = logMethod;
        }

        public NeuralNetTrainer SetNet(INeuralNet net)
        {
            Net = net;
            return this;
        }

        public NeuralNetTrainer SetDataSets(Tuple<double[], double[]>[] dataSets)
        {
            DataSets = dataSets;
            return this;
        }

        public NeuralTrainResult SimpleTrain()
        {
            if (DataSets == null || Net == null) return null;
            Net.Randomize();

            var result = new NeuralTrainResult
            {
                EpochCount = EpochsCount,
                ErrorWriteCycle = ErrorWriteCycle,
                Start = DateTime.UtcNow,
                TolerableError = TolerableError,
                TrainSets = DataSets,
                TargetNet = Net,
                ErrorHistory = new List<double>(),
                EpochFinished = 0,
                LearnRate = LearnRate
            };

            var counter = EpochsCount;
            var totalError = 0d;

            for (; counter > 0; counter--)
            {
                result.EpochFinished++;
                totalError = 0d;

                for(var i = 0; i < DataSets.Length; i++) 
                {
                    var input = DataSets[i].Item1;
                    var correctOut = DataSets[i].Item2;

                    var currentOut = Net.Activate(input);
                    var outDelta = new double[currentOut.Length];

                    for(var j = 0; j < outDelta.Length; j++)
                    {
                        outDelta[j] = correctOut[j] - currentOut[j];
                        totalError += Math.Pow(outDelta[j], 2);
                    }

                    Net.Adjust(correctOut, LearnRate);
                }

                totalError /= DataSets.Length;

                if(Log != null && counter % ErrorWriteCycle == 0)
                {
                    result.ErrorHistory.Add(totalError);
                    Log(EpochsCount, EpochsCount - counter, TolerableError, LearnRate, totalError);
                }


                if (totalError <= TolerableError) break;
            }

            result.ResultError = totalError;
            result.Stop = DateTime.UtcNow;

            return result;
        }
    }

    public class NeuralTrainResult
    {
        public DateTime Start { get; set; }
        public DateTime Stop { get; set; }

        [JsonIgnore]
        public double TotalTimeMs => Stop.Subtract(Start).TotalMilliseconds;

        public int EpochCount { get; set; }
        public int EpochFinished { get; set; }

        public double TolerableError { get; set; }
        public double ResultError { get; set; }

        public double LearnRate { get; set; }

        public int ErrorWriteCycle { get; set; }
        public List<double> ErrorHistory { get; set; }


        public INeuralNet TargetNet { get; set; }
        public Tuple<double[], double[]>[] TrainSets { get; set; }

        [JsonIgnore]
        public int TrainSetsCount => TrainSets?.Length ?? 0;
    }
}

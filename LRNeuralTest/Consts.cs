using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MRNeuralTest
{
    public static class Consts
    {
        public static Tuple<double[], double[]>[] XorSet = new Tuple<double[], double[]>[]
        {
            new Tuple<double[], double[]>(new double[]{0d, 0d}, new double[]{0d }),
            new Tuple<double[], double[]>(new double[]{0d, 1d}, new double[]{1d }),
            new Tuple<double[], double[]>(new double[]{1d, 0d}, new double[]{1d }),
            new Tuple<double[], double[]>(new double[]{1d, 1d}, new double[]{0d }),
        };

        public static void TraceLog(int epochs, int current, double toleranceError, double learnRate, double currentError)
        {
            Trace.WriteLine($"({epochs} : {current})\tRate:{learnRate}\t{currentError} : {toleranceError}");
        }

        public static List<string> RAW_WORDS_LIST = new List<string>
        {
            "Gunmen 'abduct mayor of Tripoli Abdelraouf Beitelmal'",
            "Australia coach quits over cheating scandal",
            "Uganda newspaper's 'wine for abuse stories' contest angers readers",
            "China singer denied bank account because he’s blind",
            "Russian spy: Yulia Skripal 'improving rapidly'",
            "PAOK Salonika's president Ivan Savvidis has been banned for three years for coming onto the pitch with a gun.",
            "Trump calls to congratulate TV's Roseanne",
            "Australian ball-tampering: Steve Smith, David Warner and Cameron Bancroft apologise",
            "Trump attacks Amazon for paying 'little or no taxes'",
            "VW to buy back cars hit by German city diesel bans",
            "PAOK Salonika: Gun incident sees Greek club's president banned"
        };

        public static string VOCAB_PATH = "D:/vectors/google_vokab.bin";
        public static string GAME_OF_THRONES_PATH = "D:/GAME_OF_THRONES.txt";
    }
}

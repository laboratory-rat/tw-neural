using System;
using System.Collections.Generic;
using System.Text;

namespace LRNeuralClientTest
{
    static class Configuration
    {
        public static List<string> RawDataList = new List<string>
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

        public static string VocabularyPath = "D:/vectors/google_vokab.bin";
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MRNeural
{
    public class WordBag
    {
        public static readonly char[] REMOVABLE = new char[] { '/', '.', '-', '?', '!', '=', '(', ')', ','};
        const char SPACE_SEPARATOR = ' ';

        const string REMOVABLE_REGEX = "[\"'@/_:/[/\\#$%^&*()]";
        const string SEPARATION_REGEX = @"[.!?]";

        public string[] Sequences { get; set; }
        public int Count => Sequences?.Length ?? 0;
        public string Original { get; set; }

        public int Step { get; set; } = 3;

        public WordBag() { }

        public WordBag(IEnumerable<string> list, int step)
        {
            if (list != null && list.Any())
                Sequences = list.ToArray();

            step = Math.Max(1, step);
            step = Math.Min(10, step);
            Step = step;
        }

        public IEnumerable<string[]> Read()
        {
            var cursor = 0;
            if (Count == 0) yield break;

            while (true)
            {
                var response = new List<string>();

                if (cursor + Step <= Count)
                {
                    yield return Sequences.Skip(cursor).Take(Step).ToArray();
                    cursor++;

                    if (cursor == Count) break;
                }
                else
                {
                    var s = cursor + Step - Count;
                    var result = Sequences.TakeLast(Step - s).ToList();

                    result.AddRange(Enumerable.Repeat(string.Empty, s));
                    yield return result.ToArray();
                    cursor++;

                    if (cursor == Count) break;
                }
            }
        }

        public WordBag Add(WordBag wb)
        {
            var list = Sequences?.ToList() ?? new List<string>();
            list.AddRange(wb.Sequences);
            Sequences = list.ToArray();
            Original += wb.Original;
            return this;
        }

        public static WordBag CreateToWords(string phrase, int step)
        {
            var original = phrase;
            phrase = phrase.Trim();

            phrase = new Regex(REMOVABLE_REGEX).Replace(phrase, string.Empty);
            phrase = new Regex(SEPARATION_REGEX).Replace(phrase, string.Empty);

            return new WordBag
            {
                Sequences = phrase.Split(SPACE_SEPARATOR),
                Step = step,
                Original = original
            };
        }

        public static WordBag CreateToPhrases(string phrase, int step)
        {
            var original = phrase;
            phrase = phrase.Trim();

            phrase = new Regex(REMOVABLE_REGEX).Replace(phrase, string.Empty);
            var list = new Regex(SEPARATION_REGEX).Split(phrase);

            return new WordBag
            {
                Sequences = list,
                Original = original,
                Step = step
            };
        }
    }
}

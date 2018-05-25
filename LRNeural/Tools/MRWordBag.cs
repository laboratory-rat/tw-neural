using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MRNeural.Tools
{
    public class MRWordBag
    {
        public static readonly char[] REMOVABLE = new char[] { '/', '.', '-', '?', '!', '=', '(', ')', ',' };
        const char SPACE_SEPARATOR = ' ';
        const string REMOVABLE_REGEX = "[\"'@/_:/[/\\#$%^&*()]";
        const string SEPARATION_REGEX = @"[.!?]";

        public string[] Sequences { get; set; }
        public int Count => Sequences?.Length ?? 0;
        public string Original { get; set; }

        public int Step { get; set; } = 3;

        public MRWordBag() { }

        public MRWordBag(string data, int step) : this(new string[] { data }, step) { }

        public MRWordBag(IEnumerable<string> list, int step)
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

        public MRWordBag Add(MRWordBag wb)
        {
            var list = Sequences?.ToList() ?? new List<string>();
            list.AddRange(wb.Sequences);
            Sequences = list.ToArray();
            Original += wb.Original;
            return this;
        }

        public static MRWordBag CreateToWords(string phrase, int step)
        {
            var original = phrase;
            phrase = phrase.Trim();

            phrase = new Regex(REMOVABLE_REGEX).Replace(phrase, string.Empty);
            phrase = new Regex(SEPARATION_REGEX).Replace(phrase, string.Empty);

            return new MRWordBag
            {
                Sequences = phrase.Split(SPACE_SEPARATOR),
                Step = step,
                Original = original
            };
        }

        public static MRWordBag CreateToPhrases(string phrase, int step)
        {
            var original = phrase;
            phrase = phrase.Trim();

            phrase = new Regex(REMOVABLE_REGEX).Replace(phrase, string.Empty);
            var list = new Regex(SEPARATION_REGEX).Split(phrase);

            return new MRWordBag
            {
                Sequences = list,
                Original = original,
                Step = step
            };
        }
    }
}

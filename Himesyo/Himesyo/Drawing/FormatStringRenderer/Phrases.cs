using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace Himesyo.Drawing
{
    public sealed class Phrases
    {
        private readonly Regex regexPhrases;

        public static Phrases Default { get; } = new Phrases(Phrase.Default);

        private string GetRegex(IEnumerable<char> chars)
        {
            StringBuilder pc = new StringBuilder();
            char? start = null;
            char up = default;
            foreach (char c in chars.OrderBy(c => c))
            {
                if (start.HasValue)
                {
                    if (up + 1 == c)
                    {
                        up = c;
                    }
                    else
                    {
                        if (start == up)
                        {
                            pc.Append(Regex.Escape(up.ToString()));
                        }
                        else
                        {
                            pc.Append(Regex.Escape(start.ToString())).Append('-').Append(Regex.Escape(up.ToString()));
                            start = c;
                            up = c;
                        }
                    }
                }
                else
                {
                    start = c;
                    up = c;
                }
            }
            if (start.HasValue)
            {
                if (start == up)
                {
                    pc.Append(Regex.Escape(up.ToString()));
                }
                else
                {
                    pc.Append(Regex.Escape(start.ToString())).Append('-').Append(Regex.Escape(up.ToString()));
                }
            }
            return $"[{pc}]+";
        }

        public Phrases(IEnumerable<char> phrases)
            : this(Phrase.FromChars(phrases))
        {

        }

        public Phrases(IEnumerable<Phrase> phrases)
        {
            if (phrases == null)
                throw new ArgumentNullException(nameof(phrases));

            HashSet<char> phrasesChar = new HashSet<char>();
            HashSet<string> patterns = new HashSet<string>();
            foreach (Phrase phrase in phrases)
            {
                if (phrase?.Value?.Length > 0)
                {
                    switch (phrase.Type)
                    {
                        case PhraseType.Char:
                            foreach (char c in phrase.Value)
                            {
                                phrasesChar.Add(c);
                            }
                            break;
                        case PhraseType.Phrase:
                            patterns.Add(Regex.Escape(phrase.Value));
                            break;
                        case PhraseType.Regex:
                            try
                            {
                                new Regex(phrase.Value, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture);
                            }
                            catch (Exception ex)
                            {
                                throw new ArgumentException($"无效的正则表达式 '{phrase.Value}'。", ex);
                            }
                            patterns.Add(phrase.Value);
                            break;
                        default:
                            break;
                    }
                }
            }
            if (phrasesChar.Count > 0)
            {
                patterns.Add(GetRegex(phrasesChar));
            }
            if (patterns.Count > 0)
            {
                string pattern = $"({string.Join(")|(", patterns)})";
                regexPhrases = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.ExplicitCapture | RegexOptions.Compiled);
            }
        }

        public Phrases(params Phrase[] phrases)
            : this((IEnumerable<Phrase>)phrases)
        {

        }

        public Match Find(string input, int startIndex)
        {
            if (input == null)
                throw new ArgumentNullException(nameof(input));
            if (startIndex < 0 || startIndex >= input.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));

            return regexPhrases?.Match(input, startIndex) ?? Match.Empty;
        }
    }
}

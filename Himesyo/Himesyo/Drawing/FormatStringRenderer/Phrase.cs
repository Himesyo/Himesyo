using System.Collections.Generic;
using System.Linq;

namespace Himesyo.Drawing
{
    public sealed class Phrase
    {
        public static Phrase Empty { get; } = new Phrase(PhraseType.Char, null);

        public static Phrase Default { get; } = Phrase.FromRegex(@"[a-z0-9_\.]+");

        public static Phrase FromChars(IEnumerable<char> chars)
        {
            if (chars == null || !chars.Any())
            {
                return Phrase.Empty;
            }
            string value = chars as string ?? new string(chars.ToArray());
            return new Phrase(PhraseType.Char, value);
        }

        public static Phrase FromText(string text)
        {
            return new Phrase(PhraseType.Phrase, text);
        }

        public static Phrase FromRegex(string pattern)
        {
            return new Phrase(PhraseType.Regex, pattern);
        }

        public PhraseType Type { get; }
        public string Value { get; }

        public Phrase(PhraseType type, string value)
        {
            Type = type;
            Value = value;
        }
    }
}

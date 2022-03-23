using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml.Linq;

using static System.Windows.Forms.AxHost;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace Himesyo.Drawing
{
    public sealed class FormatStringRenderer
    {
        private static readonly StringFormat measureFormat = StringFormat.GenericTypographic;
        private static readonly Regex regexUserSet = new Regex(@"^\s*@");

        public Graphics Graphics { get; }

        public Font Font { get; set; }
        public Brush Brush { get; set; }
        public Brush BackBrush { get; set; }

        public float Left { get; set; }
        public float Right { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }
        public float WordSpace { get; set; } = 1;
        public float LineSpace { get; set; } = 5;

        public Phrases Phrases { get; set; }

        public Func<string, Brush> UserBrush { get; set; }

        public FormatStringRenderer(Graphics graphics)
        {
            Graphics = graphics;
        }

        public void DrawString(string text, RectangleF rect)
        {
            if (string.IsNullOrEmpty(text))
                return;
            RendererState state = CreateState().Format();
            state.Rect = rect.Foramt();
            state.X = state.Rect.X;
            state.Y = state.Rect.Y;
            state.Cache.Add(text, 0);
            state.DrawCache();
        }

        public void DrawFormatString(string formatText, RectangleF rect)
        {
            if (string.IsNullOrEmpty(formatText))
                return;
            RendererState state = CreateState().Format();
            state.Rect = rect.Foramt();
            state.X = state.Rect.X;
            state.Y = state.Rect.Y;
            int length = formatText.Length;
            for (int i = 0; i < length; i++)
            {
                int index = i;
                char? result = AnalyseFormat(formatText, state, true, ref i);
                if (result.HasValue)
                {
                    state.Cache.Add(result.Value, index);
                }
            }
            if (state.IsEscape)
                throw new StringFormatException(length - 1, 1, formatText, $"转义符不能作为结束。");

            state.DrawCache();
        }

        public SizeF MeasureFormatString(string formatText, float width)
        {
            if (string.IsNullOrEmpty(formatText))
                return SizeF.Empty;
            RendererState state = CreateState().Format();
            state.Rect = new RectangleF(0, 0, width, float.MaxValue);
            state.X = state.Rect.X;
            state.Y = state.Rect.Y;
            int length = formatText.Length;
            for (int i = 0; i < length; i++)
            {
                int index = i;
                char? result = AnalyseFormat(formatText, state, false, ref i);
                if (result.HasValue)
                {
                    char c = result.Value;
                    if (char.IsLetterOrDigit(c))
                    {
                        state.Cache.Add(c, index);
                    }
                    else
                    {
                        state.MeasureCache();
                        state.MeasureChar(c, index);
                    }
                }
            }
            state.MeasureCache();
            state.MaxWidth = Math.Max(state.MaxWidth, state.X);
            state.Y += state.MaxHeight;
            state.MaxHeight = 0;
            return new SizeF(state.MaxWidth, state.Y - state.Rect.Y);
        }

        public SizeF MeasureFormatString(string formatText, RectangleF rect)
        {
            if (string.IsNullOrEmpty(formatText))
                return SizeF.Empty;
            RendererState state = CreateState().Format();
            state.Rect = rect.Foramt();
            state.X = state.Rect.X;
            state.Y = state.Rect.Y;
            int length = formatText.Length;
            for (int i = 0; i < length; i++)
            {
                int index = i;
                char? result = AnalyseFormat(formatText, state, false, ref i);
                if (result.HasValue)
                {
                    char c = result.Value;
                    if (char.IsLetterOrDigit(c))
                    {
                        state.Cache.Add(c, index);
                    }
                    else
                    {
                        state.MeasureCache();
                        state.MeasureChar(c, index);
                    }
                }
            }
            state.MeasureCache();
            state.MaxWidth = Math.Max(state.MaxWidth, state.X);
            state.Y += state.MaxHeight;
            state.MaxHeight = 0;
            return new SizeF(state.MaxWidth, state.Y - state.Rect.Y);
        }

        public void MeasureFormatString(string formatText, int startIndex, RectangleF rect, out SizeF size, out int count)
        {
            if (string.IsNullOrEmpty(formatText))
            {
                size = SizeF.Empty;
                count = 0;
            }
            RendererState state = CreateState().Format();
            state.Rect = rect;
            state.X = state.Rect.X;
            state.Y = state.Rect.Y;
            int length = formatText.Length;
            for (int i = startIndex; i < length; i++)
            {
                int index = i;
                char? result = AnalyseFormat(formatText, state, false, ref i);
                if (result.HasValue)
                {
                    char c = result.Value;
                    if (char.IsLetterOrDigit(c))
                    {
                        state.Cache.Add(c, index);
                    }
                    else
                    {
                        state.MeasureCache();
                        state.MeasureChar(c, index);
                    }
                }
            }
            state.MeasureCache();
            state.MaxWidth = Math.Max(state.MaxWidth, state.X);
            state.Y += state.MaxHeight;
            state.MaxHeight = 0;
            size = new SizeF(state.MaxWidth, state.Y - state.Rect.Y);
            count = 0;
        }

        private char? AnalyseFormat(string formatText, RendererState state, bool draw, ref int i)
        {
            int length = formatText.Length;
            int location = i - 1;
            char c = formatText[i];
            if (state.IsEscape)
            {
                state.IsEscape = false;
                switch (c)
                {
                    case '\\': break;
                    case '0': c = '\0'; break;
                    case 'a': c = '\a'; break;
                    case 'b': c = '\b'; break;
                    case 'f': c = '\f'; break;
                    case 'n': c = '\n'; break;
                    case 'r': c = '\r'; break;
                    case 't': c = '\t'; break;
                    case '\n': return null;
                    case '\r':
                        if (i + 1 < length && formatText[i + 1] == '\n')
                            i++;
                        return null;
                    case ' ':
                        while (i + 1 < length && formatText[i + 1] == ' ')
                            i++;
                        return null;
                    case 'c':
                        i++;
                        if (i < length && formatText[i] == '[')
                        {
                            i++;
                            for (int j = i; j < length; j++)
                            {
                                c = formatText[j];
                                if (c == '@' || c == '#' || c == ',')
                                    continue;
                                if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                                    continue;
                                if (c == ']')
                                {
                                    string body = formatText.Substring(i, j - i);
                                    if (string.IsNullOrWhiteSpace(body))
                                        throw new StringFormatException(location, j - location + 1, formatText, $"无效的颜色设定项。");
                                    i = j;
                                    if (regexUserSet.IsMatch(body))
                                    {
                                        if (draw)
                                        {
                                            Brush brush = UserBrush?.Invoke(body) ?? Brushes.Black;
                                            state.AddStateSet(location, (obj) => obj.Brush = brush);
                                        }
                                        return null;
                                    }
                                    try
                                    {
                                        Color color = ColorTranslator.FromHtml(body);
                                        if (draw)
                                        {
                                            state.AddStateSet(location, (obj) => obj.Brush = new SolidBrush(color));
                                        }
                                        return null;
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new StringFormatException(location, j - location + 1, formatText, $"无法将 '{body}' 转换为颜色。", ex);
                                    }
                                }
                            }
                        }
                        throw new StringFormatException(location, 2, formatText, $"无效的颜色设定项。");
                    case 'u':
                        string unext = formatText.Substring(i + 1, Math.Min(4, length - i - 1));
                        if (unext.Length == 4 && int.TryParse(unext, NumberStyles.AllowHexSpecifier, null, out int u))
                        {
                            i += 4;
                            c = (char)u;
                        }
                        else
                        {
                            throw new StringFormatException(location, 2 + unext.Length, formatText, $"无法识别的转义字符 '\\u{unext}' 。");
                        }
                        break;
                    case '[':
                        i++;
                        for (int j = i; j < length; j++)
                        {
                            c = formatText[j];
                            if (c == '@' || c == '=' || c == ',' || c == '.')
                                continue;
                            if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                                continue;
                            if (c == ']')
                            {
                                string body = formatText.Substring(i, j - i);
                                if (string.IsNullOrWhiteSpace(body))
                                    return null;
                                i = j;
                                var pros = body.Split(',')
                                    .Where(p => !string.IsNullOrWhiteSpace(p))
                                    .Select(p =>
                                    {
                                        int index = p.IndexOf('=');
                                        if (index > -1)
                                        {
                                            string name = p.Substring(0, index).Trim();
                                            string value = p.Substring(index + 1).Trim();
                                            return new KeyValuePair<string, string>(name, value);
                                        }
                                        else
                                        {
                                            return new KeyValuePair<string, string>(string.Empty, p.Trim());
                                        }
                                    });
                                foreach (var pair in pros)
                                {
                                    string name = pair.Key.ToLowerInvariant();
                                    switch (name)
                                    {
                                        case "":
                                            foreach (char sign in pair.Value)
                                            {
                                                switch (sign)
                                                {
                                                    case 'b':
                                                        state.AddStateSet(location, obj => obj.Bold = true);
                                                        break;
                                                    case 'B':
                                                        state.AddStateSet(location, obj => obj.Bold = false);
                                                        break;
                                                    case 'i':
                                                        state.AddStateSet(location, obj => obj.Italic = true);
                                                        break;
                                                    case 'I':
                                                        state.AddStateSet(location, obj => obj.Italic = false);
                                                        break;
                                                    case 'u':
                                                        state.AddStateSet(location, obj => obj.Underline = true);
                                                        break;
                                                    case 'U':
                                                        state.AddStateSet(location, obj => obj.Underline = false);
                                                        break;
                                                    case 's':
                                                        state.AddStateSet(location, obj => obj.Strikeout = true);
                                                        break;
                                                    case 'S':
                                                        state.AddStateSet(location, obj => obj.Strikeout = false);
                                                        break;
                                                    default:
                                                        break;
                                                }
                                            }
                                            break;
                                        case "fontname":
                                        case "fn":
                                        case "字体":
                                            state.AddStateSet(location, obj => obj.FontFamily = new FontFamily(pair.Value));
                                            break;
                                        case "fontsize":
                                        case "fs":
                                        case "字号":
                                            if (float.TryParse(pair.Value, out float fontSize))
                                                state.AddStateSet(location, obj => obj.FontSize = fontSize);
                                            break;
                                        case "fontbold":
                                        case "fb":
                                        case "粗体":
                                            if (bool.TryParse(pair.Value, out bool bold))
                                                state.AddStateSet(location, obj => obj.Bold = bold);
                                            break;
                                        case "fontitalic":
                                        case "fi":
                                        case "斜体":
                                            if (bool.TryParse(pair.Value, out bool italic))
                                                state.AddStateSet(location, obj => obj.Italic = italic);
                                            break;
                                        case "fontunderline":
                                        case "fu":
                                        case "下划线":
                                            if (bool.TryParse(pair.Value, out bool underline))
                                                state.AddStateSet(location, obj => obj.Underline = underline);
                                            break;
                                        case "fontstrikeout":
                                        case "fo":
                                        case "删除线":
                                            if (bool.TryParse(pair.Value, out bool strikeout))
                                                state.AddStateSet(location, obj => obj.Strikeout = strikeout);
                                            break;
                                        default:
                                            break;
                                    }
                                }
                                return null;
                            }
                        }
                        return null;
                    default:
                        throw new StringFormatException(location, 2, formatText, $"无法识别的转义字符 '\\{c}' 。");
                }
            }
            else if (c == '\\')
            {
                state.IsEscape = true;
                return null;
            }
            return c;
        }

        private RendererState CreateState()
        {
            RendererState state = new RendererState(Graphics)
            {
                Font = Font,
                Brush = Brush,
                Left = Left,
                Right = Right,
                Top = Top,
                Bottom = Bottom,
                WordSpace = WordSpace,
                LineSpace = LineSpace,
                Phrases = Phrases ?? Phrases.Default
            };
            return state;
        }

        #region 内部成员

        private class RendererState
        {
            private Font cacheFont;
            private float fontSize;
            private bool bold;
            private bool italic;
            private bool strikeout;
            private bool underline;
            private FontFamily fontFamily;

            public Graphics Graphics { get; }

            public Font Font
            {
                get
                {
                    if (cacheFont == null)
                    {
                        FontFamily family = FontFamily ?? FontFamily.Families.FirstOrDefault();
                        if (family == null)
                            throw new Exception("未找到可使用的字体。");
                        FontStyle style = 0;
                        if (Bold) style |= FontStyle.Bold;
                        if (Italic) style |= FontStyle.Italic;
                        if (Strikeout) style |= FontStyle.Strikeout;
                        if (Underline) style |= FontStyle.Underline;
                        cacheFont = new Font(family, FontSize, style);
                    }
                    return cacheFont;
                }
                set
                {
                    cacheFont = value;
                    if (value == null)
                    {
                        FontFamily = null;
                        FontSize = 9;
                        Bold = false;
                        Italic = false;
                        Strikeout = false;
                        Underline = false;
                    }
                    else
                    {
                        fontFamily = value.FontFamily;
                        fontSize = value.SizeInPoints;
                        bold = value.Bold;
                        italic = value.Italic;
                        strikeout = value.Strikeout;
                        underline = value.Underline;
                    }
                }
            }
            public FontFamily FontFamily
            {
                get => fontFamily;
                set
                {
                    cacheFont = null;
                    fontFamily = value;
                }
            }
            public float FontSize
            {
                get => fontSize;
                set
                {
                    cacheFont = null;
                    fontSize = value;
                }
            }
            public bool Bold
            {
                get => bold;
                set
                {
                    cacheFont = null;
                    bold = value;
                }
            }
            public bool Italic
            {
                get => italic;
                set
                {
                    cacheFont = null;
                    italic = value;
                }
            }
            public bool Strikeout
            {
                get => strikeout;
                set
                {
                    cacheFont = null;
                    strikeout = value;
                }
            }
            public bool Underline
            {
                get => underline;
                set
                {
                    cacheFont = null;
                    underline = value;
                }
            }

            public Brush Brush { get; set; }
            public Phrases Phrases { get; set; }

            public float Left { get; set; }
            public float Right { get; set; }
            public float Top { get; set; }
            public float Bottom { get; set; }
            public float WordSpace { get; set; }
            public float LineSpace { get; set; }

            public RectangleF Rect { get; set; }
            public float X { get; set; }
            public float Y { get; set; }
            public float MaxHeight { get; set; }

            public bool IsEscape { get; set; }
            public StringCache Cache { get; } = new StringCache();
            public Queue<StateSet> StateSets { get; private set; } = new Queue<StateSet>();

            public float MaxWidth { get; set; }

            public RendererState(Graphics graphics)
            {
                Graphics = graphics;
            }

            public void DrawCache()
            {
                if (Cache.Length == 0)
                    return;

                string text = Cache.ToString();
                int index = 0;
                Match match = Phrases.Find(text, 0);
                while (match.Success)
                {
                    if (index < match.Index)
                    {
                        DrawCache(index, match.Index);
                        index = match.Index;
                    }
                    if (index != match.Index)
                        throw new Exception("不应该出现的异常，但它就是出现了。");
                    // 使用副本测量，不能影响当前状态。
                    float width = Clone().PushFormatString(match.Index, match.Index + match.Value.Length);
                    if (MaxHeight > 0 && X + width > Rect.Right)
                    {
                        Y += MaxHeight + LineSpace;
                        X = Rect.Left;
                        MaxHeight = 0;
                    }
                    match = match.NextMatch();
                }
                DrawCache(index, Cache.Length);
                Cache.Clear();
            }

            /// <summary>
            /// 不包含结束索引。
            /// </summary>
            /// <param name="statrIndex"></param>
            /// <param name="endIndex"></param>
            private void DrawCache(int statrIndex, int endIndex)
            {
                for (int i = statrIndex; i < endIndex; i++)
                {
                    DrawChar(Cache[i], Cache.GetSourceIndex(i));
                }
            }

            public void DrawChar(char c, int sourceIndex)
            {
                if (HandleWhiteSpace(c))
                    return;

                HandleState(sourceIndex);
                string text = c.ToString();
                SizeF size = MeasureString(text);
                if (MaxHeight > 0 && X + size.Width > Rect.Right)
                {
                    Y += MaxHeight + LineSpace;
                    X = Rect.Left;
                    MaxHeight = 0;
                }
                Graphics.DrawString(text, Font, Brush, X, Y);
                X += size.Width + WordSpace;
                X = Math.Min(X, Rect.Right);
                MaxHeight = Math.Max(MaxHeight, size.Height);
            }

            public void MeasureCache()
            {
                if (Cache.Length == 0)
                    return;

                string text = Cache.ToString();
                int index = 0;
                Match match = Phrases.Find(text, 0);
                while (match.Success)
                {
                    if (index < match.Index)
                    {
                        MeasureCache(index, match.Index);
                        index = match.Index;
                    }
                    if (index != match.Index)
                        throw new Exception("不应该出现的异常，但它就是出现了。");
                    // 使用副本测量，不能影响当前状态。
                    float width = Clone().PushFormatString(match.Index, match.Index + match.Value.Length);
                    if (MaxHeight > 0 && X + width > Rect.Right)
                    {
                        Y += MaxHeight + LineSpace;
                        X = Rect.Left;
                        MaxHeight = 0;
                    }
                    match = match.NextMatch();
                }
                MeasureCache(index, Cache.Length);
                Cache.Clear();
            }

            /// <summary>
            /// 不包含结束索引。
            /// </summary>
            /// <param name="statrIndex"></param>
            /// <param name="endIndex"></param>
            private void MeasureCache(int statrIndex, int endIndex)
            {
                for (int i = statrIndex; i < endIndex; i++)
                {
                    MeasureChar(Cache[i], Cache.GetSourceIndex(i));
                }
            }

            public void MeasureChar(char c, int sourceIndex)
            {
                if (HandleWhiteSpace(c))
                    return;

                HandleState(sourceIndex);
                string text = c.ToString();
                SizeF size = MeasureString(text);
                if (MaxHeight > 0 && X + size.Width > Rect.Right)
                {
                    Y += MaxHeight + LineSpace;
                    MaxWidth = Math.Max(MaxWidth, X);
                    X = Rect.Left;
                    MaxHeight = 0;
                }
                X += size.Width + WordSpace;
                X = Math.Min(X, Rect.Right);
                MaxHeight = Math.Max(MaxHeight, size.Height);
            }

            private bool HandleWhiteSpace(char c)
            {
                switch (c)
                {
                    case '\n':
                        if (MaxHeight == 0)
                            MaxHeight = Font.Height;
                        Y += MaxHeight + LineSpace;
                        MaxWidth = Math.Max(MaxWidth, X);
                        X = Rect.Left;
                        MaxHeight = 0;
                        return true;
                    case ' ':
                        SizeF size = MeasureString("0");
                        if (MaxHeight > 0 && X + size.Width > Rect.Right)
                        {
                            Y += MaxHeight + LineSpace;
                            MaxWidth = Math.Max(MaxWidth, X);
                            X = Rect.Left;
                            MaxHeight = 0;
                        }
                        MaxHeight = Math.Max(MaxHeight, size.Height);
                        X += size.Width + WordSpace;
                        X = Math.Min(X, Rect.Right);
                        return true;
                    default:
                        return false;
                }
            }

            public RendererState Format()
            {
                if (Brush == null)
                {
                    Brush = Brushes.Black;
                }
                if (Phrases == null)
                {
                    Phrases = Phrases.Default;
                }
                return this;
            }

            /// <summary>
            /// 副本。
            /// </summary>
            /// <returns></returns>
            public RendererState Clone()
            {
                RendererState other = (RendererState)MemberwiseClone();
                other.StateSets = new Queue<StateSet>(StateSets);
                return other;
            }

            public void AddStateSet(int index, Action<RendererState> action)
            {
                StateSets.Enqueue(new StateSet()
                {
                    Index = index,
                    Action = action
                });
            }

            private void HandleState(int index)
            {
                while (StateSets.Count > 0)
                {
                    StateSet set = StateSets.Peek();
                    if (set.Index > index)
                        break;

                    set.Execute(this);
                    StateSets.Dequeue();
                }
            }

            /// <summary>
            /// 推进指定位置的字符，并返回在当前行上需要的长度。不包含结束索引。
            /// </summary>
            /// <param name="startIndex"></param>
            /// <param name="endIndex"></param>
            /// <returns></returns>
            private float PushFormatString(int startIndex, int endIndex)
            {
                float result = 0;
                for (int i = startIndex; i < endIndex; i++)
                {
                    char c = Cache[i];
                    if (c == '\n')
                        break;
                    HandleState(Cache.GetSourceIndex(i));
                    SizeF size = MeasureString(c.ToString());
                    result += size.Width + WordSpace;
                }
                if (result >= WordSpace)
                {
                    result -= WordSpace;
                }
                return result;
            }

            /// <summary>
            /// 以当前样式测量字符串的大小。
            /// </summary>
            /// <param name="text"></param>
            /// <returns></returns>
            private SizeF MeasureString(string text)
            {
                return Graphics.MeasureString(text, Font, PointF.Empty, measureFormat);
            }
        }

        private class StringCache
        {
            private readonly StringBuilder builder = new StringBuilder(32);
            private readonly List<int> sourceIndexs = new List<int>(32);

            public int Length => builder.Length;

            public char this[int index]
            {
                get
                {
                    return builder[index];
                }
            }

            public void Add(char c, int sourceIndex)
            {
                sourceIndexs.Add(sourceIndex);
                builder.Append(c);
            }

            public void Add(string c, int sourceIndex)
            {
                if (c == null)
                    return;

                builder.Append(c);
                sourceIndexs.AddRange(Enumerable.Range(sourceIndex, c.Length));
            }

            public void Clear()
            {
                builder.Clear();
                sourceIndexs.Clear();
            }

            public int GetSourceIndex(int index)
            {
                return sourceIndexs[index];
            }

            public override string ToString()
            {
                return builder.ToString();
            }
        }

        private class StateSet
        {
            public int Index { get; set; }
            public Action<RendererState> Action { get; set; }

            public void Execute(RendererState state)
            {
                Action?.Invoke(state);
            }
        }

        #endregion
    }
}

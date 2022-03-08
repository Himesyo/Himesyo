using System;
using System.Runtime.Serialization;

namespace Himesyo.Drawing
{
    [Serializable]
    public class StringFormatException : Exception
    {
        private const string defaultMessage = "解析格式字符串时发生异常。";

        public int Index { get; } = -1;
        public int Length { get; }
        public string FormatString { get; }

        public override string Message
        {
            get => GenMessage(base.Message);
        }

        public StringFormatException()
            : this(-1, 0, null, null, null) { }
        public StringFormatException(string message)
            : this(-1, 0, null, message, null) { }
        public StringFormatException(int index, int length, string formatString)
            : this(index, length, formatString, null, null) { }
        public StringFormatException(int index, int length, string formatString, string message)
            : this(index, length, formatString, message, null) { }
        public StringFormatException(int index, int length, string formatString, string message, Exception inner)
            : base(message ?? defaultMessage, inner)
        {
            Index = index;
            Length = length;
            FormatString = formatString;
        }
        protected StringFormatException(
          SerializationInfo info,
          StreamingContext context) : base(info, context)
        {
            Index = info.GetInt32(nameof(Index));
            Length = info.GetInt32(nameof(Length));
            FormatString = info.GetString(nameof(FormatString));
        }

        private string GenMessage(string message)
        {
            string index = Index >= 0 ? $"位置 {Index} 。" : string.Empty;
            string formatString = string.IsNullOrEmpty(FormatString) ? string.Empty : $"\r\n解析的格式字符串：{FormatString}";
            return $"{message}{index}{formatString}";
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue(nameof(Index), Index);
            info.AddValue(nameof(Length), Length);
            info.AddValue(nameof(FormatString), FormatString);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Himesyo.ComponentModel.Design
{
    /// <summary>
    /// 数据长度格式化器。
    /// </summary>
    public class FileLengthFormatter : IFormatProvider, ICustomFormatter
    {
        private static readonly string[] units = new[] { " B", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB" };
        private static readonly int defaultLevel = 4; 

        private Regex regexLevel = new Regex(@"\s*(?<type>L)(?<level>\d*)\s*", RegexOptions.IgnoreCase | RegexOptions.ECMAScript);

        /// <inheritdoc/>
        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        /// <inheritdoc/>
        public string Format(string format, object arg, IFormatProvider formatProvider)
        {
            if (arg is IConvertible convertible)
            {
                IFormatProvider provider = formatProvider == this ? null : formatProvider;
                decimal value = convertible.ToDecimal(provider);
                if (!string.IsNullOrWhiteSpace(format))
                {
                    if (regexLevel.Match(format) is Match matchLevel && matchLevel.Success)
                    {
                        string type = matchLevel.Groups["type"].Value;
                        string inputLevel = matchLevel.Groups["level"].Value;
                        int level;
                        if (string.IsNullOrWhiteSpace(inputLevel))
                        {
                            level = defaultLevel;
                        }
                        else
                        {
                            int.TryParse(matchLevel.Groups["level"].Value, out level);
                            level = level < 0 ? defaultLevel : level;
                            level = level >= units.Length ? units.Length - 1 : level;
                        }
                        decimal currValue = value;
                        int currLevel = 0;
                        while (currLevel < level && Math.Abs(currValue) >= 1000)
                        {
                            currValue = currValue / 1024;
                            currLevel++;
                        }
                        string number;
                        if (Math.Abs(currValue) >= 1000)
                        {
                            number = currValue.ToString("F0");
                        }
                        else
                        {
                            number = currValue.ToString("F3");
                        }
                        string unit = units[currLevel];
                        if (type == "l")
                        {
                            unit = unit.ToLower();
                        }
                        return $"{number} {unit}";
                    }
                }
            }
            try
            {
                return HandleOtherFormats(format, arg);
            }
            catch (FormatException e)
            {
                throw new FormatException($"格式化字符串 '{format}' 无效。", e);
            }
        }

        private string HandleOtherFormats(string format, object arg)
        {
            if (arg is IFormattable formattable)
                return formattable.ToString(format, CultureInfo.CurrentCulture);
            else if (arg != null)
                return arg.ToString();
            else
                return string.Empty;
        }
    }
}

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows.Forms;

namespace Himesyo.ComponentModel
{
    /// <summary>
    /// 将空字符串转换为 <see langword="null"/> 值的转换器。只可应用于引用类型。
    /// </summary>
    public class NullObjectConverter : TypeConverter
    {
        /// <inheritdoc/>
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string);
        }

        /// <inheritdoc/>
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string);
        }

        /// <inheritdoc/>
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is string input)
            {
                if (string.IsNullOrWhiteSpace(input))
                {
                    return null;
                }
                else if (context is GridItem gridItem)
                {
                    return gridItem.Value;
                }
                else
                {
                    return context?.PropertyDescriptor?.GetValue(context.Instance);
                }
            }
            throw new InvalidCastException();
        }

        /// <inheritdoc/>
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return value?.ToString();
            }
            throw new InvalidCastException();
        }
    }
}

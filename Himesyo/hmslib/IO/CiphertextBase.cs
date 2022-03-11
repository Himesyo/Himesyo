using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Xml.Serialization;

namespace Himesyo.IO
{
    /// <summary>
    /// 表示以密文保存的字符串。此类为抽象类。
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(EncryptionConverter))]
    public abstract class CiphertextBase
    {
        /// <summary>
        /// 此对象存储的实际值。
        /// </summary>
        [XmlIgnore]
        public string Value { get; protected set; }

        /// <summary>
        /// 序列化时使用的加密值。
        /// </summary>
        [Browsable(false)]
        [XmlAttribute("value")]
        public virtual string Ciphertext
        {
            get
            {
                ICryptoTransform transform = GetEncryptionTransform();

                if (transform == null || string.IsNullOrEmpty(Value))
                    return Value;

                byte[] input = Encoding.UTF8.GetBytes(Value);
                byte[] output = transform.TransformFinalBlock(input, 0, input.Length);
                return Convert.ToBase64String(output);
            }
            set
            {
                ICryptoTransform transform = GetDecryptTransform();

                if (transform == null || string.IsNullOrEmpty(value))
                {
                    Value = value;
                    return;
                }

                byte[] input = Convert.FromBase64String(value);
                byte[] output = transform.TransformFinalBlock(input, 0, input.Length);
                Value = Encoding.UTF8.GetString(output);
            }
        }

        protected CiphertextBase() { }

        protected CiphertextBase(string value)
        {
            Value = value;
        }

        protected virtual ICryptoTransform GetEncryptionTransform()
        {
            return null;
        }

        protected virtual ICryptoTransform GetDecryptTransform()
        {
            return null;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj is CiphertextBase p)
            {
                return Value == p.Value;
            }
            if (obj is string value)
            {
                return Value == value;
            }
            if (obj == null)
            {
                return Value == null;
            }

            return false;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        /// <inheritdoc/>
        public override string ToString()
        {
            return Value;
        }

        /// <inheritdoc/>
        public static bool operator ==(CiphertextBase left, CiphertextBase right)
        {
            if (left is null)
                return right?.Value == null;
            return left.Equals(right);
        }

        /// <inheritdoc/>
        public static bool operator !=(CiphertextBase left, CiphertextBase right)
        {
            if (left is null)
                return right?.Value != null;
            return !left.Equals(right);
        }

        private class EncryptionConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return sourceType == typeof(string);
            }

            public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
            {
                return destinationType == typeof(string);
            }

            public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
            {
                Type resultType = context?.PropertyDescriptor?.PropertyType;
                if (resultType != null && value is string plaintext && typeof(CiphertextBase).IsAssignableFrom(resultType))
                {
                    CiphertextBase result = (CiphertextBase)resultType.Assembly.CreateInstance(resultType.FullName);
                    result.Value = plaintext;
                    return result;
                }
                return value;
            }

            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                if (destinationType == typeof(string) && value is CiphertextBase plaintext)
                {
                    return plaintext.Value;
                }
                return value;
            }
        }
    }
}

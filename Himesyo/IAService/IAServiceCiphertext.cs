using System.Security.Cryptography;

using Himesyo.IO;

namespace IAService
{
    /// <summary>
    /// 以密文形式保存到文件的字符串。
    /// </summary>
    [Serializable]
    public class IAServiceCiphertext : CiphertextBase
    {
        internal static SymmetricAlgorithm? Algorithm { get; set; }

        public IAServiceCiphertext() { }
        public IAServiceCiphertext(string? value) : base(value) { }

        protected override ICryptoTransform? CreateEncryptor()
        {
            return Algorithm?.CreateEncryptor();
        }
        protected override ICryptoTransform? CreateDecryptor()
        {
            return Algorithm?.CreateDecryptor();
        }

        public static implicit operator IAServiceCiphertext(string? value)
        {
            return new IAServiceCiphertext(value);
        }
        public static implicit operator string?(IAServiceCiphertext? value)
        {
            return value?.Value;
        }
    }
}

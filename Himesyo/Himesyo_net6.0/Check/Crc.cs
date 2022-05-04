using System.Security.Cryptography;

namespace Himesyo.Check
{
    /// <summary>
    /// CRC 算法实现。
    /// </summary>
    public class Crc
    {
        /// <summary>
        /// 使用算法名称创建 <see cref="Crc"/> 对象。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Crc? Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return null;
        }

        private uint[]? crcTable;
        private readonly int bit;
        private readonly uint poly;
        private readonly uint crcInit;
        private readonly uint crcResultXor;

        private readonly uint top;
        private readonly uint down;

        /// <summary>
        /// 初始化指定类型的 <see cref="Crc"/> 实例。
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public Crc(CrcType type) : this(
            type == null ? throw new ArgumentNullException(nameof(type)) : type.CrcMode,
            type.Poly, type.CrcInit, type.CrcResultXor)
        {

        }

        /// <summary>
        /// 使用算法、生成多项式、CRC 初始值、CRC 结果异或运算值初始化 Crc 的新实例。
        /// </summary>
        /// <param name="mode">CRC 算法模式。</param>
        /// <param name="poly">生成多项式。</param>
        /// <param name="crcInit">CRC 初始值。</param>
        /// <param name="crcResultXor">对 CRC 结果进行异或处理的值。0 表示对结果不进行处理。</param>
        /// <param name="crcTable">预计算的 CRC 表。可使用 <see cref="InitCrcTable"/> 自动计算。</param>
        public Crc(CrcMode mode, uint poly, uint crcInit = 0, uint crcResultXor = 0, uint[]? crcTable = null)
        {
            int bit = (int)mode;
            if (bit <= 0)
                throw new ArgumentOutOfRangeException(nameof(mode), mode, "算法类型必须大于 0 且小于等于 64 。");

            this.bit = bit;
            this.poly = poly;
            this.crcInit = crcInit;
            this.crcResultXor = crcResultXor;
            if (crcTable?.Length == 256)
            {
                this.crcTable = crcTable.ToArray();
            }

            top = 1u << bit - 1;
            down = uint.MaxValue >> 32 - bit;
        }

        /// <summary>
        /// 初始化 CRC 表。
        /// </summary>
        public void InitCrcTable()
        {
            uint[] table = new uint[256];
            for (uint i = 0; i <= byte.MaxValue; i++)
            {
                table[i] = ComputeCrc((byte)i);
            }
            crcTable = table;
        }

        /// <summary>
        /// 计算指定字节数组的 CRC 值。 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] ComputeCrc(byte[] data)
        {
            ExceptionHelper.ThrowNull(data, nameof(data));
            uint crc = crcInit;
            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                uint dataResult = ComputeCrc(data[i]);
                crc ^= dataResult;
            }
            crc ^= crcResultXor;
            return ToResult(bit, crc);

        }

        /// <summary>
        /// 根据预计算的 CRC 表计算 CRC 值，能大幅度提高效率。如果未设定 CRC 表，则引发 <see cref="InvalidOperationException"/> 。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] ComputeCrcByTable(byte[] data)
        {
            ExceptionHelper.ThrowNull(data, nameof(data));
            ExceptionHelper.ThrowInvalid(crcTable is null, "未初始化 CRC 表。");
            uint crc = crcInit;
            int length = data.Length;
            for (int i = 0; i < length; i++)
            {
                uint dataResult = crcTable![data[i]];
                crc ^= dataResult;
            }
            crc ^= crcResultXor;
            return ToResult(bit, crc);
        }

        private static byte[] ToResult(int bit, uint crc)
        {
            (int byteLength, int rem) = Math.DivRem(bit, 8);
            if (rem > 0)
                byteLength++;

            byte[] result = new byte[byteLength];
            for (int i = 0; i < byteLength; i++)
            {
                uint value = crc & byte.MaxValue;
                result[i] = (byte)value;
                crc >>= 8;
            }
            return result;
        }

        /// <summary>
        /// 计算与初始值、结果异或值无关的单个字节的 CRC 值。其计算结果与 CRC 表一致。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public uint ComputeCrc(byte data)
        {
            uint rem = data;
            for (int i = 0; i < bit; i++)
            {
                if ((rem & top) == 0)
                    rem <<= 1;
                else
                    rem = (rem << 1) ^ poly;
            }
            rem &= down;
            return rem;
        }
    }


    /// <summary>
    /// CRC 算法实现。
    /// </summary>
    public abstract class CRC : HashAlgorithm
    {
        /// <summary>
        /// 使用算法名称创建 <see cref="CRC"/> 对象。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static new CRC? Create(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            return null;
        }

        protected static byte[] ToResult(int bit, uint crc)
        {
            (int byteLength, int rem) = Math.DivRem(bit, 8);
            if (rem > 0)
                byteLength++;

            byte[] result = new byte[byteLength];
            for (int i = 0; i < byteLength; i++)
            {
                uint value = crc & byte.MaxValue;
                result[i] = (byte)value;
                crc >>= 8;
            }
            return result;
        }
        protected static byte[] ToResult(int bit, ulong crc)
        {
            (int byteLength, int rem) = Math.DivRem(bit, 8);
            if (rem > 0)
                byteLength++;

            byte[] result = new byte[byteLength];
            for (int i = 0; i < byteLength; i++)
            {
                ulong value = crc & byte.MaxValue;
                result[i] = (byte)value;
                crc >>= 8;
            }
            return result;
        }


        /// <summary>
        /// 初始化指定类型的 <see cref="Crc"/> 实例。
        /// </summary>
        /// <param name="type"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected CRC() 
        {

        }

        /// <summary>
        /// 初始化 CRC 表。
        /// </summary>
        public void InitCrcTable()
        {
            InitCrcTableCore();
        }

        /// <summary>
        /// 初始化 CRC 表的实现。
        /// </summary>
        protected abstract void InitCrcTableCore();
        /// <summary>
        /// 计算与初始值、结果异或值无关的单个字节的 CRC 值的实现。其计算结果应与 CRC 表一致。
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        protected abstract byte[] ComputeCrcCore(byte data);
    }

    public class CRC8 : CRC
    {
        private const int bit = 8;
        private const uint top = 0x80;
        private const uint down = 0xFF;

        private ushort[]? crcTable;
        private readonly ushort poly;
        private readonly ushort crcInit;
        private readonly ushort crcResultXor;

        protected override void InitCrcTableCore()
        {
            throw new NotImplementedException();
        }

        protected override byte[] ComputeCrcCore(byte data)
        {
            throw new NotImplementedException();
        }

        protected override void HashCore(byte[] array, int ibStart, int cbSize)
        {
            throw new NotImplementedException();
        }

        protected override byte[] HashFinal()
        {
            throw new NotImplementedException();
        }

        public override void Initialize()
        {
            throw new NotImplementedException();
        }

    }
}

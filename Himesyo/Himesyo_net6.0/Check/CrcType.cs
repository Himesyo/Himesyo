namespace Himesyo.Check
{
    /// <summary>
    /// 预定义的 CRC 算法类型。
    /// </summary>
    public class CrcType
    {
        /// <summary>
        /// x^16 + x^12 + x^5 + 1 (0x1021) (X25, V.41, Bluetooth, PPP, IrDA)
        /// </summary>
        public static CrcType CRC16_CCITT { get; } = new CrcType(CrcMode.Crc16, 0x1021, 0x0000, 0x0000);

        /// <summary>
        /// CRC 算法模式。
        /// </summary>
        public CrcMode CrcMode { get; }
        /// <summary>
        /// 生成多项式。
        /// </summary>
        public uint Poly { get; }
        /// <summary>
        /// CRC 初始值。
        /// </summary>
        public uint CrcInit { get; }
        /// <summary>
        /// 对 CRC 结果进行异或处理的值。0 表示对结果不进行处理。
        /// </summary>
        public uint CrcResultXor { get; }
        /// <summary>
        /// 预计算的 CRC 表。
        /// </summary>
        public ReadOnlyMemory<uint> CrcTable { get; }
        private CrcType(CrcMode mode, uint poly, uint crcInit, uint crcResultXor, uint[]? crcTable = null)
        {
            CrcMode = mode;
            Poly = poly;
            CrcInit = crcInit;
            CrcResultXor = crcResultXor;
            if (crcTable?.Length == 256)
            {
                CrcTable = new ReadOnlyMemory<uint>(crcTable.ToArray());
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Himesyo
{
    /// <summary>
    /// 提供一些常用类型的扩展方法
    /// </summary>
    public static class ExtendCorrelation
    {
        #region string 格式
        /// <summary>
        /// 将当前字符串转化为数字。遇到数字中不能出现的字符会停止，可以获取开头的负号和最多一个小数点。
        /// </summary>
        /// <param name="str">要进行转化的字符串</param>
        /// <returns></returns>
        public static double ToDouble(this string str)
        {
            string strvalue = str?.Trim() ?? "";
            string value = "";
            bool isgetfu = true;
            bool isgetxiaosshu = true;
            if (strvalue.Length == 0)
            {
                return 0;
            }
            foreach (char c in str)
            {
                if (isgetfu && c == '-')
                {
                    value += c;
                    isgetfu = false;
                    continue;
                }
                if (isgetxiaosshu && c == '.')
                {
                    if (value == "" || value == "-")
                    {
                        value += "0";
                    }
                    value += c;
                    isgetxiaosshu = false;
                    continue;
                }
                if (c >= '0' && c <= '9')
                {
                    value += c;
                    isgetfu = false;
                }
                else
                {
                    break;
                }
            }
            if (value.Length == 0)
            {
                return 0;
            }
            if (value == "-" || value == "0." || value == "-0.")
            {
                return 0;
            }
            return Convert.ToDouble(value);
        }
        /// <summary>
        /// 将当前字符串转化为数字。遇到整型数字中不能出现的字符会停止，可以获取开头的负号。
        /// </summary>
        /// <param name="str">要进行转化的字符串</param>
        /// <returns></returns>
        public static int ToInt32(this string str)
        {
            string strvalue = str?.Trim() ?? "";
            string value = "";
            bool isgetfu = true;
            if (strvalue.Length == 0)
            {
                return 0;
            }
            foreach (char c in str)
            {
                if (isgetfu && c == '-')
                {
                    value += c;
                    isgetfu = false;
                    continue;
                }
                if (c >= '0' && c <= '9')
                {
                    value += c;
                    isgetfu = false;
                }
                else if (c == '0' || c == '9')
                {
                    value += c;
                    isgetfu = false;
                }
                else
                {
                    break;
                }
            }
            if (value.Length == 0 || value == "-")
            {
                return 0;
            }
            return Convert.ToInt32(value);
        }
        /// <summary>
        /// 将当前字符串转化为数字。遇到整型数字中不能出现的字符会停止，可以获取开头的负号。
        /// </summary>
        /// <param name="str">要进行转化的字符串</param>
        /// <returns></returns>
        public static long ToInt64(this string str)
        {
            string strvalue = str?.Trim() ?? "";
            string value = "";
            bool isgetfu = true;
            if (strvalue.Length == 0)
            {
                return 0;
            }
            foreach (char c in str)
            {
                if (isgetfu && c == '-')
                {
                    value += c;
                    isgetfu = false;
                    continue;
                }
                if (c >= '0' && c <= '9')
                {
                    value += c;
                    isgetfu = false;
                }
                else if (c == '0' || c == '9')
                {
                    value += c;
                    isgetfu = false;
                }
                else
                {
                    break;
                }
            }
            if (value.Length == 0 || value == "-")
            {
                return 0;
            }
            return Convert.ToInt64(value);
        }
        #endregion

        #region string 创建

        private static readonly int[] zeros = new int[] {
            0x30, 0x660, 0x6f0, 0x7c0,
            0x966, 0x9e6, 0xa66, 0xae6, 0xb66,
            0xbe6, 0xc66, 0xce6, 0xd66, 0xde6,
            0xe50, 0xe51, 0xed0, 0xf20,
            0x1040, 0x1090, 0x17e0, 0x1810,
            0x1946, 0x19d0, 0x1a80, 0x1a90,
            0x1b50, 0x1bb0, 0x1c40, 0x1c50,
            0xa620, 0xa8d0, 0xa900, 0xa9d0,
            0xa9f0, 0xaa50, 0xabf0, 0xff10
        };
        /// <summary>
        /// 获取下一个值。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string NextValue(this string value)
        {
            return value.NextValue(DigitType.ArabicNumerals | DigitType.AutoName);
        }
        /// <summary>
        /// 使用指定类型获取下一个值。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digitType"></param>
        /// <returns></returns>
        public static string NextValue(this string value, DigitType digitType)
        {
            int digitValue = (int)digitType & 0xffff;
            if (digitValue > 1 && !Enum.IsDefined(typeof(DigitType), digitValue))
            {
                throw new ArgumentException("无效的十进制数类型。", nameof(digitType));
            }
            bool maxError = digitType.HasFlag(DigitType.MaxError);
            bool continuity = digitType.HasFlag(DigitType.Continuity);
            bool fixedLength = digitType.HasFlag(DigitType.FixedLength);
            bool noNumberAdd = digitType.HasFlag(DigitType.NoNumberAdd);
            bool trimStart = digitType.HasFlag(DigitType.TrimStart);
            bool trimEnd = !trimStart && digitType.HasFlag(DigitType.TrimEnd);
            bool trimOther = digitType.HasFlag(DigitType.TrimOther);

            StringBuilder valueArr = new StringBuilder(value);
            int index = -1;
            int zero = 0;
            int digitZero, num;
            for (int i = valueArr.Length - 1; i >= 0; i--)
            {
                digitZero = GetDigitZero(valueArr[i], out num);
                if (digitZero > 0)
                {
                    if (digitValue == (int)DigitType.First)
                    {
                        digitValue = digitZero;
                    }
                    else if (digitValue != 0 && digitValue != digitZero)
                    {
                        if (continuity && index > -1)
                        {
                            break;
                        }
                    }
                    else
                    {
                        index = i;
                        zero = digitZero;
                        if (num == 9)
                        {
                            valueArr[i] = (char)zero;
                        }
                        else
                        {
                            valueArr[i] = (char)(zero + num + 1);
                            return valueArr.ToString();
                        }
                    }
                }
                else if (continuity && index > -1)
                {
                    break;
                }
            }
            if (index > -1)
            {
                int indexOffset = TryTrim(valueArr, trimStart, trimEnd, trimOther);
                if (indexOffset == int.MaxValue)
                {
                    if (fixedLength)
                    {
                        if (maxError)
                        {
                            throw new ArgumentOutOfRangeException(nameof(value));
                        }
                        else
                        {
                            return null;
                        }
                    }
                    else
                    {
                        indexOffset = 0;
                    }
                }
                valueArr.Insert(index + indexOffset, (char)(zero + 1));
                return valueArr.ToString();
            }
            else if (noNumberAdd)
            {
                zero = digitValue > 1 ? digitValue : (int)DigitType.ArabicNumerals;
                int indexOffset = TryTrim(valueArr, trimStart, trimEnd, trimOther);
                if (fixedLength && indexOffset == int.MaxValue)
                {
                    if (maxError)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value));
                    }
                    else
                    {
                        return null;
                    }
                }
                valueArr.Append((char)(zero + 1));
                return valueArr.ToString();
            }
            throw new ArgumentException("未找到符合条件的十进制数类型。", nameof(value));
        }
        private static int TryTrim(StringBuilder value, bool trimStart, bool trimEnd, bool trimOther)
        {
            if (value.Length == 0)
            {
                return int.MaxValue;
            }
            if (trimStart)
            {
                if (char.IsWhiteSpace(value[value.Length - 1]))
                {
                    value.Remove(0, 1);
                    return -1;
                }
                else if (trimOther && char.IsWhiteSpace(value[value.Length - 1]))
                {
                    value.Length--;
                    return 0;
                }
            }
            else if (trimEnd)
            {
                if (char.IsWhiteSpace(value[value.Length - 1]))
                {
                    value.Length--;
                    return 0;
                }
                else if (trimOther && char.IsWhiteSpace(value[0]))
                {
                    value.Remove(0, 1);
                    return -1;
                }
            }
            return int.MaxValue;
        }
        private static int GetDigitZero(int value, out int digitValue)
        {
            foreach (var zero in zeros)
            {
                digitValue = value - zero;
                if (digitValue > -1 && digitValue < 10)
                {
                    return zero;
                }
            }
            digitValue = 0;
            return 0;
        }

        #endregion

        #region StringBuilder 内容
        /// <summary>
        /// 确认字符串的结尾是否是指定字符串，如果不是则添加。
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="str"></param>
        /// <returns></returns>
        public static StringBuilder SureEndString(this StringBuilder sb, string str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return sb;
            }
            else if (sb.Length < str.Length)
            {
                return sb.Append(str);
            }
            else if (str.Length == 1)
            {
                if (str[0] != sb[sb.Length - 1])
                {
                    sb.Append(str);
                }
                return sb;
            }
            else if (!sb.ToString().EndsWith(str))
            {
                sb.Append(str);
            }
            return sb;
        }
        /// <summary>
        /// 移除尾部所有符合条件的字符(串)。
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="str">要移除的字符串</param>
        /// <param name="isCompleteStr">是否将字符串视为整体。如果为 <see langword="false"/> ，则将字符串视为字符集合。</param>
        /// <returns></returns>
        public static StringBuilder TrimEnd(this StringBuilder sb, string str, bool isCompleteStr = true)
        {
            if (string.IsNullOrEmpty(str))
            {
                return sb;
            }
            else if (isCompleteStr)
            {
                if (sb.Length < str.Length)
                {
                    return sb;
                }
                if (str.Length == 1)
                {
                    while (sb.Length > 0 && str[0] == sb[sb.Length - 1])
                    {
                        sb.Length--;
                    }
                    return sb;
                }
                else
                {
                    string currValue = sb.ToString();
                    int length = 0;
                    while (currValue.Length - length >= str.Length && currValue.Substring(currValue.Length - length - str.Length, str.Length) != str)
                    {
                        length = str.Length;
                    }
                    sb.Length -= length;
                    return sb;
                }
            }
            else
            {
                while (sb.Length > 0 && str.Contains(sb[sb.Length - 1]))
                {
                    sb.Length--;
                }
                return sb;
            }
        }
        #endregion
    }

    #region DigitType 定义 (string 创建)
    /// <summary>
    /// 十进制类型和状态。可包含一个非状态位和任意个状态位。
    /// </summary>
    public enum DigitType
    {
        /// <summary>
        /// 默认（全部类型，全部数字）
        /// </summary>
        Default = 0x0000,
        /// <summary>
        /// 只修改找到的第一个类型。
        /// </summary>
        First = 0x0001,

        /// <summary>
        /// <see langword="[状态]"/> 如果超过最大值则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。否则返回 <see langword="null"/> 。
        /// </summary>
        MaxError = 0x010000,
        /// <summary>
        /// <see langword="[状态]"/> 搜索连续的数字。
        /// </summary>
        Continuity = 0x020000,
        /// <summary>
        /// <see langword="[状态]"/> 总长度固定。
        /// 如果指定了 <see cref="TrimStart"/> 或 <see cref="TrimEnd"/> ，则向前进位并移除一个空字符。
        /// 如果未指定或首尾均不包含空字符，则根据 <see cref="MaxError"/> 返回 <see langword="null"/> 或抛出异常。
        /// </summary>
        FixedLength = 0x040000,
        /// <summary>
        /// <see langword="[状态]"/> 如果未找到符合的数字，则在最后添加数字。
        /// </summary>
        NoNumberAdd = 0x080000,
        /// <summary>
        /// <see langword="[状态]"/> 进位时首先移除尾部空字符。
        /// 与 <see cref="TrimEnd"/> 同时存在时仅 <see cref="TrimStart"/> 生效。
        /// </summary>
        TrimStart = 0x100000,
        /// <summary>
        /// <see langword="[状态]"/> 进位时首先移除首部空字符。
        /// 仅当含有 <see cref="FixedLength"/> 值时有效。
        /// 与 <see cref="TrimStart"/> 同时存在时仅 <see cref="TrimStart"/> 生效。
        /// </summary>
        TrimEnd = 0x200000,
        /// <summary>
        /// <see langword="[状态]"/> 首尾均可移除。
        /// 仅当含有 <see cref="TrimEnd"/> 或 <see cref="TrimStart"/> 时有效。
        /// </summary>
        TrimOther = 0x400000,
        /// <summary>
        /// <see langword="[状态]"/> 对于默认名称的推荐状态。
        /// </summary>
        AutoName = Continuity | NoNumberAdd | TrimStart | TrimOther,
        /// <summary>
        /// <see langword="[状态]"/> 对于数据库定长字符串的推荐状态。
        /// </summary>
        CharState = Continuity | FixedLength | NoNumberAdd | TrimStart | TrimOther,

        /// <summary>
        /// 只搜索半角阿拉伯数字
        /// <para>0, 1, 2, 3, 4, 5, 6, 7, 8, 9</para>
        /// </summary>
        ArabicNumerals = 0x0030,
        /// <summary>
        /// 只搜索全角阿拉伯数字
        /// <para>０, １, ２, ３, ４, ５, ６, ７, ８, ９</para>
        /// </summary>
        ArabicNumeralsFullWidth = 0xff10,
        /// <summary>
        /// 由 0x0660 (01632) 开始的十个数字。 
        /// <para>٠, ١, ٢, ٣, ٤, ٥, ٦, ٧, ٨, ٩</para>
        /// </summary>
        Digit01632 = 0x0660,
        /// <summary>
        /// 由 0x06f0 (01776) 开始的十个数字。 
        /// <para>۰, ۱, ۲, ۳, ۴, ۵, ۶, ۷, ۸, ۹</para>
        /// </summary>
        Digit01776 = 0x06f0,
        /// <summary>
        /// 由 0x07c0 (01984) 开始的十个数字。 
        /// <para>߀, ߁, ߂, ߃, ߄, ߅, ߆, ߇, ߈, ߉</para>
        /// </summary>
        Digit01984 = 0x07c0,
        /// <summary>
        /// 由 0x0966 (02406) 开始的十个数字。 
        /// <para>०, १, २, ३, ४, ५, ६, ७, ८, ९</para>
        /// </summary>
        Digit02406 = 0x0966,
        /// <summary>
        /// 由 0x09e6 (02534) 开始的十个数字。 
        /// <para>০, ১, ২, ৩, ৪, ৫, ৬, ৭, ৮, ৯</para>
        /// </summary>
        Digit02534 = 0x09e6,
        /// <summary>
        /// 由 0x0a66 (02662) 开始的十个数字。 
        /// <para>੦, ੧, ੨, ੩, ੪, ੫, ੬, ੭, ੮, ੯</para>
        /// </summary>
        Digit02662 = 0x0a66,
        /// <summary>
        /// 由 0x0ae6 (02790) 开始的十个数字。 
        /// <para>૦, ૧, ૨, ૩, ૪, ૫, ૬, ૭, ૮, ૯</para>
        /// </summary>
        Digit02790 = 0x0ae6,
        /// <summary>
        /// 由 0x0b66 (02918) 开始的十个数字。 
        /// <para>୦, ୧, ୨, ୩, ୪, ୫, ୬, ୭, ୮, ୯</para>
        /// </summary>
        Digit02918 = 0x0b66,
        /// <summary>
        /// 由 0x0be6 (03046) 开始的十个数字。 
        /// <para>௦, ௧, ௨, ௩, ௪, ௫, ௬, ௭, ௮, ௯</para>
        /// </summary>
        Digit03046 = 0x0be6,
        /// <summary>
        /// 由 0x0c66 (03174) 开始的十个数字。 
        /// <para>౦, ౧, ౨, ౩, ౪, ౫, ౬, ౭, ౮, ౯</para>
        /// </summary>
        Digit03174 = 0x0c66,
        /// <summary>
        /// 由 0x0ce6 (03302) 开始的十个数字。 
        /// <para>೦, ೧, ೨, ೩, ೪, ೫, ೬, ೭, ೮, ೯</para>
        /// </summary>
        Digit03302 = 0x0ce6,
        /// <summary>
        /// 由 0x0d66 (03430) 开始的十个数字。 
        /// <para>൦, ൧, ൨, ൩, ൪, ൫, ൬, ൭, ൮, ൯</para>
        /// </summary>
        Digit03430 = 0x0d66,
        /// <summary>
        /// 由 0x0de6 (03558) 开始的十个数字。 
        /// <para>෦, ෧, ෨, ෩, ෪, ෫, ෬, ෭, ෮, ෯</para>
        /// </summary>
        Digit03558 = 0x0de6,
        /// <summary>
        /// 由 0x0e50 (03664) 开始的十个数字。 
        /// <para>๐, ๑, ๒, ๓, ๔, ๕, ๖, ๗, ๘, ๙</para>
        /// </summary>
        Digit03664 = 0x0e50,
        /// <summary>
        /// 由 0x0e51 (03665) 开始的十个数字。 
        /// <para>๑, ๒, ๓, ๔, ๕, ๖, ๗, ๘, ๙, ๚</para>
        /// </summary>
        Digit03665 = 0x0e51,
        /// <summary>
        /// 由 0x0ed0 (03792) 开始的十个数字。 
        /// <para>໐, ໑, ໒, ໓, ໔, ໕, ໖, ໗, ໘, ໙</para>
        /// </summary>
        Digit03792 = 0x0ed0,
        /// <summary>
        /// 由 0x0f20 (03872) 开始的十个数字。 
        /// <para>༠, ༡, ༢, ༣, ༤, ༥, ༦, ༧, ༨, ༩</para>
        /// </summary>
        Digit03872 = 0x0f20,
        /// <summary>
        /// 由 0x1040 (04160) 开始的十个数字。 
        /// <para>၀, ၁, ၂, ၃, ၄, ၅, ၆, ၇, ၈, ၉</para>
        /// </summary>
        Digit04160 = 0x1040,
        /// <summary>
        /// 由 0x1090 (04240) 开始的十个数字。 
        /// <para>႐, ႑, ႒, ႓, ႔, ႕, ႖, ႗, ႘, ႙</para>
        /// </summary>
        Digit04240 = 0x1090,
        /// <summary>
        /// 由 0x17e0 (06112) 开始的十个数字。 
        /// <para>០, ១, ២, ៣, ៤, ៥, ៦, ៧, ៨, ៩</para>
        /// </summary>
        Digit06112 = 0x17e0,
        /// <summary>
        /// 由 0x1810 (06160) 开始的十个数字。 
        /// <para>᠐, ᠑, ᠒, ᠓, ᠔, ᠕, ᠖, ᠗, ᠘, ᠙</para>
        /// </summary>
        Digit06160 = 0x1810,
        /// <summary>
        /// 由 0x1946 (06470) 开始的十个数字。 
        /// </summary>
        Digit06470 = 0x1946,
        /// <summary>
        /// 由 0x19d0 (06608) 开始的十个数字。 
        /// <para>᧐, ᧑, ᧒, ᧓, ᧔, ᧕, ᧖, ᧗, ᧘, ᧙</para>
        /// </summary>
        Digit06608 = 0x19d0,
        /// <summary>
        /// 由 0x1a80 (06784) 开始的十个数字。 
        /// </summary>
        Digit06784 = 0x1a80,
        /// <summary>
        /// 由 0x1a90 (06800) 开始的十个数字。 
        /// </summary>
        Digit06800 = 0x1a90,
        /// <summary>
        /// 由 0x1b50 (06992) 开始的十个数字。 
        /// </summary>
        Digit06992 = 0x1b50,
        /// <summary>
        /// 由 0x1bb0 (07088) 开始的十个数字。 
        /// </summary>
        Digit07088 = 0x1bb0,
        /// <summary>
        /// 由 0x1c40 (07232) 开始的十个数字。 
        /// </summary>
        Digit07232 = 0x1c40,
        /// <summary>
        /// 由 0x1c50 (07248) 开始的十个数字。 
        /// <para>᱐, ᱑, ᱒, ᱓, ᱔, ᱕, ᱖, ᱗, ᱘, ᱙</para>
        /// </summary>
        Digit07248 = 0x1c50,
        /// <summary>
        /// 由 0xa620 (42528) 开始的十个数字。 
        /// <para>꘠, ꘡, ꘢, ꘣, ꘤, ꘥, ꘦, ꘧, ꘨, ꘩</para>
        /// </summary>
        Digit42528 = 0xa620,
        /// <summary>
        /// 由 0xa8d0 (43216) 开始的十个数字。 
        /// </summary>
        Digit43216 = 0xa8d0,
        /// <summary>
        /// 由 0xa900 (43264) 开始的十个数字。 
        /// </summary>
        Digit43264 = 0xa900,
        /// <summary>
        /// 由 0xa9d0 (43472) 开始的十个数字。 
        /// <para>꧐, ꧑, ꧒, ꧓, ꧔, ꧕, ꧖, ꧗, ꧘, ꧙</para>
        /// </summary>
        Digit43472 = 0xa9d0,
        /// <summary>
        /// 由 0xa9f0 (43504) 开始的十个数字。 
        /// <para>꧰, ꧱, ꧲, ꧳, ꧴, ꧵, ꧶, ꧷, ꧸, ꧹</para>
        /// </summary>
        Digit43504 = 0xa9f0,
        /// <summary>
        /// 由 0xaa50 (43600) 开始的十个数字。 
        /// </summary>
        Digit43600 = 0xaa50,
        /// <summary>
        /// 由 0xabf0 (44016) 开始的十个数字。 
        /// </summary>
        Digit44016 = 0xabf0
    }
    #endregion

}

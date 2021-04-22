using System;

namespace Himesyo.Drawing
{
    /// <summary>
    /// 复数域的数学函数类
    /// </summary>
    public static class MathComplex
    {
        /// <summary>
        /// 指数函数，返回以e为底的指定复数次幂
        /// </summary>
        /// <param name="a">一个复数</param>
        /// <returns></returns>
        public static Complex Exp(this Complex a)
        {
            return Math.Exp(a.Re) * new Complex(Math.Cos(a.Im), Math.Sin(a.Im));
        }
        /// <summary>
        /// 余弦函数，返回指定复数的余弦值
        /// </summary>
        /// <param name="a">一个复数</param>
        /// <returns></returns>
        public static Complex Cos(Complex a)
        {
            return new Complex(Math.Cos(a.Re) * Math.Cosh(a.Im), Math.Sin(a.Re) * Math.Sinh(a.Im));

        }
        /// <summary>
        /// 正弦函数，返回指定复数的正弦值
        /// </summary>
        /// <param name="a">一个复数</param>
        /// <returns></returns>
        public static Complex Sin(Complex a)
        {
            return new Complex(Math.Sin(a.Re) * Math.Cosh(a.Im), Math.Cos(a.Re) * Math.Sinh(a.Im));
        }
        /// <summary>
        /// 正切函数，返回指定复数的正切值
        /// </summary>
        /// <param name="a">一个复数</param>
        /// <returns></returns>
        public static Complex Tan(Complex a)
        {
            return Sin(a) / Cos(a);
        }
        /// <summary>
        /// 余切函数，返回指定复数的余切值
        /// </summary>
        /// <param name="a">一个复数</param>
        /// <returns></returns>
        public static Complex Cot(Complex a)
        {
            return Cos(a) / Sin(a);
        }
        /// <summary>
        /// 正割函数，返回指定复数的正割值
        /// </summary>
        /// <param name="a">一个复数</param>
        /// <returns></returns>
        public static Complex Sec(Complex a)
        {
            return 1 / Cos(a);
        }
        /// <summary>
        /// 余割函数，返回指定复数的余割值
        /// </summary>
        /// <param name="a">一个复数</param>
        /// <returns></returns>
        public static Complex Csc(Complex a)
        {
            return 1 / Csc(a);
        }
        /// <summary>
        /// 对数函数，返回以e为底的指定复数的对数
        /// </summary>
        /// <param name="a">一个复数</param>
        /// <returns></returns>
        public static Complex Log(Complex a)
        {
            double m = a.Modulus();
            if (m > 0)
            {
                return new Complex(Math.Log(a.Modulus()), a.Arg());
            }
            else
            {
                return new Complex(0, a.Arg());
            }
        }
        /// <summary>
        /// 返回指定复数的平方根
        /// </summary>
        /// <param name="a">一个复数</param>
        /// <returns></returns>
        public static Complex Sqrt(Complex a)
        {
            double m = a.Modulus();
            return new Complex(Math.Sqrt((m + a.Re) / 2), a.Im > 0 ? Math.Sqrt((m - a.Re) / 2) : -Math.Sqrt((m - a.Re) / 2));
        }
        /// <summary>
        /// 指数函数，返回指定复数为底，指定复数次幂
        /// </summary>
        /// <param name="a">指数</param>
        /// <param name="b">底数</param>
        /// <returns></returns>
        public static Complex Pow(this Complex a, Complex b)
        {
            return Exp(b * Log(a));
        }
        /// <summary>
        /// 返回指定底的对数
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex Log(Complex a, Complex b)
        {
            return Log(a) / Log(b);
        }
        /// <summary>
        /// 反正弦函数
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Asin(Complex a)
        {
            Complex i = new Complex(0, 1);
            return -i * Log(Sqrt(1 - a * a) + i * a);
        }
        /// <summary>
        /// 反余弦函数
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Acos(Complex a)
        {
            Complex i = new Complex(0, 1);
            return -i * Log(a + i * Sqrt(1 - a * a));
        }
        /// <summary>
        /// 反正切函数
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex Atan(Complex a)
        {
            Complex i = new Complex(0, 1);
            return 0.5 * i * Log((i + a) / (i - a));
        }
    }

}

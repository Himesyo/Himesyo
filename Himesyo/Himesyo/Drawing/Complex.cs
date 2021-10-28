using System;

namespace Himesyo.Drawing
{
    /// <summary>
    /// 复数类型
    /// </summary>
    public struct Complex
    {
        /// <summary>
        /// 实部
        /// </summary>
        public double Re { get; }
        /// <summary>
        /// 虚部
        /// </summary>
        public double Im { get; }
        /// <summary>
        /// 使用指定实部和虚部初始化 <see cref="Complex"/> 类型的新实例。
        /// </summary>
        /// <param name="re"></param>
        /// <param name="im"></param>
        public Complex(double re, double im)
        {
            Re = re;
            Im = im;
        }

        /// <summary>
        /// 返回共轭
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator !(Complex a)
        {
            return new Complex(a.Re, -a.Im);
        }
        /// <summary>
        /// 返回模长
        /// </summary>
        /// <returns></returns>
        public double Modulus()
        {
            return Math.Sqrt(Re * Re + Im * Im);
        }
        /// <summary>
        /// 返回模长的平方
        /// </summary>
        /// <returns></returns>
        public double ModulusSquare()
        {
            return Re * Re + Im * Im;
        }
        /// <summary>
        /// 返回幅角
        /// </summary>
        /// <returns></returns>
        public double Arg()
        {
            if (Im > 0)
                return Math.Acos(Re / (Modulus() * 1.0000000000001));
            else
                return -Math.Acos(Re / (Modulus() * 1.0000000000001));
        }

        /*
         * 复数的四则运算
         */
        //a+b
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator +(Complex a, Complex b)
        {
            return new Complex(a.Re + b.Re, a.Im + b.Im);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator +(double a, Complex b)
        {
            return new Complex(a + b.Re, b.Im);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator +(Complex a, double b)
        {
            return new Complex(a.Re + b, a.Im);
        }
        //a-b
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator -(Complex a, Complex b)
        {
            return new Complex(a.Re - b.Re, a.Im - b.Im);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator -(double a, Complex b)
        {
            return new Complex(a - b.Re, b.Im);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator -(Complex a, double b)
        {
            return new Complex(a.Re - b, a.Im);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <returns></returns>
        public static Complex operator -(Complex a)
        {
            return new Complex(-a.Re, -a.Im);
        }
        //a*b
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator *(Complex a, Complex b)
        {
            return new Complex(a.Re * b.Re - a.Im * b.Im, a.Re * b.Im + a.Im * b.Re);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator *(double a, Complex b)
        {
            return new Complex(a * b.Re, a * b.Im);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator *(Complex a, double b)
        {
            return new Complex(a.Re * b, a.Im * b);
        }
        //a/b
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator /(Complex a, Complex b)
        {
            double m = b.ModulusSquare();
            return new Complex((a.Re * b.Re + a.Im * b.Im) / m, (a.Im * b.Re - a.Re * b.Im) / m);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator /(double a, Complex b)
        {
            double m = b.ModulusSquare();
            return new Complex(a * b.Re / m, -a * b.Im / m);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static Complex operator /(Complex a, double b)
        {
            return new Complex(a.Re / b, a.Im / b);
        }

        /// <summary>
        /// 将一个实数类型强制转换为一个仅有实部的复数类型(事实上一般是不必要的，因为所有复数四则运算都重载了以实数为参数之一)
        /// </summary>
        public static implicit operator Complex(double a)
        {
            return new Complex(a, 0);
        }
        /// <summary>
        /// 返回复数的字符串形式
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Im == 0)
            {
                return Re.ToString();
            }
            else if (Re == 0)
            {
                return $"{Im}i";
            }
            else
            {
                return $"{Re}{(Im > 0 ? "+" : "")}{Im}i";
            }
        }
    }

}

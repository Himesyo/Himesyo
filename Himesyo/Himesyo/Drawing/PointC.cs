using System.Drawing;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace Himesyo.Drawing
{
    /// <summary>
    /// 复平面点
    /// </summary>
    public struct PointC
    {
        /// <summary>
        /// 表示 <see cref="PointC"/> 类的、成员数据未被初始化的新实例。
        /// </summary>
        public static readonly PointC Empty = new PointC();

        /// <summary>
        /// 实轴坐标
        /// </summary>
        public float R { get; }
        /// <summary>
        /// 虚轴坐标
        /// </summary>
        public float I { get; }

        /// <summary>
        /// 用指定坐标初始化 <see cref="PointC"/> 类的新实例。
        /// </summary>
        /// <param name="r">实轴坐标</param>
        /// <param name="i">虚轴坐标</param>
        public PointC(float r, float i)
        {
            R = r;
            I = i;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static implicit operator PointC(PointF point)
        {
            return new PointC(point.X, point.Y);
        }
        public static implicit operator PointF(PointC point)
        {
            return new PointF(point.R, point.I);
        }

        public static implicit operator Complex(PointC point)
        {
            return new Complex(point.R, point.I);
        }
        public static implicit operator PointC(Complex complex)
        {
            return new PointC((float)complex.Re, (float)complex.Im);
        }

        public static bool operator ==(PointC left, PointC right)
        {
            return left.R == right.R && left.I == right.I;
        }
        public static bool operator !=(PointC left, PointC right)
        {
            return left.R != right.R || left.I != right.I;
        }

        public override string ToString()
        {
            return $"{R}, {I}";

            //if (I == 0)
            //{
            //    return R.ToString();
            //}
            //else if (R == 0)
            //{
            //    return $"{I}i";
            //}
            //else if (I > 0)
            //{
            //    return $"{R} + {I}i";
            //}
            //else
            //{
            //    return $"{R} - {-I}i";
            //}
        }
    }
}

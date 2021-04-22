using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace Himesyo.Drawing
{
    /// <summary>
    /// 极坐标点
    /// </summary>
    public struct PointP
    {
        /// <summary>
        /// 表示 <see cref="PointP"/> 类的、成员数据未被初始化的新实例。
        /// </summary>
        public static readonly PointP Empty = new PointP();

        /// <summary>
        /// 半径坐标
        /// </summary>
        public float R { get; }
        /// <summary>
        /// 极角
        /// </summary>
        private float polarAngle;
        /// <summary>
        /// 极角
        /// </summary>
        public float T
        {
            get { return polarAngle; }
            private set
            {
                float pi2 = (float)(2 * Math.PI);
                if (value >= 0)
                {
                    polarAngle = value % pi2;
                }
                else
                {
                    polarAngle = value % pi2 + pi2;
                }
            }
        }

        /// <summary>
        /// 用指定坐标初始化 <see cref="PointP"/> 类的新实例。
        /// </summary>
        /// <param name="r">半径坐标</param>
        /// <param name="t">极角</param>
        public PointP(float r, float t)
        {
            R = r;
            polarAngle = t;
            T = t;
        }

        public static implicit operator PointP(PointF point)
        {
            return new PointP();
        }
        public static implicit operator PointF(PointP point)
        {
            return new PointF();
        }

        public static bool operator ==(PointP left, PointP right)
        {
            return left.R == right.R && left.T == right.T;
        }
        public static bool operator !=(PointP left, PointP right)
        {
            return left.R != right.R || left.T != right.T;
        }

        public override string ToString()
        {
            return $"{R}, {T}";
        }
    }

}

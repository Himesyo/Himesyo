using System.Drawing;

namespace Himesyo.Drawing
{
    /// <summary>
    /// 坐标转换器
    /// </summary>
    public struct PointConverter
    {
        private PointType type;
        private PointF? cartesianPoint;
        private PointC? complexPoint;
        private PointP? polarPoint;

        public PointF CartesianPoint
        {
            get
            {
                if (cartesianPoint == null)
                {
                    cartesianPoint = new PointF();
                }
                return cartesianPoint.Value;
            }
            set
            {
                type = PointType.CartesianPoint;
                if (cartesianPoint != value)
                {
                    complexPoint = null;
                    polarPoint = null;
                }
            }
        }
        public PointC ComplexPoint
        {
            get
            {
                if (complexPoint == null)
                {
                    complexPoint = new PointC();
                }
                return complexPoint.Value;
            }
            set
            {
                type = PointType.ComplexPoint;
                if (complexPoint != value)
                {
                    cartesianPoint = null;
                    polarPoint = null;
                }
            }
        }
        public PointP PolarPoint
        {
            get
            {
                if (polarPoint == null)
                {
                    polarPoint = new PointP();
                }
                return polarPoint.Value;
            }
            set
            {
                type = PointType.PolarPoint;
                if (polarPoint != value)
                {
                    cartesianPoint = null;
                    complexPoint = null;
                }
            }
        }

        public PointConverter(PointF point)
        {
            type = PointType.CartesianPoint;
            cartesianPoint = point;
            complexPoint = null;
            polarPoint = null;
        }
        public PointConverter(PointC point)
        {
            type = PointType.ComplexPoint;
            cartesianPoint = null;
            complexPoint = point;
            polarPoint = null;
        }
        public PointConverter(PointP point)
        {
            type = PointType.PolarPoint;
            cartesianPoint = null;
            complexPoint = null;
            polarPoint = point;
        }

        public static implicit operator PointConverter(PointF point)
        {
            return new PointConverter(point);
        }
        public static implicit operator PointConverter(PointC point)
        {
            return new PointConverter(point);
        }
        public static implicit operator PointConverter(PointP point)
        {
            return new PointConverter(point);
        }
        public static implicit operator PointF(PointConverter point)
        {
            return point.CartesianPoint;
        }
        public static implicit operator PointC(PointConverter point)
        {
            return point.ComplexPoint;
        }
        public static implicit operator PointP(PointConverter point)
        {
            return point.PolarPoint;
        }

        /// <summary>
        /// 坐标类型
        /// </summary>
        private enum PointType
        {
            /// <summary>
            /// 未指定
            /// </summary>
            Empty,
            /// <summary>
            /// 笛卡尔坐标
            /// </summary>
            CartesianPoint,
            /// <summary>
            /// 复平面坐标
            /// </summary>
            ComplexPoint,
            /// <summary>
            /// 极坐标
            /// </summary>
            PolarPoint
        }
    }
}

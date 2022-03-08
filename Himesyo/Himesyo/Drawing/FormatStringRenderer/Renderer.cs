using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;

namespace Himesyo.Drawing
{
    public static class Renderer
    {
        public static Rectangle UpperLeftCut(this Rectangle rect, Size size)
        {
            rect = rect.Foramt();
            return new Rectangle(rect.Left + size.Width, rect.Top + size.Height, rect.Width - size.Width, rect.Height - size.Height);
        }
        public static Rectangle UpperLeftCut(this Rectangle rect, int width, int height)
        {
            rect = rect.Foramt();
            return new Rectangle(rect.Left + width, rect.Top + height, rect.Width - width, rect.Height - height);
        }
        public static RectangleF UpperLeftCut(this RectangleF rect, SizeF size)
        {
            rect = rect.Foramt();
            return new RectangleF(rect.Left + size.Width, rect.Top + size.Height, rect.Width - size.Width, rect.Height - size.Height);
        }
        public static RectangleF UpperLeftCut(this RectangleF rect, float width, float height)
        {
            rect = rect.Foramt();
            return new RectangleF(rect.Left + width, rect.Top + height, rect.Width - width, rect.Height - height);
        }

        public static Rectangle LowerLeftCut(this Rectangle rect, Size size)
        {
            rect = rect.Foramt();
            return new Rectangle(rect.Left + size.Width, rect.Top, rect.Width - size.Width, rect.Height - size.Height);
        }
        public static Rectangle LowerLeftCut(this Rectangle rect, int width, int height)
        {
            rect = rect.Foramt();
            return new Rectangle(rect.Left + width, rect.Top, rect.Width - width, rect.Height - height);
        }
        public static RectangleF LowerLeftCut(this RectangleF rect, SizeF size)
        {
            rect = rect.Foramt();
            return new RectangleF(rect.Left + size.Width, rect.Top, rect.Width - size.Width, rect.Height - size.Height);
        }
        public static RectangleF LowerLeftCut(this RectangleF rect, float width, float height)
        {
            rect = rect.Foramt();
            return new RectangleF(rect.Left + width, rect.Top, rect.Width - width, rect.Height - height);
        }

        public static Rectangle UpperRightCut(this Rectangle rect, Size size)
        {
            rect = rect.Foramt();
            return new Rectangle(rect.Left, rect.Top + size.Height, rect.Width - size.Width, rect.Height - size.Height);
        }
        public static Rectangle UpperRightCut(this Rectangle rect, int width, int height)
        {
            rect = rect.Foramt();
            return new Rectangle(rect.Left, rect.Top + height, rect.Width - width, rect.Height - height);
        }
        public static RectangleF UpperRightCut(this RectangleF rect, SizeF size)
        {
            rect = rect.Foramt();
            return new RectangleF(rect.Left, rect.Top + size.Height, rect.Width - size.Width, rect.Height - size.Height);
        }
        public static RectangleF UpperRightCut(this RectangleF rect, float width, float height)
        {
            rect = rect.Foramt();
            return new RectangleF(rect.Left, rect.Top + height, rect.Width - width, rect.Height - height);
        }

        public static Rectangle LowerRightCut(this Rectangle rect, Size size)
        {
            rect = rect.Foramt();
            return new Rectangle(rect.Left, rect.Top, rect.Width - size.Width, rect.Height - size.Height);
        }
        public static Rectangle LowerRightCut(this Rectangle rect, int width, int height)
        {
            rect = rect.Foramt();
            return new Rectangle(rect.Left, rect.Top, rect.Width - width, rect.Height - height);
        }
        public static RectangleF LowerRightCut(this RectangleF rect, SizeF size)
        {
            rect = rect.Foramt();
            return new RectangleF(rect.Left, rect.Top, rect.Width - size.Width, rect.Height - size.Height);
        }
        public static RectangleF LowerRightCut(this RectangleF rect, float width, float height)
        {
            rect = rect.Foramt();
            return new RectangleF(rect.Left, rect.Top, rect.Width - width, rect.Height - height);
        }

        /* 无法更改本体
        //public static void UpperLeftOffset(this Rectangle rect, Size size)
        //{
        //    rect.X = rect.Left + size.Width;
        //    rect.Y = rect.Top + size.Height;
        //    rect.Width = rect.Width - size.Width;
        //    rect.Height = rect.Height - size.Height;
        //}
        //public static void UpperLeftOffset(this Rectangle rect, int width, int height)
        //{
        //    rect.X = rect.Left + width;
        //    rect.Y = rect.Top + height;
        //    rect.Width = rect.Width - width;
        //    rect.Height = rect.Height - height;
        //}
        //public static void UpperLeftOffset(this RectangleF rect, SizeF size)
        //{
        //    rect.X = rect.Left + size.Width;
        //    rect.Y = rect.Top + size.Height;
        //    rect.Width = rect.Width - size.Width;
        //    rect.Height = rect.Height - size.Height;
        //}
        //public static void UpperLeftOffset(this RectangleF rect, float width, float height)
        //{
        //    rect.X = rect.Left + width;
        //    rect.Y = rect.Top + height;
        //    rect.Width = rect.Width - width;
        //    rect.Height = rect.Height - height;
        //}

        //public static void LowerLeftOffset(this Rectangle rect, Size size)
        //{
        //    rect.X = rect.Left + size.Width;
        //    rect.Y = rect.Top;
        //    rect.Width = rect.Width - size.Width;
        //    rect.Height = rect.Height - size.Height;
        //}
        //public static void LowerLeftOffset(this Rectangle rect, int width, int height)
        //{
        //    rect.X = rect.Left + width;
        //    rect.Y = rect.Top;
        //    rect.Width = rect.Width - width;
        //    rect.Height = rect.Height - height;
        //}
        //public static void LowerLeftOffset(this RectangleF rect, SizeF size)
        //{
        //    rect.X = rect.Left + size.Width;
        //    rect.Y = rect.Top;
        //    rect.Width = rect.Width - size.Width;
        //    rect.Height = rect.Height - size.Height;
        //}
        //public static void LowerLeftOffset(this RectangleF rect, float width, float height)
        //{
        //    rect.X = rect.Left + width;
        //    rect.Y = rect.Top;
        //    rect.Width = rect.Width - width;
        //    rect.Height = rect.Height - height;
        //}

        //public static void UpperRightOffset(this Rectangle rect, Size size)
        //{
        //    rect.X = rect.Left;
        //    rect.Y = rect.Top + size.Height;
        //    rect.Width = rect.Width - size.Width;
        //    rect.Height = rect.Height - size.Height;
        //}
        //public static void UpperRightOffset(this Rectangle rect, int width, int height)
        //{
        //    rect.X = rect.Left;
        //    rect.Y = rect.Top + height;
        //    rect.Width = rect.Width - width;
        //    rect.Height = rect.Height - height;
        //}
        //public static void UpperRightOffset(this RectangleF rect, SizeF size)
        //{
        //    rect.X = rect.Left;
        //    rect.Y = rect.Top + size.Height;
        //    rect.Width = rect.Width - size.Width;
        //    rect.Height = rect.Height - size.Height;
        //}
        //public static void UpperRightOffset(this RectangleF rect, float width, float height)
        //{
        //    rect.X = rect.Left;
        //    rect.Y = rect.Top + height;
        //    rect.Width = rect.Width - width;
        //    rect.Height = rect.Height - height;
        //}

        //public static void LowerRightOffset(this Rectangle rect, Size size)
        //{
        //    rect.X = rect.Left;
        //    rect.Y = rect.Top;
        //    rect.Width = rect.Width - size.Width;
        //    rect.Height = rect.Height - size.Height;
        //}
        //public static void LowerRightOffset(this Rectangle rect, int width, int height)
        //{
        //    rect.X = rect.Left;
        //    rect.Y = rect.Top;
        //    rect.Width = rect.Width - width;
        //    rect.Height = rect.Height - height;
        //}
        //public static void LowerRightOffset(this RectangleF rect, SizeF size)
        //{
        //    rect.X = rect.Left;
        //    rect.Y = rect.Top;
        //    rect.Width = rect.Width - size.Width;
        //    rect.Height = rect.Height - size.Height;
        //}
        //public static void LowerRightOffset(this RectangleF rect, float width, float height)
        //{
        //    rect.X = rect.Left;
        //    rect.Y = rect.Top;
        //    rect.Width = rect.Width - width;
        //    rect.Height = rect.Height - height;
        //}
        */

        public static Rectangle Foramt(this Rectangle rect)
        {
            if (rect.Width < 0)
            {
                rect.X = rect.Right;
                rect.Width = -rect.Width;
            }
            if (rect.Height < 0)
            {
                rect.Y = rect.Bottom;
                rect.Height = -rect.Height;
            }
            return rect;
        }
        public static RectangleF Foramt(this RectangleF rect)
        {
            if (rect.Width < 0)
            {
                rect.X = rect.Right;
                rect.Width = -rect.Width;
            }
            if (rect.Height < 0)
            {
                rect.Y = rect.Bottom;
                rect.Height = -rect.Height;
            }
            return rect;
        }
    }
}

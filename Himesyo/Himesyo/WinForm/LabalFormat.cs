using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

using Himesyo.ComponentModel;
using Himesyo.Drawing;

namespace Himesyo.WinForm
{
    [DefaultProperty(nameof(FormatText))]
    [DefaultEvent(nameof(Click))]
    public class LabalFormat : Label
    {
        private readonly VScrollBar vBar;

        /// <summary>
        /// 带格式的文本。
        /// </summary>
        [Category("外观")]
        [Description("带格式的文本。")]
        [Editor(ConstValue.MultilineStringEditor, typeof(UITypeEditor))]
        public string FormatText
        {
            get => base.Text;
            set => base.Text = value;
        }

        [DefaultValue(System.Drawing.ContentAlignment.TopLeft)]
        public override System.Drawing.ContentAlignment TextAlign
        {
            get => base.TextAlign;
            set => base.TextAlign = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override string Text
        {
            get => string.Empty;
            set { }
            //get => base.Text;
            //set => base.Text = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [DefaultValue(false)]
        public sealed override bool AutoSize
        {
            get => false;
            set { }
        }

        [Category("外观")]
        [Description("当前滚动条的位置。")]
        [DefaultValue(0)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public int ScrollBarPosition
        {
            get => vBar.Value;
            set
            {
                CheckVBar(value);
                Invalidate();
            }
        }

        [Description("初始化格式字符串渲染器时触发。")]
        public event Action<LabalFormat, FormatStringRenderer> InitStringRenderer;

        public LabalFormat()
        {
            base.AutoSize = false;
            TextAlign = System.Drawing.ContentAlignment.TopLeft;
            Paint += labalFormat_Paint;

            vBar = new VScrollBar();
            vBar.Dock = DockStyle.Right;
            vBar.Visible = false;
            vBar.Scroll += vBar_Scroll;
            this.Controls.Add(vBar);
            this.MouseWheel += labalFormat_MouseWheel;
        }

        /// <summary>
        /// 将 <see cref="FormatText"/> 属性重置为其默认值。
        /// </summary>
        public override void ResetText()
        {
            FormatText = string.Empty;
        }

        /// <summary>
        /// 校验并设置垂直滚动条的新位置。
        /// </summary>
        /// <param name="moveTo"></param>
        private void CheckVBar(int moveTo)
        {
            int value = moveTo;
            int minValue = vBar.Minimum;
            int maxValue = vBar.Maximum - vBar.LargeChange;
            if (value < minValue) value = minValue;
            if (value > maxValue) value = maxValue;
            vBar.Value = value;
        }

        private void labalFormat_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            rect = rect.UpperLeftCut(2, 2);
            FormatStringRenderer renderer = new FormatStringRenderer(e.Graphics) { Font = Font };
            InitStringRenderer?.Invoke(this, renderer);
            string formatString = FormatText;
            RectangleF drawRect = rect;
            SizeF drawSize = renderer.MeasureFormatString(formatString, drawRect.Width);
            if (drawSize.Height > drawRect.Height)
            {
                drawRect.Width -= vBar.Width + 2;
                drawSize = renderer.MeasureFormatString(formatString, drawRect.Width);
                vBar.Maximum = (int)Math.Ceiling(drawSize.Height);
                vBar.LargeChange = (int)Math.Floor(drawRect.Height);
                CheckVBar(vBar.Value);
                drawRect = drawRect.UpperRightCut(0, -vBar.Value);
                vBar.Visible = true;
            }
            else
            {
                vBar.Visible = false;
            }
            if (DesignMode)
            {
                try
                {
                    renderer.DrawFormatString(formatString, drawRect);
                }
                catch (Exception ex)
                {
                    RadioButtonRenderer.DrawParentBackground(e.Graphics, ClientRectangle, this);
                    e.Graphics.DrawString(ex.Message, Font, Brushes.Red, ClientRectangle);
                }
            }
            else
            {
                renderer.DrawFormatString(formatString, drawRect);
            }
        }

        private void vBar_Scroll(object sender, ScrollEventArgs e)
        {
            Invalidate();
        }

        private void labalFormat_MouseWheel(object sender, MouseEventArgs e)
        {
            if (vBar.Visible)
            {
                float delta = e.Delta * 0.5f;
                int move = (int)Math.Round(delta);
                CheckVBar(vBar.Value - move);
                Invalidate();
            }
        }

    }
}

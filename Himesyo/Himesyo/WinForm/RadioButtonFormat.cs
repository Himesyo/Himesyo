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

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace Himesyo.WinForm
{
    [DefaultProperty(nameof(FormatText))]
    [DefaultEvent(nameof(Click))]
    public class RadioButtonFormat : RadioButton
    {
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

        /// <inheritdoc/>
        [DefaultValue(System.Drawing.ContentAlignment.TopLeft)]
        public override System.Drawing.ContentAlignment TextAlign
        {
            get => base.TextAlign;
            set => base.TextAlign = value;
        }

        [DefaultValue(System.Drawing.ContentAlignment.TopLeft)]
        public new System.Drawing.ContentAlignment CheckAlign
        {
            get => base.CheckAlign;
            set => base.CheckAlign = value;
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

        [Description("初始化格式字符串渲染器时触发。")]
        public event Action<RadioButtonFormat, FormatStringRenderer> InitStringRenderer;

        public RadioButtonFormat()
        {
            base.AutoSize = false;
            CheckAlign = System.Drawing.ContentAlignment.TopLeft;
            TextAlign = System.Drawing.ContentAlignment.TopLeft;
            Paint += radioButtonFormat_Paint;
        }

        /// <summary>
        /// 将 <see cref="FormatText"/> 属性重置为其默认值。
        /// </summary>
        public override void ResetText()
        {
            FormatText = string.Empty;
        }

        private void radioButtonFormat_Paint(object sender, PaintEventArgs e)
        {
            Rectangle rect = ClientRectangle;
            Size sizeGlyph = RadioButtonRenderer.GetGlyphSize(e.Graphics, RadioButtonState.CheckedNormal);
            Padding padding = new Padding(2);
            switch (CheckAlign)
            {
                case System.Drawing.ContentAlignment.TopLeft:
                case System.Drawing.ContentAlignment.MiddleLeft:
                case System.Drawing.ContentAlignment.BottomLeft:
                default:
                    rect = rect.UpperLeftCut(sizeGlyph.Width + Padding.Left + padding.Left, Padding.Top + padding.Top).LowerRightCut(Padding.Right + padding.Right, Padding.Bottom + padding.Bottom);
                    break;
                case System.Drawing.ContentAlignment.TopRight:
                case System.Drawing.ContentAlignment.MiddleRight:
                case System.Drawing.ContentAlignment.BottomRight:
                    rect = rect.UpperLeftCut(Padding.Left + padding.Left, Padding.Top + padding.Top).LowerRightCut(sizeGlyph.Width + Padding.Right + padding.Right, Padding.Bottom + padding.Bottom);
                    break;
                case System.Drawing.ContentAlignment.TopCenter:
                    rect = rect.UpperLeftCut(Padding.Left + padding.Left, sizeGlyph.Height + Padding.Top + padding.Top).LowerRightCut(Padding.Right + padding.Right, Padding.Bottom + padding.Bottom);
                    break;
                case System.Drawing.ContentAlignment.MiddleCenter:
                    break;
                case System.Drawing.ContentAlignment.BottomCenter:
                    rect = rect.UpperLeftCut(Padding.Left + padding.Left, Padding.Top + padding.Top).LowerRightCut(Padding.Right + padding.Right, sizeGlyph.Height + Padding.Bottom + padding.Bottom);
                    break;
            }
            RadioButtonRenderer.DrawParentBackground(e.Graphics, rect, this);
            FormatStringRenderer renderer = new FormatStringRenderer(e.Graphics) { Font = Font };
            InitStringRenderer?.Invoke(this, renderer);
            string formatString = FormatText;
            RectangleF drawRect = rect;
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
    }
}

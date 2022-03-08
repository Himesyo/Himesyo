using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Layout;

namespace Himesyo.WinForm
{
    /// <summary>
    /// 处理其组件的布局并以纵向列表的形式自动排列它们。
    /// </summary>
    [DefaultProperty(nameof(ListLayoutPanel.Anchor))]
    [DefaultEvent(nameof(ListLayoutPanel.Layout))]
    [Docking(DockingBehavior.Ask)]
    [Description("处理其组件的布局并以纵向列表的形式自动排列它们。")]
    public partial class ListLayoutPanel : Panel
    {
        /// <summary>
        /// 使用默认设置初始化 <see cref="ListLayoutPanel"/> 的新实例。
        /// </summary>
        public ListLayoutPanel()
        {
            InitializeComponent();
        }

        /// <inheritdoc/>
        public override LayoutEngine LayoutEngine => ListLayoutEngine.Instance;
    }

    /// <summary>
    /// 纵向列表的排列引擎。
    /// </summary>
    public class ListLayoutEngine : LayoutEngine
    {
        /// <summary>
        /// <see cref="ListLayoutEngine"/> 默认实例。
        /// </summary>
        public static ListLayoutEngine Instance { get; } = new ListLayoutEngine();

        /// <inheritdoc/>
        public override bool Layout(object container, LayoutEventArgs layoutEventArgs)
        {
            Control parent = container as Control;

            Rectangle parentDisplayRectangle = parent.DisplayRectangle;
            Point nextControlLocation = parentDisplayRectangle.Location;
            int parentUsableWidth = parent.ClientSize.Width - parent.Padding.Horizontal;
            int nextUsableWidth = parentUsableWidth;

            foreach (Control c in parent.Controls)
            {
                if (!c.Visible)
                {
                    continue;
                }

                nextControlLocation.Offset(c.Margin.Left, c.Margin.Top);
                nextUsableWidth -= c.Margin.Horizontal;

                c.Location = nextControlLocation;
                //c.AutoScrollOffset = nextControlLocation;

                Size newSize = c.AutoSize ? c.GetPreferredSize(parentDisplayRectangle.Size) : c.Size;
                newSize = new Size(nextUsableWidth, newSize.Height);
                if (newSize != c.Size)
                {
                    c.Size = newSize;
                }

                nextControlLocation.X = parentDisplayRectangle.X;

                nextControlLocation.Y += c.Height + c.Margin.Bottom;

                nextUsableWidth = parentUsableWidth;
            }
            return false;
        }
    }
}

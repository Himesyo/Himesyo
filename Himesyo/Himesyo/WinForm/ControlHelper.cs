using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Himesyo.WinForm
{
    /// <summary>
    /// 对部分控件提供一些扩展功能和辅助方法。
    /// </summary>
    public static class ControlHelper
    {
        #region PropertyGrid
        private static MemberToProperty<int> memberSplitterDistance;
        /// <summary>
        /// 获取封装分割器分割位置的属性。
        /// </summary>
        /// <param name="grid"></param>
        /// <returns></returns>
        public static Property<int> GetSplitterDistanceProperty(this PropertyGrid grid)
        {
            if (grid == null)
                return null;
            // System.Windows.Forms.PropertyGridInternal.PropertyGridView
            Control gridView = grid?.Controls
                .OfType<Control>()
                .FirstOrDefault(control => control.GetType().Name == "PropertyGridView");
            if (gridView == null)
                return null;
            if (memberSplitterDistance == null)
            {
                Type type = gridView.GetType();
                PropertyInfo internalLabelWidth = type.GetProperty("InternalLabelWidth", BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo moveSplitterTo = type.GetMethod("MoveSplitterTo", BindingFlags.NonPublic | BindingFlags.Instance);
                memberSplitterDistance = new MemberToProperty<int>(internalLabelWidth, moveSplitterTo);
            }
            return memberSplitterDistance.CreateProperty(gridView);
        }
        #endregion
    }
}

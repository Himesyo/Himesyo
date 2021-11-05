using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Himesyo.ComponentModel
{
    /// <summary>
    /// 表示对一个对象的封装用于自定义属性说明符。
    /// </summary>
    public class ShowObject : CustomTypeDescriptor
    {
        private object showObject;

        /// <summary>
        /// 要封装的对象。
        /// </summary>
        /// <param name="obj"></param>
        public ShowObject(object obj)
        {
            showObject = obj;
        }

        /// <summary>
        /// 是否显示隐藏的属性。
        /// </summary>
        public bool ShowHide { get; set; }
        /// <summary>
        /// 设定要显示的属性和属性的信息。
        /// </summary>
        public Dictionary<string, ShowPropertyInfo> ShowItems { get; } = new Dictionary<string, ShowPropertyInfo>();

        #region CustomTypeDescriptor 成员

        /// <summary>
        /// 获取包含指定特性的属性说明符集合。
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            var pds = TypeDescriptor.GetProperties(showObject)
                .Cast<PropertyDescriptor>()
                .Where(pd => ShowItems.ContainsKey(pd.Name))
                .Select(pd => new { Descriptor = pd, ShowInfo = ShowItems[pd.Name] ?? new ShowPropertyInfo() })
                .Where(property => ShowHide || property.ShowInfo.Hide)
                .Select(property => TypeDescriptor.CreateProperty(property.Descriptor.ComponentType, property.Descriptor, property.ShowInfo.Attributes.ToArray()))
                .ToArray();
            return new PropertyDescriptorCollection(pds);
        }

        /// <summary>
        /// 返回包含指定的属性描述符所描述的属性的对象。
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            if (showObject is ICustomTypeDescriptor obj)
            {
                return obj.GetPropertyOwner(pd);
            }
            else
            {
                return showObject;
            }
        }

        #endregion
    }

    public class ShowPropertyInfo
    {
        public bool Hide { get; set; }
        public List<Attribute> Attributes { get; } = new List<Attribute>();

        public void AddDisplayName(string name)
        {
            Attributes.Add(new DisplayNameAttribute(name));
        }
        public void AddCategory(string category)
        {
            Attributes.Add(new CategoryAttribute(category));
        }
        public void AddDescription(string description)
        {
            Attributes.Add(new DescriptionAttribute(description));
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
namespace Himesyo.ComponentModel
{
    /// <summary>
    /// 表示对一个对象的封装用于自定义属性说明符。
    /// </summary>
    public class ShowObject : CustomTypeDescriptor
    {
        public event EventHandler ReturningPropertiesBefore;

        /// <summary>
        /// 封装的对象。
        /// </summary>
        public object ComponentObject { get; }

        /// <summary>
        /// 要封装的对象。
        /// </summary>
        /// <param name="obj"></param>
        public ShowObject(object obj)
        {
            ComponentObject = obj;
        }

        /// <summary>
        /// 设定要显示的属性和属性的信息。
        /// </summary>
        public Dictionary<string, ShowPropertyInfo> ShowItems { get; } = new Dictionary<string, ShowPropertyInfo>();
        /// <summary>
        /// 是否显示隐藏的属性。
        /// </summary>
        public bool ShowHide { get; set; }
        /// <summary>
        /// 排序依据。
        /// </summary>
        public string[] Sort { get; set; }

        /// <summary>
        /// 添加要显示的属性和其显示信息。
        /// </summary>
        /// <param name="name"></param>
        /// <param name="displayName"></param>
        /// <param name="category"></param>
        /// <param name="hide"></param>
        public void Add(string name, string displayName, string category, bool hide = false)
        {
            ShowPropertyInfo info = new ShowPropertyInfo();
            info.Hide = hide;
            if (!string.IsNullOrWhiteSpace(displayName))
            {
                info.AddDisplayName(displayName);
            }
            if (!string.IsNullOrWhiteSpace(category))
            {
                info.AddCategory(category);
            }
            ShowItems.Add(name, info);
        }
        /// <summary>
        /// 自动添加显示项。
        /// </summary>
        /// <param name="defaultInfo"></param>
        /// <returns></returns>
        public string[] AutoAddShowItems(ShowPropertyInfo defaultInfo)
        {
            if (defaultInfo == null)
            {
                defaultInfo = new ShowPropertyInfo();
            }
            var names = TypeDescriptor.GetProperties(ComponentObject)
                .Cast<PropertyDescriptor>()
                .Where(pd => !ShowItems.ContainsKey(pd.Name))
                .Select(pd =>
                {
                    ShowPropertyInfo info = (ShowPropertyInfo)defaultInfo.Clone();
                    ShowItems.Add(pd.Name, info);
                    return pd.Name;
                })
                .ToArray();
            return names;
        }

        #region CustomTypeDescriptor 成员

        /// <summary>
        /// 获取包含指定特性的属性说明符集合。
        /// </summary>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            //AutoAddShowItems(new ShowPropertyInfo(false));
            ReturningPropertiesBefore?.Invoke(this, EventArgs.Empty);

            var pds = TypeDescriptor.GetProperties(ComponentObject)
                .Cast<PropertyDescriptor>()
                .Where(pd => ShowItems.ContainsKey(pd.Name))
                .Select(pd => new { Descriptor = pd, ShowInfo = ShowItems[pd.Name] ?? new ShowPropertyInfo() })
                .Where(property => ShowHide || !property.ShowInfo.Hide)
                .Select(property => new ShowObjectPropertyDescriptor(property.Descriptor, property.ShowInfo.Attributes.ToArray()))
                .ToArray();
            PropertyDescriptorCollection collection = new PropertyDescriptorCollection(pds);
            if (Sort != null)
            {
                collection = collection.Sort(Sort);
            }
            return collection;
        }

        /// <summary>
        /// 返回包含指定的属性描述符所描述的属性的对象。
        /// </summary>
        /// <param name="pd"></param>
        /// <returns></returns>
        public override object GetPropertyOwner(PropertyDescriptor pd)
        {
            if (ComponentObject is ICustomTypeDescriptor obj && pd is ShowObjectPropertyDescriptor p)
            {
                object propertyOwner = obj.GetPropertyOwner(p.Property);
                return propertyOwner;
            }
            else
            {
                return ComponentObject;
            }
        }

        #endregion

        private class ShowObjectPropertyDescriptor : PropertyDescriptor
        {
            public PropertyDescriptor Property { get; }

            public override Type ComponentType => Property.ComponentType;
            public override bool IsReadOnly => Property.IsReadOnly;
            public override Type PropertyType => Property.PropertyType;

            public ShowObjectPropertyDescriptor(PropertyDescriptor descriptor, Attribute[] attributes)
                : base(descriptor, attributes)
            {
                Property = descriptor;
            }

            public override bool CanResetValue(object component)
            {
                return false;
            }

            public override object GetValue(object component)
            {
                return Property.GetValue(component);
            }

            public override void ResetValue(object component)
            {
                throw new NotImplementedException();
            }

            public override void SetValue(object component, object value)
            {
                Property.SetValue(component, value);
            }

            public override bool ShouldSerializeValue(object component)
            {
                return false;
            }
        }
    }

    public class ShowPropertyInfo : ICloneable
    {
        public bool Hide { get; set; }
        public List<Attribute> Attributes { get; private set; } = new List<Attribute>();

        public ShowPropertyInfo() { }
        public ShowPropertyInfo(bool hide)
        {
            Hide = hide;
        }

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

        public virtual object Clone()
        {
            ShowPropertyInfo other = (ShowPropertyInfo)MemberwiseClone();
            other.Attributes = Attributes.ToList();
            return other;
        }
    }
}

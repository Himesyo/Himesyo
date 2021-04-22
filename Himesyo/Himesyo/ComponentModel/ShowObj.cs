using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Himesyo.ComponentModel
{
    /// <summary>
    /// 表示一个可在 <see cref="PropertyGrid"/> 显示的对象。它所显示的属性，由一个 <see cref="T"/> 类型的集合决定。
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ShowObj<T> : CustomTypeDescriptor where T : IShowProperty
    {
        private static object PropertyOwner { get; } = new object();

        private PropertyDescriptorCollection PropertyDescriptor { get; set; }
        /// <summary>
        /// 显示的属性集合。
        /// </summary>
        protected ICollection<T> Collection { get; set; }
        /// <summary>
        /// 初始化空集合实例。
        /// </summary>
        protected ShowObj()
        {

        }
        /// <summary>
        /// 使用指定集合初始化新实例。
        /// </summary>
        /// <param name="objs"></param>
        public ShowObj(IEnumerable<T> objs)
        {
            var result = from obj in objs
                         select new ShowPropertyDescriptor(obj);
            PropertyDescriptor = new PropertyDescriptorCollection(result.ToArray());
        }

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override PropertyDescriptorCollection GetProperties(Attribute[] attributes)
        {
            if (PropertyDescriptor != null)
            {
                return PropertyDescriptor;
            }
            if (Collection == null)
            {
                throw new NullReferenceException("要显示的 Collection 为 null ，请在显示前对 Collection 赋值。如果没有需要显示的属性，请设置为空集合。");
            }
            var result = from obj in Collection
                         select new ShowPropertyDescriptor(obj);
            PropertyDescriptor = new PropertyDescriptorCollection(result.ToArray());
            return PropertyDescriptor;
        }

        public sealed override object GetPropertyOwner(PropertyDescriptor pd)
        {
            return PropertyOwner;
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }

    /// <summary>
    /// 表示一个可显示的属性。
    /// </summary>
    public interface IShowProperty
    {
        /// <summary>
        /// 显示的名称
        /// </summary>
        string Name { get; }
        /// <summary>
        /// 属性类型
        /// </summary>
        Type PropertyType { get; }

        /// <summary>
        /// 特性集合
        /// </summary>
        ICollection<Attribute> Attributes { get; }

        /// <summary>
        /// 获取值。
        /// </summary>
        /// <returns></returns>
        object GetValue();
        /// <summary>
        /// 设置值。
        /// </summary>
        /// <param name="newValue"></param>
        void SetValue(object newValue);
    }

    /// <summary>
    /// 为 <see cref="IShowProperty"/> 所指示的属性提供属性说明。
    /// </summary>
    public class ShowPropertyDescriptor : PropertyDescriptor
    {
        private IShowProperty componentObject;

#pragma warning disable CS1591 // 缺少对公共可见类型或成员的 XML 注释
        public override Type ComponentType { get; } = typeof(object);
        public override bool IsReadOnly { get; }
        public override Type PropertyType => componentObject.PropertyType;

        /// <summary>
        /// 为 <see cref="IShowProperty"/> 所指示的属性提供属性说明。
        /// </summary>
        public ShowPropertyDescriptor(IShowProperty obj) : base(obj.Name, obj.Attributes.ToArray())
        {
            componentObject = obj;
        }

        public override bool CanResetValue(object component)
        {
            return false;
        }

        public override object GetValue(object component)
        {
            return componentObject.GetValue();
        }

        public override void ResetValue(object component)
        {
            throw new InvalidOperationException();
        }

        public override void SetValue(object component, object value)
        {
            componentObject.SetValue(value);
        }

        public override bool ShouldSerializeValue(object component)
        {
            return false;
        }
#pragma warning restore CS1591 // 缺少对公共可见类型或成员的 XML 注释
    }
}

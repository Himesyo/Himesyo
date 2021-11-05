using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Himesyo
{
    /// <summary>
    /// 对指定动态值的封装，可便捷使用指定值。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class Property<TValue>
    {
        /// <summary>
        /// 不会引发异常的空属性。
        /// </summary>
        public static Property<TValue> Empty { get; } = new Property<TValue>(Getter<TValue>.Empty, Setter<TValue>.Empty);

        private readonly Getter<TValue> getter;
        private readonly Setter<TValue> setter;
        /// <summary>
        /// 获取此属性是否可读。
        /// </summary>
        public bool CanRead => getter != null;
        /// <summary>
        /// 获取此属性是否可写。
        /// </summary>
        public bool CanWrite => setter != null;
        /// <summary>
        /// 获取或设置属性的值。如果使用的索引器不存在，将引发 <see cref="NotSupportedException"/> 异常。
        /// </summary>
        public TValue Value
        {
            get => (getter ?? throw new NotSupportedException()).Value;
            set => (setter ?? throw new NotSupportedException()).Value = value;
        }

        /// <summary>
        /// 使用指定索引器封装一个属性.
        /// </summary>
        /// <param name="getter"></param>
        /// <param name="setter"></param>
        public Property(Getter<TValue> getter, Setter<TValue> setter)
        {
            this.getter = getter;
            this.setter = setter;
        }
    }

    /// <summary>
    /// 将成员封装为属性。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    public class MemberToProperty<TValue>
    {
        public MemberInfo GetterMember { get; }
        public MemberInfo SetterMember { get; }

        /// <summary>
        /// 从指定成员生成属性。
        /// </summary>
        /// <param name="member"></param>
        public MemberToProperty(MemberInfo member)
        {
            GetterMember = member ?? throw new ArgumentNullException(nameof(member));
            SetterMember = member;
        }

        /// <summary>
        /// 从不同的成员生成属性。
        /// </summary>
        /// <param name="getterMember"></param>
        /// <param name="setterMember"></param>
        public MemberToProperty(MemberInfo getterMember, MemberInfo setterMember)
        {
            GetterMember = getterMember ?? throw new ArgumentNullException(nameof(getterMember));
            SetterMember = setterMember ?? throw new ArgumentNullException(nameof(setterMember));
        }

        /// <summary>
        /// 对指定实例创建 <see cref="Getter{TValue}"/> 。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public Getter<TValue> CreateGetter(object instance)
        {
            Delegate getValue;
            switch (GetterMember)
            {
                case MethodInfo method:
                    getValue = Delegate.CreateDelegate(typeof(Func<TValue>), instance, method);
                    return new Getter<TValue>((Func<TValue>)getValue);
                case PropertyInfo property:
                    MethodInfo[] properties = property.GetAccessors(true);
                    MethodInfo getter = properties.FirstOrDefault(p => p.Name.StartsWith("get"));
                    if (getter != null)
                    {
                        getValue = Delegate.CreateDelegate(typeof(Func<TValue>), instance, getter);
                        return new Getter<TValue>((Func<TValue>)getValue);
                    }
                    return null;
                case FieldInfo field:
                    return new Getter<TValue>(() => (TValue)field.GetValue(instance));
                case null:
                    return null;
                default:
                    throw new InvalidOperationException("无法用作属性的成员类型。");
            }
        }
        /// <summary>
        /// 对指定实例创建 <see cref="Setter{TValue}"/> 。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public Setter<TValue> CreateSetter(object instance)
        {
            Delegate setValue;
            switch (SetterMember)
            {
                case MethodInfo method:
                    setValue = Delegate.CreateDelegate(typeof(Action<TValue>), instance, method);
                    return new Setter<TValue>((Action<TValue>)setValue);
                case PropertyInfo property:
                    MethodInfo[] properties = property.GetAccessors(true);
                    MethodInfo setter = properties.FirstOrDefault(p => p.Name.StartsWith("set"));
                    if (setter != null)
                    {
                        setValue = Delegate.CreateDelegate(typeof(Action<TValue>), instance, setter);
                        return new Setter<TValue>((Action<TValue>)setValue);
                    }
                    return null;
                case FieldInfo field:
                    return new Setter<TValue>(value => field.SetValue(instance, value));
                case null:
                    return null;
                default:
                    throw new InvalidOperationException("无法用作属性的成员类型。");
            }
        }
        /// <summary>
        /// 对指定实例创建 <see cref="Property{TValue}"/> 。
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public Property<TValue> CreateProperty(object instance)
        {
            return new Property<TValue>(CreateGetter(instance), CreateSetter(instance));
        }
    }

    public class Setter<TValue>
    {
        private static readonly Action<TValue> Default = value => { };
        public static Setter<TValue> Empty { get; } = new Setter<TValue>(Default);

        private readonly Action<TValue> setValue;
        public TValue Value
        {
            set => setValue(value);
        }

        public Setter(Action<TValue> setValue)
        {
            this.setValue = setValue ?? throw new ArgumentNullException(nameof(setValue));
        }
    }

    public class Getter<TValue>
    {
        private static readonly Func<TValue> Default = () => default;
        public static Getter<TValue> Empty { get; } = new Getter<TValue>(Default);

        private readonly Func<TValue> getValue;

        /// <summary>
        /// 获取值。
        /// </summary>
        public TValue Value => getValue();

        public Getter(Func<TValue> getValue)
        {
            this.getValue = getValue ?? throw new ArgumentNullException(nameof(getValue));
        }
    }

}

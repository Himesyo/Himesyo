using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Himesyo
{
    /// <summary>
    /// 冻结器管理
    /// </summary>
    public sealed class FreezerManger
    {
        private readonly Dictionary<string, Freezer> freezers = new Dictionary<string, Freezer>();
        private int value;
        private int maxValue;

        /// <summary>
        /// 初始化默认冻结器的值为 0 的具有默认最大值的 <see cref="FreezerManger"/> 。
        /// </summary>
        public FreezerManger() : this(0, 2100000000) { }
        /// <summary>
        /// 初始化自定义的默认冻结器的初始值的 <see cref="FreezerManger"/> 。
        /// </summary>
        /// <param name="defaultValue"></param>
        /// <param name="defaultMaxValue"></param>
        public FreezerManger(int defaultValue, int defaultMaxValue)
        {
            value = defaultValue;
            maxValue = defaultMaxValue;
        }

        /// <summary>
        /// 获取或设置指定名称的冻结器。如果指定的名称不存在，则创建默认的冻结器。
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public Freezer this[string name]
        {
            get
            {
                if (!freezers.ContainsKey(name))
                {
                    freezers[name] = new Freezer(value, maxValue);
                }
                return freezers[name];
            }
            set
            {
                freezers[name] = value;
            }
        }
    }


    /// <summary>
    /// 冻结器。它是一个计数器，当值大于 0 时，表示冻结状态。
    /// </summary>
    public sealed class Freezer
    {
        private readonly int max;
        private int value;
        /// <summary>
        /// 初始化具有足够最大值（2100000000）的冻结器。
        /// </summary>
        public Freezer()
        {
            max = 2100000000;
        }
        /// <summary>
        /// 初始化具有指定最大值的冻结器。
        /// </summary>
        public Freezer(int maxValue)
        {
            max = maxValue;
            if (max < 1)
            {
                max = 1;
            }
        }
        /// <summary>
        /// 初始化具有指定初始值和最大值的冻结器。
        /// </summary>
        public Freezer(int initialValue, int maxValue) : this(maxValue)
        {
            value = initialValue;

            if (value < 0)
            {
                value = 0;
            }
            else if (value > max)
            {
                value = max;
            }
        }
        /// <summary>
        /// 冻结。其值不会超过最大值。
        /// </summary>
        public void Frozen()
        {
            value++;
            if (value > max)
            {
                value = max;
            }
        }
        /// <summary>
        /// 解冻。其值不会低于 0 。
        /// </summary>
        public void Unfreeze()
        {
            value--;
            if (value < 0)
            {
                value = 0;
            }
        }
        /// <summary>
        /// 完全恢复。将值置为 0 。
        /// </summary>
        public void Recovery()
        {
            value = 0;
        }
        /// <summary>
        /// 获取表示当前状态的字符串。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{{ {value > 0} , {value}/{max} }}";
        }

        /// <summary>
        /// 获取是否在冻结状态。
        /// </summary>
        /// <param name="freezer"></param>
        public static implicit operator bool(Freezer freezer)
        {
            return freezer.value > 0;
        }
        /// <summary>
        /// 创建最大值为 1 的冻结器并指定初始值。
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Freezer(bool value)
        {
            return new Freezer(value ? 1 : 0, 1);
        }
        /// <summary>
        /// 获取冻结器的当前值。
        /// </summary>
        /// <param name="freezer"></param>
        public static implicit operator int(Freezer freezer)
        {
            return freezer.value;
        }
        /// <summary>
        /// 创建最大值为默认值的冻结器并指定初始值。
        /// </summary>
        /// <param name="value"></param>
        public static implicit operator Freezer(int value)
        {
            return new Freezer(value, 2100000000);
        }
    }


    /// <summary>
    /// 设置一个在释放此对象时执行的动作。
    /// </summary>
    public sealed class Recovery : IDisposable
    {
        private readonly Action recovery;
        /// <summary>
        /// 在此对象释放时调用的动作。
        /// </summary>
        /// <param name="recoveryAction"></param>
        public Recovery(Action recoveryAction)
        {
            if (recoveryAction == null)
                throw new ArgumentNullException(nameof(recoveryAction));

            recovery = recoveryAction;
        }
        /// <summary>
        /// 冻结冻结器，在此对象释放时再解冻冻结器。
        /// </summary>
        /// <param name="freezer"></param>
        public Recovery(Freezer freezer)
        {
            freezer.Frozen();
            recovery = freezer.Unfreeze;
        }
        /// <summary>
        /// 在此对象释放时解冻或恢复冻结器。
        /// </summary>
        /// <param name="freezer"></param>
        /// <param name="isRecovery"></param>
        public Recovery(Freezer freezer, bool isRecovery)
        {
            freezer.Frozen();
            if (isRecovery)
            {
                recovery = freezer.Recovery;
            }
            else
            {
                recovery = freezer.Unfreeze;
            }
        }
        /// <summary>
        /// 立即解冻或执行动作。
        /// </summary>
        public void Dispose()
        {
            recovery();
        }
    }
}

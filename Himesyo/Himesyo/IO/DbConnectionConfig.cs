using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Common;
using System.Data.OracleClient;
using System.Diagnostics;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

using Himesyo.ComponentModel;
using Himesyo.Data;
using Himesyo.Logger;
using Himesyo.WinForm;

namespace Himesyo.IO
{
    /// <summary>
    /// 表示一个连接字符串配置。它以密文的方式保存连接字符串。
    /// </summary>
    [Serializable]
    [Editor(typeof(DbConnConfigEditer), typeof(UITypeEditor))]
    public class DbConnectionConfig<TCiphertext> : DbConnectionConfig
        where TCiphertext : CiphertextBase, new()
    {
        /// <summary>
        /// 连接字符串。加密保存的连接字符串。
        /// </summary>
        [Browsable(false)]
        [XmlElement("ConnString")]
        public TCiphertext ConnStringCiphertext
        {
            get => new TCiphertext() { Value = ConnStringValue };
            set => ConnStringValue = value?.Value;
        }

        /// <summary>
        /// 获取或设置连接字符串。
        /// </summary>
        [XmlIgnore]
        public override string ConnStringValue { get; set; }
    }

    /// <summary>
    /// 表示一个连接字符串配置。它是一个抽象类。
    /// </summary>
    [Serializable]
    [TypeConverter(typeof(NullObjectConverter))]
    public abstract class DbConnectionConfig
    {
        /// <summary>
        /// <see cref="ConnType"/> 或 <see cref="ConnStringValue"/> 未设定值时为 <see langword="true"/>，否则为 <see langword="false"/> 。
        /// </summary>
        [Browsable(false)]
        [XmlIgnore]
        public bool ExistEmpty => string.IsNullOrWhiteSpace(ConnType) || string.IsNullOrWhiteSpace(ConnStringValue);

        /// <summary>
        /// 连接类型。继承自 <see cref="DbConnectionStringBuilder"/> 类型的程序集限定名。
        /// </summary>
        public string ConnType { get; set; }

        /// <summary>
        /// 获取或设置连接字符串。
        /// </summary>
        [XmlIgnore]
        public abstract string ConnStringValue { get; set; }

        /// <summary>
        /// 创建连接字符串对象。不会创建连接对象。
        /// </summary>
        /// <returns></returns>
        public virtual ConnectionResult CreateStringBuilder()
        {
            ExceptionHelper.ThrowInvalid(string.IsNullOrWhiteSpace(ConnType), "未配置连接类型。");
            //ExceptionHelper.ThrowInvalid(string.IsNullOrWhiteSpace(ConnString), "未配置连接字符串。");

            Type type = Type.GetType(ConnType);
            DbConnectionStringBuilder builder = (DbConnectionStringBuilder)type.Assembly.CreateInstance(type.FullName);
            builder.ConnectionString = ConnStringValue;
            ConnectionResult result = new ConnectionResult();
            result.ConnectionStringBuilder = builder;
            return result;
        }

        /// <summary>
        /// 获取配置状态。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (ExistEmpty)
            {
                return "(未配置完全)";
            }
            else
            {
                return "(已配置数据库连接)";
            }
        }
    }

    /// <summary>
    /// 为 <see cref="DbConnectionConfig"/> 提供编辑器。
    /// </summary>
    public class DbConnConfigEditer : UITypeEditor
    {
        /// <inheritdoc/>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        /// <inheritdoc/>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            Type type = value?.GetType() ?? context.PropertyDescriptor.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(DbConnectionConfig<>))
            {
                object source = value;
                DbConnectionConfig edit = value as DbConnectionConfig;

                // 调用一下 OracleConnectionStringBuilder 加载 Oracle 的程序集
#pragma warning disable CS0618 // 类型或成员已过时
                typeof(OracleConnectionStringBuilder).ToString();
#pragma warning restore CS0618 // 类型或成员已过时
                using (DatabaseConnectionDialog dialog = new DatabaseConnectionDialog())
                {
                    dialog.ConnectionResultMode = ConnectionResultMode.None;
                    if (edit != null)
                    {
                        if (!string.IsNullOrWhiteSpace(edit.ConnType))
                        {
                            try
                            {
                                Type connType = Type.GetType(edit.ConnType, false);
                                if (connType == null || !typeof(DbConnectionStringBuilder).IsAssignableFrom(connType))
                                {
                                    LoggerSimple.WriteWarning($"无法将 '{edit.ConnType}' 转换为数据库类型。");
                                }
                                else
                                {
                                    DbConnectionStringBuilder stringBuilder = (DbConnectionStringBuilder)connType.Assembly.CreateInstance(connType.FullName);
                                    stringBuilder.ConnectionString = edit.ConnStringValue;
                                    dialog.ConnectionStringBuilder = stringBuilder;
                                }
                            }
                            catch (Exception ex)
                            {
                                LoggerSimple.WriteError("获取数据库类型时出错。", ex);
                            }
                        }
                    }
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        DbConnectionStringBuilder stringBuilder = dialog.Result.ConnectionStringBuilder;
                        DbConnectionConfig result = (DbConnectionConfig)type.Assembly.CreateInstance(type.FullName);
                        result.ConnType = stringBuilder.GetType().AssemblyQualifiedName;
                        result.ConnStringValue = stringBuilder.ConnectionString;
                        return result;
                    }
                    else
                    {
                        return source;
                    }
                }
            }
            if (value is DbConnectionConfig config)
            {
            }
            else
            {
                return value;
            }
            return base.EditValue(context, provider, value);
        }
    }

}

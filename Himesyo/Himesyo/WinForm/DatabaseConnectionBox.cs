using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

using Himesyo;
using Himesyo.ComponentModel;
using Himesyo.Data;
using Himesyo.Logger;
using Himesyo.Runtime;

using Microsoft.VisualBasic;

namespace Himesyo.WinForm
{
    /// <summary>
    /// 提供对数据库的连接。
    /// </summary>
    public partial class DatabaseConnectionBox : UserControl
    {
        /// <summary>
        /// 提供对数据库的连接。
        /// </summary>
        public DatabaseConnectionBox()
        {
            InitializeComponent();
            OnConnectionItems();
            RefreshConnectionType();
        }

        /// <summary>
        /// 用户成功确认后引发的事件。
        /// </summary>
        public event EventHandler Opened;
        /// <summary>
        /// 获取现有连接列表的事件。
        /// </summary>
        public event ConnectionItemsEventHandler ConnectionItems;

        /// <summary>
        /// 获取或设置返回的结果模式。默认为 <see cref="ConnectionResultMode.OpenConnection"/> 。
        /// </summary>
        public ConnectionResultMode ConnectionResultMode { get; set; } = ConnectionResultMode.OpenConnection;

        /// <summary>
        /// 成功连接的结果对象。
        /// </summary>
        public ConnectionResult Result { get; } = new ConnectionResult();

        /// <summary>
        /// 获取或设置当前选中的连接字符串拼接对象。
        /// </summary>
        public DbConnectionStringBuilder SelectedItem
        {
            get
            {
                if (propertyEditor.SelectedObject is ShowObject show)
                {
                    return show.ComponentObject as DbConnectionStringBuilder;
                }
                return null;
            }
            set
            {
                if (value != null)
                {
                    Type valueType = value.GetType();
                    comboTypes.SelectedItem = valueType;
                    if (comboTypes.SelectedItem is ShowItem<Type> type && type.Value == valueType)
                    {
                        ShowObject show = CreateConnectionStringShow(value);
                        type.Tag = show;
                        propertyEditor.SelectedObject = show;
                    }
                }
            }
        }

        /// <summary>
        /// 刷新数据库连接的类型。
        /// </summary>
        public void RefreshConnectionType()
        {
            if (comboTypes.DataSource is BindingList<IShowItem> types)
            {
                foreach (IShowItem item in GetTypes().Except(types).ToList())
                {
                    types.Insert(types.Count - 1, item);
                }
            }
            else
            {
                types = new BindingList<IShowItem>(GetTypes());
                types.Add(new ShowItem<string>("<添加新驱动...>"));
                comboTypes.DataSource = types;
            }
        }

        /// <summary>
        /// 显示指定数据库连接记录的连接字符串。
        /// </summary>
        /// <param name="connection"></param>
        public void ShowConnection(DbConnection connection)
        {
            if (connection != null)
            {
                DbConnectionStringBuilder connString = connection.Create<DbConnectionStringBuilder>();
                connString.ConnectionString = connection.ConnectionString;
                Type typeConnString = connString.GetType();
                comboTypes.SelectedItem = typeConnString;
                if (comboTypes.SelectedItem is ShowItem<Type> type && type.Value == typeConnString)
                {
                    ShowObject show = CreateConnectionStringShow(connString);
                    type.Tag = show;
                    propertyEditor.SelectedObject = show;
                }
            }
        }

        #region 自己用

        private static List<IShowItem> GetTypes()
        {
            try
            {
                Type connString = typeof(DbConnectionStringBuilder);
                var types = AppDomain.CurrentDomain
                    .GetAssemblies()
                    .SelectMany(ass => ass.GetTypes())
                    .Where(type => connString.IsAssignableFrom(type) && !type.IsAbstract && type != connString)
                    .Select(type => new ShowItem<Type>(type, $"{type.Name.Replace("ConnectionStringBuilder", ""),-10} ({type.AssemblyQualifiedName})"))
                    .Cast<IShowItem>()
                    .ToList();
                return types;
            }
            catch (Exception ex)
            {
                MsgBox.Show($"获取数据库类型失败。{ex.Message}");
                LoggerSimple.WriteError(@"获取数据库类型失败。", ex);
                return null;
            }
        }

        private static ShowObject CreateConnectionStringShow(object connectionString)
        {
            ShowObject show = new ShowObject(connectionString);
            // SQL
            //show.Add("UserInstance", "用户实例", "扩展");
            // OLE DB
            show.Add("Provider", "提供程序", "基本");
            show.Add("FileName", "文件名", "扩展");
            // ODBC
            show.Add("Dsn", "数据源(DSN)", "基本");
            show.Add("Driver", "驱动程序", "基本");
            // Oracle
            //show.Add("Dsn", "数据源(DSN)", "基本");

            show.Add("DataSource", "数据源", "基本");
            show.Add("InitialCatalog", "数据库名称", "基本");
            show.Add("UserID", "用户名", "基本");
            show.Add("User ID", "用户名", "基本");
            show.Add("Password", "密码", "基本");

            show.Add("PersistSecurityInfo", "连接后在连接字符串中保存密码", "安全性");

            show.Sort = show.ShowItems.Keys.ToArray();
            show.ReturningPropertiesBefore += Show_ReturningPropertiesBefore;
            return show;
        }

        private static void Show_ReturningPropertiesBefore(object sender, EventArgs e)
        {
            ShowObject show = (ShowObject)sender;
            show.AutoAddShowItems(new ShowPropertyInfo(true));
            show.ShowItems.Remove("ConnectionString");
        }

        private void OnOpened()
        {
            OnOpened(EventArgs.Empty);
        }

        private ConnectionItemsEventArgs OnConnectionItems()
        {
            ConnectionItemsEventArgs args = new ConnectionItemsEventArgs();
            try
            {
                OnConnectionItems(args);
            }
            catch (Exception ex)
            {
                MsgBox.Show($"获取连接项时出错。{ex.Message}");
            }
            return args;
        }

        private void AddDriver()
        {
            using (OpenFileDialog open = new OpenFileDialog())
            {
                open.DefaultExt = "dll";
                open.Filter = "程序集文件|*.dll;*.exe|所有文件|*.*";
                if (open.ShowDialog() == DialogResult.OK)
                {
                    foreach (var file in open.FileNames)
                    {
                    重试:
                        try
                        {
                            Assembly.LoadFrom(file);
                        }
                        catch (Exception ex)
                        {
                            var box = MsgBox.Show(ex.Message, btns: MessageBoxButtons.AbortRetryIgnore, icon: MessageBoxIcon.Error);
                            if (box.Result == DialogResult.Retry)
                            {
                                goto 重试;
                            }
                            else if (box.Result == DialogResult.Abort)
                            {
                                break;
                            }
                        }
                    }
                    RefreshConnectionType();
                }
            }
        }

        #endregion

        #region 引发事件

        /// <summary>
        /// 引发 <see cref="Opened"/> 事件。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnOpened(EventArgs e)
        {
            Opened?.Invoke(this, e);
        }

        /// <summary>
        /// 引发 <see cref="ConnectionItems"/> 事件。
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnConnectionItems(ConnectionItemsEventArgs e)
        {
            ConnectionItems?.Invoke(this, e);
        }

        #endregion

        private void DatabaseConnectionBox_Load(object sender, EventArgs e)
        {
            OnConnectionItems();
            RefreshConnectionType();
        }

        private void comboTypes_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboTypes.SelectedItem is ShowItem<Type> type)
            {
                if (type.Tag is ShowObject up)
                {
                    propertyEditor.SelectedObject = up;
                }
                else
                {
                    try
                    {
                        object connString = type.Value.Assembly.CreateInstance(type.Value.FullName);

                        ShowObject show = CreateConnectionStringShow(connString);
                        type.Tag = show;
                        propertyEditor.SelectedObject = show;
                    }
                    catch (Exception ex)
                    {
                        ErrorMessage errorMessage = new ErrorMessage("未能创建字符串连接对象。");
                        errorMessage.ExceptionMessage = ex.GetBaseException()?.Message;
                        errorMessage.DetailedInfo = ex.ToString()?.Replace("--->", $"\r\n--->");
                        propertyEditor.SelectedObject = errorMessage;
                    }
                }
            }
            else
            {
                AddDriver();
                if (propertyEditor.SelectedObject is ShowObject show && show.ComponentObject is DbConnectionStringBuilder connectionString)
                {
                    comboTypes.SelectedItem = connectionString.GetType();
                }
            }
        }

        private void buttonSwitch_Click(object sender, EventArgs e)
        {
            if (propertyEditor.SelectedObject is ShowObject show)
            {
                show.ShowHide = !show.ShowHide;
                propertyEditor.Refresh();
            }
        }

        private void buttonTest_Click(object sender, EventArgs e)
        {
            if (propertyEditor.SelectedObject is ShowObject show && show.ComponentObject is DbConnectionStringBuilder connectionString)
            {
                DbConnection connection = null;
                try
                {
                    connection = connectionString.Create<DbConnection>();
                    connection.ConnectionString = connectionString.ConnectionString;
                    connection.Open();
                    MsgBox.Show("连接成功！");
                }
                catch (Exception ex)
                {
                    LoggerSimple.WriteError($"测试连接数据库失败。{connectionString.ConnectionString}", ex);
                    MsgBox.Show(ex.Message);
                }
                finally
                {
                    connection?.Close();
                    connection?.Dispose();
                }
            }
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            if (propertyEditor.SelectedObject is ShowObject show && show.ComponentObject is DbConnectionStringBuilder connectionString)
            {
                if (Result.Connection != null)
                {
                    Result.Connection.Close();
                    Result.Connection.Dispose();
                    Result.Connection = null;
                }
                Result.ConnectionStringBuilder = connectionString;
                if (ConnectionResultMode == ConnectionResultMode.None)
                {
                    OnOpened();
                    return;
                }
                DbConnection newConnection = null;
                try
                {
                    newConnection = connectionString.Create<DbConnection>();
                    newConnection.ConnectionString = connectionString.ConnectionString;
                    newConnection.Open();
                    if (ConnectionResultMode == ConnectionResultMode.ConfirmConnection)
                    {
                        newConnection.Close();
                        newConnection.Dispose();
                        OnOpened();
                        return;
                    }
                    Result.Connection = newConnection;
                }
                catch (Exception ex)
                {
                    MsgBox.Show(ex.Message);
                    newConnection?.Dispose();
                    return;
                }
                OnOpened();
            }
            else if (ConnectionResultMode == ConnectionResultMode.None)
            {
                if (Result.Connection != null)
                {
                    Result.Connection.Close();
                    Result.Connection.Dispose();
                    Result.Connection = null;
                }
                Result.ConnectionStringBuilder = null;
                OnOpened();
            }
        }

        private void MenuItem_Click(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuItem && menuItem.Tag is ShowItem<DbConnectionStringBuilder> item)
            {
                SelectedItem = item.Value;
            }
        }

        private void buttonString_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            ShowItem<DbConnectionStringBuilder>[] items = OnConnectionItems().GetItems();
            if (items.Length > 0)
            {
                contextMenuStrip1.Items.Clear();
                for (int i = 0; i < items.Length; i++)
                {
                    ToolStripMenuItem menuItem = new ToolStripMenuItem();
                    menuItem.Text = items[i].ToString();
                    menuItem.Tag = items[i];
                    menuItem.Click += MenuItem_Click;
                    contextMenuStrip1.Items.Add(menuItem);
                }
                contextMenuStrip1.Show(MousePosition);
            }
        }

        private void buttonConnection_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (propertyEditor.SelectedObject is ShowObject show && show.ComponentObject is DbConnectionStringBuilder connectionString)
            {
                string connString = connectionString.ConnectionString;
                connString = Interaction.InputBox("请复制或输入连接字符串：", "连接字符串", connString);
                if (!string.IsNullOrWhiteSpace(connString))
                {
                    connectionString.ConnectionString = connString;
                    propertyEditor.Refresh();
                }
            }
        }

        private class ErrorMessage
        {
            [Category("错误"), DisplayName("错误消息")]
            [ReadOnly(true)]
            public string Message { get; internal set; }

            [Category("错误"), DisplayName("异常消息")]
            [ReadOnly(true)]
            public string ExceptionMessage { get; internal set; }

            [Browsable(false)]
            public string DetailedInfo { get; set; }

            [Category("详细"), DisplayName("详细信息")]
            [Editor(ConstValue.MultilineStringEditor, typeof(UITypeEditor))]
            public string DetailedInfoReadOnly
            {
                get => DetailedInfo;
                set { }
            }

            public ErrorMessage() { }
            public ErrorMessage(string message, string detailedInfo = null)
            {
                Message = message;
                DetailedInfo = detailedInfo;
            }
        }
    }

    /// <summary>
    /// 表示 <see cref="DatabaseConnectionBox.ConnectionItems"/> 事件使用的委托。
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ConnectionItemsEventHandler(object sender, ConnectionItemsEventArgs e);

    /// <summary>
    /// 为 <see cref="DatabaseConnectionBox.ConnectionItems"/> 事件参数提供类型。
    /// </summary>
    public class ConnectionItemsEventArgs : EventArgs
    {
        private readonly List<ShowItem<DbConnectionStringBuilder>> items = new List<ShowItem<DbConnectionStringBuilder>>();

        /// <summary>
        /// 添加现有连接项。
        /// </summary>
        /// <param name="connection"></param>
        public void Add(ShowItem<DbConnectionStringBuilder> connection)
        {
            items.Add(connection);
        }

        /// <summary>
        /// 获取当前显示项集合。
        /// </summary>
        /// <returns></returns>
        public ShowItem<DbConnectionStringBuilder>[] GetItems()
        {
            return items.ToArray();
        }
    }

    /// <summary>
    /// 连接结果类型。
    /// </summary>
    public class ConnectionResult
    {
        /// <summary>
        /// 连接字符串拼接对象。
        /// </summary>
        public DbConnectionStringBuilder ConnectionStringBuilder { get; set; }

        /// <summary>
        /// 数据库连接。
        /// </summary>
        public DbConnection Connection { get; set; }

        /// <summary>
        /// 当前连接是否在打开状态。
        /// </summary>
        public bool IsOpen => Connection?.State == ConnectionState.Open;

        /// <summary>
        /// 使用当前 <see cref="ConnectionStringBuilder"/> 打开新连接。
        /// </summary>
        /// <returns></returns>
        public DbConnection OpenNewConnection()
        {
            ExceptionHelper.ThrowInvalid(ConnectionStringBuilder == null, "连接对象为 null 。");
            DbConnection connection = ConnectionStringBuilder.Create<DbConnection>();
            connection.ConnectionString = ConnectionStringBuilder.ConnectionString;
            try
            {
                connection.Open();
            }
            catch
            {
                connection.Dispose();
                throw;
            }
            return connection;
        }
    }

    /// <summary>
    /// 表示返回结果的模式。
    /// </summary>
    public enum ConnectionResultMode
    {
        /// <summary>
        /// 结果可任意返回。
        /// </summary>
        None = 0,
        /// <summary>
        /// 结果需要保证可正确连接。
        /// </summary>
        ConfirmConnection,
        /// <summary>
        /// 结果需要打开连接。
        /// </summary>
        OpenConnection
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Himesyo.Data;

namespace Himesyo.WinForm
{
    /// <summary>
    /// 表示一个提供连接数据库的对话框。
    /// </summary>
    public class DatabaseConnectionDialog : CommonDialog
    {
        /// <summary>
        /// 获取现有连接列表的事件。
        /// </summary>
        public event ConnectionItemsEventHandler ConnectionItems;

        private DatabaseConnectionBox connectionBox = new DatabaseConnectionBox();
        private Form form = new Form();

        /// <summary>
        /// 表示一个提供连接数据库的对话框。
        /// </summary>
        public DatabaseConnectionDialog()
        {
            New();
        }

        /// <summary>
        /// 对话框的标题。
        /// </summary>
        public string Text
        {
            get => form.Text;
            set => form.Text = value;
        }

        /// <summary>
        /// 成功连接的结果对象。
        /// </summary>
        public ConnectionResult Result => connectionBox.Result;

        /// <summary>
        /// 获取或设置连接字符串拼接对象。
        /// </summary>
        public DbConnectionStringBuilder ConnectionStringBuilder
        {
            get => connectionBox.SelectedItem;
            set => connectionBox.SelectedItem = value;
        }

        /// <summary>
        /// 显示指定数据库连接记录的连接字符串。
        /// </summary>
        /// <param name="connection"></param>
        public void SetConnection(DbConnection connection)
        {
            connectionBox.ShowConnection(connection);
        }

        /// <summary>
        /// 将通用对话框的属性重置为默认值。
        /// </summary>
        public override void Reset()
        {
            New();
        }

        /// <summary>
        /// 指定通用对话框。
        /// </summary>
        /// <param name="hwndOwner"></param>
        /// <returns></returns>
        protected override bool RunDialog(IntPtr hwndOwner)
        {
            DialogResult result = form.ShowDialog(new DialogOwner(hwndOwner));
            return result == DialogResult.OK;
        }

        private void New()
        {
            connectionBox = new DatabaseConnectionBox();
            connectionBox.Dock = DockStyle.Fill;
            connectionBox.Opened += (sender, e) =>
            {
                form.DialogResult = DialogResult.OK;
                form.Close();
            };
            connectionBox.ConnectionItems += (sender, e) =>
            {
                ConnectionItems?.Invoke(this, e);
            };
            form = new Form();
            form.Text = "数据连接";
            form.Size = new Size(500, 600);
            form.StartPosition = FormStartPosition.CenterParent;
            form.Controls.Add(connectionBox);
        }

        private class DialogOwner : IWin32Window
        {
            public IntPtr Handle { get; }

            public DialogOwner(IntPtr hwndOwner)
            {
                Handle = hwndOwner;
            }
        }
    }
}

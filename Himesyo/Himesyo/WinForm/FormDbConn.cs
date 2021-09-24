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
        FormDbConn form = new FormDbConn();

        public DbConnection DbConnection
        {
            get => form.ConnectionBox.Connection;
            set
            {
                if (value != null)
                {
                    DbConnectionStringBuilder connectionString = value.Create<DbConnectionStringBuilder>();
                    connectionString.ConnectionString = value.ConnectionString;
                    form.ConnectionBox.SelectedString = connectionString;
                }
            }
        }

        public DbConnectionStringBuilder ConnectionString
        {
            get => form.ConnectionBox.SelectedString;
            set => form.ConnectionBox.SelectedString = value;
        }

        public override void Reset()
        {
            form = new FormDbConn();
        }

        protected override bool RunDialog(IntPtr hwndOwner)
        {
            DialogResult result = form.ShowDialog(new DialogOwner(hwndOwner));
            return result == DialogResult.OK;
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

    public partial class FormDbConn : Form
    {
        public FormDbConn()
        {
            InitializeComponent();
            connBox.RefreshDataSource();
        }

        public DatabaseConnectionBox ConnectionBox => connBox;

        private void connBox_Opened(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Himesyo.WinForm
{
    internal partial class FormShowMessage : Form, IShowText
    {
        /// <summary>
        /// 用户单击取消按钮引发的事件。
        /// </summary>
        public event CancelEventHandler Cancelled;

        /// <summary>
        /// 使用此对话框显示并执行指定委托。
        /// </summary>
        /// <param name="showTask"></param>
        /// <param name="cancelled">用户单击取消按钮引发的事件</param>
        /// <returns></returns>
        public static DialogResult ShowMessage(ShowTaskMethod showTask, CancelEventHandler cancelled)
        {
            using (EventWaitHandle waitShow = new EventWaitHandle(false, EventResetMode.AutoReset))
            using (EventWaitHandle waitRun = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                FormShowMessage form = null;
                Task task = new Task(() =>
                {
                    try
                    {
                        form = new FormShowMessage();
                        if (cancelled != null)
                        {
                            form.Cancelled += cancelled;
                        }
                    }
                    finally
                    {
                        waitShow.Set();
                    }
                    waitRun.WaitOne();
                    showTask(form);
                });
                task.Start();
                waitShow.WaitOne();
                waitRun.Set();
                if (form != null)
                {
                    return form.ShowDialog();
                }
                else
                {
                    throw new ApplicationException("显示窗体初始化失败。", task.Exception);
                }
            }
        }

        internal FormShowMessage()
        {
            InitializeComponent();
        }

        #region IShowText 接口成员

        private void SetText(Control control, string value)
        {
            if (control.InvokeRequired)
            {
                control.Invoke((Action<Control, string>)SetText, control, value);
            }
            else
            {
                control.Text = value;
            }
        }

        /// <summary>
        /// 用户是否已经单击取消。
        /// </summary>
        public bool IsCancel { get; private set; }

        /// <summary>
        /// 设置窗体的标题。
        /// </summary>
        /// <param name="value"></param>
        public void SetFormTitle(string value)
        {
            SetText(this, value);
        }

        /// <summary>
        /// 设置当前进度的标题。
        /// </summary>
        /// <param name="value"></param>
        public void SetTitle(string value)
        {
            SetText(label1, value);
        }
        /// <summary>
        /// 设置当前进度的详细说明。
        /// </summary>
        /// <param name="value"></param>
        public void SetCaption(string value)
        {
            SetText(label2, value);
        }
        /// <summary>
        /// 设置当前进度和样式。
        /// </summary>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <param name="style"></param>
        public void SetProgress(int max, int value, ProgressBarStyle style)
        {
            if (progressBar1.InvokeRequired)
            {
                progressBar1.Invoke((Action<int, int, ProgressBarStyle>)SetProgress, max, value, style);
            }
            else
            {
                if (progressBar1.Style != style)
                {
                    progressBar1.Style = style;
                }
                if (progressBar1.Maximum != max)
                {
                    if (max < progressBar1.Value)
                    {
                        progressBar1.Value = max;
                    }
                    progressBar1.Maximum = max;
                }
                value = value > max ? max : value;
                progressBar1.Value = value;
            }
        }
        /// <summary>
        /// 完成任务。等待用户单击 完成 按钮。
        /// </summary>
        public void Complete()
        {
            button1.Text = "完成";
        }
        /// <summary>
        /// 取消任务。关闭对话框并返回 <see cref="DialogResult.Cancel"/> 。
        /// </summary>
        public void Cancel()
        {
            Close(DialogResult.Cancel);
        }

        /// <summary>
        /// 使用指定结果直接关闭对话框。
        /// </summary>
        /// <param name="result"></param>
        public void Close(DialogResult result)
        {
            if (this.InvokeRequired)
            {
                this.Invoke((Action<DialogResult>)Close, result);
            }
            else
            {
                DialogResult = result;
                Close();
            }
        }

        /// <summary>
        /// 将指定窗体显示为模式对话框。
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        public DialogResult ShowDialog(Form form)
        {
            if (this.InvokeRequired)
            {
                return (DialogResult)this.Invoke((Func<Form, DialogResult>)ShowDialog, form);
            }
            else
            {
                return form.ShowDialog(this);
            }
        }

        #endregion

        private void FormShowMessage_Load(object sender, EventArgs e)
        {

        }

        private void Button1_Click(object sender, EventArgs e)
        {
            CancelEventArgs cancel = new CancelEventArgs(false);
            Cancelled?.Invoke(this, cancel);
            if (!cancel.Cancel)
            {
                if (button1.Text == "完成")
                {
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    button1.Enabled = false;
                    IsCancel = true;
                    SetTitle("正在取消...");
                    SetCaption("正在等待任务取消...");
                }
            }
        }

        private void FormShowMessage_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing && DialogResult == DialogResult.No)
            {
                if (button1.Enabled)
                {
                    Button1_Click(null, null);
                }
                e.Cancel = true;
            }
        }

    }

    /// <summary>
    /// 使用显示窗体执行的委托。
    /// </summary>
    /// <param name="showText"></param>
    public delegate void ShowTaskMethod(IShowText showText);

    /// <summary>
    /// 表示一个可显示说明和进度的对话框。句柄用于显示模式对话框。
    /// </summary>
    public interface IShowText : IWin32Window
    {
        /// <summary>
        /// 用户单击取消引发的事件。
        /// </summary>
        event CancelEventHandler Cancelled;

        /// <summary>
        /// 获取用户是否单击了取消按钮。
        /// </summary>
        bool IsCancel { get; }

        /// <summary>
        /// 设置对话框的标题。
        /// </summary>
        /// <param name="value"></param>
        void SetFormTitle(string value);
        /// <summary>
        /// 设置提示的主标题。
        /// </summary>
        /// <param name="value"></param>
        void SetTitle(string value);
        /// <summary>
        /// 设置进度的说明或详细信息。
        /// </summary>
        /// <param name="value"></param>
        void SetCaption(string value);
        /// <summary>
        /// 设置进度条的最大值，当前值和进度条样式。
        /// </summary>
        /// <param name="max"></param>
        /// <param name="value"></param>
        /// <param name="style"></param>
        void SetProgress(int max, int value, ProgressBarStyle style);
        /// <summary>
        /// 任务完成，等待用户单击按钮关闭对话框。
        /// </summary>
        void Complete();
        /// <summary>
        /// 任务取消，关闭对话框并返回 <see cref="DialogResult.Cancel"/> 。
        /// </summary>
        void Cancel();
        /// <summary>
        /// 关闭对话框。
        /// </summary>
        void Close(DialogResult result);
    }

    /// <summary>
    /// 可显示进度的模式对话框。
    /// </summary>
    public class ProgressWindow
    {
        /// <summary>
        /// 使用此对话框显示并执行指定委托。
        /// </summary>
        /// <param name="showTask"></param>
        /// <param name="cancelled">用户单击取消按钮引发的事件</param>
        /// <returns></returns>
        public static DialogResult Show(ShowTaskMethod showTask, CancelEventHandler cancelled)
        {
            using (EventWaitHandle waitShow = new EventWaitHandle(false, EventResetMode.AutoReset))
            using (EventWaitHandle waitRun = new EventWaitHandle(false, EventResetMode.AutoReset))
            {
                FormShowMessage form = null;
                Task task = new Task(() =>
                {
                    try
                    {
                        form = new FormShowMessage();
                        if (cancelled != null)
                        {
                            form.Cancelled += cancelled;
                        }
                    }
                    finally
                    {
                        waitShow.Set();
                    }
                    waitRun.WaitOne();
                    showTask(form);
                });
                task.Start();
                waitShow.WaitOne();
                waitRun.Set();
                if (form != null)
                {
                    return form.ShowDialog();
                }
                else
                {
                    throw new ApplicationException("显示窗体初始化失败。", task.Exception);
                }
            }
        }

        internal static void ShowTest()
        {
            FormShowMessage form = new FormShowMessage();
            form.FormClosing += (sender, e) =>
              {
                  e.Cancel = false;
              };
            form.Show();
        }

    }
}

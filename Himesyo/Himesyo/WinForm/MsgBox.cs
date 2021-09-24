using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace Himesyo.WinForm
{
    [DebuggerDisplay("{Result} [Buttons = {Buttons}, Message = {Message}]")]
    public class MsgBox
    {
        public static MsgBox Show(string msg, string title = "消息", MessageBoxButtons btns = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information, IWin32Window owner = null)
        {
            MsgBox box = new MsgBox()
            {
                Message = msg,
                Title = title,
                Buttons = btns,
                Icon = icon,
                Owner = owner
            };
            box.Show();
            return box;
        }

        public DialogResult Result { get; set; }
        public IWin32Window Owner { get; set; }
        public string Message { get; set; }
        public string Title { get; set; } = "消息";
        public MessageBoxButtons Buttons { get; set; } = MessageBoxButtons.OK;
        public MessageBoxIcon Icon { get; set; } = MessageBoxIcon.Information;

        public void Show()
        {
            if (Owner is Control control && control.InvokeRequired)
            {
                control.Invoke((MethodInvoker)Show);
            }
            else
            {
                Result = MessageBox.Show(Owner, Message, Title, Buttons, Icon);
            }
        }

        public void Show(string newMsg)
        {
            Message = newMsg;
            Show();
        }

        public void Show(string newMsg, MessageBoxButtons btns)
        {
            Message = newMsg;
            Buttons = btns;
            Show();
        }
    }
}

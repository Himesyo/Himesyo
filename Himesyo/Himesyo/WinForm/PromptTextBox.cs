using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Himesyo.WinForm
{
    /// <summary>
    /// 表示一个带说明的文本框。
    /// </summary>
    public partial class PromptTextBox : UserControl
    {
        /// <summary>
        /// 文本框的值改变时发生。
        /// </summary>
        public event EventHandler ValueChange;

        /// <summary>
        /// 初始化新实例。
        /// </summary>
        public PromptTextBox()
        {
            InitializeComponent();
            labelPrompt.BackColor = textValue.BackColor;
            labelPrompt.ForeColor = Color.FromArgb(200, 200, 200, 200);
            textValue.Size = new Size(Width, Height);
            Size = new Size(textValue.Width, textValue.Height);
        }

        /// <summary>
        /// 提示文本
        /// </summary>
        [Category("外观")]
        [Description("在文本框上显示的提示文本。")]
        public string TextPrompt
        {
            get { return labelPrompt.Text; }
            set { labelPrompt.Text = value; }
        }

        /// <summary>
        /// 获取或设置文本框的文本值。
        /// </summary>
        [Category("外观")]
        [Description("文本框的文本值。")]
        public string Value
        {
            get { return textValue.Text; }
            set { textValue.Text = value; }
        }

        /// <summary>
        /// 提示文本的字体大小是否跟随文本框的字体大小自动改变
        /// </summary>
        [Category("外观")]
        [Description("提示文本的字体。")]
        public Font FontPromptText
        {
            get => labelPrompt.Font;
            set => labelPrompt.Font = value;
        }

        /// <summary>
        /// 提示文本在文本框垂直方向上的对齐方式
        /// </summary>
        [Category("外观")]
        [Description("提示文本的对齐方式。")]
        [DefaultValue(ContentAlignment.MiddleLeft)]
        public ContentAlignment TextAlignPrompt
        {
            get => labelPrompt.TextAlign;
            set => labelPrompt.TextAlign = value;
        }

        /// <summary>
        /// 是否支持多行文本
        /// </summary>
        [Category("行为")]
        [Description("指示控件的文本能否跨越多行。")]
        [DefaultValue(false)]
        public bool Multiline
        {
            get { return textValue.Multiline; }
            set { textValue.Multiline = value; }
        }

        /// <summary>
        /// 获取或设置一个值，该值指示 <see cref="System.Windows.Forms.TextBox"/> 控件中的文本是否作为默认密码显示。
        /// </summary>
        [Category("行为")]
        [Description("是否使用默认密码字符隐藏输入文本。")]
        [DefaultValue(false)]
        public bool UseSystemPasswordChar
        {
            get { return TextBox.UseSystemPasswordChar; }
            set { TextBox.UseSystemPasswordChar = value; }
        }

        /// <summary>
        /// 获取或设置字符，该字符用于屏蔽单行 <see cref="System.Windows.Forms.TextBox"/> 控件中的密码字符。
        /// </summary>
        [Category("行为")]
        [Description("该字符用于屏蔽单行文本框中的密码字符。")]
        public char PasswordChar
        {
            get { return TextBox.PasswordChar; }
            set { TextBox.PasswordChar = value; }
        }

        /// <summary>
        /// 所显示的文本框
        /// </summary>
        //[Browsable(false)]
        public TextBox TextBox
        {
            get { return textValue; }
        }

        private void TextBox_TextChanged(object sender, EventArgs e)
        {
            labelPrompt.Visible = string.IsNullOrEmpty(textValue.Text);
            ValueChange?.Invoke(this, e);
        }

        private void TextBox_BackColorChanged(object sender, EventArgs e)
        {
            labelPrompt.BackColor = textValue.BackColor;
        }

        private void LabelPrompt_Click(object sender, EventArgs e)
        {
            textValue.Focus();
        }

        private void TextBox_MultilineChanged(object sender, EventArgs e)
        {
            Size = new Size(Size.Width, textValue.Size.Height);
        }

        private void TextBox_SizeChanged(object sender, EventArgs e)
        {
            Size = new Size(Size.Width, textValue.Size.Height);
            labelPrompt.Size = textValue.ClientSize - new Size(3, 3);
        }

        private void PromptTextBox_SizeChanged(object sender, EventArgs e)
        {
            textValue.Size = Size;// new Size(Width, Height);
            Size = new Size(Size.Width, textValue.Size.Height);
        }
    }
}

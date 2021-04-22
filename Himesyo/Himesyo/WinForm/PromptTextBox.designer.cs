namespace Himesyo.WinForm
{
    partial class PromptTextBox 
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region 组件设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要修改
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.textValue = new System.Windows.Forms.TextBox();
            this.labelPrompt = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textValue
            // 
            this.textValue.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textValue.Location = new System.Drawing.Point(0, 0);
            this.textValue.Name = "textValue";
            this.textValue.Size = new System.Drawing.Size(141, 21);
            this.textValue.TabIndex = 1;
            this.textValue.MultilineChanged += new System.EventHandler(this.TextBox_MultilineChanged);
            this.textValue.BackColorChanged += new System.EventHandler(this.TextBox_BackColorChanged);
            this.textValue.SizeChanged += new System.EventHandler(this.TextBox_SizeChanged);
            this.textValue.TextChanged += new System.EventHandler(this.TextBox_TextChanged);
            // 
            // labelPrompt
            // 
            this.labelPrompt.BackColor = System.Drawing.Color.Transparent;
            this.labelPrompt.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.labelPrompt.Location = new System.Drawing.Point(2, 2);
            this.labelPrompt.Margin = new System.Windows.Forms.Padding(3);
            this.labelPrompt.Name = "labelPrompt";
            this.labelPrompt.Size = new System.Drawing.Size(83, 12);
            this.labelPrompt.TabIndex = 0;
            this.labelPrompt.Text = "PromptTextBox";
            this.labelPrompt.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.labelPrompt.Click += new System.EventHandler(this.LabelPrompt_Click);
            // 
            // PromptTextBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.labelPrompt);
            this.Controls.Add(this.textValue);
            this.Name = "PromptTextBox";
            this.Size = new System.Drawing.Size(141, 47);
            this.SizeChanged += new System.EventHandler(this.PromptTextBox_SizeChanged);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label labelPrompt;
        private System.Windows.Forms.TextBox textValue;
    }
}

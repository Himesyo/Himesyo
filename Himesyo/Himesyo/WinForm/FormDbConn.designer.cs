
namespace Himesyo.WinForm
{
    partial class FormDbConn
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.connBox = new Himesyo.WinForm.DatabaseConnectionBox();
            this.SuspendLayout();
            // 
            // connBox
            // 
            this.connBox.Connection = null;
            this.connBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.connBox.Location = new System.Drawing.Point(0, 0);
            this.connBox.Name = "connBox";
            this.connBox.Size = new System.Drawing.Size(578, 564);
            this.connBox.TabIndex = 0;
            this.connBox.Opened += new System.EventHandler(this.connBox_Opened);
            // 
            // FormDbConn
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(578, 564);
            this.Controls.Add(this.connBox);
            this.Name = "FormDbConn";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "连接到数据库";
            this.ResumeLayout(false);

        }

        #endregion

        private Himesyo.WinForm.DatabaseConnectionBox connBox;
    }
}
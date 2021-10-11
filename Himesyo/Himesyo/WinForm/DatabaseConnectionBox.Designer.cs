
namespace Himesyo.WinForm
{
    partial class DatabaseConnectionBox
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
            this.components = new System.ComponentModel.Container();
            this.labelTypes = new System.Windows.Forms.Label();
            this.comboTypes = new System.Windows.Forms.ComboBox();
            this.buttonAdd = new System.Windows.Forms.LinkLabel();
            this.propertyEditor = new System.Windows.Forms.PropertyGrid();
            this.buttonSwitch = new System.Windows.Forms.Button();
            this.buttonTest = new System.Windows.Forms.Button();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonConnection = new System.Windows.Forms.LinkLabel();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.buttonString = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // labelTypes
            // 
            this.labelTypes.AutoSize = true;
            this.labelTypes.Location = new System.Drawing.Point(10, 10);
            this.labelTypes.Name = "labelTypes";
            this.labelTypes.Size = new System.Drawing.Size(89, 12);
            this.labelTypes.TabIndex = 0;
            this.labelTypes.Text = "选择数据库类型";
            // 
            // comboTypes
            // 
            this.comboTypes.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comboTypes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboTypes.FormattingEnabled = true;
            this.comboTypes.Location = new System.Drawing.Point(105, 7);
            this.comboTypes.Name = "comboTypes";
            this.comboTypes.Size = new System.Drawing.Size(198, 20);
            this.comboTypes.TabIndex = 1;
            this.comboTypes.SelectedIndexChanged += new System.EventHandler(this.comboTypes_SelectedIndexChanged);
            // 
            // buttonAdd
            // 
            this.buttonAdd.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonAdd.AutoSize = true;
            this.buttonAdd.Location = new System.Drawing.Point(309, 10);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(107, 12);
            this.buttonAdd.TabIndex = 2;
            this.buttonAdd.TabStop = true;
            this.buttonAdd.Text = "添加数据库驱动...";
            this.buttonAdd.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.buttonAdd_LinkClicked);
            // 
            // propertyEditor
            // 
            this.propertyEditor.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.propertyEditor.Location = new System.Drawing.Point(12, 33);
            this.propertyEditor.Name = "propertyEditor";
            this.propertyEditor.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.propertyEditor.Size = new System.Drawing.Size(534, 375);
            this.propertyEditor.TabIndex = 3;
            this.propertyEditor.ToolbarVisible = false;
            // 
            // buttonSwitch
            // 
            this.buttonSwitch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonSwitch.Location = new System.Drawing.Point(12, 417);
            this.buttonSwitch.Name = "buttonSwitch";
            this.buttonSwitch.Size = new System.Drawing.Size(134, 38);
            this.buttonSwitch.TabIndex = 4;
            this.buttonSwitch.Text = "显示/隐藏高级选项";
            this.buttonSwitch.UseVisualStyleBackColor = true;
            this.buttonSwitch.Click += new System.EventHandler(this.buttonSwitch_Click);
            // 
            // buttonTest
            // 
            this.buttonTest.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.buttonTest.Location = new System.Drawing.Point(152, 417);
            this.buttonTest.Name = "buttonTest";
            this.buttonTest.Size = new System.Drawing.Size(134, 38);
            this.buttonTest.TabIndex = 5;
            this.buttonTest.Text = "测试连接";
            this.buttonTest.UseVisualStyleBackColor = true;
            this.buttonTest.Click += new System.EventHandler(this.buttonTest_Click);
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.Location = new System.Drawing.Point(412, 417);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(134, 38);
            this.buttonOK.TabIndex = 5;
            this.buttonOK.Text = "确认";
            this.buttonOK.UseVisualStyleBackColor = true;
            this.buttonOK.Click += new System.EventHandler(this.buttonOK_Click);
            // 
            // buttonConnection
            // 
            this.buttonConnection.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConnection.AutoSize = true;
            this.buttonConnection.Location = new System.Drawing.Point(493, 10);
            this.buttonConnection.Name = "buttonConnection";
            this.buttonConnection.Size = new System.Drawing.Size(53, 12);
            this.buttonConnection.TabIndex = 6;
            this.buttonConnection.TabStop = true;
            this.buttonConnection.Text = "现有连接";
            this.buttonConnection.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.buttonString_LinkClicked);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // buttonString
            // 
            this.buttonString.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonString.AutoSize = true;
            this.buttonString.Location = new System.Drawing.Point(422, 10);
            this.buttonString.Name = "buttonString";
            this.buttonString.Size = new System.Drawing.Size(65, 12);
            this.buttonString.TabIndex = 6;
            this.buttonString.TabStop = true;
            this.buttonString.Text = "连接字符串";
            this.buttonString.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.buttonConnection_LinkClicked);
            // 
            // DatabaseConnectionBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.buttonString);
            this.Controls.Add(this.buttonConnection);
            this.Controls.Add(this.buttonOK);
            this.Controls.Add(this.buttonTest);
            this.Controls.Add(this.buttonSwitch);
            this.Controls.Add(this.propertyEditor);
            this.Controls.Add(this.buttonAdd);
            this.Controls.Add(this.comboTypes);
            this.Controls.Add(this.labelTypes);
            this.Name = "DatabaseConnectionBox";
            this.Size = new System.Drawing.Size(557, 464);
            this.Load += new System.EventHandler(this.DatabaseConnectionBox_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelTypes;
        private System.Windows.Forms.ComboBox comboTypes;
        private System.Windows.Forms.LinkLabel buttonAdd;
        private System.Windows.Forms.PropertyGrid propertyEditor;
        private System.Windows.Forms.Button buttonSwitch;
        private System.Windows.Forms.Button buttonTest;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.LinkLabel buttonConnection;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.LinkLabel buttonString;
    }
}

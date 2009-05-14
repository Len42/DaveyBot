namespace DaveyBot
{
    partial class DlgSetup
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
			this.btnOK = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.comboVideoSource = new System.Windows.Forms.ComboBox();
			this.btnCrossbar = new System.Windows.Forms.Button();
			this.label2 = new System.Windows.Forms.Label();
			this.comboCommPort = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.textLagAdjust = new System.Windows.Forms.TextBox();
			this.btnCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			this.comboNoteFinder = new System.Windows.Forms.ComboBox();
			this.chkDelayStrum = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// btnOK
			// 
			this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnOK.Location = new System.Drawing.Point(161, 420);
			this.btnOK.Name = "btnOK";
			this.btnOK.Size = new System.Drawing.Size(75, 23);
			this.btnOK.TabIndex = 8;
			this.btnOK.Text = "&OK";
			this.btnOK.UseVisualStyleBackColor = true;
			this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(6, 22);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(72, 13);
			this.label1.TabIndex = 1;
			this.label1.Text = "&Video source:";
			// 
			// comboVideoSource
			// 
			this.comboVideoSource.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboVideoSource.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboVideoSource.FormattingEnabled = true;
			this.comboVideoSource.Location = new System.Drawing.Point(6, 38);
			this.comboVideoSource.Name = "comboVideoSource";
			this.comboVideoSource.Size = new System.Drawing.Size(293, 21);
			this.comboVideoSource.TabIndex = 2;
			this.comboVideoSource.SelectionChangeCommitted += new System.EventHandler(this.comboVideoSource_SelectionChangeCommitted);
			// 
			// btnCrossbar
			// 
			this.btnCrossbar.Location = new System.Drawing.Point(6, 76);
			this.btnCrossbar.Name = "btnCrossbar";
			this.btnCrossbar.Size = new System.Drawing.Size(111, 23);
			this.btnCrossbar.TabIndex = 3;
			this.btnCrossbar.Text = "Select Video &Input";
			this.btnCrossbar.UseVisualStyleBackColor = true;
			this.btnCrossbar.Click += new System.EventHandler(this.btnCrossbar_Click);
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 22);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(60, 13);
			this.label2.TabIndex = 4;
			this.label2.Text = "Comm &port:";
			// 
			// comboCommPort
			// 
			this.comboCommPort.FormattingEnabled = true;
			this.comboCommPort.Location = new System.Drawing.Point(6, 38);
			this.comboCommPort.Name = "comboCommPort";
			this.comboCommPort.Size = new System.Drawing.Size(94, 21);
			this.comboCommPort.TabIndex = 5;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(6, 22);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(147, 13);
			this.label3.TabIndex = 6;
			this.label3.Text = "&Lag adjustment (milliseconds):";
			// 
			// textLagAdjust
			// 
			this.textLagAdjust.Location = new System.Drawing.Point(6, 38);
			this.textLagAdjust.Name = "textLagAdjust";
			this.textLagAdjust.Size = new System.Drawing.Size(46, 20);
			this.textLagAdjust.TabIndex = 7;
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(242, 420);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 9;
			this.btnCancel.Text = "&Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnCrossbar);
			this.groupBox1.Controls.Add(this.comboVideoSource);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(305, 116);
			this.groupBox1.TabIndex = 10;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Video Input";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.comboCommPort);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Location = new System.Drawing.Point(12, 134);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(305, 71);
			this.groupBox2.TabIndex = 11;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "Controller Output";
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.chkDelayStrum);
			this.groupBox3.Controls.Add(this.label4);
			this.groupBox3.Controls.Add(this.comboNoteFinder);
			this.groupBox3.Controls.Add(this.textLagAdjust);
			this.groupBox3.Controls.Add(this.label3);
			this.groupBox3.Location = new System.Drawing.Point(12, 211);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(305, 155);
			this.groupBox3.TabIndex = 12;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "Analysis";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(6, 105);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(80, 13);
			this.label4.TabIndex = 8;
			this.label4.Text = "&Note detection:";
			// 
			// comboNoteFinder
			// 
			this.comboNoteFinder.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.comboNoteFinder.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboNoteFinder.FormattingEnabled = true;
			this.comboNoteFinder.Location = new System.Drawing.Point(6, 121);
			this.comboNoteFinder.Name = "comboNoteFinder";
			this.comboNoteFinder.Size = new System.Drawing.Size(293, 21);
			this.comboNoteFinder.Sorted = true;
			this.comboNoteFinder.TabIndex = 8;
			// 
			// chkDelayStrum
			// 
			this.chkDelayStrum.AutoSize = true;
			this.chkDelayStrum.Location = new System.Drawing.Point(6, 73);
			this.chkDelayStrum.Name = "chkDelayStrum";
			this.chkDelayStrum.Size = new System.Drawing.Size(133, 17);
			this.chkDelayStrum.TabIndex = 8;
			this.chkDelayStrum.Text = "&Delay strumming briefly";
			this.chkDelayStrum.UseVisualStyleBackColor = true;
			// 
			// DlgSetup
			// 
			this.AcceptButton = this.btnOK;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.btnOK;
			this.ClientSize = new System.Drawing.Size(329, 455);
			this.ControlBox = false;
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnOK);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DlgSetup";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Video Setup";
			this.Load += new System.EventHandler(this.DlgVideoSetup_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox comboVideoSource;
		private System.Windows.Forms.Button btnCrossbar;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboCommPort;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox textLagAdjust;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ComboBox comboNoteFinder;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox chkDelayStrum;
    }
}